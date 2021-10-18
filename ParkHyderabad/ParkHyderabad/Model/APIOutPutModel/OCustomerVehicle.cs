namespace ParkHyderabad.Model.APIOutPutModel
{
    public class OCustomerVehicle
    {
        public int CustomerVehicleMapperID { get; set; }
        public int CustomerVehicleID { get; set; }
        public int CustomerID { get; set; }
        public string VehicleTypeCode { get; set; }
        public int VehicleTypeID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationNumberType { get; set; }
        public bool IsPrimaryVehicle { get; set; }
        public bool IsActive { get; set; }
        public string VehicleImage { get; set; }
        public string CreatedBy { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int LocationID { get; set; }
        public string PrimaryVehicleImage { get; set; }
        public int ModelFontsize { get; set; }
 
    }
}