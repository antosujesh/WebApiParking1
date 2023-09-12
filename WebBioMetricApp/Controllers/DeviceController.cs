using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebBioMetricApp.Models;
namespace WebBioMetricApp.Controllers
{
    //[ApiController]
    //[Route("api/devices")]
    //public class DeviceController : ControllerBase
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly string _connectionString;

    //    public DeviceController(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    //    }

    //    // GET: api/devices
    //    [HttpGet]
    //    public IActionResult GetDevices()
    //    {
    //        try
    //        {
    //            List<Device> devices = new List<Device>();
    //            using (SqlConnection connection = new SqlConnection(_connectionString))
    //            {
    //                string query = "SELECT * FROM ControllerConfig";
    //                using (SqlCommand command = new SqlCommand(query, connection))
    //                {
    //                    connection.Open();
    //                    using (SqlDataReader reader = command.ExecuteReader())
    //                    {
    //                        while (reader.Read())
    //                        {
    //                            Device device = new Device
    //                            {
    //                                DeviceID = Convert.ToInt32(reader["DeviceID"]),
    //                                DeviceName = reader["DeviceName"].ToString(),
    //                                CreatedBy = reader["CreatedBy"].ToString(),
    //                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
    //                                DeviceLocation = reader["DeviceLocation"].ToString(),
    //                                DeviceSeries = reader["DeviceSeries"].ToString()
    //                            };
    //                            devices.Add(device);
    //                        }
    //                    }
    //                }
    //            }
    //            return Ok(devices);
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, $"Internal server error: {ex.Message}");
    //        }
    //    }

    //    // POST: api/devices
    //    [HttpPost]
    //    public IActionResult CreateDevice(Device device)
    //    {
    //        try
    //        {
    //            using (SqlConnection connection = new SqlConnection(_connectionString))
    //            {
    //                string query = "INSERT INTO Devices (DeviceID,DeviceName, CreatedBy, CreatedDate, DeviceLocation, DeviceSeries) " +
    //                               "VALUES (@DeviceID,@DeviceName, @CreatedBy, @CreatedDate, @DeviceLocation, @DeviceSeries)";
    //                using (SqlCommand command = new SqlCommand(query, connection))
    //                {
    //                    command.Parameters.AddWithValue("@DeviceID", device.DeviceID);
    //                    command.Parameters.AddWithValue("@DeviceName", device.DeviceName);
    //                    command.Parameters.AddWithValue("@CreatedBy", device.CreatedBy);
    //                    command.Parameters.AddWithValue("@CreatedDate", device.CreatedDate);
    //                    command.Parameters.AddWithValue("@DeviceLocation", device.DeviceLocation);
    //                    command.Parameters.AddWithValue("@DeviceSeries", device.DeviceSeries);

    //                    connection.Open();
    //                    command.ExecuteNonQuery();
    //                }
    //            }
    //            return Ok("Device created successfully");
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, $"Internal server error: {ex.Message}");
    //        }
    //    }

    //    // PUT: api/devices/{id}
    //    [HttpPut("{id}")]
    //    public IActionResult UpdateDevice(int id, Device device)
    //    {
    //        try
    //        {
    //            using (SqlConnection connection = new SqlConnection(_connectionString))
    //            {
    //                string query = "UPDATE Devices SET DeviceName = @DeviceName, CreatedBy = @CreatedBy, " +
    //                               "CreatedDate = @CreatedDate, DeviceLocation = @DeviceLocation, " +
    //                               "DeviceSeries = @DeviceSeries WHERE DeviceID = @DeviceID";
    //                using (SqlCommand command = new SqlCommand(query, connection))
    //                {
    //                    command.Parameters.AddWithValue("@DeviceName", device.DeviceName);
    //                    command.Parameters.AddWithValue("@CreatedBy", device.CreatedBy);
    //                    command.Parameters.AddWithValue("@CreatedDate", device.CreatedDate);
    //                    command.Parameters.AddWithValue("@DeviceLocation", device.DeviceLocation);
    //                    command.Parameters.AddWithValue("@DeviceSeries", device.DeviceSeries);
    //                    command.Parameters.AddWithValue("@DeviceID", id);

    //                    connection.Open();
    //                    int rowsAffected = command.ExecuteNonQuery();
    //                    if (rowsAffected == 0)
    //                    {
    //                        return NotFound("Device not found");
    //                    }
    //                }
    //            }
    //            return Ok("Device updated successfully");
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, $"Internal server error: {ex.Message}");
    //        }
    //    }

    //    // DELETE: api/devices/{id}
    //    [HttpDelete("{id}")]
    //    public IActionResult DeleteDevice(int id)
    //    {
    //        try
    //        {
    //            using (SqlConnection connection = new SqlConnection(_connectionString))
    //            {
    //                string query = "DELETE FROM Devices WHERE DeviceID = @DeviceID";
    //                using (SqlCommand command = new SqlCommand(query, connection))
    //                {
    //                    command.Parameters.AddWithValue("@DeviceID", id);

    //                    connection.Open();
    //                    int rowsAffected = command.ExecuteNonQuery();
    //                    if (rowsAffected == 0)
    //                    {
    //                        return NotFound("Device not found");
    //                    }
    //                }
    //            }
    //            return Ok("Device deleted successfully");
    //        }
    //        catch (Exception ex)
    //        {
    //            return StatusCode(500, $"Internal server error: {ex.Message}");
    //        }
    //    }
    //}
}
