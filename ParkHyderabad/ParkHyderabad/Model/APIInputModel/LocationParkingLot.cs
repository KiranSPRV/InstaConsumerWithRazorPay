namespace ParkHyderabad.Model.APIInputModel
{
    public class LocationParkingLot
    {       
        public int LocationParkingLotID { get; set; }
        public int LocationID { get; set; }
        public int LocationName { get; set; }
        public string LocationParkingLotName { get; set; }
        public string Address { get; set; }
        public string CapacityColour { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
        public int VehicleTypeID { get; set; }
    }
}