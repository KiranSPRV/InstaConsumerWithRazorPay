using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkHyderabad.Model.APIInputModel
{
    public class OTPVerification
    {       
        public int OTPVerificationID { get; set; }
        public int CustomerID { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] ProfileImage { get; set; }
        public string OTP { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int Status{ get; set; }
        public int CreatedBy { get; set; }
        public string DeviceID { get; set; }
    }
}
