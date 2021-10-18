using System;

namespace ParkHyderabad.Model.APIInputModel
{
    public class CustomerParkingSlot
    {
        public int CustomerParkingSlotID { get; set; }
        public int CustomerID { get; set; }
        public int VehicleTypeID { get; set; }
        public int PaymentTypeID { get; set; }
        public int LocationParkingLotID { get; set; }
        public string LocationParkingLotName { get; set; }
        public string LocationName { get; set; }
        public int ParentCustomerParkingSlotID { get; set; }
        public string CurrentTime { get; set; }
        public string ExpectedStartTime { get; set; }
        public string ExpectedEndTime { get; set; }
        public string ActualStartTime { get; set; }
        public string ActualEndTime { get; set; }
        public string Duration { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public int StatusID { get; set; }
        public int CustomerVehicleID { get; set; }
        public int ApplicationTypeID { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ExtendAmount { get; set; }
        public int ParkingBayID { get; set; }
        public decimal PaidDueAmount { get; set; }
        public bool IsDueAmountPaid { get; set; }
        public string DueAmountPaidOn { get; set; }
        public int DueCustomerParkingSlotID { get; set; }
        public decimal DueAmount { get; set; }
    }
}