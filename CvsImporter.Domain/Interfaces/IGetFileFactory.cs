namespace CsvImporter.Domain.Interfaces
{
    public interface IGetFileFactory
    {
        IGetFile GetFileProvider(string sourceFile);
    }
}