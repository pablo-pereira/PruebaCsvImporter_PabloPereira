using System.IO;
using System.Threading.Tasks;

namespace CsvImporter.Domain.Interfaces
{
    /// <summary>
    /// Provee acceso a un archivo 
    /// </summary>
    public interface IGetFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<StreamReader> GetFileSrteamAsync(string source);
    }
}
