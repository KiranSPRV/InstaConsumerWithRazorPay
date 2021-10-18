using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParkHyderabad.Model.APIOutPutModel;

namespace ParkHyderabad
{
    public partial class App : Application
    {
        decimal Latitude;
        decimal Longitude;
        public App()
        {
            InitializeComponent();
            Model.APIOutPutModel.Location location = new Model.APIOutPutModel.Location();
            MainPage = new NavigationPage(new Home(null, 0));
        }
        public async void GetLocation()
        {
            await GetLocationPermission();
        }
        private async Task GetLocationPermission()
        {
            var status1 = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            try
            {
                if (status1 == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location != null)
                    {
                        Latitude = (decimal)location.Latitude;
                        Longitude = (decimal)location.Longitude;
                    }
                }
                else
                {
                    DisplayAlert("", "Please turn on device location to use ParkHyderabad", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void DisplayAlert(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }
        private void SetPreference(string key, string value)
        {
            Preferences.Set(key, value);
        }
        protected override void OnStart()
        {
            // Handle when your app starts  

            if (VersionTracking.IsFirstLaunchEver)
            {
                Application.Current.Properties.Clear();

                /*QA:http://35.202.198.25:81/InstaParkingConsumerAPIQA/ UAT:http://35.202.198.25:81/InstaParkingConsumerAPIPROD/ PROD:https://cusapi.instaparking.in/  */
                /*Razor Pay QA:http://35.202.198.25:81/RazorPayAPI/Payment.aspx? http://35.202.198.25:81/RazorPayAPI/Charge.aspx UAT:http://35.202.198.25:81/RazorPayAPIPROD/Payment.aspx? PROD:http://razor.instaparking.in/Payment.aspx  */
                /* http://35.202.198.25:81/RazorPayTest/Payment.aspx?  http://35.202.198.25:81/RazorPayTest/PaymentClosePage.aspx   */

                /*Prod New APIS :http://devcusapi.instaparking.in/ */
                /*Prod New RazorPayAPIS :http://devrazor.instaparking.in/ */

                App.Current.Properties["BaseURL"] = "http://devcusapi.instaparking.in/";
App.Current.Properties["PaymentTypeID"] = 2;
App.Current.Properties["ApplicationTypeID"] = 1;
App.Current.Properties["PassApplicationTypeID"] = 4;
App.Current.Properties["StatusID"] = 4;
}
}
protected override async void OnSleep()
{

// Handle when your app sleeps             
}
protected override async void OnResume()
{

}
}
}
