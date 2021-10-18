using Plugin.Connectivity;
using System;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnableLocationService : ContentPage
    {
        public EnableLocationService()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {          
            GetLocation();            
        }

        public async void GetLocation()
        {
            try
            {
                bool IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    ShowLoading(true);
                    var status1 = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();   
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(5)
                    }
                    );

                    if (location == null)
                    {
                        location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                        {
                            DesiredAccuracy = GeolocationAccuracy.High,
                            Timeout = TimeSpan.FromSeconds(5)
                        }
                        );
                    }
                    else
                    {
                        int CustomerID = 0;
                        if (App.Current.Properties.ContainsKey("CustomerID"))
                        {
                            CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        }
                      
                        //await Navigation.PushAsync(new MasterPage(CustomerID));
                        ShowLoading(false);
                    }

                    ShowLoading(false);
                }
                else
                {
                    ShowLoading(false);
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }

        private async void btn_TurnOnClicked(object sender, EventArgs e)
        {
            try
            {
                bool IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    var status1 = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    var location = await Geolocation.GetLastKnownLocationAsync();

                    if (location == null)
                    {
                        if (Device.RuntimePlatform == global::Xamarin.Forms.Device.Android)
                        {
                            global::Xamarin.Forms.DependencyService.Get<global::ParkHyderabad.Helper.ILocSettings>().OpenSettings();
                        }
                        else if (Device.RuntimePlatform == global::Xamarin.Forms.Device.iOS)
                        {
                            global::Xamarin.Forms.DependencyService.Get<global::ParkHyderabad.Helper.ILocSettings>().OpenSettings();
                        }
                    }
                    else
                    {
                        int CustomerID = 0;
                        if (App.Current.Properties.ContainsKey("CustomerID"))
                        {
                            CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        }

                        //await Navigation.PushAsync(new MasterPage(CustomerID));
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {

            }
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }

        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                absLayout.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                absLayout.Opacity = 0.5;
            }
            else
            {
                absLayout.BackgroundColor = Xamarin.Forms.Color.Transparent;
                absLayout.Opacity = 1;
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}