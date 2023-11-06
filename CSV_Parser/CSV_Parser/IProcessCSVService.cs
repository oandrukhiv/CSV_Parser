using System.Data;

namespace CSV_Parser
{
    public interface IProcessCSVService
    {
        public static abstract DataTable CreateDataTableFromCSV();
    }
}
