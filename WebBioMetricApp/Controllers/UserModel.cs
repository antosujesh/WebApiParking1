namespace WebApiParking.Controllers
{
    public class UserModel
    {
        public int Id { get; set; }     
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        

        public string FirstName { get; set; }   
        public string LastName { get; set; } 
        
        public bool Status { get; set; }    
        public bool IsReset { get; set; }

        public string CreatedBy { get; set; } 
        public string UpdatedBy { get; set; }   
        public DateTime CreatedDate  { get; set;}
        public DateTime UpdatedDate { get; set;}



         
        
    }
}