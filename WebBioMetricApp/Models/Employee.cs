using Newtonsoft.Json;

namespace WebBioMetricApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int BiometricId { get; set; }
        public int CardNo { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public int CompanyId { get; set; }
        public string? Address { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Status { get; set; }
        public string?[] DeviceID { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string?[] DeviceIDs { get; set; }
        public int BranchID { get; set; }

        public string? CompanyName { get; set; }

        public string? BranchName { get; set; }
    }
    public class Employee_Act
    {
        public int EmployeeId { get; set; }

    }
    public class EmployeeModelRes
    {
        public int DeviceID { get; set; }
        public string Result { get; set; }

    }
    public class EmployeeResModel
    {
        public string Result { get; set; }

    }

    public class ResDuplicateCardModel
    {
        public string Status { get; set; }
        public string Result { get; set; }

    }

}
