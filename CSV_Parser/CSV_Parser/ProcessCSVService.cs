using System.Data;
using System.Text;

namespace CSV_Parser
{
    public class ProcessCSVService : IProcessCSVService
    {
        public static DataTable CreateDataTableFromCSV()
        {
            DataTable dt = new();
            using var reader = new StreamReader("spreadsheets/sample-cab-data.csv");
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
        public static void SeedData(ApplicationContext db, DataTable dt) 
        {
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
                if (!distinctNumbers.Any(c => c.PickupDatetime.Equals(item.PickupDatetime))
                    && !distinctNumbers.Any(n => n.DropoffDatetime.Equals(item.DropoffDatetime))
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

            if (File.Exists("spreadsheets/duplicates.csv")) File.Delete("spreadsheets/duplicates.csv");
            string separator = ",";
            StringBuilder output = new();
            var headings = typeof(CSVData).GetProperties().Where(p => p.CanRead
                && (p.PropertyType.IsValueType
                || p.PropertyType == typeof(string))
                && p.GetIndexParameters().Length == 0).ToList();
            output.AppendLine(string.Join(separator, headings));
            using (StreamWriter duplicateFiles = new("spreadsheets/duplicates.csv"))
            {
                foreach (var line in duplicates)
                {
                    string[] newLine = 
                        { 
                            line.ID.ToString(), 
                            line.PickupDatetime.ToString(), 
                            line.DropoffDatetime.ToString(), 
                            line.PassengerCount.ToString(),
                            line.TripDistance.ToString(),
                            line.StoreAndFwdFlag.ToString(),
                            line.PULocationID.ToString(),
                            line.DOLocationID.ToString(),
                            line.FareAmount.ToString(),
                            line.TipAmount.ToString()
                        };
                    output.AppendLine(string.Join(separator, newLine));
                }
                duplicateFiles.WriteLine( output.ToString());
            }
            db.SaveChanges();
        }
        public static int? Converter(string? value)
        {
            if (int.TryParse(value, out int i)) return i;
            return null;
        }
    }
}
