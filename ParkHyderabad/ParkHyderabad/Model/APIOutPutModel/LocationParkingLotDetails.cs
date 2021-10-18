using System;
using System.Collections.Generic;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class LocationParkingLotDetails
    {
        public string FilterView { get; set; }
        public int LocationParkingLotID { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationParkingLotName { get; set; }
        public string LotVehicleAvailabilityCode { get; set; }
        public bool TwoWheelerVisibility { get; set; }
        public bool FourWheelerVisibility { get; set; }
        public int TwoWheelerCount { get; set; }
        public int FourWheelerCount { get; set; }
        //public DateTime LotCloseTime { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
        public List<LocationParkingLotService> LocationParkingLotService { get; set; }
        public string Address { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
        public int VehicleTypeID { get; set; }
        public string LotImageName { get; set; }
        public string LotImageName2 { get; set; }
        public string LotImageName3 { get; set; }
        public string ServiceTypeName { get; set; }
        public string Distance { get; set; }
        public string DayOfWeek { get; set; }
        public string DisplayTiming { get; set; }
        public List<string> ListDisplayTiming { get; set; }
        public string BikePrice { get; set; }
        public string CarPrice { get; set; }
        public string CapacityColour { get; set; }
        public string WeekDay { get; set; }
        public string WeekEnd { get; set; }

        // New Columns

        public string DisabledParking { get; set; }
        public string CoveredParking { get; set; }
        public string Mechanic { get; set; }
        public string EvCharging { get; set; }
        public string BikeWash { get; set; }
        public string CarWash { get; set; }
        public bool CustomerExists { get; set; }
        public bool CustomerNotExists { get; set; }
    }
}