using CsvImporter.Application.Interfaces;
using CsvImporter.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace CsvImporter.Application
{
    public class App : IApp
    {
        private readonly ILogger<App> _logger;
        private readonly IConfiguration _config;
        private readonly IStockService _stockService;
        private readonly IValidator _appSettingsValidator;


        public App(ILogger<App> logger, IStockService stockService, IConfiguration config, IValidator appSettingsValidator)
        {
            _logger = logger;
            _stockService = stockService;
            _config = config;
            _appSettingsValidator = appSettingsValidator;
        }
        public void Run()
        {
            try
            {
                _logger.LogInformation("Iniciando...");
                if (_appSettingsValidator.IsValid())
                {
                    _stockService.ProcessFileAsynk(_config.GetValue<string>("SourceFile"), _config.GetValue<int>( "BatchSize"));                
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
            
        }
    }
}
