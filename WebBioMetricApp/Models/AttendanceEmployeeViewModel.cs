namespace WebBioMetricApp.Models
{
    public class AttendanceEmployeeViewModel
    {
        public int BiometricId { get; set; }
        public string DateTime { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
    }

    public class SwipeRecord
    {
        public string f_Index { get; set; }
        public string ReadDate { get; set; }
        public string CardNO { get; set; }
        public string DoorNO { get; set; }
        public string InOut { get; set; }
        public string ReaderNO { get; set; }
        public string ControllerSN { get; set; }
        public string f_EventCategory { get; set; }
        public string f_ReasonNo { get; set; }
        public string f_RecordAll { get; set; }

        
    }
    public class MISRestore
    {
        public int ISRestore { get; set; }
    }
    public class AttendanceRecord
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public string BranchName { get; set; }

        public string Role { get; set; }
        public string CardNo { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Intime { get; set; }
        public string OutTime { get; set; }
        public string LastTime { get; set; }
        public string InOut { get; set; }
        public string TotalMinute { get; set; }
        public string BreakHours { get; set; }

        public string CurrentStatus { get; set; }
    }
    public class AttendanceRecordRequest
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int?[] EmployeeId { get; set; }
    }
    public class MovementRecord
    {
        public string ReadTime { get; set; }
        public string CardNo { get; set; }
        public string DoorNo { get; set; }
        public string InOut { get; set; }
        public string ReaderNo { get; set; }
    }
    public class MovementReq
    {

        public string CardNo { get; set; }

    }
    
}
