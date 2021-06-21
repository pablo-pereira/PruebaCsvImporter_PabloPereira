using System.Threading.Tasks;

namespace CsvImporter.Domain.Interfaces
{
    public interface IStockService
    {
        Task<int> ProcessFileAsync(string source, int batchSize);
    }
}
