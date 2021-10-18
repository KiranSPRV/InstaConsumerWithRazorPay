using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyPasses : ContentPage
    {
        static int CustomerVehicleID;
        static int VehicleTypeID;
        static string RegistrationNumber;
        DALVehicle dal_Vehicle;
        OCustomerVehicle obj_CustomerVehicle = new OCustomerVehicle();
        DALPass dal_Pass;
        static bool IsOnline = false;
        public MyPasses()
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Vehicle = new DALVehicle();
            dal_Pass = new DALPass();
            GetListOfPassesByCustomerID();
            ShowLoading(false);
        }
        private async void GetListOfPassesByCustomerID()
        {
            IsOnline = VerifyInternet();

            if (IsOnline)
            {
                ShowLoading(true);
                CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                objCustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                List<OCustomerVehiclePass> lst_ObjOCustomerVehiclePass = new List<OCustomerVehiclePass>();
                try
                {
                    await Task.Run(() =>
                    {
                        lst_ObjOCustomerVehiclePass = dal_Pass.GetListOfPassesByCustomerID(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);
                    });

                    //lstVehiclePasses.ItemsSource = null;

                    //if (lst_ObjOCustomerVehiclePass.Count > 0)
                    //{
                    for (int i = 0; i < lst_ObjOCustomerVehiclePass.Count; i++)
                    {
                        lst_ObjOCustomerVehiclePass[i].VehicleImage = "resource://ParkHyderabad.Resources." + lst_ObjOCustomerVehiclePass[i].VehicleImage;
                        lst_ObjOCustomerVehiclePass[i].StartDateTime = Convert.ToString(lst_ObjOCustomerVehiclePass[i].StartDate.ToString("d MMM yyyy hh:mm tt")); //Convert.ToString(DateTime.ParseExact(Convert.ToString(lst_ObjOCustomerVehiclePass[i].StartDate), "d MMM yyyy h:mm tt", CultureInfo.InvariantCulture));
                        lst_ObjOCustomerVehiclePass[i].ExpiryDateTime = Convert.ToString(lst_ObjOCustomerVehiclePass[i].ExpiryDate.ToString("d MMM yyyy hh:mm tt"));  //Convert.ToString(DateTime.ParseExact(Convert.ToString(lst_ObjOCustomerVehiclePass[i].ExpiryDate), "d MMM yyyy h:mm tt", CultureInfo.InvariantCulture));
                        if (DateTime.Now.Date > ((lst_ObjOCustomerVehiclePass[i].ExpiryDate).Date))
                        {
                            lst_ObjOCustomerVehiclePass[i].PassExpiryColor = "Red";
                            lst_ObjOCustomerVehiclePass[i].PassExpiryMessage = "Expired";
                        }

                        if (lst_ObjOCustomerVehiclePass[i].IsMultiLot == true)
                        {
                            List<Location> lstLocationsComma = new List<Location>();
                            for (var j = 0; j < lst_ObjOCustomerVehiclePass[i].LocationName.Split(',').Length; j++)
                            {
                                Location lstLocationObj = new Location();
                                lstLocationObj.LocationNumber = j + 1;
                                lstLocationObj.LocationName = lst_ObjOCustomerVehiclePass[i].LocationName.Split(',')[j];
                                lstLocationsComma.Add(lstLocationObj);
                            }
                            lst_ObjOCustomerVehiclePass[i].lstLocation = lstLocationsComma;
                        }
                        else
                        {
                            List<Location> lstLocationsComma = new List<Location>();
                            Location lstLocationObj = new Location();
                            lstLocationObj.LocationNumber = 1;
                            lstLocationObj.LocationName = lst_ObjOCustomerVehiclePass[i].LocationName;
                            lstLocationsComma.Add(lstLocationObj);
                            lst_ObjOCustomerVehiclePass[i].lstLocation = lstLocationsComma;
                        }
                    }

                    lstVehiclePasses.ItemsSource = lst_ObjOCustomerVehiclePass;
                    ShowLoading(false);
                    //}
                    //else
                    //{
                    //    lstVehiclePasses.ItemsSource = null;
                    //    ShowLoading(false);
                    //    await DisplayAlert("", "No pass purchased", "Ok");
                    //}
                }
                catch (Exception ex)
                {
                    lstVehiclePasses.ItemsSource = null;
                    ShowLoading(false);
                    await DisplayAlert("Failed - GetListOfPassesByCustomerID", Convert.ToString(ex.Message), "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void lstVehiclePasses_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            ((ListView)sender).SelectedItem = null;
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;
            activity.IsVisible = show;
            activity.IsRunning = show;
            if (show)
            {
                //absLayout.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                //  absLayout.Opacity = 0.5;
            }
            else
            {
                // absLayout.BackgroundColor = Xamarin.Forms.Color.Transparent;
                //  absLayout.Opacity = 1;
            }
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        private async void btn_BuyaPassClicked(object sender, EventArgs e)
        {
            int CustomerID = 0;
            if (App.Current.Properties.ContainsKey("CustomerID"))
            {
                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
            }
            //Navigation.PushAsync(new BuyAPass(obj_CustomerVehicle));
            await Navigation.PushAsync(new Home(null, CustomerID));
        }
    }
}