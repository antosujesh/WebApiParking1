using Microsoft.Data.SqlClient;
using NPOI.OpenXmlFormats.Dml;
using System.Data;

namespace WebApiParking.Helper
{
    public class DataAccessDAO 

    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        private SqlConnection _conn;

        private SqlCommand _command;

        private SqlDataReader _reader;

        private SqlDataAdapter _adapter;
        public DataAccessDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (_conn == null)
                _conn = new SqlConnection(_connectionString);
        }
        
        public DataTable GetData (string query)
        {
            DataTable dtResult = new DataTable();  
            _adapter = new SqlDataAdapter(query,_conn);
            _adapter.Fill(dtResult);
            return dtResult;    
        }

        public void SaveData(SqlCommand command)
        {
            command.Connection = _conn;
            command.ExecuteNonQuery();
            
        }
    }
}
