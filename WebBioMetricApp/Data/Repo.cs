using Microsoft.Data.SqlClient;
using System.Data;
using WebBioMetricApp.Repository;

namespace WebBioMetricApp.Data
{
    public class Repo
    {

        public static string CheckDuplicate(string cardNo)
        {
            string returnvalue = "";
            try
            {
                SqlConnection con = new SqlConnection(MyConnection.EEmyconnection());
           
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select Name from Employees where CardNo=@CardNo", con);
                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    returnvalue = dt.Rows[0]["Name"].ToString();

                }

            }

            catch (Exception ex)
            {


            }
            return returnvalue;

        }
    }
}
