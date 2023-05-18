using System.Net;
using System.Net.Sockets;
using LoanService;
using Microsoft.AspNetCore;
using NLog;
using NLog.Web;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile($@"ocelot.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true,
        true);
    builder.Services.AddOcelot(builder.Configuration);


    builder.WebHost.UseUrls("http://*:5003");
// builder.Host.UseWindowsService();
// builder.WebHost.UseKestrel().UseUrls("http://localhost:5001");
// WebHost.CreateDefaultBuilder(args).UseUrls("https://localhost:7171");

    var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    NLog.GlobalDiagnosticsContext.Set("LogDirectory",logPath);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();
    app.MapGet("/", () => "Hello World!");
    app.MapControllers();
    app.UseOcelotMiddleware();
    await app.UseOcelot();
    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}