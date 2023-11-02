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
            ProcessExcel(this);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CSVData>()
                .HasKey(b => b.ID);
        }
        private static void ProcessExcel(ApplicationContext db)
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

            IList<CSVData> items = dt.AsEnumerable().Select(row =>
                new CSVData
                {
                    PickupDatetime = Convert.ToDateTime(row.Field<string>("tpep_pickup_datetime"))
                    .ToUniversalTime(),
                    DropoffDatetime = Convert.ToDateTime(row.Field<string>("tpep_dropoff_datetime"))
                    .ToUniversalTime(),
                    //PassengerCount = Convert.ToInt32(row.Field<string>("passenger_count")),
                    TripDistance = Convert.ToDouble(row.Field<string>("trip_distance")),
                    StoreAndFwdFlag = Convert.ToString(row.Field<string>("store_and_fwd_flag"))
                    .Replace("N", "NO")
                    .Replace("Y", "YES"),
                    PULocationID = Convert.ToInt32(row.Field<string>("PULocationID")),
                    DOLocationID = Convert.ToInt32(row.Field<string>("DOLocationID")),
                    FareAmount = Convert.ToDouble(row.Field<string>("fare_amount")),
                    TipAmount = Convert.ToDouble(row.Field<string>("tip_amount"))
                }).ToList();

            foreach (var item in items) { 
            db.Data.Add(item);
                db.SaveChanges();
            }
        }
    }
}
