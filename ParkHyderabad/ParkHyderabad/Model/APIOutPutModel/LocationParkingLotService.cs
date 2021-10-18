using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class LocationParkingLotService
    {
        public int ParkingLotServiceMapperID { get; set; }
        public int LocationParkingLotID { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceTypeImage { get; set; }
        public string IconType { get; set; }
        public string IconName { get; set; }
    }
}