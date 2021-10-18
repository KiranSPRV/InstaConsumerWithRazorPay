using System;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int CustomerParkingSlotID { get; set; }
        public int CustomerID { get; set; }
        public string DeviceID { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string ActualEndTime { get; set; }
        public int Minutes { get; set; }
        public string NotificationMessage { get; set; }
        public int NotificationCounter { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
    }
}