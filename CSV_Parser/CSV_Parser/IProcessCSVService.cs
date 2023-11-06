using System.Data;

namespace CSV_Parser
{
    public interface IProcessCSVService
    {
        public static abstract DataTable CreateDataTableFromCSV();
        public static abstract void SeedData(ApplicationContext context, DataTable dt);
    }
}
