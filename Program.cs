using Umbraco.Cms.Infrastructure.ModelsBuilder.Building;
using UUGS2025.Business;
using UUGS2025.Business.Extensions;
using UUGS2025.Business.Services;
using UUGS2025.Business.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.GetConnectionString("umbracoDbDSN");

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .ConfigureAuthenticationUsers()
    .Build();

builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<IModelsGenerator, CustomModelsGenerator>();
builder.Services.AddSingleton<ISearchService, SearchService>();

WebApplication app = builder.Build();

app.MapBlazorHub();

await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();