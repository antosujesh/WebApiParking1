using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Functions;
using System.Data;
using WebBioMetricApp.Data;
using WebBioMetricApp.Models;
using WG3000_COMM.Core;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebBioMetricApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/employees
        [HttpPost("Listemployees")]
        public IActionResult GetEmployees(ControllerListModelResq SearchID)
        {
            try
            {
                List<Employee> employees = new List<Employee>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "select  (select top 1 CompanyName from Companies b where b.CompanyID=a.CompanyId) CompanyName ,(select RoleName from tbl_Roles where RoleID = a.Role) RoleName,   (select top 1 BranchName from tbl_Branch b where b.BranchID=a.BranchId and b.CompanyID=a.CompanyId) BranchName , * from Employees a "; string x = "";
                    if (SearchID.SearchID == 1)
                    {
                        x = " where a.Status = 1";
                    }
                    else if (SearchID.SearchID == 0)
                    {
                        x = " where a.Status = 0";
                    }
                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employee employee = new Employee
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    BiometricId = Convert.ToInt32(reader["BiometricId"]),
                                    CardNo = Convert.ToInt32(reader["CardNo"]),
                                    Name = reader["Name"].ToString(),
                                    Role = reader["RoleName"].ToString(),
                                    Department = reader["Department"].ToString(),
                                    PhoneNo = reader["PhoneNo"].ToString(),
                                    EmailId = reader["EmailId"].ToString(),
                                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                                    Address = reader["Address"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    DeviceID = reader["DeviceID"].ToString().Split(','),
                                   // DeviceIDs = reader["DeviceIDs"].ToString().Split(','),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    BranchName = reader["BranchName"].ToString(),
                                    Status = Convert.ToInt32(reader["Status"]),
                                    BranchID = Convert.ToInt32(reader["BranchId"])
                                };
                                employees.Add(employee);
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

        // POST: api/employees
        [HttpPost("CreateEmployee")]
        public ResDuplicateCardModel CreateEmployee(Employee employee)
        {
            try
            {
                EmployeeResModel employeeResModel = new EmployeeResModel();
                ResDuplicateCardModel resDuplicateCardModel = new ResDuplicateCardModel();
                string returnDuplicatecards = "";
                string vv = Repo.CheckDuplicate(employee.CardNo.ToString());
                if (vv != "")
                {
                    returnDuplicatecards = "Duplicate recored Found !! " + employee.CardNo + "-" + vv + ", ";
                    resDuplicateCardModel.Result = returnDuplicatecards;
                    resDuplicateCardModel.Status = "D";
                    return resDuplicateCardModel;
                }



                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string[] DeviceIDarray = employee.DeviceID;
                    string DeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDarray);
                    //string[] DeviceIDsDeviceIDarray = employee.DeviceIDs;
                    //string DeviceIDsDeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDsDeviceIDarray);
                    string query = "INSERT INTO Employees (BiometricId, Name, Department, PhoneNo, EmailId, CompanyId, Address, CreatedBy, CreatedDate,CardNo,Role,DeviceIDs,Status,BranchId) " +
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
                        command.Parameters.AddWithValue("@DeviceID", DeviceIDarraycommaSeparatedString );
                        //command.Parameters.AddWithValue("@DeviceIDs", DeviceIDsDeviceIDarraycommaSeparatedString);
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
                privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 0;
                privilegeRequestModel.FControlSegID2 = 0;
                privilegeRequestModel.FControlSegID3 = 0;
                privilegeRequestModel.FControlSegID4 = 0;
                string[] values = employee.DeviceID;
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
                    if (Acc1 == "1") { privilegeRequestModel.FControlSegID1 = 1; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, employee.CardNo.ToString(), privilegeRequestModel.ControllerSN);  }
                    if (Acc2 == "1") { privilegeRequestModel.FControlSegID2 = 1; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, employee.CardNo.ToString(), privilegeRequestModel.ControllerSN);  }
                    if (Acc3 == "1") { privilegeRequestModel.FControlSegID3 = 1; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, employee.CardNo.ToString(), privilegeRequestModel.ControllerSN);  }
                    if (Acc4 == "1") { privilegeRequestModel.FControlSegID4 = 1; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, employee.CardNo.ToString(), privilegeRequestModel.ControllerSN);  }
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
                SqlCommand cmd = new SqlCommand("select SN from ControllerConfig where Location='"+ ControllerName + "'", con);
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

        public static string deletePrivilege(PrivilegeRequestModel requestModel, string _connectionString)
        {
            string returnMsg = "";
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
                            returnMsg = "Privilege Addedd Successfully .";
                        }

                    }
                    catch (Exception ex)
                    {
                        returnMsg = "Problem in adding Privilege data .";
                    }

                }


                // Return a success response

            }
            catch (Exception ex)
            {
                // Return an error response
                returnMsg = "Problem in adding Privilege data .";
            }
            return returnMsg;
        }


        [HttpPost]
        public IActionResult UpdateEmployee(int id, Employee employee)
        {
            try
            {

                EmployeeResModel employeeResModel = new EmployeeResModel();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string[] DeviceIDarray = employee.DeviceID;
                    string DeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDarray);
                    //string[] DeviceIDsDeviceIDarray = employee.DeviceIDs;
                    //string DeviceIDsDeviceIDarraycommaSeparatedString = string.Join(",", DeviceIDsDeviceIDarray);
                    string query = "UPDATE Employees SET BiometricId = @BiometricId, Name = @Name,Status=@Status, Department = @Department,CardNo=@CardNo,Role=@Role,DeviceID=@DeviceID,BranchId=@BranchID, " +
                                   "PhoneNo = @PhoneNo, EmailId = @EmailId, CompanyId = @CompanyId, " +
                                   "Address = @Address  " +
                                   "WHERE EmployeeId = @EmployeeId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BiometricId", employee.BiometricId);
                        command.Parameters.AddWithValue("@DeviceID", DeviceIDarraycommaSeparatedString);
                        //command.Parameters.AddWithValue("@DeviceIDs", DeviceIDsDeviceIDarraycommaSeparatedString);
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@Department", employee.Department);
                        command.Parameters.AddWithValue("@PhoneNo", employee.PhoneNo);
                        command.Parameters.AddWithValue("@Role", employee.Role);
                        command.Parameters.AddWithValue("@CardNo", employee.CardNo);
                        command.Parameters.AddWithValue("@EmailId", employee.EmailId);
                        command.Parameters.AddWithValue("@CompanyId", employee.CompanyId);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@EmployeeId", id);
                        command.Parameters.AddWithValue("@BranchID", employee.BranchID);
                        command.Parameters.AddWithValue("@Status", employee.Status);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            employeeResModel.Result = "Employee not found";
                        }
                    }
                }
                employeeResModel.Result = "Employee updated successfully";

                PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                privilegeRequestModel.FCardNo = uint.Parse(employee.CardNo.ToString());
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 0;
                privilegeRequestModel.FControlSegID2 = 0;
                privilegeRequestModel.FControlSegID3 = 0;
                privilegeRequestModel.FControlSegID4 = 0;
                string[] values = employee.DeviceID;


                foreach (string value in values)
                {

                    string[] parts = value.Split('-');

                    //foreach (string part in parts)
                    //{
                    //    Console.WriteLine(part);
                    //}
                    string ReaderNo = parts[0].ToString();
                    if (ReaderNo == "1")
                    {
                        if (ReaderNo == "1")
                        {
                            privilegeRequestModel.FControlSegID1 = 1;
                        }
                        else
                        {
                            privilegeRequestModel.FControlSegID1 = 0;
                        }
                    }
                    if (ReaderNo == "2")
                    {
                        if (ReaderNo == "2")
                        {
                            privilegeRequestModel.FControlSegID2 = 1;
                        }
                        else
                        {
                            privilegeRequestModel.FControlSegID2 = 0;
                        }
                    }
                    if (ReaderNo == "3")
                    {
                        if (ReaderNo == "3")
                        {
                            privilegeRequestModel.FControlSegID3 = 1;
                        }
                        else
                        {
                            privilegeRequestModel.FControlSegID3 = 0;
                        }
                    }
                    if (ReaderNo == "4")
                    {
                        if (ReaderNo == "4")
                        {
                            privilegeRequestModel.FControlSegID4 = 1;
                        }
                        else
                        {
                            privilegeRequestModel.FControlSegID4 = 0;
                        }
                    }
                    privilegeRequestModel.ControllerSN = GetControllerID(parts[1].ToString());

                }

                AddPrivilege(privilegeRequestModel);
                return Ok(employeeResModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // disable: 
        [HttpPost("disable1")]
        public IActionResult disableEmployee1(Employee_Act employeeId)
        {
            try
            {
                EmployeeResModel employeeResModel = new EmployeeResModel();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Employees SET Status = 0 WHERE EmployeeId = @EmployeeId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId.EmployeeId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            employeeResModel.Result = "Employee not found";
                            return NotFound(employeeResModel);
                        }
                    }
                }
                PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                ReqAddPrivilegeUser addPrivilegeUser = new ReqAddPrivilegeUser();
                addPrivilegeUser = GetCardNoID(employeeId.EmployeeId);
                privilegeRequestModel.FCardNo = uint.Parse(addPrivilegeUser.CardNo.ToString());
                privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                privilegeRequestModel.DoorName = "";
                privilegeRequestModel.FPIN = "0";
                privilegeRequestModel.FControlSegID1 = 0;
                privilegeRequestModel.FControlSegID2 = 0;
                privilegeRequestModel.FControlSegID3 = 0;
                privilegeRequestModel.FControlSegID4 = 0;

                deletePrivilege(privilegeRequestModel, _connectionString);
               



                return Ok(employeeResModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("enable")]
        public IActionResult enableEmployee(Employee_Act employeeId)
        {
            try
            {
                EmployeeResModel employeeResModel = new EmployeeResModel();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Employees SET Status = 1 WHERE EmployeeId = @EmployeeId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeId", employeeId.EmployeeId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            employeeResModel.Result = "Employee not found";
                            return NotFound(employeeResModel);
                        }
                    }
                }
                employeeResModel.Result = "Employee enable successfully";
                return Ok(employeeResModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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



        [HttpPost("disable")]
        public IActionResult disableEmployee(Employee_Act employeeId)
        {
            string returnMsg = "";
            EmployeeResModel employeeResModel = new EmployeeResModel();
            ResAddPrivilege resAddPrivilege = new ResAddPrivilege();
            try
            {

                int value = employeeId.EmployeeId;
               
               
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

                    string[] Devicevalues = addPrivilegeUser.DeviceIDs.Split(",");



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
                        if ("1" == "add")
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
                    if (Acc1 == "0") { privilegeRequestModel.FControlSegID1 = 0; } else { privilegeRequestModel.FControlSegID1 = 0; }
                    if (Acc2 == "0") { privilegeRequestModel.FControlSegID2 = 0; } else { privilegeRequestModel.FControlSegID2 = 0; }
                    if (Acc3 == "0") { privilegeRequestModel.FControlSegID3 = 0; } else { privilegeRequestModel.FControlSegID3 = 0; }
                    if (Acc4 == "0") { privilegeRequestModel.FControlSegID4 = 0; } else { privilegeRequestModel.FControlSegID4 = 0; }
                    returnMsg = AddPrivilege1(privilegeRequestModel, _connectionString, employeeId.EmployeeId.ToString());
                }
                 
                      
                    if (returnMsg == "Yes")
                    {
                        if ("1" == "add")
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
                        returnMsg = "Cant find controller, Problem in adding Privilege data .";
                    }
       

                resAddPrivilege.Result = returnMsg;
                // Return a success response
                return Ok(resAddPrivilege);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }




        }
        public static string AddPrivilege1(PrivilegeRequestModel requestModel, string _connectionString, string EmployeeId)
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
                            using (SqlConnection connection = new SqlConnection(_connectionString))
                            {
                                string query = "UPDATE Employees SET Status = 0 WHERE EmployeeId = @EmployeeId";
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                                    connection.Open();
                                    int rowsAffected = command.ExecuteNonQuery();
                                    if (rowsAffected == 0)
                                    {
                                        //employeeResModel.Result = "Employee not found";
                                        //return NotFound(employeeResModel);
                                    }
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

    }
}
