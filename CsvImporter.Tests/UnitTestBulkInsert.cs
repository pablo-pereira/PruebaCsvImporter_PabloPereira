using CsvImporter.Domain.Entities;
using CsvImporter.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CsvImporter.Tests
{
    [TestClass]
    public class UnitTestBulkInsert
    {
        private Mock<IConfiguration> _configuration;
        
        private BulkInsert _bulkInsert;

        public UnitTestBulkInsert()
        {
            // TODO: fix connectionString
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(a => a.GetConnectionString(It.Is<string>(s => s == "ConnectionStrings"))).Returns("connectionString"); ;

            var _logger = new NullLogger<BulkInsert>();

            _bulkInsert = new BulkInsert(_configuration.Object, _logger);
        }

        [TestMethod]
        public void InsertStockVoid()
        {
            List<StockModel> list = new List<StockModel>() { };
            int result = _bulkInsert.InsertDataAsync(list, 10).Result;

            Assert.AreEqual(0, result);
        }
    }
}
