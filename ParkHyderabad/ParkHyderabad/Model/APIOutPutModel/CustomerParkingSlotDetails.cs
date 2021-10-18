using System;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class CustomerParkingSlotDetails
    {
        public int CustomerParkingSlotID { get; set; }
        public int ApplicationTypeID { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public int ParkingBayID { get; set; }
        public string ParkingBayRange { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RegistrationNumber { get; set; }
        public string PaidAmount { get; set; }
        public string PriceAmount { get; set; }
        public string PaidDueAmount { get; set; }
        public string Duration { get; set; }
        public string PaymentTypeName { get; set; }
        public string ActualEndTime { get; set; }
        public int VehicleTypeID { get; set; }
        public int CustomerID { get; set; }
        public int CustomerVehicleID { get; set; }
        public string VehicleImage { get; set; }
        public decimal Price { get; set; }
        public bool IsWarning { get; set; }
        public int ViolationWarningCount { get; set; }
        public bool IsClamp { get; set; }
        public string ClampReason { get; set; }
        public decimal ClampFee { get; set; }
        public decimal DueAmount { get; set; }
        public string SupervisorName { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public byte[] CustomerProfileImage { get; set; }
        public DateTime LotOpenTime { get; set; }
        public DateTime LotCloseTime { get; set; }
        public string LotOpeningTime { get; set; }
        public string LotClosingTime { get; set; }

        // New Columns            
        public string ServiceTypeCode { get; set; }
        public string LotOpenCloseTime { get; set; }
        public string Address { get; set; }
        public string Model { get; set; }
        public string HourlyPrice { get; set; }
        public string Distance { get; set; }
        public bool TwoWheelerVisibility { get; set; }
        public bool FourWheelerVisibility { get; set; }
        public int TwoWheelerCount { get; set; }
        public int FourWheelerCount { get; set; }

        public string LotImageName { get; set; }
        public string LotImageName2 { get; set; }
        public string LotImageName3 { get; set; }

        public string DisabledParking { get; set; }
        public string CoveredParking { get; set; }
        public string Mechanic { get; set; }
        public string EvCharging { get; set; }
        public string BikeWash { get; set; }
        public string CarWash { get; set; }
    }
}
