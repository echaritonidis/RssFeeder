using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Repositories.Implementations;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Services.Implementations;
using RssFeeder.Shared.Model;
using RssFeeder.Server.Infrastructure.Validators;
using RssFeeder.Server.Infrastructure.Utils;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("Default", op =>
{
    op.BaseAddress = new Uri("https://localhost:5001");
    op.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();


builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient(typeof(ISqLiteRepository<>), typeof(SqLiteRepository<>));
builder.Services.AddTransient<ILabelRepository, LabelRepository>();
builder.Services.AddTransient<IFeedNavigationGroupRepository, FeedNavigationGroupRepository>();
builder.Services.AddTransient<IFeedNavigationRepository, FeedNavigationRepository>();

builder.Services.AddTransient<IFeedNavigationGroupService, FeedNavigationGroupService>();
builder.Services.AddTransient<IFeedNavigationService, FeedNavigationService>();

builder.Services.AddTransient<IExtractContent, ExtractContent>();
builder.Services.AddTransient<IExtractImage, ExtractImage>();

builder.Services.AddSingleton<DateRegexUtil>();
builder.Services.AddSingleton<LinkRegexUtil>();

// Api
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine
    (
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version")
    );
});

// Validators
builder.Services.AddScoped<IValidator<FeedNavigation>, FeedNavigationValidator>();
builder.Services.AddScoped<IValidator<FeedNavigationGroup>, FeedNavigationGroupValidator>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
