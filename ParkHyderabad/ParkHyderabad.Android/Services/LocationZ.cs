using Android.Content;
using Android.Locations;
using ParkHyderabad.Droid.Services;
using ParkHyderabad.Helper;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LocationZ))]

namespace ParkHyderabad.Droid.Services
{
    public class LocationZ : ILocSettings
    {
        public void OpenSettings()
        {
            LocationManager LM = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);
            
            if (LM.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                Context ctx = Forms.Context;
                ctx.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
            }
            else
            {
                //this is handled in the PCL
            }
        }
    }
}