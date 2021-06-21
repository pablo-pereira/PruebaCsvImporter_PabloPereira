namespace CsvImporter.Domain.Interfaces
{
    public interface IStockService
    {
        void ProcessFileAsynk(string source, int batchSize);
    }
}
