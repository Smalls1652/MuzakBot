using MuzakBot.Database.Extensions;
using MuzakBot.Database.Models;
using MuzakBot.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables()
    .AddCommandLine(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

DatabaseConfig databaseConfig = builder.Configuration.GetDatabaseConfig();

builder.Services
    .AddLyricsAnalyzerDbContextFactory(databaseConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseAntiforgery();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
