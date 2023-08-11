namespace WebBioMetricApp.Models
{
    public class Device
    {
        public int DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DeviceLocation { get; set; }
        public string DeviceSeries { get; set; }
    }
}
