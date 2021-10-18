using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabad.Model.APIInputModel
{
    public class ParkingDetails
    {
        public int CustomerParkingSlotID { get; set; }
        public string ActualEndTime { get; set; }
        public string CurrentTime { get; set; }
        public int LocationParkingLotID { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeCode { get; set; }
        public int ParkingHours { get; set; }
        public decimal Amount { get; set; }
    }
}