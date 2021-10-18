using ParkHyderabad.DAL;
using ParkHyderabad.Model;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ParkHyderabad.Helper;
using Plugin.Connectivity;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyVehicles : ContentPage
    {
        DALVehicle dal_Vehicle;
        static OCustomerVehicle customerVehicle;
        static int VehicleTypeID;
        static bool IsOnline = false;

        public MyVehicles()
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Vehicle = new DALVehicle();
            GetListOfCustomerVehicleWithType(VehicleTypeCodes.TwoWheelerTypeID);
            lblmyvehicle.Text = "My Bikes";
            VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;
            ShowLoading(false);
        }        
        public void ShowLoading(bool show)
        {
            //StklauoutactivityIndicator.IsVisible = show;
            // activity.IsVisible = show;
            // activity.IsRunning = show;
            if (show)
            {
                // absLayout.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                // absLayout.Opacity = 0.5;
            }
            else
            {
                //  absLayout.BackgroundColor = Xamarin.Forms.Color.Transparent;
                // absLayout.Opacity = 1;
            }
        }
        private async void ColView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = e.CurrentSelection.FirstOrDefault() as OCustomerVehicle;
            if (model != null)
            {
                if (model.CustomerVehicleID != 0)
                {
                    await Navigation.PushAsync(new UpdateVehicle(model));
                }
                else
                {
                    await Navigation.PushAsync(new AddVehicle(Convert.ToInt32(App.Current.Properties["CustomerID"]), null, VehicleTypeID == VehicleTypeCodes.TwoWheelerTypeID ? VehicleTypeCodes.TwoWheeler : VehicleTypeCodes.FourWheeler));
                }

                if (ColView.SelectedItem != null)
                {
                    ColView.SelectedItem = null;
                }
            }
        }
        private async void TwoWheeler_Tapped(object sender, EventArgs e)
        {
            svgTwo.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
            svgFour.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
            VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;
            lblmyvehicle.Text = "My Bikes";
            GetListOfCustomerVehicleWithType(VehicleTypeID);
        }
        private async void FourWheeler_Tapped(object sender, EventArgs e)
        {
            svgTwo.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
            svgFour.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
            VehicleTypeID = VehicleTypeCodes.FourWheelerTypeID;
            lblmyvehicle.Text = "My Cars";
            GetListOfCustomerVehicleWithType(VehicleTypeID);
        }
        public void GetListOfCustomerVehicleWithType(int VehicleTypeID)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {
                        if (Convert.ToInt32(App.Current.Properties["CustomerID"]) != 0)
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                client.BaseAddress = new Uri(Convert.ToString(App.Current.Properties["BaseURL"]));
                                // We want the response to be JSON.
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                                postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                                postData.Add(new KeyValuePair<string, string>("username", "Kiran"));
                                postData.Add(new KeyValuePair<string, string>("password", "1234"));
                                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                                // Post to the Server and parse the response.
                                var response = client.PostAsync("Token", content).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    var jsonString = response.Content.ReadAsStringAsync();
                                    OAuthToken responseData = JsonConvert.DeserializeObject<OAuthToken>(jsonString.Result);
                                    string Access_Token = responseData.access_token;
                                    App.Current.Properties["apitoken"] = Access_Token;
                                }
                            }

                            if (App.Current.Properties.ContainsKey("apitoken"))
                            {
                                CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                                obj_CustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                obj_CustomerVehicle.VehicleTypeID = Convert.ToInt32(VehicleTypeID);
                                List<OCustomerVehicle> obj_listCustomerVehicle = new List<OCustomerVehicle>();
                                obj_listCustomerVehicle = dal_Vehicle.GetListOfCustomerVehicleWithType(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                                int x = 0;

                                if (obj_listCustomerVehicle.Count > 0)
                                {
                                    ColView.ItemsSource = null;

                                    var index = obj_listCustomerVehicle.Count;
                                    OCustomerVehicle ocustvehObj = new OCustomerVehicle();
                                    ocustvehObj.VehicleImage = "resource://ParkHyderabad.Resources.addvehicle.svg";
                                    ocustvehObj.Model = "Add Vehicle";
                                    ocustvehObj.PrimaryVehicleImage = "";
                                    ocustvehObj.RegistrationNumber = "";
                                    ocustvehObj.ModelFontsize = 10;
                                    obj_listCustomerVehicle.Insert(obj_listCustomerVehicle.Count, ocustvehObj);

                                    ColView.ItemsSource = obj_listCustomerVehicle;
                                }
                                else
                                {
                                    ColView.ItemsSource = null;

                                    OCustomerVehicle ocustvehObj = new OCustomerVehicle();
                                    ocustvehObj.VehicleImage = "resource://ParkHyderabad.Resources.addvehicle.svg";
                                    ocustvehObj.Model = "Add Vehicle";
                                    ocustvehObj.PrimaryVehicleImage = "";
                                    ocustvehObj.RegistrationNumber = "";
                                    ocustvehObj.ModelFontsize = 10;
                                    obj_listCustomerVehicle.Insert(0, ocustvehObj);

                                    ColView.ItemsSource = obj_listCustomerVehicle;
                                    //DisplayAlert("", "No Vehicles found to park! Please add/activate your Vehicle", "Ok");
                                }
                            }
                        }
                        else
                        {
                            DisplayAlert("", "Profile not found!", "Ok");
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Customer Key does not exists", "Ok");
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
                DisplayAlert("Failed - GetListOfCustomerVehicles", Convert.ToString(ex.Message), "Ok");
            }
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        private async void btn_BackClicked(object sender, EventArgs e)
        {
            int CustomerID = 0;
            if (App.Current.Properties.ContainsKey("CustomerID"))
            {
                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
            }

            await Navigation.PushAsync(new Home(null, CustomerID));
        }
    }
}
