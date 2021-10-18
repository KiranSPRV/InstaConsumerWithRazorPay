using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabad.Model.APIOutPutModel;

namespace ParkHyderabad.Model
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
        public string Country { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string BikeWash { get; set; }
        public string CarWash { get; set; }
        public string Mechanic { get; set; }
        public string Petrol { get; set; }
        public string DisabledParking { get; set; }
        public string CoveredParking { get; set; }
        public List<LocationParkingLotService> lstServices { get; set; }
    }
}

