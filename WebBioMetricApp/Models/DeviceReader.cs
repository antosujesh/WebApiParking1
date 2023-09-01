namespace WebApiParking.Models
{
    public class DeviceReader
    {
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
     
    }
}
