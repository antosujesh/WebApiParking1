using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.Design;
using WebBioMetricApp.Data;
using WebBioMetricApp.Models;
using WebBioMetricApp.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebBioMetricApp.Controllers
{
    public class BulkuploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BulkuploadController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("Upload")]
        public async Task<ResDuplicateCardModel> UploadExcel([FromForm] FileUploadModel model)
        {
            ResDuplicateCardModel resDuplicateCardModel = new ResDuplicateCardModel();
            string returnDuplicatecards = "";
            uploadEmp uploadEmp = new uploadEmp();
            if (model.File == null || model.File.Length == 0)
            {
                resDuplicateCardModel.Status = "N";
                resDuplicateCardModel.Result = "No file selected.";
                return resDuplicateCardModel;
            }
            var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ////// Save the uploaded file to a temporary location
            //var filePath = projectDirectory;// Path.GetTempFileName();
            DateTime now = DateTime.Now;
            string formattedDateTime = now.ToString("ddMMyyyyhhmmss");
            string filePath = Path.Combine(projectDirectory, formattedDateTime + "Book1.xlsx");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            //// Process the Excel file
            var excelApp = new Application();
            var workbook = excelApp.Workbooks.Open(filePath);
            var worksheet = workbook.Sheets[1] as Worksheet;

            //// Access the Excel data and process it
            //// For example:
            var range = worksheet.UsedRange;
            var rowCount = range.Rows.Count;
            var columnCount = range.Columns.Count;


            for (int row = 2; row <= rowCount; row++)
            {
                string cardNumber = string.Empty;

                for (int col = 1; col <= columnCount; col++)
                {
                    cardNumber = "";
                    var cellValue = (range.Cells[row, col] as Microsoft.Office.Interop.Excel.Range).Value2;

                    switch (col)
                    {

                        case 2: // Card Number
                            cardNumber = cellValue.ToString();
                            break;

                            // Add more cases for additional columns if needed
                    }
                    string vv = Repo.CheckDuplicate(cardNumber);
                    if (vv != "")
                    {
                        returnDuplicatecards += cardNumber + "-" + Repo.CheckDuplicate(cardNumber) + ", ";
                    }
                    // Process other columns as needed
                }

            }
            if (returnDuplicatecards != "")
            {
                resDuplicateCardModel.Result = returnDuplicatecards;
                resDuplicateCardModel.Status = "D";
                return resDuplicateCardModel;
            }

            Employee employeeResModel = new Employee();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Assuming you have a table named 'Employee' with columns 'Name', 'CardNumber', 'Department', 'PhoneNo', and 'EmailId'
                string insertQuery = "INSERT INTO Employees (Name, CardNo, department, phoneNo, EmailId,CompanyId,Role,BranchId,DeviceID,DeviceIDs,address,CreatedBy,CreatedDate,Status,BiometricId) " +
                                     "VALUES (@Name, @CardNo, @Department, @PhoneNo, @EmailId,@CompanyId,@Role,@BranchId,@DeviceID,@DeviceIDs,'','','','1','1')";

                for (int row = 2; row <= rowCount; row++)
                {
                    string name = string.Empty;
                    string cardNumber = string.Empty;
                    string department = string.Empty;
                    string phoneNo = string.Empty;
                    string emailId = string.Empty;

                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = (range.Cells[row, col] as Microsoft.Office.Interop.Excel.Range).Value2;

                        switch (col)
                        {
                            case 1: // Employee Name
                                name = cellValue.ToString();
                                break;
                            case 2: // Card Number
                                cardNumber = cellValue.ToString();
                                break;
                            case 3: // Department
                                department = cellValue.ToString();
                                break;
                            case 4: // Phone No
                                phoneNo = cellValue.ToString();
                                break;
                            case 5: // Email Id
                                emailId = cellValue.ToString();
                                break;
                                // Add more cases for additional columns if needed
                        }

                        // Process other columns as needed
                    }


                    PrivilegeRequestModel privilegeRequestModel = new PrivilegeRequestModel();
                    privilegeRequestModel.FCardNo = uint.Parse(cardNumber.ToString());
                    privilegeRequestModel.FBeginYMD = DateTime.Parse("2023-05-01");
                    privilegeRequestModel.FEndYMD = DateTime.Parse("2099-05-01");
                    privilegeRequestModel.DoorName = "";
                    privilegeRequestModel.FPIN = "0";
                    privilegeRequestModel.FControlSegID1 = 0;
                    privilegeRequestModel.FControlSegID2 = 0;
                    privilegeRequestModel.FControlSegID3 = 0;
                    privilegeRequestModel.FControlSegID4 = 0;
                    string[] values = model.DeviceIDs.Split(',');

                    foreach (string value in values)
                    {
                        string Acc1 = "No", Acc2 = "No", Acc3 = "No", Acc4 = "No";
                        string[] parts = value.Split('-');

                        //foreach (string part in parts)
                        //{
                        //    Console.WriteLine(part);
                        //}
                        string ReaderNo = parts[0].ToString().Replace("\"", "").Replace("\\", "").Replace("[", "").Replace("]", "");
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

                        privilegeRequestModel.ControllerSN = uploadEmp.GetControllerID(parts[1].ToString().Replace("\"", "").Replace("\\", "").Replace("[", "").Replace("]", ""));
                        if (Acc1 == "1") { privilegeRequestModel.FControlSegID1 = 1; } else { privilegeRequestModel.FControlSegID1 = (byte)getaccessdata(1, _connectionString, cardNumber.ToString(), privilegeRequestModel.ControllerSN); }
                        if (Acc2 == "1") { privilegeRequestModel.FControlSegID2 = 1; } else { privilegeRequestModel.FControlSegID2 = (byte)getaccessdata(2, _connectionString, cardNumber.ToString(), privilegeRequestModel.ControllerSN); }
                        if (Acc3 == "1") { privilegeRequestModel.FControlSegID3 = 1; } else { privilegeRequestModel.FControlSegID3 = (byte)getaccessdata(3, _connectionString, cardNumber.ToString(), privilegeRequestModel.ControllerSN); }
                        if (Acc4 == "1") { privilegeRequestModel.FControlSegID4 = 1; } else { privilegeRequestModel.FControlSegID4 = (byte)getaccessdata(4, _connectionString, cardNumber.ToString(), privilegeRequestModel.ControllerSN); }
                        uploadEmp.AddPrivilege(privilegeRequestModel);
                    }




                    // Insert the values into the table
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Department", department);
                        command.Parameters.AddWithValue("@PhoneNo", phoneNo);
                        command.Parameters.AddWithValue("@EmailId", emailId);
                        command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                        command.Parameters.AddWithValue("@CardNo", cardNumber);
                        command.Parameters.AddWithValue("@Role", model.Role);
                        command.Parameters.AddWithValue("@BranchId", model.BranchId);
                        command.Parameters.AddWithValue("@DeviceID", model.DeviceID);
                        command.Parameters.AddWithValue("@DeviceIDs", model.DeviceIDs);
                        command.ExecuteNonQuery();
                    }
                }
            }

            //// Clean up Excel objects
            workbook.Close();
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);




            //// Delete the temporary file
            System.IO.File.Delete(filePath);

            resDuplicateCardModel.Result = "File uploaded successfully.";
            resDuplicateCardModel.Status = "Y";
            return resDuplicateCardModel;


        }
        private static int getaccessdata(int i, string connects, string cardNo, int SN)
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
        private bool IsExcelFile(string fileName)
        {
            // Check if the file has a valid Excel extension
            var validExtensions = new[] { ".xls", ".xlsx" };
            var extension = Path.GetExtension(fileName);
            return validExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private void getss(ReqAddPrivilegeToUser requestModel)
        {

            ResAddPrivilege resAddPrivilege = new ResAddPrivilege();
            uploadEmp uploadEmp = new uploadEmp();
            try
            {

                int[] values = requestModel.employeeid;
                foreach (int value in values)
                {
                    ReqAddPrivilegeUser addPrivilegeUser = new ReqAddPrivilegeUser();
                    addPrivilegeUser = uploadEmp.GetCardNoID(value);
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
                            if (t == 1)
                            {
                                if (ReaderNo == "1")
                                {

                                    privilegeRequestModel.FControlSegID1 = 1;
                                }
                                else
                                {
                                    privilegeRequestModel.FControlSegID1 = (byte)uploadEmp.getaccessdata(t, _connectionString, addPrivilegeUser.CardNo.ToString());
                                }
                            }
                            if (t == 2)
                            {
                                if (ReaderNo == "2")
                                {
                                    privilegeRequestModel.FControlSegID2 = 1;
                                }
                                else
                                {
                                    privilegeRequestModel.FControlSegID2 = (byte)uploadEmp.getaccessdata(t, _connectionString, addPrivilegeUser.CardNo.ToString());
                                }
                            }
                            if (t == 3)
                            {
                                if (ReaderNo == "3")
                                {
                                    privilegeRequestModel.FControlSegID3 = 1;
                                }
                                else
                                {
                                    privilegeRequestModel.FControlSegID3 = (byte)uploadEmp.getaccessdata(t, _connectionString, addPrivilegeUser.CardNo.ToString());
                                }
                            }
                            if (t == 4)
                            {
                                if (ReaderNo == "4")
                                {
                                    privilegeRequestModel.FControlSegID4 = 1;
                                }
                                else
                                {
                                    privilegeRequestModel.FControlSegID4 = (byte)uploadEmp.getaccessdata(t, _connectionString, addPrivilegeUser.CardNo.ToString());
                                }
                            }
                        }
                        else
                        {

                            if (ReaderNo == "1")
                            {

                                privilegeRequestModel.FControlSegID1 = 0;

                            }
                            if (ReaderNo == "2")
                            {

                                privilegeRequestModel.FControlSegID2 = 0;

                            }
                            if (ReaderNo == "3")
                            {

                                privilegeRequestModel.FControlSegID3 = 0;

                            }
                            if (ReaderNo == "4")
                            {

                                privilegeRequestModel.FControlSegID4 = 0;

                            }

                        }
                        privilegeRequestModel.ControllerSN = uploadEmp.GetControllerID(parts[1].ToString());

                    }

                    uploadEmp.AddPrivilege1(privilegeRequestModel);
                }

                resAddPrivilege.Result = "Privilege data received and saved successfully.";
                // Return a success response

            }
            catch (Exception ex)
            {
                resAddPrivilege.Result = "An error occurred while processing and saving the privilege data.";
                // Return an error response

            }



        }

    }
}
