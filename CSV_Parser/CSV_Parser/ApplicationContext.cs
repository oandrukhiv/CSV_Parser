using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CSV_Parser
{
    public class ApplicationContext: DbContext
    {
        public DbSet<CSVData> Data { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            var dataTable = ProcessExcel();
            SeedData(this, dataTable);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CSVData>()
                .HasKey(b => b.ID);
        }
        private static int? Converter(string? value)
        {
            if (int.TryParse(value, out int i)) return i;
            return null;
        }
        private static void SeedData(ApplicationContext db, DataTable dt)
        {
            var c = new List<CSVData>();
            IList<CSVData> items = dt.AsEnumerable().Select(row =>
                    new CSVData
                    {
                        PickupDatetime = Convert.ToDateTime(row.Field<string>("tpep_pickup_datetime"))
                        .ToUniversalTime(),
                        DropoffDatetime = Convert.ToDateTime(row.Field<string>("tpep_dropoff_datetime"))
                        .ToUniversalTime(),
                        PassengerCount = Converter(row.Field<string>("passenger_count")),
                        TripDistance = Convert.ToDouble(row.Field<string>("trip_distance")),
                        StoreAndFwdFlag = row.Field<string>("store_and_fwd_flag")?
                        .Replace("N", "NO")
                        .Replace("Y", "YES"),
                        PULocationID = Convert.ToInt32(row.Field<string>("PULocationID")),
                        DOLocationID = Convert.ToInt32(row.Field<string>("DOLocationID")),
                        FareAmount = Convert.ToDouble(row.Field<string>("fare_amount")),
                        TipAmount = Convert.ToDouble(row.Field<string>("tip_amount"))
                    }).ToList();

            List<CSVData> distinctNumbers = new();
            List<CSVData> duplicates = new();

            foreach (var item in items)
            {
                if (!distinctNumbers.Any(c=>c.PickupDatetime.Equals(item.PickupDatetime))
                    && !distinctNumbers.Any(n=>n.DropoffDatetime.Equals(item.DropoffDatetime))
                    )
                {
                    distinctNumbers.Add(item);
                    db.Data.Add(item);
                }
                else
                {
                    duplicates.Add(item);
                }                
            }
            if (File.Exists("duplicates.csv")) File.Delete("duplicates.csv");
            using (StreamWriter duplicateFiles = new("duplicates.csv"))
            {
                var plist = typeof(CSVData).GetProperties().Where(p => p.CanRead && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)) && p.GetIndexParameters().Length == 0).ToList();
                if (plist.Count > 0)
                {
                    _ = string.Join(";", plist.Select(p => p.Name));
                    foreach (var line in duplicates)
                    {
                        var values = new List<object>();
                        foreach (var p in plist) values.Add(p.GetValue(line, null));
                        duplicateFiles.WriteLine(string.Join(";", values));
                    }
                        
                }
            }
            db.SaveChanges();

        }
        private static DataTable ProcessExcel()
        {
            DataTable dt = new();
            using var reader = new StreamReader("sample-cab-data.csv");
            string[]? headers = reader?.ReadLine()?.Split(',');
            if (headers is not null)
            foreach (string header in headers)
            {
                dt.Columns.Add(header);             
            }
            if (reader is not null)
            while (!reader.EndOfStream)
            {
                List<string> toReturn = new();
                string? s = reader.ReadLine();
                if (s is not null)
                toReturn = s.Split(",").ToList();

                DataRow row = dt.NewRow();
                row.ItemArray = toReturn.ToArray();
                dt.Rows.Add(row);
            }
            
            return dt;
        }
    }
}
