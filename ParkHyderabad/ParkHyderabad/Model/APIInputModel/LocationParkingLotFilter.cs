namespace ParkHyderabad.Model.APIInputModel
{
    public class LocationParkingLotFilter
    {
        public int CustomerID { get; set; }
        public int LocationParkingLotID { get; set; }
        public string TwoWheelerTypeCode { get; set; }
        public string FourWheelerTypeCode { get; set; }
        public string Distance { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal CLatitude { get; set; }
        public decimal CLongitude { get; set; }
        public string CoveredParking { get; set; }
        public string Handicapped { get; set; }
        public string EvCharging { get; set; }
        public string Mechanic { get; set; }
        public string CarWash { get; set; }
        public string BikeWash { get; set; }

        public string LocationName { get; set; }
        public decimal SelectedLatitude { get; set; }
        public decimal SelectedLongitude { get; set; }
    }
}
