using MathNet.Numerics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Reflection;
using WebApiParking.Models;
using WebBioMetricApp.Data;
using WebBioMetricApp.Models;
using WG3000_COMM.Core;
using System.IO;
using NPOI.HPSF;

namespace WebApiParking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public VehicleController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        static string GetFileExtensionFromBase64(string base64String)
        {
            string[] parts = base64String.Split(',');

            if (parts.Length > 0)
            {
                string mimeType = parts[0].Replace("data:", "").Split(';')[0];
                string[] mimeMap = {
                    "image/jpeg:jpg", "image/png:png", "image/gif:gif",
                    "application/pdf:pdf", "application/msword:doc",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document:docx",
                    "application/vnd.ms-excel:xls",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet:xlsx"
                    // Add more MIME types and extensions as needed
                };

                foreach (string mapping in mimeMap)
                {
                    string[] pair = mapping.Split(':');
                    if (pair[0] == mimeType)
                    {
                        return pair[1];
                    }
                }
            }

            return null;
        }
        static string ExtractBase64FromDataUrl(string dataUrl)
        {
            int commaIndex = dataUrl.IndexOf(",");
            if (commaIndex != -1)
            {
                return dataUrl.Substring(commaIndex + 1);
            }
            return null;
        }
        [HttpPost("CreateVehicle")]
        public ResDuplicateCardModel CreateVehicle(VehicleModel VehicleResModel)
        {
            try
            {

                ResDuplicateCardModel resDuplicateCardModel = new ResDuplicateCardModel();
                string returnDuplicatecards = "";
                string vv = Repo.CheckDuplicate(VehicleResModel.SerialNumber.ToString());
                if (vv != "")
                {
                    returnDuplicatecards = "Duplicate recored Found !! " + VehicleResModel.SerialNumber + "-" + vv + ", ";
                    resDuplicateCardModel.Result = returnDuplicatecards;
                    resDuplicateCardModel.Status = "D";
                    return resDuplicateCardModel;
                }

                string FromBase64 = VehicleResModel.DriverImg.Trim();
                string FromBase641 = VehicleResModel.CarImg.Trim();

                string fileExtension = GetFileExtensionFromBase64(FromBase64);
                string fileExtension1 = GetFileExtensionFromBase64(FromBase641);
                string fileName = ""; string fileName1 = "";
                string outputPath = ""; string outputPath1 = "";
                string CarImg = "";
                GetImages(FromBase64, fileExtension, ref fileName, ref outputPath);

                GetImages(FromBase641, fileExtension1, ref fileName1, ref outputPath1);



                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string[] DeviceIDarray = VehicleResModel.DeviceID;
                    string DeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDarray);

                    string query = "INSERT INTO tbl_Vehicles (Driver_Img, Car_Img, Car_No, Owner_Name, Phone_No_01, Phone_No_02, EmailID, Resident_Address, KYC_Address, Serial_Number, CreatedBy, CreatedDate, Status, DeviceID) " +
                                   "VALUES (@Driver_Img, @Car_Img, @Car_No, @Owner_Name, @Phone_No_01, @Phone_No_02, @EmailID, @Resident_Address, @KYC_Address, @Serial_Number, 'admin', '', @Status, @DeviceID)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Driver_Img", fileName);
                        command.Parameters.AddWithValue("@Car_Img", fileName1);
                        command.Parameters.AddWithValue("@Car_No", VehicleResModel.CarNo);
                        command.Parameters.AddWithValue("@Owner_Name", VehicleResModel.OwnerName);
                        command.Parameters.AddWithValue("@Phone_No_01", VehicleResModel.PhoneNo01);
                        command.Parameters.AddWithValue("@Phone_No_02", VehicleResModel.PhoneNo02);
                        command.Parameters.AddWithValue("@EmailID", VehicleResModel.EmailID);
                        command.Parameters.AddWithValue("@Resident_Address", VehicleResModel.ResidentAddress);
                        command.Parameters.AddWithValue("@KYC_Address", VehicleResModel.KYCAddress);
                        command.Parameters.AddWithValue("@Serial_Number", VehicleResModel.SerialNumber);
                        command.Parameters.AddWithValue("@CreatedBy", VehicleResModel.CreatedBy);
                        command.Parameters.AddWithValue("@CreatedDate", VehicleResModel.CreatedDate);
                        command.Parameters.AddWithValue("@Status", VehicleResModel.Status);
                        command.Parameters.AddWithValue("@DeviceID", DeviceIDarraycommaSeparatedString);
                        //command.Parameters.AddWithValue("@BranchID", VehicleResModel.BranchID);
                        //command.Parameters.AddWithValue("@CompanyName", VehicleResModel.CompanyName);
                        //command.Parameters.AddWithValue("@BranchName", VehicleResModel.BranchName);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //employeeResModel.Result = "Employee created successfully";
                PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                privilegeRequestModel.FCardNo = uint.Parse(VehicleResModel.SerialNumber.ToString());
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 0;
                privilegeRequestModel.FControlSegID2 = 0;
                privilegeRequestModel.FControlSegID3 = 0;
                privilegeRequestModel.FControlSegID4 = 0;
                string[] values = VehicleResModel.DeviceID;
                int t = 1;
                foreach (string value in values)
                {
                    string Acc1 = "No", Acc2 = "No", Acc3 = "No", Acc4 = "No";
                    string[] parts = value.Split('-');

                    //foreach (string part in parts)
                    //{
                    //    Console.WriteLine(part);
                    //}
                    string ReaderNo = parts[0].ToString();
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

                    privilegeRequestModel.ControllerSN = GetControllerID(parts[1].ToString());
                    if (Acc1 == "1") { privilegeRequestModel.FControlSegID1 = 1; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc2 == "1") { privilegeRequestModel.FControlSegID2 = 1; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc3 == "1") { privilegeRequestModel.FControlSegID3 = 1; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc4 == "1") { privilegeRequestModel.FControlSegID4 = 1; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    AddPrivilege(privilegeRequestModel);
                }



                // AddPrivilege(privilegeRequestModel);
                resDuplicateCardModel.Result = "Employee created successfully.";
                resDuplicateCardModel.Status = "Y";
                return resDuplicateCardModel;
            }
            catch (Exception ex)
            {
                ResDuplicateCardModel resDuplicateCardModel1 = new ResDuplicateCardModel();
                resDuplicateCardModel1.Result = "Problem in creating employee details ";
                resDuplicateCardModel1.Status = "N";
                return resDuplicateCardModel1;

            }
        }

        private static void GetImages(string FromBase64, string fileExtension, ref string fileName, ref string outputPath)
        {
            if (!string.IsNullOrEmpty(fileExtension))
            {
                byte[] fileBytes = Convert.FromBase64String(ExtractBase64FromDataUrl(FromBase64));
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "SieChat." + fileExtension;
                string ss = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;
                outputPath = Path.Combine(@"D:\API\MobileAPP\Chat_files\", fileName);
                string saveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                Directory.CreateDirectory(saveFolderPath);
                string filePath = Path.Combine(saveFolderPath, fileName);
                System.IO.File.WriteAllBytes(filePath, fileBytes);
            }
        }

        [HttpPost("FetchSerialNumber")]
        public IActionResult getParkingRunningValue(MISController mISController)
        {
            try
            {
                List<SwipeRecord> swipeRecords1 = new List<SwipeRecord>();
                //  swipeRecords1 = getatt(223216509, "192.168.29.66", 60000);
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCmd = new SqlCommand(" select SN,IP,TCPPort from ControllerConfig where SN='" + mISController.srController + "' ", connection))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        sqlCmd.CommandTimeout = 0;
                        sqlDa.Fill(dt);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                getRunningValue(int.Parse(dt.Rows[i]["SN"].ToString()), dt.Rows[i]["IP"].ToString(), int.Parse(dt.Rows[i]["TCPPort"].ToString()), 0);
                            }
                        }
                    }
                }


                return Ok(swipeRecords1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("SaveImage")]
        public IActionResult SaveCarImage(VehicleModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.SerialNumber))
            {
                return BadRequest("Vehicle Serial Number is Required");
            }

            if (string.IsNullOrWhiteSpace(model?.CarImg) || string.IsNullOrWhiteSpace(model?.DriverImg))
            {
                return BadRequest("Both Driver and Car Images Null");
            }

            string _driverImage = model.DriverImg.Trim();
            string _carImage = model.CarImg.Trim();

            string _driverfileExtension = GetFileExtensionFromBase64(_driverImage);
            string _carfileExtension = GetFileExtensionFromBase64(_carImage);
            string _driverfileName = ""; string _carfileName1 = "";
            string DriverImg = ""; string CarImg = "";

            GetImages(_driverImage, _driverfileExtension, ref _driverfileName, ref DriverImg);

            GetImages(_carImage, _carfileExtension, ref _carfileName1, ref CarImg);



            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE tbl_Vehicles set Driver_Img=@Driver_Img, Car_Img= @Car_Img WHERE serial_number = @Serial_Number";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Driver_Img", _driverfileName);
                    command.Parameters.AddWithValue("@Car_Img", _carfileName1);
                    command.Parameters.AddWithValue("@Serial_Number", model.SerialNumber);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return Ok();
        }

        [HttpGet("GetAllVehicles")]
        public DataTable GetAllVehicles()
        {

            return null;
        }

        [HttpPost("VehicleAccess")]
        public IActionResult VehicleAccess(VehicleModel model)
        {
            return Ok();
        }

        [HttpPost("SaveReader")]
        public IActionResult SaveReader(Device device)
        {
            return Ok();
        }

        [HttpGet("GetReaders")]
        public DataTable GetReaders()
        {
            return null;
        }

        [HttpPost("EditPermission")]
        public IActionResult UpdatePermission(VehicleModel model)
        {
            return Ok();
        }
        public List<SwipeRecord> getRunningValue(int ControllerSN, string IP, int PORT, int ISRestore)
        {
            List<SwipeRecord> swipeRecords = new List<SwipeRecord>();
            try
            {
                DataTable dtSwipeRecord;
                dtSwipeRecord = new DataTable("SwipeRecord");
                dtSwipeRecord.Columns.Add("f_Index", System.Type.GetType("System.UInt32"));//Recording the index
                dtSwipeRecord.Columns.Add("f_ReadDate", System.Type.GetType("System.DateTime"));  //Credit card date/time
                dtSwipeRecord.Columns.Add("f_CardNO", System.Type.GetType("System.UInt32"));  //User card number
                dtSwipeRecord.Columns.Add("f_DoorNO", System.Type.GetType("System.UInt32"));  //Door No.
                dtSwipeRecord.Columns.Add("f_InOut", System.Type.GetType("System.UInt32"));// =0 is out;  =1 is in
                dtSwipeRecord.Columns.Add("f_ReaderNO", System.Type.GetType("System.UInt32")); // Reader No.
                dtSwipeRecord.Columns.Add("f_EventCategory", System.Type.GetType("System.UInt32")); // Event type
                dtSwipeRecord.Columns.Add("f_ReasonNo", System.Type.GetType("System.UInt32"));// Hardware reasons
                dtSwipeRecord.Columns.Add("f_ControllerSN", System.Type.GetType("System.UInt32"));// ControllerSN
                dtSwipeRecord.Columns.Add("f_RecordAll", System.Type.GetType("System.String")); // the all of Record value
                string result = "";
                int num = -1;

                if (ISRestore == 1)
                {
                    wgMjController wgMjController1 = new wgMjController();
                    wgMjController1.ControllerSN = ControllerSN;
                    wgMjController1.IP = IP;
                    wgMjController1.PORT = PORT;
                    wgMjController1.RestoreAllSwipeInTheControllersIP();

                }



                wgMjControllerSwipeOperate swipe4GetRecords = new wgMjControllerSwipeOperate(); //)

                swipe4GetRecords.Clear(); //Clear Record 
                num = swipe4GetRecords.GetSwipeRecords(ControllerSN, IP, PORT, ref dtSwipeRecord);


                if (dtSwipeRecord.Rows.Count > 0)
                {

                    for (int i = 0; i < dtSwipeRecord.Rows.Count; i++)
                    {
                        SwipeRecord swipeRecord = new SwipeRecord
                        {
                            f_Index = dtSwipeRecord.Rows[i]["f_Index"].ToString(),
                            ReadDate = dtSwipeRecord.Rows[i]["f_ReadDate"].ToString(),
                            CardNO = dtSwipeRecord.Rows[i]["f_CardNO"].ToString(),
                            DoorNO = dtSwipeRecord.Rows[i]["f_DoorNO"].ToString(),
                            InOut = dtSwipeRecord.Rows[i]["f_InOut"].ToString(),
                            ReaderNO = dtSwipeRecord.Rows[i]["f_ReaderNO"].ToString(),
                            ControllerSN = dtSwipeRecord.Rows[i]["f_ControllerSN"].ToString(),
                            f_EventCategory = dtSwipeRecord.Rows[i]["f_EventCategory"].ToString(),
                            f_ReasonNo = dtSwipeRecord.Rows[i]["f_ReasonNo"].ToString(),
                            f_RecordAll = dtSwipeRecord.Rows[i]["f_RecordAll"].ToString()
                        };
                        InsertSwipeRecord(swipeRecord);
                        swipeRecords.Add(swipeRecord);
                    }

                }


            }
            catch (Exception ex) { }
            return swipeRecords;
        }
        public void InsertSwipeRecord(SwipeRecord swipeRecords1)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("InsertOrUpdateSwipeRecord", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@f_Index", swipeRecords1.f_Index);
                command.Parameters.AddWithValue("@ReadDate", swipeRecords1.ReadDate);
                command.Parameters.AddWithValue("@CardNO", swipeRecords1.CardNO);
                command.Parameters.AddWithValue("@DoorNO", swipeRecords1.DoorNO);
                command.Parameters.AddWithValue("@InOut", swipeRecords1.InOut);
                command.Parameters.AddWithValue("@ReaderNO", swipeRecords1.ReaderNO);
                command.Parameters.AddWithValue("@ControllerSN", swipeRecords1.ControllerSN);
                command.Parameters.AddWithValue("@f_EventCategory", swipeRecords1.f_EventCategory);
                command.Parameters.AddWithValue("@f_ReasonNo", swipeRecords1.f_ReasonNo);
                command.Parameters.AddWithValue("@f_RecordAll", swipeRecords1.f_RecordAll);
                command.ExecuteNonQuery();

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

        [HttpPost("UpdateVehicle")]
        public ResDuplicateCardModel UpdateVehicle(VehicleModel VehicleResModel)
        {
            try
            {

                ResDuplicateCardModel resDuplicateCardModel = new ResDuplicateCardModel();
                string returnDuplicatecards = "";
                string vv = Repo.CheckDuplicate(VehicleResModel.SerialNumber.ToString());
                if (vv != "")
                {
                    returnDuplicatecards = "Duplicate recored Found !! " + VehicleResModel.SerialNumber + "-" + vv + ", ";
                    resDuplicateCardModel.Result = returnDuplicatecards;
                    resDuplicateCardModel.Status = "D";
                    return resDuplicateCardModel;
                }

                string FromBase64 = VehicleResModel.DriverImg.Trim();
                string FromBase641 = VehicleResModel.CarImg.Trim();

                string fileExtension = GetFileExtensionFromBase64(FromBase64);
                string fileExtension1 = GetFileExtensionFromBase64(FromBase641);
                string fileName = ""; string fileName1 = "";
                string outputPath = ""; string outputPath1 = "";
                string CarImg = "";
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    byte[] fileBytes = Convert.FromBase64String(ExtractBase64FromDataUrl(FromBase64));
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "SieChat." + fileExtension;
                    string ss = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;
                    outputPath = Path.Combine(@"D:\API\MobileAPP\Chat_files\", fileName);
                    string saveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                    Directory.CreateDirectory(saveFolderPath);
                    string filePath = Path.Combine(saveFolderPath, fileName);
                    System.IO.File.WriteAllBytes(filePath, fileBytes);
                }
                if (!string.IsNullOrEmpty(fileExtension1))
                {
                    byte[] fileBytes1 = Convert.FromBase64String(ExtractBase64FromDataUrl(FromBase641));
                    fileName1 = DateTime.Now.ToString("yyyyMMddHHmmss") + "SieChat." + fileExtension1;
                    string ss = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;
                    outputPath1 = Path.Combine(@"D:\API\MobileAPP\Chat_files\", fileName1);
                    string saveFolderPath1 = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                    Directory.CreateDirectory(saveFolderPath1);
                    string filePath1 = Path.Combine(saveFolderPath1, fileName);
                    System.IO.File.WriteAllBytes(filePath1, fileBytes1);
                }


                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string[] DeviceIDarray = VehicleResModel.DeviceID;
                    string DeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDarray);

                    string query = "UPDATE tbl_Vehicles SET Driver_Img = @New_Driver_Img, Car_Img = @New_Car_Img, Car_No = @New_Car_No, Owner_Name = @New_Owner_Name, " +
                        "Phone_No_01 = @New_Phone_No_01, Phone_No_02 = @New_Phone_No_02, " +
                        "EmailID = @New_EmailID, Resident_Address = @New_Resident_Address, KYC_Address = @New_KYC_Address, Serial_Number = @New_Serial_Number, Status = @New_Status, " +
                        "DeviceID = @New_DeviceID, ModifiedBy = 'admin', ModifiedDate = GETDATE()  WHERE ID=@ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", VehicleResModel.ID);
                        command.Parameters.AddWithValue("@Driver_Img", fileName);
                        command.Parameters.AddWithValue("@Car_Img", fileName1);
                        command.Parameters.AddWithValue("@Car_No", VehicleResModel.CarNo);
                        command.Parameters.AddWithValue("@Owner_Name", VehicleResModel.OwnerName);
                        command.Parameters.AddWithValue("@Phone_No_01", VehicleResModel.PhoneNo01);
                        command.Parameters.AddWithValue("@Phone_No_02", VehicleResModel.PhoneNo02);
                        command.Parameters.AddWithValue("@EmailID", VehicleResModel.EmailID);
                        command.Parameters.AddWithValue("@Resident_Address", VehicleResModel.ResidentAddress);
                        command.Parameters.AddWithValue("@KYC_Address", VehicleResModel.KYCAddress);
                        command.Parameters.AddWithValue("@Serial_Number", VehicleResModel.SerialNumber);
                        command.Parameters.AddWithValue("@CreatedBy", VehicleResModel.CreatedBy);
                        command.Parameters.AddWithValue("@CreatedDate", VehicleResModel.CreatedDate);
                        command.Parameters.AddWithValue("@Status", VehicleResModel.Status);
                        command.Parameters.AddWithValue("@DeviceID", DeviceIDarraycommaSeparatedString);
                        //command.Parameters.AddWithValue("@BranchID", VehicleResModel.BranchID);
                        //command.Parameters.AddWithValue("@CompanyName", VehicleResModel.CompanyName);
                        //command.Parameters.AddWithValue("@BranchName", VehicleResModel.BranchName);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //employeeResModel.Result = "Employee created successfully";
                PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                privilegeRequestModel.FCardNo = uint.Parse(VehicleResModel.SerialNumber.ToString());
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 0;
                privilegeRequestModel.FControlSegID2 = 0;
                privilegeRequestModel.FControlSegID3 = 0;
                privilegeRequestModel.FControlSegID4 = 0;
                string[] values = VehicleResModel.DeviceID;
                int t = 1;
                foreach (string value in values)
                {
                    string Acc1 = "No", Acc2 = "No", Acc3 = "No", Acc4 = "No";
                    string[] parts = value.Split('-');

                    //foreach (string part in parts)
                    //{
                    //    Console.WriteLine(part);
                    //}
                    string ReaderNo = parts[0].ToString();
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

                    privilegeRequestModel.ControllerSN = GetControllerID(parts[1].ToString());
                    if (Acc1 == "1") { privilegeRequestModel.FControlSegID1 = 1; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc2 == "1") { privilegeRequestModel.FControlSegID2 = 1; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc3 == "1") { privilegeRequestModel.FControlSegID3 = 1; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    if (Acc4 == "1") { privilegeRequestModel.FControlSegID4 = 1; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, VehicleResModel.SerialNumber.ToString(), privilegeRequestModel.ControllerSN); }
                    AddPrivilege(privilegeRequestModel);
                }



                // AddPrivilege(privilegeRequestModel);
                resDuplicateCardModel.Result = "Employee created successfully.";
                resDuplicateCardModel.Status = "Y";
                return resDuplicateCardModel;
            }
            catch (Exception ex)
            {
                ResDuplicateCardModel resDuplicateCardModel1 = new ResDuplicateCardModel();
                resDuplicateCardModel1.Result = "Problem in creating employee details ";
                resDuplicateCardModel1.Status = "N";
                return resDuplicateCardModel1;

            }
        }

        [HttpPost("ListVehicle")]
        public IActionResult GetEmployees(ControllerListModelResq SearchID)
        {
            try
            {
                List<VehicleModel> employees = new List<VehicleModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT ID,Driver_Img, Car_Img, Car_No, Owner_Name, Phone_No_01, Phone_No_02, EmailID, Resident_Address, KYC_Address, Serial_Number, CreatedBy, CreatedDate, Status, DeviceID " +
                                   "FROM tbl_Vehicles ";

                    if (SearchID.SearchID == 1)
                    {
                        query += "WHERE Status = 1";
                    }
                    else if (SearchID.SearchID == 0)
                    {
                        query += "WHERE Status = 0";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VehicleModel vehicleModel = new VehicleModel
                                {
                                    ID = reader["ID"].ToString(),
                                    DriverImg = reader["Driver_Img"].ToString(),
                                    CarImg = reader["Car_Img"].ToString(),
                                    CarNo = reader["Car_No"].ToString(),
                                    OwnerName = reader["Owner_Name"].ToString(),
                                    PhoneNo01 = reader["Phone_No_01"].ToString(),
                                    PhoneNo02 = reader["Phone_No_02"].ToString(),
                                    EmailID = reader["EmailID"].ToString(),
                                    ResidentAddress = reader["Resident_Address"].ToString(),
                                    KYCAddress = reader["KYC_Address"].ToString(),
                                    SerialNumber = reader["Serial_Number"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedDate = reader["CreatedDate"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"]),
                                    DeviceID = reader["DeviceID"].ToString().Split(',')

                                };
                                employees.Add(vehicleModel);
                            }
                        }
                    }
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }


}
