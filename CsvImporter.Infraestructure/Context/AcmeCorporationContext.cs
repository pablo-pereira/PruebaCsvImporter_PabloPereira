using CsvImporter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CsvImporter.Infraestructure
{
    public class AcmeCorporationContext : DbContext
    {
        public AcmeCorporationContext(DbContextOptions<AcmeCorporationContext> options) : base(options) { }
        public DbSet<StockModel> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Property Configurations
            modelBuilder.Entity<StockModel>().HasNoKey();
        }
    }
}
