using CsvImporter.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Services
{
    ///<inheritdoc/>
    public class GetFileFromUrl : IGetFile
    {
        private readonly ILogger<GetFileFromUrl> _logger;       

        public GetFileFromUrl(ILogger<GetFileFromUrl> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Obtiene archivo de una URL.
        /// </summary>
        /// <param name="source">URL donde se encuentra el archivo.</param>
        /// <returns></returns>
        public async Task<StreamReader> GetFileSrteamAsync(string source)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    _logger.LogInformation("Obteniendo archivo desde host remoto.");
                    Stream data = await webClient.OpenReadTaskAsync(new Uri(source));
                    var reader = new StreamReader(data);
                    return reader;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio un error.");
                _logger.LogError(ex.Message, ex);
                return StreamReader.Null;
            }
        }        
    }
}
