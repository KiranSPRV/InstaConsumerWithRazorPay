using Android.Content;
using ParkHyderabad.Droid;
using ParkHyderabad.Helper;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AppSettingsInterface))]
namespace ParkHyderabad.Droid
{
    public class AppSettingsInterface : IAppSettingsHelper
    {
        public void OpenAppSettings()
        {
            /*var intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);*/
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            string package_name = "com.sprvtec.InstaParking";
            var uri = Android.Net.Uri.FromParts("package", package_name, null);
            intent.SetData(uri);
            Application.Context.StartActivity(intent);
        }
    }
}