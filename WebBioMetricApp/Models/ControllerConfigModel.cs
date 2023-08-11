using Newtonsoft.Json;

namespace WebBioMetricApp.Models
{
    public class ControllerConfigSetModel
    {
       
        public string StrSN { get; set; }
        public string StrMac { get; set; }
        public string StrIP { get; set; }
        public string StrMask { get; set; }
        public string StrGateway { get; set; }
        public string StrTCPPort { get; set; }
        public string PcIPAddr { get; set; }
  
    }
    public class ControllerConfigModel
    {
        public int DeviceID { get; set; }
        public string StrSN { get; set; }
        public string StrMac { get; set; }
        public string StrIP { get; set; }
        public string StrMask { get; set; }
        public string StrGateway { get; set; }
        public string StrTCPPort { get; set; }
        public string DeviceLocation { get; set; }
        public string CompanyID { get; set; }
        public string PcIPAddr { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdatedDate { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string BranchID { get; set; }

    }
    public class PrivilegeRequestModel
    {
        public uint FCardNo { get; set; }
        public DateTime FBeginYMD { get; set; }
        public DateTime FEndYMD { get; set; }
        public string FPIN { get; set; }
        public byte FControlSegID1 { get; set; }
        public byte FControlSegID2 { get; set; }
        public byte FControlSegID3 { get; set; }
        public byte FControlSegID4 { get; set; }
        public int ControllerSN { get; set; }
        public string IP { get; set; }
        public int PORT { get; set; }
        public string DoorName { get; set; }
    }
    public class ControllerConfigModelRes
    {
        public string DeviceID { get; set; }
        public string Result { get; set; }

    }

    public class ControllerListModelResq
    {
        public int SearchID { get; set; }

    }
    public class ResAddPrivilege
    {
        public string Result { get; set; }

    }
    public class ReqAddPrivilegeToUser
    {
        public int[] employeeid { get; set; }
        public string[] reader { get; set; }
        public string flag { get; set; }

    }

    public class ReqAddPrivilegeUser
    {
        public int CardNo { get; set; }
        public string DeviceIDs { get; set; }

    }
    public class FilterModel
    {
        public int SearchID { get; set; }

    }
    public class ControllerParaModel
    {
        public int StrSN { get; set; }
        public string StrIP { get; set; }
        public int StrTCPPort { get; set; }
        public int StrType { get; set; }

        public List<ControllerParaDoorModel> ParaDoor { get; set; }
    }
    public class ControllerParaDoorModel
    {
        public int StrSN { get; set; }
        public int DoorNo { get; set; }
        public int ReaderNo { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        
    }
    public class ReqControllerSetDateTimeModel
    {
        public int StrSN { get; set; }
    }
    public class ReSControllerSetDateTimeModel
    {
        public string Result { get; set; }
        public string Status { get; set; }
    }
    public class ReqControllerGetPermissionListModel
    {
        public int[] StrSN { get; set; }
    }
    public class ReqControllerGetAccessListModel
    {
        public int EmployeeID { get; set; }
    }
    public class ControllerGetPermissionListModel
    {
        public int StrSN { get; set; }
        
        public int DoorNo { get; set; }
        public int ReaderNo { get; set; }
        public string Location { get; set; }

    }

    public class ControllerGetAccessListModel
    {
        public string Location { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? f_ControlSegID { get; set; }


        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public int? f_ControlSegID2 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public int? f_ControlSegID3 { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public int? f_ControlSegID4 { get; set; }

    }
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
        public int CompanyId { get; set; }

        public string Role { get; set; }

        public int BranchId { get; set; }

        //public string CardNo { get; set; }

        public string DeviceID { get; set; }

        public string DeviceIDs { get; set; }
    }
}
