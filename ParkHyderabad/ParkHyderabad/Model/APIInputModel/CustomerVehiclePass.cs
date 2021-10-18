using System;
using System.Collections.Generic;
using ParkHyderabad.Model.APIOutPutModel;

namespace ParkHyderabad.Model.APIInputModel
{
    public class CustomerVehiclePass
    {
        public int CustomerVehiclePassID { get; set; }
        public int CustomerVehicleID { get; set; }
        public int CustomerID { get; set; }
        public int PaymentTypeID { get; set; }
        public bool IsMultiLot { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IssuedCard { get; set; }
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal CardAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionID { get; set; }
        public int StatusID { get; set; }
        public int PassTypeID { get; set; }
        public string StationAccess { get; set; }
        public int PassPriceID { get; set; }
        public int LocationID { get; set; }
        public List<Location> lstLocation { get; set; }
        public decimal PaidDueAmount { get; set; }
        public bool IsDueAmountPaid { get; set; }
        public string DueAmountPaidOn { get; set; }
        public int DueCustomerParkingSlotID { get; set; }
    }
}
