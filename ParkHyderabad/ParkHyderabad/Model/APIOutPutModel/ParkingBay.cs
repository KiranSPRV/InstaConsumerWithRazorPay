using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkHyderabad.Model.APIOutPutModel
{
    public class ParkingBay
    {
        public int ParkingBayID { get; set; }
        public string ParkingBayCode { get; set; }
        public string ParkingBayName { get; set; }
        public string ParkingBayRange { get; set; }
        public string CapacityColour { get; set; }
    }
}
