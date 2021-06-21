using CsvImporter.Domain.Entities;
using CsvImporter.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Services
{
    /// <summary>
    /// Inserta StockModel en la base de deatos.
    /// </summary>
    public class BulkInsert : IDataInsert
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BulkInsert> _logger;

        public BulkInsert(IConfiguration configuration, ILogger<BulkInsert> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Metodo principal, Inserta datos en la base de datos.
        /// </summary>
        /// <param name="stocks">Lista de objetos StockModel.</param>
        /// <param name="batchSize">Cantidad de registros a insertar.</param>
        /// <returns>Retorna cantidad de registros insertados.</returns>
        public async Task<int> InsertDataAsync(List<StockModel> stocks, int batchSize)
        {
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AcmeCorporationConnection"));
            try
            {
                using (var data = CreateDataTable(stocks))
                {
                    await connection.OpenAsync();
                    using var sqlCopy = new SqlBulkCopy(connection);
                    sqlCopy.DestinationTableName = "[dbo].[Stocks]";
                    sqlCopy.BatchSize = batchSize;
                    await sqlCopy.WriteToServerAsync(data);
                    await connection.CloseAsync();
                }
                int rowsAffected = stocks.Count;
                _logger.LogInformation("Se insertaron correctamente. {0} filas", rowsAffected);
                stocks.Clear();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error durante la insercion de datos.");
                _logger.LogError(ex.Message);
                return 0;
            }
        }

        private DataTable CreateDataTable(List<StockModel> stocks)
        {
            DataTable table = new DataTable();

            // Agrego columnas de la tabla           
            table.Columns.Add(new DataColumn(nameof(StockModel.PointOfSale), typeof(int)));
            table.Columns.Add(new DataColumn(nameof(StockModel.Product), typeof(string)));
            table.Columns.Add(new DataColumn(nameof(StockModel.Date), typeof(DateTime)));
            table.Columns.Add(new DataColumn(nameof(StockModel.Stock), typeof(int)));

            foreach (var stock in stocks)
            {
                DataRow dr = table.NewRow();

                dr[nameof(stock.PointOfSale)] = stock.PointOfSale;
                dr[nameof(stock.Product)] = stock.Product;
                dr[nameof(stock.Date)] = stock.Date;
                dr[nameof(stock.Stock)] = stock.Stock;

                table.Rows.Add(dr);
            }

            return table;
        }


    }
}
