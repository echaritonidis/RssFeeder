using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Repositories.Implementations;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Services.Implementations;
using RssFeeder.Shared.Model;
using RssFeeder.Server.Infrastructure.Validators;
using RssFeeder.Server.Infrastructure.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient(typeof(ISQLiteRepository<>), typeof(SQLiteRepository<>));
builder.Services.AddTransient<IFeedRepository, FeedRepository>();
builder.Services.AddTransient<IFeedService, FeedService>();

builder.Services.AddTransient<IExtractContent, ExtractContent>();
builder.Services.AddSingleton<DateRegexUtil>();

// Validators
builder.Services.AddScoped<IValidator<FeedNavigation>, FeedNavigationValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
