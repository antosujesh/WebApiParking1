using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApiParking.Helper;

namespace WebApiParking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        // GET: UserController/Details/5
        [HttpGet("GetUser")]
        public ActionResult Details(int id)
        {
            DataAccessDAO dataAccess = new(_configuration);

            DataTable dtUsers = dataAccess.GetData(string.Format("Select * from tbl_UserMaster where Userid= {0}", id));

            return Ok(Utility.DataTableToJSON(dtUsers));
        }

        [HttpPost("CreateUser")]
        public ActionResult Create(UserModel user)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    string query = "INSERT INTO tbl_UserMaster (UserName, Password, FirstName, LastName, PhoneNo, Email, Status, IsReset, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) " +
                                   "VALUES (@UserName, @Password, @FirstName, @LastName, @PhoneNo, @Email, @Status, @IsReset,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)";
                    var encryptedPassword = StringCipher.Encrypt(user.Name, user.Password);
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", user.Name);
                        command.Parameters.AddWithValue("@Password", encryptedPassword);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@PhoneNo", user.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Status", user.Status);
                        command.Parameters.AddWithValue("@IsReset", user.IsReset);
                        command.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                        command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);
                        command.Parameters.AddWithValue("@UpdatedBy", user.UpdatedBy);
                        command.Parameters.AddWithValue("@UpdatedDate", user.UpdatedDate == DateTime.MinValue ? DateTime.Today: user.UpdatedDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {

                return BadRequest();
            }
            return Ok();
        }

        [HttpGet("Login")]
        public ActionResult Login(string userName, string password)
        {
            DataAccessDAO dataAccess = new(_configuration);
            var encryptedPassword = StringCipher.Encrypt(userName, password);
            DataTable dtUsers = dataAccess.GetData(string.Format("Select userid from tbl_UserMaster where Username= '{0}' and Password = '{1}'", userName, encryptedPassword));

            if (dtUsers != null && dtUsers.Rows.Count > 0)
                return Ok("SUCCESS");

            return Ok("FAIL");
        }

        [HttpGet("CheckUserName")]
        public ActionResult CheckUserNameExists(string userName)
        {
            DataAccessDAO dataAccess = new(_configuration);

            DataTable dtUsers = dataAccess.GetData(string.Format("Select userid from tbl_UserMaster where Username= '{0}'", userName));

            if (dtUsers != null && dtUsers.Rows.Count > 0)
                return Ok("SUCCESS");

            return Ok("FAIL");
        }

    }
}
