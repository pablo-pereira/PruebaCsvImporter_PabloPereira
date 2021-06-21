using CsvImporter.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Services
{
    /// <summary>
    /// Limpia la tabla Stocks
    /// </summary>
    public class DataClean : IDataClean
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DataClean> _logger;

        public DataClean(IConfiguration config, ILogger<DataClean> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task CleanDataAsync()
        {
            string queryString = "TRUNCATE TABLE [dbo].[Stocks]";
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("AcmeCorporationConnection")))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("Conexion con la base de datos abierta.");
                    _logger.LogInformation("Limpiando tabla stock.");
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Se limpio la tabla Stocks.");
                    await connection.CloseAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);                    
                }

            }
        }
    }
}
