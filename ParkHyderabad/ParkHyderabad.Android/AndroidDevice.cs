using ParkHyderabad.Droid;
using ParkHyderabad.Helper;
using Plugin.DeviceInfo;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace ParkHyderabad.Droid
{
    public class AndroidDevice : IDevice
    {
        public string GetIdentifier()
        {
            return CrossDeviceInfo.Current.Id;
            //return Settings.Secure.GetString(Forms.Context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}