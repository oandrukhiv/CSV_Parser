using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CSV_Parser
{
    public class ApplicationContext: DbContext
    {        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();

            var dataTable = ProcessCSVService.CreateDataTableFromCSV();
            ProcessCSVService.SeedData(this, dataTable);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CSVData>()
                .HasKey(b => b.ID);
        }
        public DbSet<CSVData> Data { get; set; } = null!;
    }
}
