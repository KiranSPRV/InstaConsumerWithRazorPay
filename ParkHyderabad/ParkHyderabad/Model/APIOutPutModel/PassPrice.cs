using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabad.Model.APIOutPutModel
{
   public class PassPrice
    {        
        public int PassPriceID { get; set; }        
        public string PassCode { get; set; }
        public string PassName { get; set; }
        public string StationAccess { get; set; }
        public string Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool NFCApplicable { get; set; }
        public int NFCCardPrice { get; set; }
        public int VehicleTypeID { get; set; }
        public int Price { get; set; }
        public string PriceDisplay { get; set; }
        public string PassDescription { get; set; }        
        public decimal TotalPassPrice { get; set; }
        public int PassTypeID { get; set; }
        public string PassTypeName { get; set; }


        //New Columns
        public int PassIndex { get; set; }
        public string PassBordorColour { get; set; }
        public string PassBackgroundColour { get; set; }
        public string PassTextColour { get; set; }
        public string PassTypeCode { get; set; }
        public int DueCustomerParkingSlotID { get; set; }
        public decimal DueAmount { get; set; }
    }
}
