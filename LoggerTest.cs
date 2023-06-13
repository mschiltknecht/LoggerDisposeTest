using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Settings.Configuration;

namespace LoggerDisposeTest;

[TestClass]
public class LoggerTest
{
    [TestMethod]
    [Timeout(1000)]
    public async Task TestDisposeTiming()
    {
        var builder = WebApplication.CreateBuilder();

        var serilog = new LoggerConfiguration();
        serilog.Enrich.With(new PropertyEnricher("ServiceName", "Name"));
        serilog.Enrich.WithMachineName();
        serilog.ReadFrom.Configuration(builder.Configuration, new ConfigurationReaderOptions() {  SectionName = "Serilog" });
        builder.Logging.AddSerilog(serilog.CreateLogger(), true);

        var app = builder.Build();

        await app.StartAsync();

        //comment out next line for unit test to pass
        app.Services.GetRequiredService<ILogger<LoggerTest>>().LogInformation("hello");

        await app.StopAsync();

        var sw = Stopwatch.StartNew();

        await app.DisposeAsync();

        sw.Stop();
    }
}