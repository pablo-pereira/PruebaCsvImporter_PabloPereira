using CsvImporter.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CsvImporter.Domain.Validators
{
    /// <summary>
    /// Valida las claves de appsettings.json
    /// </summary>
    public class AppSettingsValidator : IValidator
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AppSettingsValidator> _logger;

        public AppSettingsValidator(IConfiguration config, ILogger<AppSettingsValidator> logger)
        {
            _config = config;
            _logger = logger;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(_config.GetValue<string>("SourceFile")))
            {
                _logger.LogError("Debe especificar la ruta del archivo en appsettings con la clave 'SourceFile'.");
                return false;
            }

            if (_config.GetValue<int>("BatchSize") <= 0)
            {
                _logger.LogError("Debe especificar el tamaño de Batch en appsettings con la clave 'BatchSize'.");
                return false;
            }

            if (string.IsNullOrEmpty(_config.GetConnectionString("AcmeCorporationConnection")))
            {
                _logger.LogError("Debe especificar una conexion a base de datos en  appsettings con la clave 'AcmeCorporationConnection'.");
                return false;
            }

            return true;
        }
    }
}