using System;
using System.Collections.Generic;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class OCustomerVehiclePass
    {
        public int CustomerVehiclePassID { get; set; }
        public int CustomerVehicleID { get; set; }
        public string RegistrationNumber { get; set; }
        public string Model { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleImage { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string StartDateTime { get; set; }
        public string ExpiryDateTime { get; set; }
        public string Amount { get; set; }
        public string CardAmount { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentTypeName { get; set; }
        public string PassTypeName { get; set; }
        public string PassPrice { get; set; }
        public int PassPriceID { get; set; }
        public string PassName { get; set; }
        public string PassCode { get; set; }
        public string ValidUpTo { get; set; }
        public bool IsMultiLot { get; set; }
        public string SingleOrMulti { get; set; }
        public bool IssuedCard { get; set; }
        public string CardNumber { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string TagType { get; set; }
        public string PassExpiryColor { get; set; }
        public string PassExpiryMessage { get; set; }
        public byte[] CustomerProfileImage { get; set; }
        public List<Location> lstLocation { get; set; }
    }
}