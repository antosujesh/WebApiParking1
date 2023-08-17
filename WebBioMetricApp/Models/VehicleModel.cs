namespace WebApiParking.Models
{
    public class VehicleModel
    {
        public string ID { get; set; }
        public string DriverImg { get; set; }
        public string CarImg { get; set; }
        public string CarNo { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNo01 { get; set; }
        public string PhoneNo02 { get; set; }
        public string EmailID { get; set; }
        public string ResidentAddress { get; set; }
        public string KYCAddress { get; set; }
        public string SerialNumber { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public int Status { get; set; }
        public string[] DeviceID { get; set; }

    }
}
