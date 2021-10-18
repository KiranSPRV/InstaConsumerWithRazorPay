using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuyAPass : ContentPage
    {
        static OCustomerVehicle objOCustomerVehicle;
        DALPass dal_Pass;
        DALLocationParkingLots dal_LocationParkingLots;
        DALVehicle dal_Vehicle;
        DALCustomer dal_Customer;
        static int LocationID;
        static string LocationName;
        static int VehicleTypeID;
        static int CustomerVehicleID = 0;
        static int CustomerID;
        static decimal DueAmount = 0;
        static int DueCustomerParkingSlotID = 0;
        static string RegistrationNumber;
        OCustomerVehiclePass objOCustomerVehiclePass;
        List<PassPrice> lst_PassPrice = new List<PassPrice>();
        static PassPrice objPassPrice = new PassPrice();
        List<Location> lstSelectedLocations;

        static bool IsOnline = false;
        public BuyAPass(OCustomerVehicle obj_OCustomerVehicle)
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Pass = new DALPass();
            dal_Vehicle = new DALVehicle();
            dal_Customer = new DALCustomer();
            dal_LocationParkingLots = new DALLocationParkingLots();

            objOCustomerVehicle = obj_OCustomerVehicle;
            VehicleTypeID = obj_OCustomerVehicle.VehicleTypeID;
            LocationID = obj_OCustomerVehicle.LocationID;
            CustomerID = obj_OCustomerVehicle.CustomerID;

            ShowLoading(false);
        }
        protected override void OnAppearing()
        {
            DueCustomerParkingSlotID = 0;
            DueAmount = 0;
            objPassPrice = null;
            stkDueAmount.IsVisible = false;
            stkSingleStation.IsVisible = true;
            stkAllStation.IsVisible = false;
            stkMultiStation.IsVisible = false;

            if (VehicleTypeID == 0)
            {
                VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;
            }

            if (VehicleTypeID == VehicleTypeCodes.TwoWheelerTypeID)
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
            }
            else if (VehicleTypeID == VehicleTypeCodes.FourWheelerTypeID)
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
            }
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetListOfLocationsByVehicleType(VehicleTypeID, LocationID);                
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        public void GetListOfCustomerVehicleWithType(int VehicleTypeID)
        {
            try
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
                                cvRegistrationNumber.ItemsSource = null;
                                cvRegistrationNumber.ItemsSource = obj_listCustomerVehicle;
                                CustomerVehicleID = 0;
                            }
                            else
                            {
                                cvRegistrationNumber.ItemsSource = null;
                                CustomerVehicleID = 0;
                                DisplayAlert("", "No Vehicles found to park! Please add/activate your Vehicle", "Ok");
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
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetListOfCustomerVehicles", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void GetListOfLocationsByVehicleType(int VehicleTypeID, int Location_ID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToInt32(App.Current.Properties["CustomerID"]) != 0)
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            List<Location> obj_listLocation = new List<Location>();
                            CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                            objCustomerVehicle.VehicleTypeID = VehicleTypeID;
                            obj_listLocation = dal_LocationParkingLots.GetListOfLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);

                            if (obj_listLocation.Count > 0)
                            {
                                pkLocation.ItemsSource = obj_listLocation;

                                int x = 0;

                                for (int i = 0; i < obj_listLocation.Count; i++)
                                {
                                    if (obj_listLocation[i].LocationID == Location_ID)
                                    {
                                        x = i;
                                        break;
                                    }
                                    else
                                    {
                                        x = 0;
                                    }
                                }

                                pkLocation.SelectedIndex = x;
                                LocationID = obj_listLocation[x].LocationID;
                                LocationName = obj_listLocation[x].LocationName;
                            }
                            else
                            {
                                pkLocation.ItemsSource = null;
                                DisplayAlert("", "Locations not found!", "Ok");
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
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetListOfLocations", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void GetPassTypesByVehicleTypeID(int Vehicle_Type_ID, int Location_ID)
        {
            try
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToInt32(App.Current.Properties["CustomerID"]) != 0)
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                            obj_CustomerVehicle.VehicleTypeID = Vehicle_Type_ID;
                            obj_CustomerVehicle.LocationID = Location_ID;
                            List<PassPrice> obj_PassPrice = new List<PassPrice>();
                            obj_PassPrice = dal_Pass.GetPassTypesByVehicleTypeID(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                            if (obj_PassPrice.Count > 0)
                            {
                                colView.ItemsSource = null;
                                lst_PassPrice = obj_PassPrice;
                                colView.ItemsSource = obj_PassPrice;
                            }
                            else
                            {
                                colView.ItemsSource = null;
                                DisplayAlert("", "Pass info not found!", "Ok");
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
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetPassTypesByVehicleTypeID", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void OnLocPkrSelectedIndexChanged(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                var picker = (Picker)sender;
                int selectedIndex = picker.SelectedIndex;

                if (pkLocation.SelectedItem != null)
                {
                    Location obj_Location = new Location();
                    obj_Location = (Location)picker.SelectedItem;
                    LocationID = obj_Location.LocationID;
                    LocationName = obj_Location.LocationName;
                    GetPassTypesByVehicleTypeID(VehicleTypeID, LocationID);
                }
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        public void rbRegistrationNumber(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    OCustomerVehicle customerVehicle = ((RadioButton)sender).BindingContext as OCustomerVehicle;
                    VehicleTypeID = customerVehicle.VehicleTypeID;
                    CustomerVehicleID = customerVehicle.CustomerVehicleID;
                    RegistrationNumber = customerVehicle.RegistrationNumber;

                    objOCustomerVehicle.VehicleTypeID = VehicleTypeID;
                    objOCustomerVehicle.Model = customerVehicle.Model;
                    objOCustomerVehicle.CustomerVehicleID = CustomerVehicleID;
                    objOCustomerVehicle.RegistrationNumber = RegistrationNumber;
                    objOCustomerVehicle.VehicleImage = customerVehicle.VehicleImage;

                    GetVehicleDueAmount(CustomerVehicleID);
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
        }
        public async void GetVehicleDueAmount(int customerVehicleID)
        {
            try
            {

                CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                objCustomerVehicle.CustomerVehicleID = customerVehicleID;

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    CustomerParkingSlot resultObj = dal_Vehicle.GetVehicleDueAmount(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);

                    if (resultObj.DueCustomerParkingSlotID != 0)
                    {
                        stkDueAmount.IsVisible = true;
                        DueCustomerParkingSlotID = resultObj.DueCustomerParkingSlotID;
                        DueAmount = resultObj.DueAmount;

                        if (DueAmount > 0)
                            {
                                lblDue.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount)) + "/-";
                                lblPass.Text = "₹ " + Convert.ToString(Convert.ToDouble(objPassPrice == null ? 0 : objPassPrice.Price)) + "/-";
                                lblTotal.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount + (objPassPrice == null ? 0 : objPassPrice.Price))) + "/-";
                            }
                            else
                            {
                                lblDue.Text = "₹ " + Convert.ToString(Convert.ToDouble(0)) + "/-";
                                lblPass.Text = "₹ " + Convert.ToString(Convert.ToDouble(objPassPrice == null ? 0 : objPassPrice.Price)) + "/-";
                                lblTotal.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount + (objPassPrice == null ? 0 : objPassPrice.Price))) + "/-";
                            }
                    }
                    else
                    {
                        stkDueAmount.IsVisible = false;
                        DueCustomerParkingSlotID = 0;
                        DueAmount = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetVehicleDueAmount", Convert.ToString(ex.Message), "Ok");
            }
        }
        public async void GetVehicleDueAmountDetails()
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    CustomerParkingSlot objCustomerParkingSlot = new CustomerParkingSlot();
                    objCustomerParkingSlot.CustomerParkingSlotID = DueCustomerParkingSlotID;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objCustomerParkingSlot);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        CustomerParkingSlotDetails resultObj = dal_Customer.GetVehicleDueAmountDetails(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerParkingSlot);

                        if (resultObj.CustomerParkingSlotID != 0)
                        {
                            lblDueRegNum.Text = resultObj.RegistrationNumber;
                            lblDueLocation.Text = resultObj.LocationName + " - " + resultObj.LocationParkingLotName;
                            lblDueTiming.Text = (resultObj.From).ToString("dd-MM-yyyy hh:mm tt") + " to " + (resultObj.To).ToString("dd-MM-yyyy hh:mm tt");
                            lblDueAmount.Text = "₹ " + Convert.ToString(resultObj.DueAmount);
                            lblDueDuration.Text = resultObj.Duration;
                            lblDueFOCReason.Text = resultObj.ClampReason;
                        }
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetVehicleDueAmountDetails", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void OpenPopup(object sender, EventArgs e)
        {
            if (DueCustomerParkingSlotID != 0)
            {
                popupDueView.IsVisible = true;
                GetVehicleDueAmountDetails();
            }
        }
        private async void ClosePopup(object sender, EventArgs e)
        {
            popupDueView.IsVisible = false;
            lblDueRegNum.Text = "";
            lblDueLocation.Text = "";
            lblDueTiming.Text = "";
            lblDueAmount.Text = "";
            lblDueDuration.Text = "";
            lblDueFOCReason.Text = "";
        }
        public bool ValidateCustomerVehiclePass(int customerVehicleID)
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                    objCustomerVehicle.CustomerVehicleID = customerVehicleID;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objCustomerVehicle);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        OCustomerVehiclePass resultObj = dal_Pass.ValidateCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);
                        objOCustomerVehiclePass = resultObj;
                        if (resultObj.CustomerVehiclePassID != 0)
                        {
                            DisplayAlert("", "Vehicle number " + RegistrationNumber + " has a valid pass", "Ok");
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - ValidateCustomerVehiclePass", Convert.ToString(ex.Message), "Ok");
            }
            return false;
        }
        private async void TwoWheeler_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
                VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;
                stkDueAmount.IsVisible = false;
                DueAmount = 0;
                DueCustomerParkingSlotID = 0;
                objPassPrice = null;
                stkSingleStation.IsVisible = true;
                stkAllStation.IsVisible = false;
                stkMultiStation.IsVisible = false;
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetListOfLocationsByVehicleType(VehicleTypeID, LocationID);
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        private async void FourWheeler_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
                VehicleTypeID = VehicleTypeCodes.FourWheelerTypeID;
                stkDueAmount.IsVisible = false;
                DueAmount = 0;
                DueCustomerParkingSlotID = 0;
                objPassPrice = null;
                stkSingleStation.IsVisible = true;
                stkAllStation.IsVisible = false;
                stkMultiStation.IsVisible = false;
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetListOfLocationsByVehicleType(VehicleTypeID, LocationID);
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        private async void stk_AboutPassesClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AboutPasses());
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - About Passes", Convert.ToString(ex.Message), "Ok");
            }
        }        
        private async void stk_AddVehicleTapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                await Navigation.PushAsync(new AddVehicle(CustomerID, "", VehicleTypeID == VehicleTypeCodes.TwoWheelerTypeID ? VehicleTypeCodes.TwoWheeler : VehicleTypeCodes.FourWheeler));
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                ShowLoading(true);

                var resultbox = (CheckBox)sender;
                Location ob = resultbox.BindingContext as Location;

                if (lstSelectedLocations == null)
                {
                    lstSelectedLocations = new List<Location>();
                }

                if (resultbox.IsChecked)
                {
                    resultbox.Color = Color.FromHex("#009f76");

                    Location objselected = new Location();
                    objselected.LocationID = ob.LocationID;
                    objselected.LocationName = ob.LocationName;

                    if (lstSelectedLocations.Count < 3)
                    {
                        lstSelectedLocations.Add(objselected);

                        if (lstSelectedLocations.Count == 1)
                        {
                            lblSelectedStations.Text = lblSelectedStations.Text + ob.LocationName.ToUpper();
                        }
                        else
                        {
                            lblSelectedStations.Text = lblSelectedStations.Text + ", " + ob.LocationName.ToUpper();
                        }

                        btnGeneratePass.IsEnabled = true;
                    }
                    else
                    {
                        await DisplayAlert("", "You can select max 3 Locations", "Ok");
                        btnGeneratePass.IsEnabled = false;
                        resultbox.IsChecked = false;
                    }
                }
                else
                {
                    if (lstSelectedLocations.Count > 0)
                    {
                        var item = lstSelectedLocations.SingleOrDefault(x => x.LocationID == ob.LocationID);
                        if (item != null)
                        {
                            lstSelectedLocations.Remove(item);
                            if (lblSelectedStations.Text.Contains(item.LocationName.ToUpper()))
                            {
                                lblSelectedStations.Text = lblSelectedStations.Text.Replace(item.LocationName.ToUpper(), string.Empty);
                            }

                            lblSelectedStations.Text = "You can park your vehicle at ";
                            string str = string.Empty;
                            for (int i = 0; i < lstSelectedLocations.Count; i++)
                            {
                                str = str + lstSelectedLocations[i].LocationName + ", ";
                            }
                            lblSelectedStations.Text = "You can park your vehicle at " + str.Substring(0, str.Length - 2).Trim();
                            lblSelectedStations.IsVisible = false;
                        }
                    }

                    btnGeneratePass.IsEnabled = true;
                }
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }
        private void colView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                var model = e.CurrentSelection.FirstOrDefault() as PassPrice;
                if (model != null)
                {
                    int index = Convert.ToInt32(model.PassIndex);

                    for (int i = 0; i < lst_PassPrice.Count; i++)
                    {
                        lst_PassPrice[i].PassBordorColour = "#a3a3a3";
                        lst_PassPrice[i].PassBackgroundColour = "#FFFFFF";
                        lst_PassPrice[i].PassTextColour = "#a3a3a3";
                    }

                    lst_PassPrice[index].PassBordorColour = "#1976d3";
                    lst_PassPrice[index].PassBackgroundColour = "#1976d3";
                    lst_PassPrice[index].PassTextColour = "#FFFFFF";

                    objPassPrice = lst_PassPrice[index];
                    lstSelectedLocations = new List<Location>();

                    if (DueAmount > 0)
                    {
                        stkDueAmount.IsVisible = true;
                        lblDue.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount)) + "/-";
                        lblPass.Text = "₹ " + Convert.ToString(Convert.ToDouble(objPassPrice.Price)) + "/-";
                        lblTotal.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount + objPassPrice.Price)) + "/-";
                    }
                    else
                    {
                        stkDueAmount.IsVisible = false;
                        lblDue.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount)) + "/-";
                        lblPass.Text = "₹ " + Convert.ToString(Convert.ToDouble(objPassPrice.Price)) + "/-";
                        lblTotal.Text = "₹ " + Convert.ToString(Convert.ToDouble(DueAmount + objPassPrice.Price)) + "/-";
                    }

                    if (objPassPrice.PassCode.ToUpper() == "2W MSP" || objPassPrice.PassCode.ToUpper() == "4W MSP")
                    {
                        ShowLoading(false);
                        stkSingleStation.IsVisible = false;
                        stkMultiStation.IsVisible = true;
                        stkAllStation.IsVisible = false;
                        
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            List<Location> obj_listLocation = new List<Location>();
                            CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                            objCustomerVehicle.VehicleTypeID = VehicleTypeID;
                            obj_listLocation = dal_LocationParkingLots.GetListOfMultiLocationsByVehicleType(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);

                            if (obj_listLocation.Count > 0)
                            {
                                lstStations.ItemsSource = obj_listLocation;
                            }
                            else
                            {
                                lstStations.ItemsSource = null;
                                DisplayAlert("", "Locations not found!", "Ok");
                            }
                        }
                    }
                    else if (objPassPrice.PassCode.ToUpper() == "2W ALL STATION" || objPassPrice.PassCode.ToUpper() == "4W ALL STATION")
                    {
                        ShowLoading(false);
                        stkSingleStation.IsVisible = false;
                        stkMultiStation.IsVisible = false;
                        stkAllStation.IsVisible = true;
                    }
                    else
                    {
                        stkSingleStation.IsVisible = true;
                        stkMultiStation.IsVisible = false;
                        stkAllStation.IsVisible = false;

                        List<Location> objLocation = new List<Location>();
                        Location location = new Location();
                        location.LocationID = LocationID;
                        location.LocationName = LocationName;
                        objLocation.Add(location);
                        ShowLoading(false);
                    }

                    colView.ItemsSource = null;
                    colView.ItemsSource = lst_PassPrice;
                }
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
            }
        }
        private async void btn_GeneratePassClicked(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    if (CustomerVehicleID == 0)
                    {
                        DisplayAlert("", "Please select Registration Number of your Vehicle", "Ok");
                        return;
                    }

                    if (objPassPrice == null)
                    {
                        DisplayAlert("", "Please select Pass Preference", "Ok");
                        return;
                    }

                    if (objPassPrice.PassPriceID == 0)
                    {
                        DisplayAlert("", "Please select Pass Preference", "Ok");
                        return;
                    }

                    bool pass_result = ValidateCustomerVehiclePass(CustomerVehicleID);

                    if (!pass_result)
                    {
                        PassPrice obj_passPrice = objPassPrice;

                        if (DueAmount >= 0)
                        {
                            obj_passPrice.DueCustomerParkingSlotID = DueCustomerParkingSlotID;
                            obj_passPrice.DueAmount = DueAmount;
                        }
                        else
                        {
                            obj_passPrice.DueCustomerParkingSlotID = 0;
                            obj_passPrice.DueAmount = 0;
                        }

                        if (obj_passPrice.PassCode.ToUpper() == "2W MSP" || obj_passPrice.PassCode.ToUpper() == "4W MSP")
                        {
                            ShowLoading(false);
                            if (lstSelectedLocations.Count == 0 || lstSelectedLocations.Count == 1)
                            {
                                ShowLoading(false);
                                DisplayAlert("", "Please select minimum of 2 Locations", "Ok");
                            }
                            else
                            {
                                ShowLoading(false);
                                ShowLoading(true);
                                GeneratePass pageGeneratePass = null;
                                await Task.Run(() =>
                                {
                                   
                                    pageGeneratePass = new GeneratePass(obj_passPrice, lstSelectedLocations, objOCustomerVehicle, true);
                                });
                                await Navigation.PushAsync(pageGeneratePass);
                                ShowLoading(false);
                            }
                        }
                        else
                        {
                            List<Location> objLocation = new List<Location>();
                            Location location = new Location();
                            location.LocationID = LocationID;
                            location.LocationName = LocationName;
                            objLocation.Add(location);
                            ShowLoading(false);
                            await Navigation.PushAsync(new GeneratePass(obj_passPrice, objLocation, objOCustomerVehicle, false));
                        }
                    }
                }
                else
                {
                    await DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - Generate Passes", Convert.ToString(ex.Message), "Ok");
            }
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
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
    }
}