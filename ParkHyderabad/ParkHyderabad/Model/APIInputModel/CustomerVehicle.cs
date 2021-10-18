using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkHyderabad.Model.APIInputModel
{
    public class CustomerVehicle
    {
        public int CustomerVehicleMapperID { get; set; }
        public int CustomerVehicleID { get; set; }
        public int CustomerID { get; set; }
        public int LocationID { get; set; }
        public int LocationParkingLotID { get; set; }
        public string VehicleTypeCode { get; set; }
        public int VehicleTypeID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsPrimaryVehicle { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}