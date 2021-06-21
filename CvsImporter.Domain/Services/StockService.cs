using CsvImporter.Domain.Entities;
using CsvImporter.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvImporter.Domain.Services
{
    /// <summary>
    /// Servicio con la logica para procesar un archivo y enviarlo a la base de datos.
    /// </summary>
    public class StockService : IStockService
    {
        private readonly ILogger<StockService> _logger;
        private readonly IDataInsert _dataInsert;
        private readonly IDataClean _dataClean;
        private readonly IGetFileFactory _getFileFactory;
        private IGetFile _getFile;

        public StockService(ILogger<StockService> logger, IDataInsert dataInsert, IDataClean dataClean, IGetFileFactory getFileFactory)
        {
            _logger = logger;
            _dataInsert = dataInsert;
            _dataClean = dataClean;
            _getFileFactory = getFileFactory;
        }

        /// <summary>
        /// Procesa el archivo y lo envia a la base de datos.
        /// </summary>
        /// <param name="stocks">Lista de objetos StockModel.</param>
        /// <param name="batchSize">Cantidad de registros a insertar.</param>
        /// <returns></returns>
        public async Task<int> ProcessFileAsync(string source, int batchSize)
        {
            try
            {
                Stopwatch timeElapsed = new Stopwatch();
                timeElapsed.Start();

                // Limpia la tabla Stocks.
                await _dataClean.CleanDataAsync();
                _getFile = _getFileFactory.GetFileProvider(source);

                // Obtiene el StreamReader con los datos del archivo.
                StreamReader data = await _getFile.GetFileSrteamAsync(source);

                // Se guardan los registros en la base de datos.
                int rowsInserted = await StockInsertAsync(data, batchSize);

                if (rowsInserted > 0)
                {
                    timeElapsed.Stop();
                    _logger.LogInformation($"Fin del proceso exitosamente en {timeElapsed.ElapsedMilliseconds} ms.");
                }
                return rowsInserted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrio un error.");
                return 0;
            }
        }

        private async Task<int> StockInsertAsync(StreamReader streamReader, int batchSize)
        {
            List<StockModel> stocks = new List<StockModel>();

            int counter = 0;
            int totalInserted = 0;

            using (streamReader)
            {
                while (!streamReader.EndOfStream)
                {
                    if (counter == batchSize)
                    {
                        // Envia la lista para que se guerde en la base de datos.
                        totalInserted += await InsertDataDBAsync(stocks, batchSize);
                        stocks.Clear();
                        counter = 0;
                    }

                    //Deseraliza el string.
                    StockModel stock = RowToStockModel(await streamReader.ReadLineAsync());
                    if (!(stock == null))
                    {
                        //Agrega el objeto a la lista.
                        stocks.Add(stock);
                       
                        counter++;
                    }
                }

                if (stocks.Count > 0)
                {
                    totalInserted += await InsertDataDBAsync(stocks, stocks.Count);
                    stocks.Clear();
                }

                streamReader.DiscardBufferedData();
                streamReader.Dispose();
                stocks.Clear();
            }
            return totalInserted;
        }

        public StockModel RowToStockModel(string row)
        {
            string[] rowField = row.Split(';');

            // Verifica que existan datos.
            if (rowField.Any(r => r.Length == 0))
            {
                _logger.LogWarning($"Error al procesar la fila: {rowField}.");
                return null;
            }

            bool validStock = Int32.TryParse(rowField[3], out int _stock);
            bool validDate = DateTime.TryParse(rowField[2], out DateTime _date);

            if (validStock && validDate)
            {
                StockModel stock = new StockModel
                {
                    PointOfSale = rowField[0],
                    Product = rowField[1],
                    Date = _date,
                    Stock = _stock
                };
                return stock;
            }

            return null;
        }

        private async Task<int> InsertDataDBAsync(List<StockModel> stocks, int batchSize)
        {
            Stopwatch timeElapsed = new Stopwatch();
            timeElapsed.Start();

            int rowsAffected = await _dataInsert.InsertDataAsync(stocks, batchSize);

            timeElapsed.Stop();
            _logger.LogInformation($"Se insertaron {rowsAffected} filas en {timeElapsed.ElapsedMilliseconds} ms.");
            return rowsAffected;
        }
    }
}
