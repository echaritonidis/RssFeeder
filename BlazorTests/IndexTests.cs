using RssFeeder.Client.Events;
using System;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using RichardSzalay.MockHttp;
using RssFeeder.BlazorTests;
using RssFeeder.Shared.Model;
using System.Collections.Generic;
using System.Globalization;
using AngleSharp.Dom;
using System.Linq;

namespace BlazorTests
{
    public class IndexTests : TestContext
    {
        private MockHttpMessageHandler Setup(TestContext ctx)
        {
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var mockHttpHandler = ctx.Services.AddMockHttpClient();

            // Register services
            ctx.Services.AddScoped<NotifyEventService>();

            ctx.Services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .Replace(ServiceDescriptor.Transient<IComponentActivator, ComponentActivator>());

            return mockHttpHandler;
        }

        [Fact]
        public void LoadNavItemsAndDefaultContent()
        {
            using var ctx = new TestContext();
            {
                var mockHttpHandler = this.Setup(ctx);
                mockHttpHandler.When("/api/v1.0/Feed/GetAll").RespondJson(new List<FeedNavigation>
                {
                    new()
                    {
                        Default = false,
                        Favorite = false,
                        Href = "http://feeds.foxnews.com/foxnews/scitech",
                        Title = "Fox News Science"
                    }
                });
                mockHttpHandler.When("/api/v1.0/Feed/GetContent").RespondJson(new List<FeedContent>
                {
                    new()
                    {
                         Title = "Test",
                         Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                         PubDate = DateTime.Now.AddDays(-40).ToString(CultureInfo.CurrentCulture),
                         Link = "https://google.com"
                    }
                });

                // Arrange
                var component = ctx.RenderComponent<RssFeeder.Client.Pages.Index>();

                // Act

                // Wait to load nav
                component.WaitForState(() => component.FindAll(".feed-nav .feed-item").Count.Equals(1), TimeSpan.FromMinutes(1));

                var feedNavs = component.FindAll(".feed-nav .feed-item", enableAutoRefresh: true);
                var children = feedNavs[0].Children;
                var moreIcon = children.Filter(".nav-icon").Single();

                // Click nav to load the content
                feedNavs[0].Click();

                // Click to toggle options component
                moreIcon.Click();
                
                var optionsComponent = component.WaitForElement(".feed-context");
                
                // Find the Default and Favorite icons
                var defaultIcon = component.WaitForElement(".default-icon");
                var favoriteIcon = component.WaitForElement(".favorite-icon");

                // "Click" aka MouseDown/Up (as the Clicked event uses those) to toggle the default icon
                defaultIcon.ParentElement.MouseDown();
                defaultIcon.ParentElement.MouseUp();
                // "Click" aka MouseDown/Up (as the Clicked event uses those) to toggle the favorite icon
                favoriteIcon.ParentElement.MouseDown();
                favoriteIcon.ParentElement.MouseUp();
                
                // Find the Checked Default and Favorite icons
                var defaultIconChecked = component.WaitForElement(".default-icon-checked", timeout: TimeSpan.FromMinutes(1));
                var favoriteIconChecked = component.WaitForElement(".favorite-icon-checked", timeout: TimeSpan.FromMinutes(1));
                
                // Wait to load content
                component.WaitForState(() => component.FindAll(".feed-content").Count > 0, TimeSpan.FromMinutes(1));
                var feedContents = component.FindAll(".feed-content", enableAutoRefresh: true);
                
                // Assert
                Assert.NotNull(optionsComponent);
                Assert.NotNull(moreIcon);
                Assert.NotNull(defaultIcon);
                Assert.NotNull(favoriteIcon);
                Assert.NotNull(defaultIconChecked);
                Assert.NotNull(favoriteIconChecked);
                Assert.Contains("default-icon-checked", defaultIconChecked.ClassName);
                Assert.Contains("favorite-icon-checked", favoriteIconChecked.ClassName);

                Assert.True(feedNavs.Count > 0);
                Assert.True(feedContents.Count > 0);
            }
        }
    }
}