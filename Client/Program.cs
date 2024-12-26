using Blazorise;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Blazorise.Tailwind;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RssFeeder.Client;
using RssFeeder.Client.Events;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Events
builder.Services.AddScoped<NotifyEventService>();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddTailwindProviders()
    .AddFluentUI2Providers()
    .AddFluentUIIcons();


await builder.Build().RunAsync();
