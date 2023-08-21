using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebBioMetricApp.Models;
using WG3000_COMM.Core;
namespace WebBioMetricApp.Controllers
{
    [Route("api/attendance")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AttendanceController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("GetAttendanceRecords")]
        public IActionResult GetAttendanceRecords(AttendanceRecordRequest attendanceRecord)
        {
            try
            {
                pushdata();
                string SP = "";
                if (attendanceRecord.EmployeeId.Length ==  0 && attendanceRecord.FromDate == "" & attendanceRecord.ToDate == "")
                {
                    SP = "GetAttendanceRecords_T";
                }
                else if (attendanceRecord.EmployeeId.Length == 0 && attendanceRecord.FromDate != "" & attendanceRecord.ToDate != "")
                {
                    SP = "GetAttendanceRecords_DateWise";

                }
                else if (attendanceRecord.EmployeeId.Length > 0 && attendanceRecord.FromDate != "" & attendanceRecord.ToDate != "")
                {
                    SP = "GetAttendanceRecords_DateWise_Emp";

                }
                var attendanceRecords = GetAttendanceRecords1(attendanceRecord.FromDate, attendanceRecord.ToDate, attendanceRecord.EmployeeId, SP);
                return Ok(attendanceRecords);
            }
            catch (Exception ex) {
                return Ok(ex.ToString());

            }
          
        }
        // GET: api/attendance-employee [HttpPost("PushAttendance")]
        [HttpPost("PushAttendance")]
        public IActionResult GetAttendanceEmployee(MISRestore ISRestore)
        {
            try
            {
                List<SwipeRecord> swipeRecords1 = new List<SwipeRecord>();
              //  swipeRecords1 = getatt(223216509, "192.168.29.66", 60000);
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCmd = new SqlCommand(" select SN,IP,TCPPort from ControllerConfig ", connection))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        sqlCmd.CommandTimeout = 0;
                        sqlDa.Fill(dt);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                getatt(int.Parse(dt.Rows[i]["SN"].ToString()), dt.Rows[i]["IP"].ToString(), int.Parse(dt.Rows[i]["TCPPort"].ToString()), ISRestore.ISRestore);
                            }
                        }
                    }
                }

                //List<AttendanceEmployeeViewModel> attendanceEmployees = new List<AttendanceEmployeeViewModel>();
                //using (SqlConnection connection = new SqlConnection(_connectionString))
                //{
                //    string query = "SELECT A.BiometricId, A.DateTime, E.CompanyId, E.Name, E.Department, E.PhoneNo, E.EmailId " +
                //                   "FROM Attendance A INNER JOIN Employees E ON A.BiometricId = E.BiometricId";
                //    using (SqlCommand command = new SqlCommand(query, connection))
                //    {
                //        connection.Open();
                //        using (SqlDataReader reader = command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                AttendanceEmployeeViewModel attendanceEmployee = new AttendanceEmployeeViewModel
                //                {
                //                    BiometricId = Convert.ToInt32(reader["BiometricId"]),
                //                    DateTime = reader["DateTime"].ToString(),
                //                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                //                    Name = reader["Name"].ToString(),
                //                    Department = reader["Department"].ToString(),
                //                    PhoneNo = reader["PhoneNo"].ToString(),
                //                    EmailId = reader["EmailId"].ToString()
                //                };
                //                attendanceEmployees.Add(attendanceEmployee);
                //            }
                //        }
                //    }
                //}
                return Ok(swipeRecords1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
        private void pushdata()
        {
            try
            {
                List<SwipeRecord> swipeRecords1 = new List<SwipeRecord>();
                //  swipeRecords1 = getatt(223216509, "192.168.29.66", 60000);
                DataTable dt = new DataTable();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand sqlCmd = new SqlCommand(" select SN,IP,TCPPort from ControllerConfig ", connection))
                    {
                        SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                        sqlCmd.CommandTimeout = 0;
                        sqlDa.Fill(dt);
                        connection.Close();
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                getatt(int.Parse(dt.Rows[i]["SN"].ToString()), dt.Rows[i]["IP"].ToString(), int.Parse(dt.Rows[i]["TCPPort"].ToString()), 0);
                            }
                        }
                    }
                }
            }catch(Exception ex) { }
        }

        private List<SwipeRecord> getatt(int ControllerSN,string IP, int PORT, int ISRestore)
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

                if(ISRestore ==1)
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

        private List<SwipeRecord> getRunningValue(int ControllerSN, string IP, int PORT, int ISRestore)
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


        private void InsertSwipeRecord(SwipeRecord swipeRecords1)
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

        private IEnumerable<AttendanceRecord> GetAttendanceRecords1(string fromDate, string toDate, int?[] EmployeeId, string SP)
        {
            //if(EmployeeId == null) {
            //    EmployeeId = {0};
            //}

            int?[] numbers = EmployeeId;
            string CSEmployeeId = string.Join(",", numbers.Select(n => n.ToString()));

            if (CSEmployeeId == null)
            {
                EmployeeId = new int?[] { 0 }; ;
            }

            List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(SP, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FromDate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@EmployeeId", CSEmployeeId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AttendanceRecord attendanceRecord = new AttendanceRecord();

                            attendanceRecord.EmployeeId = (int)reader["EmployeeId"];
                            attendanceRecord.Name = reader["Name"].ToString();
                            attendanceRecord.CompanyId = (int)reader["CompanyId"];
                            attendanceRecord.CompanyName = reader["CompanyName"].ToString();
                            attendanceRecord.BranchName = reader["BranchName"].ToString();
                            attendanceRecord.Role = reader["Role"].ToString();
                            attendanceRecord.CardNo = reader["CardNo"].ToString();
                            attendanceRecord.AttendanceDate = (DateTime)reader["AttendanceDate"];
                            string inputTime = reader["Intime"].ToString();
                            DateTime time = DateTime.ParseExact(inputTime, "HH:mm:ss", null);
                            attendanceRecord.Intime = time.ToString("hh:mm tt");
                            string OutTime = reader["OutTime"].ToString();
                            if (OutTime != "")
                            {
                                DateTime time1 = DateTime.ParseExact(OutTime, "HH:mm:ss", null);
                                attendanceRecord.OutTime = time1.ToString("hh:mm tt");
                            }
                            else
                            {
                                attendanceRecord.OutTime = OutTime;
                            }
                            attendanceRecord.LastTime = reader["LastTime"].ToString();
                            attendanceRecord.InOut = reader["InOut"].ToString();

                            if (attendanceRecord.InOut == "1")
                            {
                                attendanceRecord.CurrentStatus = "IN";
                            }
                            else if (attendanceRecord.InOut == "0")
                            {
                                attendanceRecord.CurrentStatus = "OUT";
                            }

                            string InMints = reader["TotalMinute"].ToString();

                            int totalMinutes = int.Parse(InMints);

                            int hours = totalMinutes / 60;
                            int minutes = totalMinutes % 60;

                            string InHours = hours.ToString() + "." + minutes.ToString("00");



                            if ((480 - int.Parse(InMints)) > 0)
                            {
                                attendanceRecord.TotalMinute = InHours.ToString();
                            }
                            else { attendanceRecord.TotalMinute = "0"; }



                           // attendanceRecord.TotalMinute =  reader["TotalMinute"].ToString();


                            string BreakInMints = reader["TotalMinute"].ToString();

                            float totalMinutes1 = (480 - float.Parse(BreakInMints));

                            int hours1 = (int)totalMinutes1 / 60;
                            int minutes1 = (int)totalMinutes1 % 60;

                            string BreakInHours = hours1.ToString() + "." + minutes1.ToString("00");


                            if ((480 - int.Parse(BreakInMints)) > 0)
                            {
                                attendanceRecord.BreakHours = BreakInHours.ToString();
                            }
                            else { attendanceRecord.BreakHours = "0";  }

                            attendanceRecords.Add(attendanceRecord);
                        }
                    }
                }
            }

            return attendanceRecords;
        }
        private IEnumerable<MovementRecord> GetSwipeRecords(string cardNo)
        {
            List<MovementRecord> swipeRecords = new List<MovementRecord>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetSwipeRecords", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CardNo", cardNo);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MovementRecord MovementRecords = new MovementRecord
                            {
                                ReadTime = DateTime.Parse(reader["ReadDate"].ToString()).ToString("hh:mm"),
                                CardNo = reader["CardNo"].ToString(),
                                DoorNo = reader["DoorNo"].ToString(),
                                InOut = reader["InOut"].ToString(),
                                ReaderNo = reader["ReaderNo"].ToString()
                            };

                            swipeRecords.Add(MovementRecords);
                        }
                    }
                }
            }

            return swipeRecords;
        }
        [HttpPost("Movement")]
        public IActionResult GetMovementRecords(MovementReq cardNo)
        {


            var swipeRecords = GetSwipeRecords(cardNo.CardNo);
            return Ok(swipeRecords);
        }
    }
}
