using System;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class CustomerPaymentHistoryDetails
    {
        public string InvoiceType { get; set; }
        public int CustomerParkingSlotID { get; set; }
        public int ApplicationTypeID { get; set; }
        public int StatusID { get; set; }
        public string LocationParkingLotName { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string LocationLotName { get; set; }
        public int ParkingBayID { get; set; }
        public string ParkingBayRange { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RegistrationNumber { get; set; }
        public string PaidAmount { get; set; }
        public string TotalAmount { get; set; }
        public string Duration { get; set; }
        public string PaymentTypeName { get; set; }
        public int VehicleTypeID { get; set; }
        public int CustomerID { get; set; }
        public int CustomerVehicleID { get; set; }
        public string VehicleImage { get; set; }
        public string FromAndTo { get; set; }
        public string FromDisplay { get; set; }
        public string ToDisplay { get; set; }
        public string SingleOrMulti { get; set; }
        public string Name { get; set; }
        public int CustomerVehiclePassID { get; set; }
        public string PassTypeName { get; set; }
        public bool NFC { get; set; }
        public int PassPriceID { get; set; }
        public bool IsClamp { get; set; }
        public string ClampFee { get; set; }
        public string ViolationFee { get; set; }

        //19032021 Raj
        public bool PassFlag { get; set; }
        public string HistoryType { get; set; }
        public string CardTypeName { get; set; }
        public string PassValidity { get; set; }
        public bool PassValidityVisible { get; set; }
        public bool PaidSectionVisible { get; set; }
        public bool DueClampSectionVisible { get; set; }
        public string FeeDueClampAmount { get; set; }
        public bool IsMultiLot { get; set; }
        public int PrimaryLocationParkingLotID { get; set; }
        public string PaidDueAmount { get; set; }
        //19032021 Raj
    }
}
