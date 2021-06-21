using CsvImporter.Domain.Interfaces;
using CsvImporter.Domain.Services;
using CsvImporter.Infraestructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;

namespace CsvImporter.Tests
{
    [TestClass]
    public class UnitTestStock
    {
        private readonly StockService _stockService;
        private readonly Mock<IDataInsert> _bulkInsert;
        private readonly Mock<IDataClean> _dataClean;
        private readonly Mock<IGetFileFactory> _getFileFactory;

        public UnitTestStock()
        {
            _bulkInsert = new Mock<IDataInsert>();
            _dataClean = new Mock<IDataClean>();
            _getFileFactory = new Mock<IGetFileFactory>();

            _getFileFactory.Setup(p => p.GetFileProvider("..\\..\\..\\Files\\TestStockOK.csv")).Returns(new GetFile());
                
            _stockService = new StockService(new NullLogger<StockService>(), _bulkInsert.Object, _dataClean.Object, _getFileFactory.Object);
        }

        [TestMethod]
        public void TestInsert()
        {
            var rowsInserted = _stockService.ProcessFileAsync("..\\..\\..\\Files\\TestStockOK.csv", 5);
            Assert.AreNotEqual(0, rowsInserted);
        }

        [TestMethod]
        public void RowToStockModelOK()
        {
            var stock = _stockService.RowToStockModel("121017;17240503103734;2019-08-17;2");
            Assert.IsTrue(stock == null ? false: true);
        }

        [TestMethod]
        public void RowToStockModelNotOK()
        {
            var stock = _stockService.RowToStockModel("121017;17240503103734;2019-08-17;");
            Assert.IsTrue(stock == null ? true : false);
        }


    }
}
