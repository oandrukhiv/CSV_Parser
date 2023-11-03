namespace CSV_Parser
{
    public class CSVData
    {
        public int ID { get; set; }
        [DataNames("tpep_pickup_datetime")]
        public DateTime PickupDatetime { get; set; }
        [DataNames("tpep_dropoff_datetime")]
        public DateTime DropoffDatetime { get; set; }
        [DataNames("passenger_count")]
        public int? PassengerCount { get; set; }
        [DataNames("trip_distance")]
        public double TripDistance { get; set; }
        [DataNames("store_and_fwd_flag")]
        public string? StoreAndFwdFlag { get; set; }
        [DataNames("PULocationID")]
        public int PULocationID { get; set; }
        [DataNames("DOLocationID")]
        public int DOLocationID { get; set; }
        [DataNames("fare_amount")]
        public double FareAmount { get; set; }
        [DataNames("tip_amount")]
        public double TipAmount { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataNamesAttribute : Attribute
    {
        protected List<string> ValueNamesAttributes { get; set; }

        public List<string> ValueNames
        {
            get
            {
                return ValueNamesAttributes;
            }
            set
            {
                ValueNamesAttributes = value;
            }
        }

        public DataNamesAttribute()
        {
            ValueNamesAttributes = new List<string>();
        }

        public DataNamesAttribute(params string[] valueNames)
        {
            ValueNamesAttributes = valueNames.ToList();
        }
    }
}
