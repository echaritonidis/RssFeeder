using FluentValidation;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Repositories.Implementations;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Services.Implementations;
using RssFeeder.Shared.Model;
using RssFeeder.Server.Infrastructure.Validators;
using RssFeeder.Server.Infrastructure.Utils;
using Asp.Versioning;
using Marten;
using Weasel.Core;
using Marten.Services;
using RssFeeder.Server.Infrastructure.Model;
using Weasel.Postgresql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("Default", op =>
{
    op.BaseAddress = new Uri("https://localhost:5001");
    op.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddNpgsqlDataSource(connectionString);
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(connectionString);
    
    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();
    options.Serializer(new JsonNetSerializer { EnumStorage = EnumStorage.AsString });

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        options.Logger(new ConsoleMartenLogger());
    }

    options.Schema
        .For<Feed>()
        .ForeignKey<FeedGroup>(x => x.FeedGroupId, fkd => fkd.OnDelete = CascadeAction.Cascade);
    options.Schema
        .For<Label>()
        .ForeignKey<Feed>(x => x.FeedId, fkd => fkd.OnDelete = CascadeAction.Cascade);
})
.InitializeWith(new InitialData(InitialDatasets.FeedGroups))
.UseLightweightSessions()
.UseNpgsqlDataSource();

// builder.Services.AddTransient(typeof(ISqLiteRepository<>), typeof(SqLiteRepository<>));
builder.Services.AddTransient(typeof(IMartenRepository<>), typeof(MartenRepository<>));
builder.Services.AddTransient<ILabelRepository, LabelRepository>();
builder.Services.AddTransient<IFeedGroupRepository, FeedGroupRepository>();
builder.Services.AddTransient<IFeedRepository, FeedRepository>();

builder.Services.AddTransient<IFeedGroupService, FeedGroupService>();
builder.Services.AddTransient<IFeedService, FeedService>();

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
