using CsvImporter.Domain.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Services
{
    ///<inheritdoc/>
    public class GetFile : IGetFile
    {
        /// <summary>
        /// Devuelve StreamReader del archivo especificado en source.
        /// </summary>
        /// <param name="source">Carpeta y nombre del archivo requerido.</param>
        /// <returns></returns>
        public async Task<StreamReader> GetFileSrteamAsync(string source)
        {
            return await Task.FromResult(File.OpenText(source));           
        }
    }
}
