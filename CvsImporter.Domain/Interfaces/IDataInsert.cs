using CsvImporter.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvImporter.Domain.Interfaces
{
    public interface IDataInsert
    {
        Task<int> InsertDataAsync(List<StockModel> stocks, int batchSize);        
    }
}
