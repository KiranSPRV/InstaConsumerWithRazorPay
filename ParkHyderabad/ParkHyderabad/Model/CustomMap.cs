using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace ParkHyderabad
{
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
    }
}
