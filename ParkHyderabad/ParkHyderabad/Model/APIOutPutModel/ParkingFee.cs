namespace ParkHyderabad.Model.APIOutPutModel
{
    public class ParkingFee
    {
        public decimal Price { get; set; }
        public int Hours { get; set; }
        public string HourlyPrice { get; set; }
        public int VehicleTypeID { get; set; }
        public string LotOpenTime { get; set; }
        public string LotCloseTime { get; set; }
        public int CustomerParkingSlotID { get; set; }
        public bool IsWarning { get; set; }
        public bool IsFullDay { get; set; }
        public int ViolationWarningCount { get; set; }
        public bool IsClamp { get; set; }
        public string ClampReason { get; set; }
        public decimal ClampFee { get; set; }
        public decimal DueAmount { get; set; }
        public string ActualEndTime { get; set; }
        public string SupervisorName { get; set; }
        public string PhoneNumber { get; set; }
        public int StatusID { get; set; }
    }
}