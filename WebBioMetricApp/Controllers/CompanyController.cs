using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebBioMetricApp.Models;

namespace WebBioMetricApp.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("ListCompanies")]
        public IActionResult GetCompanies(CompanyControls param )
        {
            try
            {
                List<Company> companies = new List<Company>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM Companies";string x = "";
                    if (param.flag == 1)
                    {
                        x = " where Status = 1";
                    }
                    else if (param.flag == 0)
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
                                Company company = new Company
                                {
                                    CompanyID = Convert.ToInt32(reader["CompanyID"]),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    Address1 = reader["Address1"].ToString(),
                                    //Address2 = reader["Address2"].ToString(),
                                    Status = int.Parse(reader["Status"].ToString()),
                                    Pincode = reader["Pincode"].ToString()
                                };
                                companies.Add(company);
                            }
                        }
                    }
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateCompany")]
        public IActionResult CreateCompany(Company company)
        {
            try
            {
                CompanyModelRes CompanyModelResModelRes = new CompanyModelRes();
                
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("InsertOrUpdateCompany", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@CompanyID", company.CompanyID); // Assuming CompanyID is present
                        command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                        command.Parameters.AddWithValue("@CreatedBy", company.CreatedBy);
                        command.Parameters.AddWithValue("@Address1", company.Address1);
                        command.Parameters.AddWithValue("@Pincode", company.Pincode);
                        command.Parameters.AddWithValue("@Status", company.Status);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CompanyModelResModelRes.Result = reader["Result"].ToString();
                                CompanyModelResModelRes.DeviceID = int.Parse(reader["CnewId"].ToString());
                            }
                        }
                    }
                }
                return Ok(CompanyModelResModelRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    
        [HttpPost("Controls")]
        public IActionResult disableCompany(CompanyControls param)
        {
            try
            {
                CompanyControlRes companyControlRes = new CompanyControlRes();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string query = "update Companies set Status=@flag WHERE CompanyID = @CompanyID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyID", param.CompanyID);
                        command.Parameters.AddWithValue("@flag", param.flag);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            companyControlRes.Result = "Company not found";
                            return NotFound(companyControlRes);
                        }
                    }
                }
                string stratus = "";
                if (param.flag == 1)
                {
                    companyControlRes.Result = "Company enabled successfully";

                }
                else
                {
                    companyControlRes.Result = "Company disabled successfully";
                }
                return Ok(companyControlRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("CreateBranch")]
        public IActionResult CreateBranch(Branch branch)
        {
            try
            {
                EmployeeModelRes employeeModelRes = new EmployeeModelRes();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("InsertOrUpdateBranch", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@BranchID", branch.BranchID);
                        command.Parameters.AddWithValue("@CompanyID", branch.CompanyID);
                        command.Parameters.AddWithValue("@BranchName", branch.BranchName);
                        command.Parameters.AddWithValue("@Address", branch.Address);
                        command.Parameters.AddWithValue("@Pincode", branch.Pincode);
                        command.Parameters.AddWithValue("@Status", branch.Status);
                        command.Parameters.AddWithValue("@CreatedBy", branch.CreatedBy);

                        connection.Open();
                        //command.ExecuteNonQuery();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employeeModelRes.Result = reader["Result"].ToString();
                                employeeModelRes.DeviceID = int.Parse(reader["CnewId"].ToString());
                            }
                        }
                    }
                }

                return Ok(employeeModelRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("ListBranch")]
        public IActionResult GetBranch(CompanyControls param)
        {
            try
            {
                List<Branch> Listbranch = new List<Branch>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT * FROM tbl_Branch"; string x = "";
               


                    if (param.flag == 1)
                    {
                        x = " where Status = 1";
                    }
                    else if (param.flag == 0)
                    {
                        x = " where Status = 0";
                    }
                    if (param.CompanyID != 0) {
                        if (x != "")
                        {
                            x = " where CompanyID = '"+ param.CompanyID+ "'";
                        }
                        else
                        {
                            x = " AND CompanyID = '"+ param.CompanyID+ "'";
                        }
                    }
                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Branch branch = new Branch
                                {
                                    BranchID = Convert.ToInt32(reader["BranchID"]),
                                    CompanyID = Convert.ToInt32(reader["CompanyID"]),
                                    BranchName = reader["BranchName"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    Status = int.Parse(reader["Status"].ToString()),
                                    Pincode = reader["Pincode"].ToString()
                                };
                                Listbranch.Add(branch);
                            }
                        }
                    }
                }
                return Ok(Listbranch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("BranchControls")]
        public IActionResult disableBranch(BranchControls param)
        {
            try
            {
                CompanyControlRes companyControlRes = new CompanyControlRes();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string query = "update tbl_Branch set Status=@flag WHERE BranchID = @BranchID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BranchID", param.BranchID);
                        command.Parameters.AddWithValue("@flag", param.flag);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            companyControlRes.Result = "Branch not found";
                            return NotFound(companyControlRes);
                        }
                    }
                }
                string stratus = "";
                if (param.flag == 1)
                {
                    companyControlRes.Result  = "Branch enabled successfully";
                }
                else
                {
                    companyControlRes.Result = "Branch disabled successfully";
                }
                return Ok(companyControlRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("CreateRole")]
        public IActionResult CreateRole(RoleModel rolemodel)
        {
            try
            {
                RoleModelRes roleModelRes = new RoleModelRes();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("InsertOrUpdateRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@RoleID", rolemodel.RoleID);
                        command.Parameters.AddWithValue("@RoleName", rolemodel.RoleName);
                        command.Parameters.AddWithValue("@RoleDescription", rolemodel.RoleDescription);
                        command.Parameters.AddWithValue("@CompanyID", rolemodel.CompanyID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roleModelRes.Result = reader["Result"].ToString();
                                roleModelRes.RoleID = int.Parse(reader["CnewId"].ToString());
                            }
                        }
                    }
                }

                return Ok(roleModelRes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ListRoles")]
        public IActionResult ListRoles()
        {
            try
            {
                List<RoleModel> rolemodel = new List<RoleModel>();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = " SELECT RoleID,RoleName,RoleDescription,a.CompanyID,isnull(CompanyName,'') CompanyName FROM tbl_Roles a left join Companies b on a.CompanyID=b.CompanyID ;"; string x = "";

                    query = query + x;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoleModel roleModel = new RoleModel
                                {
                                    RoleID = Convert.ToInt32(reader["RoleID"]),
                                    RoleName = reader["RoleName"].ToString(),
                                    RoleDescription = reader["RoleDescription"].ToString(),
                                    CompanyID = int.Parse(reader["CompanyID"].ToString()),
                                    CompanyName = reader["CompanyName"].ToString()
                                };
                                rolemodel.Add(roleModel);
                            }
                        }
                    }
                }
                return Ok(rolemodel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
