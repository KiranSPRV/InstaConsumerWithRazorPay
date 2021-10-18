using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchLocation : ContentPage
    {
        List<Model.APIOutPutModel.Location> lst_LocationParkingLot = new List<Model.APIOutPutModel.Location>();
        private bool IsOnline = false;
        DALLocationParkingLots dal_LocationParkingLot;
        static LocationParkingLotFilter objLocationParkingLotsFilter;
        public SearchLocation(LocationParkingLotFilter objLocationLotFilter)
        {
            InitializeComponent();
            dal_LocationParkingLot = new DALLocationParkingLots();
            App.Current.Properties["BaseURL"] = "http://devcusapi.instaparking.in/";
            App.Current.Properties["PaymentTypeID"] = 2;
            App.Current.Properties["ApplicationTypeID"] = 1;
            App.Current.Properties["PassApplicationTypeID"] = 4;
            App.Current.Properties["StatusID"] = 4;
            objLocationParkingLotsFilter = new LocationParkingLotFilter();
            objLocationParkingLotsFilter = objLocationLotFilter;
            GetTheFlow();
            
            
        }
        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            lstParkingLots.IsVisible = true;
            lstParkingLots.BeginRefresh();

            try
            {
                var dataEmpty = lst_LocationParkingLot.Where(i => i.LocationName.ToLower().StartsWith(e.NewTextValue.ToLower()));
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                    lstParkingLots.IsVisible = false;
                else
                    lstParkingLots.ItemsSource = lst_LocationParkingLot.Where(i => i.LocationName.ToLower().StartsWith(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                lstParkingLots.IsVisible = false;

            }
            lstParkingLots.EndRefresh();
        }
        private async void lstParkingLots_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            Model.APIOutPutModel.Location location = (Model.APIOutPutModel.Location)e.Item;
            txtSearchBar.Text = location.LocationName;
            lstParkingLots.IsVisible = false;
            ((ListView)sender).SelectedItem = null;

            int CustomerID = 0;
            if (App.Current.Properties.ContainsKey("CustomerID"))
            {
                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
            }
            App.Current.Properties["SearchLocation"] = Convert.ToString(location.LocationName);

           
            objLocationParkingLotsFilter.LocationName = location.LocationName;
            objLocationParkingLotsFilter.SelectedLatitude = location.Latitude;
            objLocationParkingLotsFilter.SelectedLongitude = location.Longitude;
            await Navigation.PushAsync(new Home(objLocationParkingLotsFilter, CustomerID));
          
            
        }
        public void GetListOfLocations()
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        List<Model.APIOutPutModel.Location> obj_listLocation = new List<Model.APIOutPutModel.Location>();
                        obj_listLocation = dal_LocationParkingLot.GetListOfLocations(Convert.ToString(App.Current.Properties["apitoken"]));

                        if (obj_listLocation.Count > 0)
                        {
                            lstParkingLots.ItemsSource = obj_listLocation;
                            lst_LocationParkingLot = obj_listLocation;
                        }
                        else
                        {
                            DisplayAlert("", "Locations not found!", "Ok");
                        }
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetListOfLocations", Convert.ToString(ex.Message), "Ok");
            }
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        private async void GetTheFlow()
        {
            GetListOfLocations();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            txtSearchBar.Focus();
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                searchabsLayout.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                searchabsLayout.Opacity = 0.5;
            }
            else
            {
                searchabsLayout.BackgroundColor = Xamarin.Forms.Color.Transparent;
                searchabsLayout.Opacity = 1;
            }
        }
    }
}