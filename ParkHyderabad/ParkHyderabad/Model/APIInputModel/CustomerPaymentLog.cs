using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabad.Model.APIInputModel
{
    public class CustomerPaymentLog
    {
        public int CustomerPaymentLogID { get; set; }
        public int CustomerID { get; set; }
        public int CustomerVehicleID { get; set; }
        public int CustomerParkingSlotID { get; set; }
        public int CustomerVehiclePassID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal PaidAmount { get; set; }
        public string TransactionID { get; set; }
        public string Comments { get; set; }
        public string ActualStartTime { get; set; }
        public string Duration { get; set; }
        public int ParkingBayID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsPassPayment { get; set; }
        public bool IsCheckInPayment { get; set; }
        public bool IsPyemntScreenShowed { get; set; }
        public int LocationParkingLotID { get; set; }
        public int LocationID { get; set; }
        public int PassTypeID { get; set; }
        public int PassPriceID { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleType { get; set; }

        public string PassLocationID { get; set; }
        public string PassLocations { get; set; }
        public bool IsMultiLot { get; set; }
        public int PassDuration { get; set; }
        public int DueCustomerParkingSlotID { get; set; }
        public Decimal PaidDueAmount { get; set; }
    }
}
