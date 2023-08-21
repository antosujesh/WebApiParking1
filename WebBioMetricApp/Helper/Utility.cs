using Newtonsoft.Json;
using System.Data;

namespace WebApiParking.Helper
{
    public static class Utility
    {

        public static string DataTableToJSON(DataTable table)
        {
            return JsonConvert.SerializeObject(table); ;
        }
    }
}