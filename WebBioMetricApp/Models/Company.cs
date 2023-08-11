namespace WebBioMetricApp.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Address1 { get; set; }
        //public string Address2 { get; set; }
        //public string City { get; set; }
        public string Pincode { get; set; }
        public int Status { get; set; }
        public int flag { get; set; }
    }
    public class CompanyControls
    {
        public int CompanyID { get; set; }
       
        public int flag { get; set; }
    }
    public class BranchControls
    {
        public int BranchID { get; set; }

        public int flag { get; set; }
    }
    public class Branch
    {
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class CompanyModelRes
    {
        public int DeviceID { get; set; }
        public string Result { get; set; }

    }
    public class CompanyControlRes
    {
        public string Result { get; set; }

    }
    public class RoleModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
    }
    public class RoleModelRes
    {
        public int RoleID { get; set; }
        public string Result { get; set; }
    }
}
