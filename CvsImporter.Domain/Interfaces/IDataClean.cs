using System.Threading.Tasks;

namespace CsvImporter.Domain.Interfaces
{
    /// <summary>
    /// Limpia la tabla de base de datos.
    /// </summary>
    public interface IDataClean
    {
        Task CleanDataAsync();
    }
}
