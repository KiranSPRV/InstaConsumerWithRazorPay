using System;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class OCustomer
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int YearOfBirth { get; set; }
        public int Age { get; set; }    
        public byte[] ProfileImage { get; set; }
    }
}
