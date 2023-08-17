using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.IO;
using System;
using WebBioMetricApp.Models;
using WG3000_COMM.Core;
using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WebBioMetricApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ControllerConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        [HttpPost("SetController")]
        public IActionResult SetController([FromBody] ControllerConfigSetModel configModel)
        {
            try
            {
                wgMjController control = new wgMjController();
                    //set the controller config info
                control.NetIPConfigure(configModel.StrSN, configModel.StrMac, configModel.StrIP, configModel.StrMask, configModel.StrGateway, configModel.StrTCPPort, configModel.PcIPAddr);
               
                return Ok("Controller configuration successful");
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while configuring the controller.\n\n" + ex.ToString());
            }
        }
        public ReSControllerSetDateTimeModel SetControllerDatetimeF(ReqControllerSetDateTimeModel configModel)
        {
            ReSControllerSetDateTimeModel reSControllerSetDateTimeModel = new ReSControllerSetDateTimeModel();
            try
            {

                wgMjController wgMjController1 = new wgMjController();
                //set the controller config info
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM ControllerConfig where SN = " + configModel.StrSN + " "; string x = "";

                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            wgMjController1.ControllerSN = int.Parse(reader["SN"].ToString()); ;
                            wgMjController1.IP = reader["IP"].ToString();
                            wgMjController1.PORT = int.Parse(reader["TCPPort"].ToString());
                            if (wgMjController1.AdjustTimeIP(DateTime.Now) > 0)
                            {
                                reSControllerSetDateTimeModel.Status = "Y";
                                reSControllerSetDateTimeModel.Result = "Date and Time sync successfully !";
                            }
                            else
                            {
                                reSControllerSetDateTimeModel.Status = "N";
                                reSControllerSetDateTimeModel.Result = "Problem with Date and Time sync !";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reSControllerSetDateTimeModel.Status = "N";
                reSControllerSetDateTimeModel.Result = "Problem with Date and Time sync !";

            }


            return reSControllerSetDateTimeModel;
        }
        [HttpPost("SetControllerDatetime")]
        public ReSControllerSetDateTimeModel SetControllerDatetime(ReqControllerSetDateTimeModel configModel)
        {
            ReSControllerSetDateTimeModel reSControllerSetDateTimeModel = new ReSControllerSetDateTimeModel();
            try
            {
               
                wgMjController wgMjController1 = new wgMjController();
                //set the controller config info
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM ControllerConfig where SN = "+ configModel.StrSN + " "; string x = "";

                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            wgMjController1.ControllerSN = int.Parse(reader["SN"].ToString()); ;
                            wgMjController1.IP = reader["IP"].ToString();
                            wgMjController1.PORT = int.Parse(reader["TCPPort"].ToString());
                            if (wgMjController1.AdjustTimeIP(DateTime.Now) > 0)
                            {
                                reSControllerSetDateTimeModel.Status = "Y";
                                reSControllerSetDateTimeModel.Result = "Date and Time sync successfully !";
                            }
                            else
                            {
                                reSControllerSetDateTimeModel.Status = "N";
                                reSControllerSetDateTimeModel.Result = "Problem with Date and Time sync !";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reSControllerSetDateTimeModel.Status = "N";
                reSControllerSetDateTimeModel.Result = "Problem with Date and Time sync !";

            }


            return reSControllerSetDateTimeModel;
        }

        [HttpPost("Add")]
        public IActionResult Post([FromBody] ControllerConfigModel configModel)
        {
            try
            {
                ControllerConfigSetModel configModels = new ControllerConfigSetModel();
                ControllerConfigModelRes controllerConfigModelRes = new ControllerConfigModelRes();
                //using (wgMjController control = new wgMjController())
                //{
                //    //set the controller config info
                //    control.NetIPConfigure(configModel.StrSN, configModel.StrMac, configModel.StrIP, configModel.StrMask, configModel.StrGateway, configModel.StrTCPPort, configModel.PcIPAddr);
                //}
                configModels.StrSN = configModel.StrSN;
                configModels.StrMac = configModel.StrMac;
                configModels.StrIP = configModel.StrIP;
                configModels.StrMask = configModel.StrMask;
                configModels.StrTCPPort = configModel.StrTCPPort;
                configModels.PcIPAddr = configModel.PcIPAddr;
                configModels.StrGateway = configModel.StrGateway;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("ConfigureController", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SN", configModel.StrSN);
                        command.Parameters.AddWithValue("@Mac", configModel.StrMac);
                        command.Parameters.AddWithValue("@IP", configModel.StrIP);
                        command.Parameters.AddWithValue("@Mask", configModel.StrMask);
                        command.Parameters.AddWithValue("@Gateway", configModel.StrGateway);
                        command.Parameters.AddWithValue("@TCPPort", configModel.StrTCPPort);
                        command.Parameters.AddWithValue("@PCIPAddr", configModel.PcIPAddr);
                        command.Parameters.AddWithValue("@CompanyID", configModel.CompanyID);
                        command.Parameters.AddWithValue("@BranchID", configModel.BranchID);
                        command.Parameters.AddWithValue("@Location", configModel.DeviceLocation);
                        command.Parameters.AddWithValue("@CreatedBy", configModel.CreatedBy);
                        command.Parameters.AddWithValue("@CreatedDate", configModel.CreatedDate);
                        command.Parameters.AddWithValue("@LastUpdatedBy", configModel.LastUpdatedBy);
                        command.Parameters.AddWithValue("@LastUpdatedDate", configModel.LastUpdatedDate);
                        command.Parameters.AddWithValue("@Status", configModel.Status);
                        command.Parameters.AddWithValue("@Type", configModel.Type);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                controllerConfigModelRes.Result = reader["Result"].ToString();
                                controllerConfigModelRes.DeviceID = reader["CnewId"].ToString();
                            }
                        }
                    }
                    connection.Close();
                    USetController(configModels);
                    ReqControllerSetDateTimeModel reSControllerSetDateTimeModel = new ReqControllerSetDateTimeModel();
                    reSControllerSetDateTimeModel.StrSN = int.Parse(configModel.StrSN);
                    SetControllerDatetimeF(reSControllerSetDateTimeModel);
                    return Ok(controllerConfigModelRes);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while configuring the controller.\n\n" + ex.ToString());
            }
        }

        [HttpPost("Update")]
        public IActionResult Update([FromBody] ControllerConfigSetModel configModel)
        {
            try
            {
                ControllerConfigSetModel configModels = new ControllerConfigSetModel();
                ControllerConfigModelRes controllerConfigModelRes = new ControllerConfigModelRes();
                //using (wgMjController control = new wgMjController())
                //{
                //    //set the controller config info
                //    control.NetIPConfigure(configModel.StrSN, configModel.StrMac, configModel.StrIP, configModel.StrMask, configModel.StrGateway, configModel.StrTCPPort, configModel.PcIPAddr);
                //} StrMac
                configModels.StrSN = configModel.StrSN;
                configModels.PcIPAddr = configModel.PcIPAddr;
                configModels.StrIP = configModel.StrIP;
                configModels.StrMask = configModel.StrMask;
                configModels.StrTCPPort = configModel.StrTCPPort;
                configModels.PcIPAddr = configModel.PcIPAddr;
                configModels.StrGateway = configModel.StrGateway;
                configModels.StrMac = configModel.StrMac;


                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("UpdateConfigureController", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SN", configModel.StrSN);
                        command.Parameters.AddWithValue("@Mac", configModel.StrMac);
                        command.Parameters.AddWithValue("@IP", configModel.StrIP);
                        command.Parameters.AddWithValue("@Mask", configModel.StrMask);
                        command.Parameters.AddWithValue("@Gateway", configModel.StrGateway);
                        command.Parameters.AddWithValue("@TCPPort", configModel.StrTCPPort);
                        command.Parameters.AddWithValue("@PCIPAddr", configModel.PcIPAddr);
                       

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                controllerConfigModelRes.Result = reader["Result"].ToString();
                                controllerConfigModelRes.DeviceID = reader["CnewId"].ToString();
                            }
                        }
                    }
                    connection.Close();
                    USetController(configModels);
                    ReqControllerSetDateTimeModel reSControllerSetDateTimeModel = new ReqControllerSetDateTimeModel();
                    reSControllerSetDateTimeModel.StrSN = int.Parse(configModel.StrSN);
                    SetControllerDatetimeF(reSControllerSetDateTimeModel);
                    return Ok(controllerConfigModelRes);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while configuring the controller.\n\n" + ex.ToString());
            }
        }


        public void USetController(ControllerConfigSetModel configModel)
        {
            try
            {
                wgMjController control = new wgMjController();
                //set the controller config info
                control.NetIPConfigure(configModel.StrSN, configModel.StrMac, configModel.StrIP, configModel.StrMask, configModel.StrGateway, configModel.StrTCPPort, configModel.PcIPAddr);

               // return Ok("Controller configuration successful");

            }
            catch (Exception ex)
            {
              //  return StatusCode(500, "An error occurred while configuring the controller.\n\n" + ex.ToString());
            }
        }
        [HttpPost("Privilege")]
        public IActionResult customroute([FromBody] PrivilegeRequestModel requestModel)
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
                string ip ="";
                int port = 60000;
                string doorName = requestModel.DoorName;

                System.Data.DataTable dt = new System.Data.DataTable();
                    using (SqlConnection connection = new SqlConnection(_connectionString))
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
                using (System.Data.DataTable dtPrivilege = new System.Data.DataTable("Privilege"))//Privilege table 
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
                    mjrc.CardID = fCardNo ; //card number 
                    mjrc.Password = uint.Parse("0"); //password
               
                    //mjrc.ymdStart = DateTime.Now;  //Start Date
                    //mjrc.ymdEnd = fEndYMD;  //End Date
                    mjrc.ControlSegIndexSet(1, byte.Parse(fControlSegID1.ToString())); //1Door TimeSeg
                    mjrc.ControlSegIndexSet(2, byte.Parse(fControlSegID2.ToString())); //2Door TimeSeg
                    mjrc.ControlSegIndexSet(3, byte.Parse(fControlSegID3.ToString())); //3Door TimeSeg
                    mjrc.ControlSegIndexSet(4, byte.Parse(fControlSegID4.ToString())); //4Door TimeSeg

                    //try
                    //{
                    int ret1 = -1;
                    wgMjControllerPrivilege pri = new wgMjControllerPrivilege();
                    ret1 = pri.AddPrivilegeOfOneCardIP(controllerSN, ip, port, mjrc);
                    if (ret1 > 1)
                    {
                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            using (var command = new SqlCommand("InsertPrivilegeData", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@FCardNo", fCardNo);
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
                    return Ok("Privilege data received and saved successfully.");
                }

               


            


                // Return a success response
               
            }
            catch (Exception ex)
            {
                // Return an error response
                return StatusCode(500, "An error occurred while processing and saving the privilege data.");
            }
        }

        [HttpPost("UserPrivilege")]
        public IActionResult UserPrivilege([FromBody] PrivilegeRequestModel requestModel)
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

                System.Data.DataTable dt = new System.Data.DataTable();
                using (SqlConnection connection = new SqlConnection(_connectionString))
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
                using (System.Data.DataTable dtPrivilege = new System.Data.DataTable("Privilege"))//Privilege table 
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

                        using (var connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            using (var command = new SqlCommand("InsertPrivilegeData", connection))
                            {
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@FCardNo", fCardNo);
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
                        return StatusCode(500, "An error occurred while processing and saving the privilege data.");
                    }


                }


                // Return a success response
                return Ok("Privilege data received and saved successfully.");
            }
            catch (Exception ex)
            {
                // Return an error response
                return StatusCode(500, "An error occurred while processing and saving the privilege data.");
            }
        }

        [HttpPost("AddPrivilege")]
        public ResAddPrivilege AddPrivilege([FromBody] ReqAddPrivilegeToUser requestModel)
        {
            string returnMsg = "";
          
            ResAddPrivilege resAddPrivilege = new ResAddPrivilege();
            try {
              
                int[] values = requestModel.employeeid;
                foreach (int value in values)
                {
                    string Acc1 = "No", Acc2 = "No", Acc3 = "No", Acc4 = "No";
                    ReqAddPrivilegeUser addPrivilegeUser = new ReqAddPrivilegeUser();
                    addPrivilegeUser = GetCardNoID(value);
                    PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                    privilegeRequestModel.FCardNo = uint.Parse(addPrivilegeUser.CardNo.ToString());
                    privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                    privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                    privilegeRequestModel.DoorName = "";
                    privilegeRequestModel.FPIN = "0";
                    privilegeRequestModel.FControlSegID1 = 0;
                    privilegeRequestModel.FControlSegID2 = 0;
                    privilegeRequestModel.FControlSegID3 = 0;
                    privilegeRequestModel.FControlSegID4 = 0;

                    string[] Devicevalues = requestModel.reader;

                

                    int t = 1;
                    // foreach (string Devicevalue in Devicevalues)
                    foreach (string Devicevalue in Devicevalues)
                    {

                        string[] parts = Devicevalue.Split('-');

                        //foreach (string part in parts)
                        //{
                        //    Console.WriteLine(part);
                        //}
                        string ReaderNo = parts[0].ToString();
                        if (requestModel.flag == "add")
                        {
                        
                                if (ReaderNo == "1")
                                {
                                    Acc1 = "1";
                                       // privilegeRequestModel.FControlSegID1 = 1;
                                }
                                if (ReaderNo == "2")
                                {
                                    Acc2 = "1";
                                        //privilegeRequestModel.FControlSegID2 = 1;
                                }
                             
                          
                                if (ReaderNo == "3")
                                {
                                    Acc3 = "1";
                                    //privilegeRequestModel.FControlSegID3 = 1;
                                }
                             
                        
                                if (ReaderNo == "4")
                                {
                                    Acc4 = "1";
                                    //privilegeRequestModel.FControlSegID4 = 1;
                                }
                             
                       
                        }
                        else
                        {

                            if (ReaderNo == "1")
                            {
                                Acc1 = "0";
                                // privilegeRequestModel.FControlSegID1 = 1;
                            }
                            if (ReaderNo == "2")
                            {
                                Acc2 = "0";
                                //privilegeRequestModel.FControlSegID2 = 1;
                            }


                            if (ReaderNo == "3")
                            {
                                Acc3 = "0";
                                //privilegeRequestModel.FControlSegID3 = 1;
                            }


                            if (ReaderNo == "4")
                            {
                                Acc4 = "0";
                                //privilegeRequestModel.FControlSegID4 = 1;
                            }

                        }

                        privilegeRequestModel.ControllerSN = GetControllerID(parts[1].ToString());
                        if (requestModel.flag == "add")
                        {
                            if (Acc1 == "1") { privilegeRequestModel.FControlSegID1 = 1; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); }
                            if (Acc2 == "1") { privilegeRequestModel.FControlSegID2 = 1; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); }
                            if (Acc3 == "1") { privilegeRequestModel.FControlSegID3 = 1; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); }
                            if (Acc4 == "1") { privilegeRequestModel.FControlSegID4 = 1; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); }
                        }
                        else
                        {
                            if (Acc1 == "0") { privilegeRequestModel.FControlSegID1 = 0; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); ; }
                            if (Acc2 == "0") { privilegeRequestModel.FControlSegID2 = 0; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); ; }
                            if (Acc3 == "0") { privilegeRequestModel.FControlSegID3 = 0; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); ; }
                            if (Acc4 == "0") { privilegeRequestModel.FControlSegID4 = 0; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, addPrivilegeUser.CardNo.ToString(), privilegeRequestModel.ControllerSN); ; }

                        }
                        returnMsg = AddPrivilege1(privilegeRequestModel, _connectionString);
                    }
                 
                    if (returnMsg == "Yes")
                    {
                        if (requestModel.flag == "add")
                        {
                            returnMsg = "Privilege Added successfully.";
                        }
                        else
                        {
                            returnMsg = "Privilege deleted successfully.";
                        }
                    }
                    else
                    {
                        returnMsg = "Problem in adding Privilege data .";
                    }
                }
               
                resAddPrivilege.Result = returnMsg;
                // Return a success response
                return resAddPrivilege;
            }
            catch (Exception ex)
            {
                resAddPrivilege.Result = "An error occurred while processing and saving the privilege data.";
                // Return an error response
                return resAddPrivilege;
            }
      
        
        
        
        }
        public static int getaccessdata(int i, string connects, string cardNo, int SN)
        {
            int returnvalue = 0;
            string columnname = "f_ControlSegID" + i.ToString();
            using (SqlConnection connection = new SqlConnection(connects))
            {
                using (SqlCommand command = new SqlCommand("select * from PrivilegeData where f_CardNO= @CardNo and ControllerSN=@ControllerSN ", connection))
                {
                    //command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CardNo", cardNo);
                    command.Parameters.AddWithValue("@ControllerSN", SN);
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
        public ReqAddPrivilegeUser GetCardNoID(int EmployeeID)
        {
            ReqAddPrivilegeUser reqAddPrivilegeUser = new ReqAddPrivilegeUser();
            int StrNo = 0;
            try
            {
                // con.Open();
                SqlConnection con = new SqlConnection(_connectionString);
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
        public int GetControllerID(string ControllerName)
        {
            int StrNo = 0;
            try
            {
                // con.Open();
                SqlConnection con = new SqlConnection(_connectionString);
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("select SN from ControllerConfig where Location='" + ControllerName + "'", con);
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

        public static string  AddPrivilege1(PrivilegeRequestModel requestModel, string _connectionString)
        {
            string retuns = "";
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
                using (SqlConnection connection = new SqlConnection(_connectionString))
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
                        if (ret1 > 0)
                        {
                            using (var connection = new SqlConnection(_connectionString))
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
                            retuns = "Yes";
                        }

                    }
                    catch (Exception ex)
                    {
                        retuns = "No";
                    }

                }

                return retuns;
                // Return a success response

            }
            catch (Exception ex)
            {
                // Return an error response
                retuns = "No";
            }
            return retuns;
        }

        [HttpPost("ListController")]
        public IActionResult ListController(ControllerListModelResq SearchID)
        {
            try
            {
                List<ControllerConfigModel> devices = new List<ControllerConfigModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM ControllerConfig "; string x = "";
                    if (SearchID.SearchID == 1)
                    {
                        x = " where Status = 1";
                    }else if (SearchID.SearchID == 0)
                    {
                        x = " where Status = 0";
                    }
                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ControllerConfigModel device = new ControllerConfigModel
                                {
                                    DeviceID = int.Parse(reader["ID"].ToString()),
                                    StrSN = reader["SN"].ToString(),
                                    StrMac = reader["Mac"].ToString(),
                                    StrIP = reader["IP"].ToString(),
                                    StrMask = reader["Mask"].ToString(),
                                    StrGateway = reader["Gateway"].ToString(),
                                    StrTCPPort = reader["TCPPort"].ToString(),
                                    DeviceLocation = reader["Location"].ToString(),
                                    CompanyID = reader["CompanyID"].ToString(),
                                    BranchID = reader["BranchID"].ToString(),
                                    PcIPAddr = reader["PCIPAddr"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedDate = reader["CreatedDate"].ToString(),
                                    LastUpdatedBy = reader["LastUpdatedBy"].ToString(),
                                    LastUpdatedDate = reader["LastUpdatedDate"].ToString(),
                                    Type = int.Parse(reader["Type"].ToString()),
                                    Status = int.Parse(reader["Status"].ToString())
                                };
                                devices.Add(device);
                            }
                        }
                    }
                }
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error:"+ ex.ToString());
            }
        }

        [HttpGet("SearchController")]
        public IActionResult SearchController()
        {
            string retunMessage = "";
            try
            {
                List<ControllerConfigModel> devices = new List<ControllerConfigModel>();
                System.Collections.ArrayList arrControllers = new System.Collections.ArrayList();
                wgMjController control = new wgMjController();
                control.SearchControlers(ref arrControllers);
                if (arrControllers != null)
                {
                    if (arrControllers.Count <= 0)
                    {
                        return Ok("Controller not found !!");

                    }
                    else
                    {
                        wgMjControllerConfigure conf;
                        for (int i = 0; i < arrControllers.Count; i++)
                        {
                            ControllerConfigModel controllerConfigModel = new ControllerConfigModel();
                            //get a controller info from arrControllers Array
                            conf = (wgMjControllerConfigure)arrControllers[i];
                            controllerConfigModel.StrSN = conf.controllerSN.ToString();
                            controllerConfigModel.StrIP = conf.ip.ToString();
                            controllerConfigModel.StrMask = conf.mask.ToString();
                            controllerConfigModel.StrGateway = conf.gateway.ToString();
                            controllerConfigModel.StrTCPPort = conf.port.ToString();
                            controllerConfigModel.StrMac = conf.MACAddr.ToString();
                            controllerConfigModel.PcIPAddr = conf.pcIPAddr.ToString();
                            devices.Add(controllerConfigModel);
                        }
                    }
                }

                // Return a success response
                return Ok(devices);
            }
            catch (Exception ex)
            {
                // Return an error response
                return StatusCode(500, "An error occurred while searching controller.");
            }
        }

        [HttpPost("GetReaderStatus")]
        public IActionResult GetReaderStatus(ControllerListModelResq SearchID)
        {
            try
            {
                string Status = "";
                wgMjController wgMjController1 = new wgMjController();
                //string ss = wgMjController1.RunInfo.driverVersion.ToString();


                wgMjController1.ControllerSN = 223216509;
                wgMjController1.IP = "192.168.29.66";
                wgMjController1.PORT = 60000;


                wgMjControllerConfigure conf = new wgMjControllerConfigure();

                if (wgMjController1.GetConfigureIP(ref conf) > 0)
                {
                   
                    //Control mode The default 3 = online door , 2 = normally closed, 1=normally open, 0 = not controlled
                    switch (conf.DoorControlGet(1))
                    {
                        case 0:
                            Status = "not controlled";
                            break;
                        case 1:
                            Status = "normally opened";
                            break;
                        case 2:
                            Status = "normally closed";
                          
                            break;
                        case 3:
                            Status = "online";
                           
                            break;
                        default:
                            break;
                    }



                }

                return Ok(Status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error:" + ex.ToString());
            }
        }

        public static string  FindReaderStatus(ControllerParaModel controllerPara, int DoorNo)
        {
            string Status = "NA";
            try
            {
                wgMjController wgMjController1 = new wgMjController();
                //string ss = wgMjController1.RunInfo.driverVersion.ToString();


                wgMjController1.ControllerSN = controllerPara.StrSN; //223216509;
                wgMjController1.IP = controllerPara.StrIP;
                wgMjController1.PORT = controllerPara.StrTCPPort;


                wgMjControllerConfigure conf = new wgMjControllerConfigure();

                if (wgMjController1.GetConfigureIP(ref conf) > 0)
                {
                    //Control mode The default 3 = online door , 2 = normally closed, 1=normally open, 0 = not controlled
                    switch (conf.DoorControlGet(DoorNo))
                    {
                        case 0:
                            Status = "not controlled";
                            break;
                        case 1:
                            Status = "normally opened";
                            break;
                        case 2:
                            Status = "normally closed";

                            break;
                        case 3:
                            Status = "online";

                            break;
                        default:
                            break;
                    }
                }
            }catch (Exception ex)
            {
                Status = "NA";

            }
            return Status;
        }
        [HttpGet("GetControllerDoor")]
        public List<ControllerParaDoorModel> GetControllerDoor()
        {
                List<ControllerParaDoorModel> controllerParaDoorModels = new List<ControllerParaDoorModel>();
                // List<ControllerParaModel> devices = new List<ControllerParaModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                 {
                string query = "SELECT * FROM ControllerConfig where Status = 1 "; string x = "";

                query = query + x;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ControllerParaModel device = new ControllerParaModel();
         
                            int StrSN = int.Parse(reader["SN"].ToString());
                            string StrIP = reader["IP"].ToString();
                            int StrTCPPort = int.Parse(reader["TCPPort"].ToString());
                            int StrType = int.Parse(reader["Type"].ToString());
                            string Location = reader["Location"].ToString();
                            device.StrSN = StrSN;
                            device.StrIP = StrIP;
                            device.StrTCPPort = StrTCPPort;

                            device.StrType = StrType;
                            int ReaderCount = device.StrType * 2; int k = 1;
                            for (int i=0; i < device.StrType; i++)
                            {
                                //for (int j = 0; j < 2; j++)
                                //{
                                    ControllerParaDoorModel paraDoorModel = new ControllerParaDoorModel();
                                    paraDoorModel.StrSN = StrSN;
                                    paraDoorModel.DoorNo = i+1;
                                    paraDoorModel.ReaderNo = k;
                                    paraDoorModel.Location = k + " " + Location;
                                    paraDoorModel.Status = FindReaderStatus(device, (i + 1));
                                    k++;
                                    controllerParaDoorModels.Add(paraDoorModel);
                                //}
                            }

                            

                           // devices.Add(device);
                        }
                    }
                }
            }
            return controllerParaDoorModels;
        }


        [HttpPost("GetPermissionList")]
        public List<ControllerGetPermissionListModel> GetPermissionList(ReqControllerGetPermissionListModel reqControllerGet)
        {
            List<ControllerGetPermissionListModel> controllerParaDoorModels = new List<ControllerGetPermissionListModel>();
            // List<ControllerParaModel> devices = new List<ControllerParaModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string commaSeparatedString = string.Join(",", reqControllerGet.StrSN);
                string query = "SELECT * FROM ControllerConfig where SN in ("+ commaSeparatedString.ToString() + ") "; string x = "";
 
                

                query = query + x;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ControllerParaModel device = new ControllerParaModel();

                            int StrSN = int.Parse(reader["SN"].ToString());
                            string StrIP = reader["IP"].ToString();
                            int StrTCPPort = int.Parse(reader["TCPPort"].ToString());
                            int StrType = int.Parse(reader["Type"].ToString());
                            string Location = reader["Location"].ToString();
                            device.StrSN = StrSN;
                            device.StrIP = StrIP;
                            device.StrTCPPort = StrTCPPort;

                            device.StrType = StrType;
                            int ReaderCount = device.StrType * 2; int k = 1;
                            for (int i = 0; i < device.StrType; i++)
                            {
                                //for (int j = 0; j < 2; j++)
                                //{
                                    ControllerGetPermissionListModel paraDoorModel = new ControllerGetPermissionListModel();
                                    paraDoorModel.StrSN = StrSN;
                                     paraDoorModel.DoorNo = i + 1;
                                    paraDoorModel.ReaderNo = k;
                                    paraDoorModel.Location = k + "-" + Location;
                                   
                                    k++;
                                    controllerParaDoorModels.Add(paraDoorModel);
                                //}
                            }



                            // devices.Add(device);
                        }
                    }
                }
            }
            return controllerParaDoorModels;
        }

        [HttpPost("GetAccessList")]
        public IActionResult GetAccessList(ReqControllerGetAccessListModel reqControllerGet)
        {
            List<ControllerGetAccessListModel> paraDoorModelList = new List<ControllerGetAccessListModel>();
            //List<ControllerGetAccessListModel> controllerParaDoorModels = new List<ControllerGetAccessListModel>();
            // List<ControllerParaModel> devices = new List<ControllerParaModel>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string commaSeparatedString = string.Join(",", reqControllerGet.EmployeeID);
                string query = "select (select Location from ControllerConfig where SN =ControllerSN) location,(select a.DeviceID from Employees a where a.CardNo =f_CardNO and a.DeviceID = ControllerSN and a.EmployeeId in (" + commaSeparatedString + ")) SN,* from PrivilegeData where f_CardNo = (select CardNo from Employees where EmployeeId in (" + commaSeparatedString + ") )  "; string x = "";



                query = query + x;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //reader.Read();
                        while (reader.Read())
                        {

                            // paraDoorModel = new List<ControllerGetAccessListModel>
                            //{
                            //    new ControllerGetAccessListModel { strSN =  int.Parse(reader["SN"].ToString()), f_ControlSegID1 = int.Parse(reader["f_ControlSegID1"].ToString()) },
                            //    new ControllerGetAccessListModel { strSN = int.Parse(reader["SN"].ToString()), f_ControlSegID2 = int.Parse(reader["f_ControlSegID2"].ToString()) },
                            //    new ControllerGetAccessListModel { strSN = int.Parse(reader["SN"].ToString()), f_ControlSegID3 = int.Parse(reader["f_ControlSegID3"].ToString()) },
                            //    new ControllerGetAccessListModel { strSN = int.Parse(reader["SN"].ToString()), f_ControlSegID4 = int.Parse(reader["f_ControlSegID4"].ToString()) }
                            //};

                        for (int i = 1; i <= 4; i++)
                        {
                                string f_ControlSegID = reader["f_ControlSegID" + i].ToString();
                                //if (i == 1) { 
                                if (f_ControlSegID != "0")
                                {
                                    ControllerGetAccessListModel paraDoorModel = new ControllerGetAccessListModel();
                                    paraDoorModel.Location = reader["location"].ToString();
                                    
                                    paraDoorModel.f_ControlSegID = i + "-"+reader["location"].ToString() ;
                                    paraDoorModelList.Add(paraDoorModel);
                                }
                                //}
                            //if (i == 2) { paraDoorModel.f_ControlSegID = int.Parse(reader["f_ControlSegID2"].ToString()); }
                            //if (i == 3) { paraDoorModel.f_ControlSegID = int.Parse(reader["f_ControlSegID3"].ToString()); }
                            //if (i == 4) { paraDoorModel.f_ControlSegID = int.Parse(reader["f_ControlSegID4"].ToString()); }
                          
                        }





                    }
                }
                }
            }
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            return new JsonResult(paraDoorModelList, options); 
        }


  
        public void CreateBulkEmployee(Employee employee)
        {
            try
            {
                EmployeeResModel employeeResModel = new EmployeeResModel();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string[] DeviceIDarray = employee.DeviceID;
                    string DeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDarray);
                    string query = "INSERT INTO Employees (BiometricId, Name, Department, PhoneNo, EmailId, CompanyId, Address, CreatedBy, CreatedDate,CardNo,Role,DeviceID,Status,BranchId) " +
                                   "VALUES (@BiometricId, @Name, @Department, @PhoneNo, @EmailId, @CompanyId, @Address, @CreatedBy, @CreatedDate,@CardNo,@Role,@DeviceID,@Status,@BranchID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BiometricId", employee.BiometricId);
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@CardNo", employee.CardNo);
                        command.Parameters.AddWithValue("@Role", employee.Role);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@PhoneNo", employee.PhoneNo);
                        command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                        command.Parameters.AddWithValue("@CompanyId", employee.CompanyId);
                        command.Parameters.AddWithValue("@DeviceID", DeviceIDarraycommaSeparatedString);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@CreatedBy", employee.CreatedBy);
                        command.Parameters.AddWithValue("@Status", employee.Status);
                        command.Parameters.AddWithValue("@BranchID", employee.BranchID);
                        command.Parameters.AddWithValue("@CreatedDate", employee.CreatedDate ?? DateTime.Now);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                employeeResModel.Result = "Employee created successfully";
                PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                privilegeRequestModel.FCardNo = uint.Parse(employee.CardNo.ToString());
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 1;
                privilegeRequestModel.FControlSegID2 = 1;
                privilegeRequestModel.FControlSegID3 = 1;
                privilegeRequestModel.FControlSegID4 = 1;
                string[] values = employee.DeviceID;

                foreach (string value in values)
                {
                    privilegeRequestModel.ControllerSN = int.Parse(value);
                   // AddPrivilege(privilegeRequestModel);
                }


                // AddPrivilege(privilegeRequestModel);

            }
            catch (Exception ex)
            {

            }
        }


    }
}
