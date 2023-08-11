using Microsoft.Data.SqlClient;
using System.Data;
using WebBioMetricApp.Models;
using WG3000_COMM.Core;

namespace WebBioMetricApp.Repository
{
    public class uploadEmp
    {

        public ReqAddPrivilegeUser GetCardNoID(int EmployeeID)
        {

            ReqAddPrivilegeUser reqAddPrivilegeUser = new ReqAddPrivilegeUser();
            int StrNo = 0;
            try
            {
                // con.Open();
                SqlConnection con = new SqlConnection(MyConnection.EEmyconnection());
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select CardNo,DeviceIDs from Employees where EmployeeId=" + EmployeeID + "", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    reqAddPrivilegeUser.CardNo = int.Parse(dt.Rows[0]["CardNo"].ToString());
                    reqAddPrivilegeUser.DeviceIDs = dt.Rows[0]["DeviceIDs"].ToString();

                }
                else
                {

                }
                con.Close();
            }
            catch (Exception)
            { }

            return reqAddPrivilegeUser;
        }

        public static int getaccessdata(int i, string connects, string cardNo)
        {
            int returnvalue = 0;
            string columnname = "f_ControlSegID" + i.ToString();
            using (SqlConnection connection = new SqlConnection(connects))
            {
                using (SqlCommand command = new SqlCommand("select * from PrivilegeData where f_CardNO= @CardNo", connection))
                {
                    //command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CardNo", cardNo);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            returnvalue = int.Parse(reader[columnname].ToString());



                        }
                    }
                }
            }

            return returnvalue;


        }
        public int GetControllerID(string ControllerName)
        {
            int StrNo = 0;
            try
            {
                // con.Open();
                SqlConnection con = new SqlConnection(MyConnection.EEmyconnection());
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select SN from ControllerConfig where Location='" + ControllerName.Replace("\"", "").ToString() + "'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    StrNo = int.Parse(dt.Rows[0]["SN"].ToString());

                }
                else
                {

                }
                con.Close();
            }
            catch (Exception)
            { }

            return StrNo;
        }



        public void AddPrivilege1(PrivilegeRequestModel requestModel)
        {
            try
            {
                uint fCardNo = requestModel.FCardNo;
                DateTime fBeginYMD = requestModel.FBeginYMD;
                DateTime fEndYMD = requestModel.FEndYMD;
                string fPIN = requestModel.FPIN;
                byte fControlSegID1 = requestModel.FControlSegID1;
                byte fControlSegID2 = requestModel.FControlSegID2;
                byte fControlSegID3 = requestModel.FControlSegID3;
                byte fControlSegID4 = requestModel.FControlSegID4;
                int controllerSN = requestModel.ControllerSN;
                string ip = "";
                int port = 60000;
                string doorName = requestModel.DoorName;

                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(MyConnection.EEmyconnection()))
                {
                    connection.Open();
                    using (SqlCommand sqlCmd = new SqlCommand(" select IP,TCPPort from ControllerConfig where SN = @controllerSN", connection))
                    {
                        sqlCmd.Parameters.AddWithValue("@controllerSN", controllerSN);
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        sqlCmd.CommandTimeout = 0;
                        sqlDa.Fill(dt);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            ip = dt.Rows[0]["IP"].ToString();
                            port = int.Parse(dt.Rows[0]["TCPPort"].ToString());
                        }
                    }
                }



                int ret = -1;
                using (DataTable dtPrivilege = new DataTable("Privilege"))//Privilege table 
                {
                    dtPrivilege.Columns.Add("f_CardNO", System.Type.GetType("System.UInt32"));//Card No.
                    dtPrivilege.Columns.Add("f_BeginYMD", System.Type.GetType("System.DateTime"));//Start Date
                    dtPrivilege.Columns.Add("f_EndYMD", System.Type.GetType("System.DateTime"));//End Date
                    dtPrivilege.Columns.Add("f_PIN", System.Type.GetType("System.String")); //password
                    dtPrivilege.Columns.Add("f_ControlSegID1", System.Type.GetType("System.Byte"));//1Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID1"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID2", System.Type.GetType("System.Byte"));//2Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID2"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID3", System.Type.GetType("System.Byte"));//3Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID3"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID4", System.Type.GetType("System.Byte"));//4Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID4"].DefaultValue = 0;  //DefaultValue

                    //Add 2000 users increasing card number from the specified initial access sequence
                    //Card number smallest, don't repeat
                    UInt32 lastCardNO = 0;
                    DataRow dr;

                    dr = dtPrivilege.NewRow();
                    dr["f_CardNO"] = fCardNo; // (i); // Card No.
                    dr["f_BeginYMD"] = fBeginYMD;
                    dr["f_EndYMD"] = fEndYMD;
                    dr["f_PIN"] = fPIN;     //must be a number
                    dr["f_ControlSegID1"] = fControlSegID1;   //1Door TimeSeg
                    dr["f_ControlSegID2"] = fControlSegID2;   //2Door TimeSeg
                    dr["f_ControlSegID3"] = fControlSegID3;   //3Door TimeSeg
                    dr["f_ControlSegID4"] = fControlSegID4;   //4Door TimeSeg


                    dtPrivilege.Rows.Add(dr);


                    dtPrivilege.AcceptChanges();

                    MjRegisterCard mjrc = new MjRegisterCard();

                    //Registration card information changes
                    mjrc.CardID = fCardNo; //card number 
                    mjrc.Password = uint.Parse("0"); //password
                    //mjrc
                    //mjrc.ymdStart = DateTime.Now;  //Start Date
                    //mjrc.ymdEnd = fEndYMD;  //End Date
                    mjrc.ControlSegIndexSet(1, byte.Parse(fControlSegID1.ToString())); //1Door TimeSeg
                    mjrc.ControlSegIndexSet(2, byte.Parse(fControlSegID2.ToString())); //2Door TimeSeg
                    mjrc.ControlSegIndexSet(3, byte.Parse(fControlSegID3.ToString())); //3Door TimeSeg
                    mjrc.ControlSegIndexSet(4, byte.Parse(fControlSegID4.ToString())); //4Door TimeSeg

                    try
                    {
                        int ret1 = -1;
                        wgMjControllerPrivilege pri = new wgMjControllerPrivilege();
                        ret1 = pri.AddPrivilegeOfOneCardIP(controllerSN, ip, port, mjrc);

                        using (var connection = new SqlConnection(MyConnection.EEmyconnection()))
                        {
                            connection.Open();

                            using (var command = new SqlCommand("InsertPrivilegeData", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@FCardNo", int.Parse(fCardNo.ToString()));
                                command.Parameters.AddWithValue("@FBeginYMD", fBeginYMD);
                                command.Parameters.AddWithValue("@FEndYMD", fEndYMD);
                                command.Parameters.AddWithValue("@FPIN", fPIN);
                                command.Parameters.AddWithValue("@FControlSegID1", fControlSegID1);
                                command.Parameters.AddWithValue("@FControlSegID2", fControlSegID2);
                                command.Parameters.AddWithValue("@FControlSegID3", fControlSegID3);
                                command.Parameters.AddWithValue("@FControlSegID4", fControlSegID4);
                                command.Parameters.AddWithValue("@ControllerSN", controllerSN);
                                command.Parameters.AddWithValue("@IP", ip);
                                command.Parameters.AddWithValue("@PORT", port);
                                command.Parameters.AddWithValue("@DoorName", doorName);

                                command.ExecuteNonQuery();
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }

                }


                // Return a success response

            }
            catch (Exception ex)
            {
                // Return an error response

            }
        }

        public void AddPrivilege(PrivilegeRequestModel requestModel)
        {
            try
            {
                uint fCardNo = requestModel.FCardNo;
                DateTime fBeginYMD = requestModel.FBeginYMD;
                DateTime fEndYMD = requestModel.FEndYMD;
                string fPIN = requestModel.FPIN;
                byte fControlSegID1 = requestModel.FControlSegID1;
                byte fControlSegID2 = requestModel.FControlSegID2;
                byte fControlSegID3 = requestModel.FControlSegID3;
                byte fControlSegID4 = requestModel.FControlSegID4;
                int controllerSN = requestModel.ControllerSN;
                string ip = "";
                int port = 60000;
                string doorName = requestModel.DoorName;

                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(MyConnection.EEmyconnection()))
                {
                    connection.Open();
                    using (SqlCommand sqlCmd = new SqlCommand(" select IP,TCPPort from ControllerConfig where SN = @controllerSN", connection))
                    {
                        sqlCmd.Parameters.AddWithValue("@controllerSN", controllerSN);
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        sqlCmd.CommandTimeout = 0;
                        sqlDa.Fill(dt);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            ip = dt.Rows[0]["IP"].ToString();
                            port = int.Parse(dt.Rows[0]["TCPPort"].ToString());
                        }
                    }
                }



                int ret = -1;
                using (DataTable dtPrivilege = new DataTable("Privilege"))//Privilege table 
                {
                    dtPrivilege.Columns.Add("f_CardNO", System.Type.GetType("System.UInt32"));//Card No.
                    dtPrivilege.Columns.Add("f_BeginYMD", System.Type.GetType("System.DateTime"));//Start Date
                    dtPrivilege.Columns.Add("f_EndYMD", System.Type.GetType("System.DateTime"));//End Date
                    dtPrivilege.Columns.Add("f_PIN", System.Type.GetType("System.String")); //password
                    dtPrivilege.Columns.Add("f_ControlSegID1", System.Type.GetType("System.Byte"));//1Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID1"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID2", System.Type.GetType("System.Byte"));//2Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID2"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID3", System.Type.GetType("System.Byte"));//3Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID3"].DefaultValue = 0;  //DefaultValue
                    dtPrivilege.Columns.Add("f_ControlSegID4", System.Type.GetType("System.Byte"));//4Door TimeSeg
                    dtPrivilege.Columns["f_ControlSegID4"].DefaultValue = 0;  //DefaultValue

                    //Add 2000 users increasing card number from the specified initial access sequence
                    //Card number smallest, don't repeat
                    UInt32 lastCardNO = 0;
                    DataRow dr;

                    dr = dtPrivilege.NewRow();
                    dr["f_CardNO"] = fCardNo; // (i); // Card No.
                    dr["f_BeginYMD"] = fBeginYMD;
                    dr["f_EndYMD"] = fEndYMD;
                    dr["f_PIN"] = fPIN;     //must be a number
                    dr["f_ControlSegID1"] = fControlSegID1;   //1Door TimeSeg
                    dr["f_ControlSegID2"] = fControlSegID2;   //2Door TimeSeg
                    dr["f_ControlSegID3"] = fControlSegID3;   //3Door TimeSeg
                    dr["f_ControlSegID4"] = fControlSegID4;   //4Door TimeSeg


                    dtPrivilege.Rows.Add(dr);


                    dtPrivilege.AcceptChanges();

                    MjRegisterCard mjrc = new MjRegisterCard();

                    //Registration card information changes
                    mjrc.CardID = fCardNo; //card number 
                    mjrc.Password = uint.Parse("0"); //password
                    //mjrc
                    //mjrc.ymdStart = DateTime.Now;  //Start Date
                    //mjrc.ymdEnd = fEndYMD;  //End Date
                    mjrc.ControlSegIndexSet(1, byte.Parse(fControlSegID1.ToString())); //1Door TimeSeg
                    mjrc.ControlSegIndexSet(2, byte.Parse(fControlSegID2.ToString())); //2Door TimeSeg
                    mjrc.ControlSegIndexSet(3, byte.Parse(fControlSegID3.ToString())); //3Door TimeSeg
                    mjrc.ControlSegIndexSet(4, byte.Parse(fControlSegID4.ToString())); //4Door TimeSeg

                    try
                    {
                        int ret1 = -1;
                        wgMjControllerPrivilege pri = new wgMjControllerPrivilege();
                        ret1 = pri.AddPrivilegeOfOneCardIP(controllerSN, ip, port, mjrc);

                        using (var connection = new SqlConnection(MyConnection.EEmyconnection()))
                        {
                            connection.Open();

                            using (var command = new SqlCommand("InsertPrivilegeData", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@FCardNo", int.Parse(fCardNo.ToString()));
                                command.Parameters.AddWithValue("@FBeginYMD", fBeginYMD);
                                command.Parameters.AddWithValue("@FEndYMD", fEndYMD);
                                command.Parameters.AddWithValue("@FPIN", fPIN);
                                command.Parameters.AddWithValue("@FControlSegID1", fControlSegID1);
                                command.Parameters.AddWithValue("@FControlSegID2", fControlSegID2);
                                command.Parameters.AddWithValue("@FControlSegID3", fControlSegID3);
                                command.Parameters.AddWithValue("@FControlSegID4", fControlSegID4);
                                command.Parameters.AddWithValue("@ControllerSN", controllerSN);
                                command.Parameters.AddWithValue("@IP", ip);
                                command.Parameters.AddWithValue("@PORT", port);
                                command.Parameters.AddWithValue("@DoorName", doorName);

                                command.ExecuteNonQuery();
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }

                }


                // Return a success response

            }
            catch (Exception ex)
            {
                // Return an error response

            }
        }


    }
}
