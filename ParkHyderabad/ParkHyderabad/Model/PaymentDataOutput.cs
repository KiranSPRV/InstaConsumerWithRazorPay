using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabad.Model
{
    public class PaymentDataOutput
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
        public string UserContact { get; set; }
        public string UserEmail { get; set; }
    }
}
