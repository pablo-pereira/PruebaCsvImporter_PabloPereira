using CsvImporter.Application.Interfaces;
using CsvImporter.Domain.Interfaces;
using CsvImporter.Domain.Services;
using CsvImporter.Domain.Validators;
using CsvImporter.Infraestructure;
using CsvImporter.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.IO;

namespace CsvImporter.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = BuildConfiguration();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = BuildlHost(builder);

            host.Services.GetService<AcmeCorporationContext>().Database.EnsureCreated();

            var app = ActivatorUtilities.CreateInstance<App>(host.Services);
            app.Run();            
            Console.Read();
        }

        private static IHost BuildlHost(IConfiguration config)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IApp, App>();
                    services.AddTransient<IStockService, StockService>();
                    services.AddTransient<IGetFileFactory, GetFileFactory>();
                    services.AddScoped<GetFile>();
                    services.AddScoped<IGetFile, GetFile>(s => s.GetService<GetFile>());
                    services.AddScoped<GetFileFromUrl>();
                    services.AddScoped<IGetFile, GetFileFromUrl>(s => s.GetService<GetFileFromUrl>());
                    services.AddTransient<IDataClean, DataClean>();
                    services.AddTransient<IValidator, AppSettingsValidator>();
                    services.AddTransient<IDataInsert, BulkInsert>();
                    services.AddTransient<IGetFile, GetFile>();
                    services.AddDbContext<AcmeCorporationContext>(
                        options => options.UseSqlServer(config.GetConnectionString("AcmeCorporationConnection")));
                })
                .UseSerilog()
                .Build();
            return host;
        }

        static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
