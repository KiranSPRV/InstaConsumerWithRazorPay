using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartSession : ContentPage
    {
        DALLocationParkingLots dal_LocationParkingLot;
        DALCustomer dal_Customer;
        DALVehicle dal_Vehicle;
        DALPass dal_Pass;
        CustomerParkingSlotDetails objCustomerParkingSlotDetails;
        List<OCustomerVehicle> objlistCustomerVehicle = new List<OCustomerVehicle>();
        static int ParkingBayID;
        static int VehicleTypeID;
        static int CustomerID;
        static int CustomerVehicleID;
        static string RegistrationNumber;
        static int CustomerParkingSlotID;
        static int LocationID;
        static int LocationParkingLotID;
        static string LocationParkingLotName;
        static string ParkingBayRange;
        string CapacityColour;
        static int Duration;
        DateTime currentTime = DateTime.Now;
        decimal Latitude;
        decimal Longitude;
        static decimal DueAmount = 0;
        static int DueCustomerParkingSlotID = 0;
        OCustomerVehiclePass objOCustomerVehiclePass = new OCustomerVehiclePass();
        static string CheckedInLocationName;
        static string CheckedInLotName;
        static DateTime ExpectedEndTime_date = DateTime.Now;
        static bool IsFullDay = false;
        static bool stkTimingFlag = false;
        List<string> lstImage = new List<string>();
        static bool IsOnline = false;

        private double startScale;
        private double currentScale;
        private double xOffset;
        private double yOffset;

        static string transactionDuplicate;

        public StartSession(CustomerParkingSlotDetails customerParkingSlotDetails)
        {
            InitializeComponent();
            ShowLoading(true);

            objCustomerParkingSlotDetails = customerParkingSlotDetails;

            objCustomerParkingSlotDetails.LotOpenCloseTime = ("(" +
                objCustomerParkingSlotDetails.LotOpeningTime + " - " +
                objCustomerParkingSlotDetails.LotClosingTime + ")").ToLower();

            this.BindingContext = objCustomerParkingSlotDetails;

            VehicleTypeID = customerParkingSlotDetails.VehicleTypeID;
            CustomerID = customerParkingSlotDetails.CustomerID;
            CustomerVehicleID = customerParkingSlotDetails.CustomerVehicleID;
            CustomerParkingSlotID = customerParkingSlotDetails.CustomerParkingSlotID;
            LocationID = customerParkingSlotDetails.LocationID;
            LocationParkingLotID = customerParkingSlotDetails.LocationParkingLotID;
            LocationParkingLotName = customerParkingSlotDetails.LocationParkingLotName;
            dal_LocationParkingLot = new DALLocationParkingLots();
            dal_Customer = new DALCustomer();
            dal_Vehicle = new DALVehicle();
            dal_Pass = new DALPass();
            Duration = 2;
            IsFullDay = false;
            ShowLoading(false);
        }
        protected override void OnAppearing()
        {
            DueAmount = 0;
            DueCustomerParkingSlotID = 0;
            lblDue.Text = "0";
            currentTime = DateTime.Now;
            //lblTiming.Text = "Today, " + currentTime.ToString("hh:mm tt", CultureInfo.CreateSpecificCulture("en-US")) +
            //" - " + currentTime.AddHours(2).ToString("hh:mm tt", CultureInfo.CreateSpecificCulture("en-US")) + " (2 hrs)";

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

            if (!objCustomerParkingSlotDetails.TwoWheelerVisibility)
            {
                svgTwo.IsEnabled = false;
            }

            if (!objCustomerParkingSlotDetails.FourWheelerVisibility)
            {
                svgFour.IsEnabled = false;
            }

            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetParkingBayByLotID(LocationParkingLotID);
                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        public void OnPickerBaySelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (pkBayNumber.SelectedItem != null)
            {
                ParkingBay obj_ParkingBay = new ParkingBay();
                obj_ParkingBay = (ParkingBay)picker.SelectedItem;
                ParkingBayID = obj_ParkingBay.ParkingBayID;
            }
        }
        public void GetParkingBayByLotID(int locationParkingLotID)
        {
            try
            {
                pkBayNumber.ItemsSource = null;
                pkBayNumber.Title = "Choose";
                LocationParkingLot objLocationParkingLot = new LocationParkingLot();
                objLocationParkingLot.LocationParkingLotID = locationParkingLotID;
                objLocationParkingLot.VehicleTypeID = VehicleTypeID;

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objLocationParkingLot);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    List<ParkingBay> resultObj = dal_LocationParkingLot.GetParkingBayByLotID(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLot);

                    if (resultObj.Count > 0)
                    {
                        pkBayNumber.ItemsSource = resultObj;

                        CapacityColour = resultObj[0].CapacityColour == null ? "" : resultObj[0].CapacityColour;

                        if (CapacityColour.ToUpper() == "RED")
                        {
                            btnPay.IsVisible = false;
                            btnCheckIn.IsVisible = false;
                            DisplayAlert("", "Sorry, '" + Convert.ToString(LocationParkingLotName) + "' lot is full. Please select another lot to park your vehicle", "Ok");
                            return;
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Bay Numbers not available", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed", Convert.ToString(ex.Message), "OK");
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
                                objlistCustomerVehicle = obj_listCustomerVehicle;
                                cvRegistrationNumber.ItemsSource = null;
                                cvRegistrationNumber.ItemsSource = obj_listCustomerVehicle;
                                CustomerVehicleID = 0;
                                RegistrationNumber = "";
                                
                            }
                            else
                            {
                                cvRegistrationNumber.ItemsSource = null;
                                CustomerVehicleID = 0;
                                RegistrationNumber = "";
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
        public void rbRegistrationNumber(object sender, CheckedChangedEventArgs e)
        {

            if (e.Value)
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    OCustomerVehicle customerVehicle = ((RadioButton)sender).BindingContext as OCustomerVehicle;
                    CustomerVehicleID = customerVehicle.CustomerVehicleID;
                    RegistrationNumber = customerVehicle.RegistrationNumber;

                    DueCustomerParkingSlotID = 0;
                    DueAmount = 0;

                    GetCustomerVehiclePassDetails(LocationID, CustomerVehicleID, LocationParkingLotID);
                    GetVehicleDueAmount(CustomerVehicleID);
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
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
                        DueAmount = resultObj.DueAmount;
                        DueCustomerParkingSlotID = resultObj.DueCustomerParkingSlotID;

                        if (DueAmount > 0)
                        {
                            lblDue.Text = Convert.ToString(Convert.ToDecimal(DueAmount));
                            lblTotal.Text = "₹" + Convert.ToString(Convert.ToDouble(DueAmount + (Convert.ToDecimal(lblParkingFee.Text))).ToString("N2"));
                        }
                        else
                        {
                            lblDue.Text = Convert.ToString(Convert.ToDecimal(0));
                            lblTotal.Text = "₹" + Convert.ToString(Convert.ToDouble(DueAmount + (Convert.ToDecimal(lblParkingFee.Text))).ToString("N2"));
                        }
                    }
                    else
                    {
                        DueAmount = 0;
                        DueCustomerParkingSlotID = resultObj.DueCustomerParkingSlotID;
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
                    await DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
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
                return;
            }
        }
        private void btn_TwoClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(2);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                btnThree.BackgroundColor = Xamarin.Forms.Color.Transparent;
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void btn_ThreeClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(3);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void btn_FourClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(4);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void btn_FiveClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(5);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void btn_SixClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(6);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private void btn_FullDayClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                Duration = Convert.ToInt32(7);

                btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
                btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

                btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
                btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

                GetParkingFee(LocationParkingLotID, Duration);
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        public void GetParkingFee(int locationParkingLotID, int duration)
        {
            try
            {
                ParkingDetails objParkingDetails = new ParkingDetails();
                objParkingDetails.LocationParkingLotID = locationParkingLotID;
                objParkingDetails.VehicleTypeID = VehicleTypeID;
                objParkingDetails.ParkingHours = duration;
                objParkingDetails.Amount = 0;

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objParkingDetails);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    ParkingFee resultObj = dal_LocationParkingLot.GetParkingFee(Convert.ToString(App.Current.Properties["apitoken"]), objParkingDetails);

                    if (resultObj.VehicleTypeID != 0)
                    {
                        lblHourlyPrice.Text = resultObj.HourlyPrice;

                        double duration_double = 0;
                        currentTime = DateTime.Now;

                        DateTime LotOpenTime = DateTime.Parse(resultObj.LotOpenTime);
                        DateTime LotCloseTime = DateTime.Parse(resultObj.LotCloseTime);

                        objCustomerParkingSlotDetails.LotOpenTime = LotOpenTime;
                        objCustomerParkingSlotDetails.LotCloseTime = LotCloseTime;

                        if (currentTime >= LotCloseTime)
                        {
                            lblExpiryMessage.Text = Convert.ToString(LotCloseTime.ToString("hh:mm tt"));
                            stkExpiry.IsVisible = false;
                            stkPrice.IsVisible = false;
                            btnPay.IsVisible = false;
                            btnCheckIn.IsVisible = false;
                            DisplayAlert("", "Parking Lot closes at " + Convert.ToString(Convert.ToDateTime(LotCloseTime).ToString("hh:mm tt")).ToLower() + " today", "Ok");
                            return;
                        }
                        else
                        {
                            TimeSpan ts = LotCloseTime - currentTime;
                            duration_double = ts.TotalHours;
                            stkTiming.IsVisible = duration_double <= 2 ? false : true;

                            stkTimingFlag = stkTiming.IsVisible;

                            duration = stkTimingFlag ? duration : Duration;
                        }

                        ExpectedEndTime_date = currentTime.AddHours(duration) <= LotCloseTime ? currentTime.AddHours(duration) : LotCloseTime;
                        lblExpiryMessage.Text = ExpectedEndTime_date.ToString("hh:mm tt");
                        //lblParkingFee.Text = Convert.ToString(resultObj.Price.ToString("N2"));
                        lblParkingFee.Text = resultObj.Price.ToString("N");
                        lblDue.Text = Convert.ToString(DueAmount);
                        lblTotal.Text = "₹" + Convert.ToString((DueAmount + resultObj.Price).ToString("N2"));
                        Duration = duration;
                        IsFullDay = resultObj.IsFullDay;

                        btnPay.IsVisible = true;
                    }
                    else
                    {
                        btnPay.IsVisible = false;
                        btnCheckIn.IsVisible = false;
                        lblParkingFee.Text = "0";
                        lblTotal.Text = "₹" + "0";
                        lblDue.Text = Convert.ToString(0);
                        lblExpiryMessage.Text = Convert.ToString(ExpectedEndTime_date.ToString("hh:mm tt"));
                        DisplayAlert("Result - GetParkingFee", "Pricing and lot timings not available", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                btnPay.IsVisible = false;
                lblParkingFee.Text = "0";
                lblTotal.Text = "₹" + "0";
                lblDue.Text = "0";
                DisplayAlert("Failed - GetParkingFee", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void btn_PayClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                btnPay.IsVisible = false;
                ShowLoading(true);

                if (CapacityColour.ToUpper() == "RED")
                {
                    btnPay.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Sorry, '" + Convert.ToString(LocationParkingLotName) + "' lot is full. Please select another lot to park your vehicle", "Ok");
                    return;
                }

                if (pkBayNumber.SelectedIndex == -1 || pkBayNumber.SelectedIndex == null)
                {
                    btnPay.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please select Spot/Bay Number", "Ok");
                    return;
                }

                if (CustomerVehicleID == 0)
                {
                    btnPay.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please select Registration Number of your Vehicle", "Ok");
                    return;
                }

                try
                {
                    if (ValidateVehicleCheckIn())
                    {
                        btnPay.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Vehicle Number - " + Convert.ToString(RegistrationNumber) + " Already Checked In at the location " + CheckedInLocationName + ", In " + CheckedInLotName + " lot ", "Ok");
                    }
                    else
                    {
                        double duration_double = 0;
                        currentTime = DateTime.Now;

                        DateTime LotOpenTime = objCustomerParkingSlotDetails.LotOpenTime;
                        DateTime LotCloseTime = objCustomerParkingSlotDetails.LotCloseTime;

                        if (currentTime >= LotCloseTime)
                        {
                            lblExpiryMessage.Text = Convert.ToString(LotCloseTime.ToString("hh:mm tt"));
                            stkExpiry.IsVisible = false;
                            stkPrice.IsVisible = false;
                            btnPay.IsVisible = false;
                            btnCheckIn.IsVisible = false;
                            ShowLoading(false);
                            await DisplayAlert("", "Parking Lot closes at " + Convert.ToString(Convert.ToDateTime(LotCloseTime).ToString("hh:mm tt")).ToLower() + " today", "Ok");
                            return;
                        }
                        else
                        {
                            TimeSpan ts = LotCloseTime - currentTime;
                            duration_double = ts.TotalHours;
                            stkTiming.IsVisible = duration_double <= 2 ? false : true;

                            stkTimingFlag = stkTiming.IsVisible;

                            if (Duration <= Math.Ceiling(duration_double))
                            {
                                ExpectedEndTime_date = currentTime.AddHours(Duration) <= LotCloseTime ? currentTime.AddHours(Duration) : LotCloseTime;
                                lblExpiryMessage.Text = ExpectedEndTime_date.ToString("hh:mm tt");


                                if (IsFullDay)
                                {
                                    ts = LotCloseTime - ExpectedEndTime_date;
                                    Duration = ts.Hours;
                                    ExpectedEndTime_date = LotCloseTime;
                                }
                            }
                            else
                            {
                                ExpectedEndTime_date = LotCloseTime;
                                lblExpiryMessage.Text = ExpectedEndTime_date.ToString("hh:mm tt");
                                //btnPay.IsVisible = false;
                                // btnCheckIn.IsVisible = false;
                                //ShowLoading(false);
                                //await DisplayAlert("", "Parking Lot closes at " + Convert.ToString(Convert.ToDateTime(LotCloseTime).ToString("hh:mm tt")).ToLower() + " today. Please select proper timings", "Ok");
                                //return;
                            }
                        }

                        CustomerParkingSlot obj_CustomerParkingSlot = new CustomerParkingSlot();
                        obj_CustomerParkingSlot.CustomerParkingSlotID = CustomerParkingSlotID;
                        obj_CustomerParkingSlot.CustomerID = CustomerID;
                        obj_CustomerParkingSlot.VehicleTypeID = VehicleTypeID;
                        obj_CustomerParkingSlot.PaymentTypeID = Convert.ToInt32(App.Current.Properties["PaymentTypeID"]);
                        obj_CustomerParkingSlot.LocationParkingLotID = LocationParkingLotID;
                        obj_CustomerParkingSlot.LocationParkingLotName = LocationParkingLotName;

                        if (currentTime <= LotOpenTime)
                        {
                            obj_CustomerParkingSlot.ExpectedStartTime = LotOpenTime.ToString("MM/dd/yyyy hh:mm tt");
                            obj_CustomerParkingSlot.ExpectedEndTime = LotOpenTime.AddHours(Duration).ToString("MM/dd/yyyy hh:mm tt");
                        }
                        else
                        {
                            obj_CustomerParkingSlot.ExpectedStartTime = currentTime.ToString("MM/dd/yyyy hh:mm tt");
                            obj_CustomerParkingSlot.ExpectedEndTime = ExpectedEndTime_date.ToString("MM/dd/yyyy hh:mm tt");
                        }

                        obj_CustomerParkingSlot.ActualStartTime = obj_CustomerParkingSlot.ExpectedStartTime;
                        obj_CustomerParkingSlot.ActualEndTime = obj_CustomerParkingSlot.ExpectedEndTime;
                        obj_CustomerParkingSlot.ExtendAmount = 0;

                        obj_CustomerParkingSlot.Duration = Convert.ToString(Duration);

                        obj_CustomerParkingSlot.TransactionID = "0";
                        obj_CustomerParkingSlot.StatusID = Convert.ToInt32(App.Current.Properties["StatusID"]);
                        obj_CustomerParkingSlot.CustomerVehicleID = CustomerVehicleID;
                        obj_CustomerParkingSlot.ApplicationTypeID = Convert.ToInt32(App.Current.Properties["ApplicationTypeID"]);

                        obj_CustomerParkingSlot.ParkingBayID = ParkingBayID;

                        if (DueAmount > 0)
                        {
                            obj_CustomerParkingSlot.Amount = Convert.ToDecimal(lblParkingFee.Text) + DueAmount;
                            obj_CustomerParkingSlot.PaidAmount = Convert.ToDecimal(lblParkingFee.Text) + DueAmount;
                            obj_CustomerParkingSlot.DueCustomerParkingSlotID = DueCustomerParkingSlotID;
                            obj_CustomerParkingSlot.PaidDueAmount = DueAmount;
                            obj_CustomerParkingSlot.IsDueAmountPaid = true;
                            obj_CustomerParkingSlot.DueAmountPaidOn = currentTime.ToString("MM/dd/yyyy hh:mm tt");
                        }
                        else
                        {
                            obj_CustomerParkingSlot.Amount = Convert.ToDecimal(lblParkingFee.Text) + DueAmount;
                            obj_CustomerParkingSlot.PaidAmount = Convert.ToDecimal(lblParkingFee.Text) + DueAmount;
                            obj_CustomerParkingSlot.DueCustomerParkingSlotID = DueCustomerParkingSlotID;
                            obj_CustomerParkingSlot.PaidDueAmount = 0;
                            obj_CustomerParkingSlot.IsDueAmountPaid = false;
                            obj_CustomerParkingSlot.DueAmountPaidOn = currentTime.ToString("MM/dd/yyyy hh:mm tt");
                        }

                        btnPay.IsVisible = true;
                        ShowLoading(false);

                        //19062021 Tried code razorpay Start
                        RazorPayment pageRazorPayment = null;
                        ShowLoading(true);
                        await Task.Run(() =>
                        {
                            pageRazorPayment = new RazorPayment(obj_CustomerParkingSlot, null);
                        });
                        await Navigation.PushAsync(pageRazorPayment);
                        ShowLoading(false);


                        // RazorPayload payload = new RazorPayload();
                        //payload.amount = Convert.ToInt32(obj_CustomerParkingSlot.PaidAmount) * 100;
                        //payload.currency = "INR";
                        //payload.receipt = GenerateRefNo();

                        //RazorUserDetails userdetials_Razor = new RazorUserDetails();
                        //if (App.Current.Properties.ContainsKey("UserName"))
                        //{
                        //    userdetials_Razor.UserName = Convert.ToString(App.Current.Properties["UserName"]);
                        //}
                        //if (App.Current.Properties.ContainsKey("Email"))
                        //{
                        //    userdetials_Razor.Email = Convert.ToString(App.Current.Properties["Email"]);
                        //}
                        //if (App.Current.Properties.ContainsKey("PhoneNumber"))
                        //{
                        //    userdetials_Razor.PhoneNumber = Convert.ToString(App.Current.Properties["PhoneNumber"]);
                        //}
                        //userdetials_Razor.Description = Convert.ToString(obj_CustomerParkingSlot.LocationParkingLotName);
                        //userdetials_Razor.PayFor = "Parking";
                        //MessagingCenter.Send<RazorPayload, RazorUserDetails>(payload, "PayNow", userdetials_Razor);



                        //MessagingCenter.Subscribe<PaymentDataOutput>(this, "PaySuccess", (paymentData) =>
                        //    {
                        //        string transactionID = string.Empty;
                        //        transactionID = paymentData.PaymentId;
                        //        transactionDuplicate = paymentData.PaymentId;

                        //        if (obj_CustomerParkingSlot.LocationName != null && obj_CustomerParkingSlot.LocationName != "")
                        //        {
                        //            if (obj_CustomerParkingSlot.LocationName.ToUpper() == "CHECKOUT")
                        //            {
                        //                if (transactionID == "")
                        //                {
                        //                    DisplayAlert("", "Payment Failed", "Ok");
                        //                    App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                        //                    App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //                    return;
                        //                }

                        //                obj_CustomerParkingSlot.TransactionID = transactionID;
                        //                var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                        //                var content = new StringContent(json, Encoding.UTF8, "application/json");
                        //                CustomerParkingSlotDetails resultObj = dal_Customer.UpdateCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        //                if (resultObj.CustomerParkingSlotID != 0)
                        //                {
                        //                    App.Current.Properties["CustomerParkingSlotID"] = "0";
                        //                    App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //                    Application.Current.SavePropertiesAsync();
                        //                    DisplayAlert("", "Thank you for parking with us!", "Ok");
                        //                    int CustomerID = 0;
                        //                    if (App.Current.Properties.ContainsKey("CustomerID"))
                        //                    {
                        //                        CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        //                    }

                        //                    Navigation.PushAsync(new Home(null, CustomerID));
                        //                }
                        //                else
                        //                {
                        //                    DisplayAlert("", "Insert Payment Failed", "Ok");
                        //                    App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //                    Application.Current.SavePropertiesAsync();
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (transactionID == "")
                        //                {
                        //                    DisplayAlert("", "Payment Failed", "Ok");
                        //                    App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                        //                    App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //                    return;
                        //                }

                        //                obj_CustomerParkingSlot.TransactionID = transactionID;
                        //                var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                        //                var content = new StringContent(json, Encoding.UTF8, "application/json");
                        //                CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        //                if (resultObj.CustomerParkingSlotID != 0)
                        //                {
                        //                    App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                        //                    App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //                    Application.Current.SavePropertiesAsync();
                        //                    transactionID = "";
                        //                    Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                        //                }
                        //                else
                        //                {
                        //                    DisplayAlert("", "Insert Payment Failed", "Ok");
                        //                    App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //                    Application.Current.SavePropertiesAsync();
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (transactionID == "")
                        //            {
                        //                DisplayAlert("", "Payment Failed", "Ok");
                        //                App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                        //                App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //                return;
                        //            }

                        //            obj_CustomerParkingSlot.TransactionID = transactionID;
                        //            var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                        //            var content = new StringContent(json, Encoding.UTF8, "application/json");
                        //            CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        //            if (resultObj.CustomerParkingSlotID != 0)
                        //            {
                        //                App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                        //                App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //                Application.Current.SavePropertiesAsync();
                        //                transactionID = "";
                        //                Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                        //            }
                        //            else
                        //            {
                        //                DisplayAlert("", "Insert Payment Failed", "Ok");
                        //                App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //                Application.Current.SavePropertiesAsync();
                        //            }
                        //        }
                        //        MessagingCenter.Unsubscribe<PaymentDataOutput>(this, "PaySuccess");
                        //        //if (paymentData.PaymentId != "" && paymentData.PaymentId != null)
                        //        //{
                        //        //    CustomerParkingSlotDetails objCustomerParkingSlotDetails = new CustomerParkingSlotDetails();
                        //        //    obj_CustomerParkingSlot.TransactionID = paymentData.PaymentId;
                        //        //    var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                        //        //    var content = new StringContent(json, Encoding.UTF8, "application/json");
                        //        //    CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        //        //    if (resultObj.CustomerParkingSlotID != 0)
                        //        //    {
                        //        //        App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                        //        //        App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //        //        Application.Current.SavePropertiesAsync();
                        //        //        Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                        //        //    }
                        //        //    else
                        //        //    {
                        //        //        DisplayAlert("", "Insert Payment Failed", "Ok");
                        //        //        App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                        //        //        Application.Current.SavePropertiesAsync();
                        //        //    }                                
                        //        //}
                        //        //else
                        //        //{
                        //        //    DisplayAlert("", "Payment Failed", "Ok");
                        //        //    App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                        //        //    return;
                        //        //}

                        //    });



                        ////19062021 Tried code razorpay End
                    }
                }
                catch (Exception ex)
                {
                    btnPay.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("Failed - Pay", Convert.ToString(ex.Message), "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        public void GetCustomerVehiclePassDetails(int locationID, int customerVehicleID, int locationParkingLotID)
        {
            try
            {
                CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                objCustomerVehicle.LocationID = locationID;
                objCustomerVehicle.CustomerVehicleID = customerVehicleID;
                objCustomerVehicle.LocationParkingLotID = locationParkingLotID;

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    OCustomerVehiclePass resultObj = dal_Pass.GetCustomerVehiclePassDetails(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);
                    objOCustomerVehiclePass = resultObj;

                    if (resultObj.CustomerVehiclePassID != 0)
                    {
                        currentTime = DateTime.Now;

                        DateTime LotOpenTime = DateTime.Parse(resultObj.LotOpenTime);
                        DateTime LotCloseTime = DateTime.Parse(resultObj.LotCloseTime);

                        objCustomerParkingSlotDetails.LotOpenTime = LotOpenTime;
                        objCustomerParkingSlotDetails.LotCloseTime = LotCloseTime;

                        if (currentTime >= LotCloseTime)
                        {
                            lblExpiryMessage.Text = Convert.ToString(LotCloseTime.ToString("hh:mm tt"));
                            stkExpiry.IsVisible = false;
                            stkPrice.IsVisible = false;
                            btnPay.IsVisible = false;
                            btnCheckIn.IsVisible = false;
                            DisplayAlert("", "Parking Lot closes at " + Convert.ToString(Convert.ToDateTime(LotCloseTime).ToString("hh:mm tt")).ToLower() + " today", "Ok");
                            return;
                        }

                        DisplayAlert("", "Vehicle number " + RegistrationNumber + " has a valid pass.Please Check In", "Ok");
                        btnCheckIn.IsVisible = true;
                        stkChoosePark.IsVisible = false;
                        gridTiming.IsVisible = false;
                        stkExpiry.IsVisible = false;
                        stkPrice.IsVisible = true;
                        btnPay.IsVisible = false;
                        stkPrice.IsVisible = false;
                        lblParkingFee.Text = "0";
                        lblTotal.Text = "₹" + "0";
                        lblDue.Text = "0";
                    }
                    else
                    {
                        btnCheckIn.IsVisible = false;

                        stkChoosePark.IsVisible = true;
                        gridTiming.IsVisible = true;
                        stkExpiry.IsVisible = true;
                        stkPrice.IsVisible = true;
                        btnPay.IsVisible = true;
                        stkPrice.IsVisible = true;
                        lblParkingFee.Text = "0";
                        lblTotal.Text = "₹" + "0";
                        lblDue.Text = Convert.ToString(0);
                        GetParkingFee(locationParkingLotID, Duration);
                    }

                    //GetParkingBayByLotID(locationParkingLotID);
                }
            }
            catch (Exception ex)
            {
                btnPay.IsVisible = false;
                stkPrice.IsVisible = false;

                btnCheckIn.IsVisible = false;

                stkChoosePark.IsVisible = false;
                gridTiming.IsVisible = false;
                stkExpiry.IsVisible = false;
                stkPrice.IsVisible = false;
                lblParkingFee.Text = "0";
                lblTotal.Text = "₹" + "0";
                lblDue.Text = "0";
                DisplayAlert("Failed - GetCustomerVehiclePassDetails", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void btn_CheckInClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                btnCheckIn.IsVisible = false;

                if (CapacityColour.ToUpper() == "RED")
                {
                    btnCheckIn.IsVisible = true;
                    DisplayAlert("", "Sorry, '" + Convert.ToString(LocationParkingLotName) + "' lot is full. Please select another lot to park your vehicle", "Ok");
                    return;
                }

                if (RegistrationNumber == "" || RegistrationNumber == null)
                {
                    btnCheckIn.IsVisible = true;
                    DisplayAlert("", "Please enter Registration Number of your Vehicle", "Ok");
                    return;
                }

                if (pkBayNumber.SelectedIndex == -1 || pkBayNumber.SelectedIndex == null)
                {
                    btnCheckIn.IsVisible = true;
                    DisplayAlert("", "Please select Spot/Bay Number", "Ok");
                    return;
                }

                if (ValidateVehicleCheckIn())
                {
                    btnCheckIn.IsVisible = true;
                    DisplayAlert("", "Vehicle Number - " + Convert.ToString(RegistrationNumber) + " Already Checked In At The Location " + CheckedInLocationName + ", In " + CheckedInLotName + " Lot ", "Ok");
                }
                else
                {
                    CustomerParkingSlot obj_CustomerParkingSlot = new CustomerParkingSlot();
                    obj_CustomerParkingSlot.CustomerParkingSlotID = CustomerParkingSlotID;
                    obj_CustomerParkingSlot.CustomerID = CustomerID;
                    obj_CustomerParkingSlot.VehicleTypeID = VehicleTypeID;
                    obj_CustomerParkingSlot.PaymentTypeID = Convert.ToInt32(App.Current.Properties["PaymentTypeID"]);
                    obj_CustomerParkingSlot.LocationParkingLotID = LocationParkingLotID;

                    DateTime LotOpenTime = DateTime.Parse(objOCustomerVehiclePass.LotOpenTime);
                    DateTime LotCloseTime = DateTime.Parse(objOCustomerVehiclePass.LotCloseTime);

                    currentTime = DateTime.Now;

                    if (currentTime <= LotOpenTime)
                    {
                        obj_CustomerParkingSlot.ExpectedStartTime = LotOpenTime.ToString("MM/dd/yyyy hh:mm tt");
                        TimeSpan ts = LotCloseTime - LotOpenTime;
                        obj_CustomerParkingSlot.Duration = Convert.ToString(ts.Hours);
                    }
                    else
                    {
                        obj_CustomerParkingSlot.ExpectedStartTime = currentTime.ToString("MM/dd/yyyy hh:mm tt");
                        TimeSpan ts = LotCloseTime - currentTime;
                        obj_CustomerParkingSlot.Duration = Convert.ToString(ts.Hours);
                    }

                    obj_CustomerParkingSlot.ExpectedEndTime = LotCloseTime.ToString("MM/dd/yyyy hh:mm tt");

                    obj_CustomerParkingSlot.ActualStartTime = obj_CustomerParkingSlot.ExpectedStartTime;
                    obj_CustomerParkingSlot.ActualEndTime = LotCloseTime.ToString("MM/dd/yyyy hh:mm tt");

                    obj_CustomerParkingSlot.ExtendAmount = 0;
                    obj_CustomerParkingSlot.Amount = 0;
                    obj_CustomerParkingSlot.TransactionID = "0";
                    obj_CustomerParkingSlot.StatusID = Convert.ToInt32(App.Current.Properties["StatusID"]);
                    obj_CustomerParkingSlot.CustomerVehicleID = CustomerVehicleID;
                    obj_CustomerParkingSlot.ApplicationTypeID = Convert.ToInt32(App.Current.Properties["PassApplicationTypeID"]);
                    obj_CustomerParkingSlot.PaidAmount = 0;
                    obj_CustomerParkingSlot.ParkingBayID = ParkingBayID;

                    obj_CustomerParkingSlot.DueCustomerParkingSlotID = 0;
                    obj_CustomerParkingSlot.PaidDueAmount = 0;
                    obj_CustomerParkingSlot.IsDueAmountPaid = false;
                    obj_CustomerParkingSlot.DueAmountPaidOn = currentTime.ToString("MM/dd/yyyy hh:mm tt");


                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        if (resultObj.CustomerParkingSlotID != 0)
                        {
                            btnCheckIn.IsVisible = true;
                            ShowLoading(true);
                            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                            await SecureStorage.SetAsync("CustomerParkingSlotID", Convert.ToString(resultObj.CustomerParkingSlotID));
                            SessionHomeReceipt pageSessionHomeReceipt = null;
                            await Task.Run(() =>
                            {
                                pageSessionHomeReceipt = new SessionHomeReceipt(resultObj);
                            });
                            await Navigation.PushAsync(pageSessionHomeReceipt);
                            ShowLoading(false);
                        }
                        else
                        {
                            btnCheckIn.IsVisible = true;
                            DisplayAlert("", "Payment Failed", "Ok");
                        }
                    }
                    else
                    {
                        btnCheckIn.IsVisible = true;
                    }
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        public bool ValidateVehicleCheckIn()
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (CustomerParkingSlotID == 0)
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                            objCustomerVehicle.CustomerVehicleID = CustomerVehicleID;

                            var json = JsonConvert.SerializeObject(objCustomerVehicle);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            dal_Vehicle = new DALVehicle();
                            CustomerParkingSlot resultObj = dal_Vehicle.ValidateVehicleCheckIn(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);

                            if (resultObj.CustomerParkingSlotID != 0)
                            {
                                CheckedInLocationName = resultObj.LocationName;
                                CheckedInLotName = resultObj.LocationParkingLotName;
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
                        return false;
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - ValidateVehicleCheckIn", Convert.ToString(ex.Message), "Ok");
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
                DueAmount = 0;
                DueCustomerParkingSlotID = 0;
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetParkingFee(LocationParkingLotID, 2);
                TwoHoursSelect();
                CustomerVehicleID = 0;
                RegistrationNumber = "";
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
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
                DueAmount = 0;
                DueCustomerParkingSlotID = 0;
                GetListOfCustomerVehicleWithType(VehicleTypeID);
                GetParkingFee(LocationParkingLotID, 2);
                TwoHoursSelect();
                CustomerVehicleID = 0;
                RegistrationNumber = "";
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        public void TwoHoursSelect()
        {
            btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
            btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

            btnThree.BackgroundColor = Xamarin.Forms.Color.Transparent;
            btnThree.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            btnFour.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnFour.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            btnFive.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnFive.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            btnSix.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnSix.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);
                Content.TranslationX = Math.Min(0, Math.Max(targetX, -Content.Width * (currentScale - 1)));
                Content.TranslationY = Math.Min(0, Math.Max(targetY, -Content.Height * (currentScale - 1)));
                Content.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }
        private void LotOneImageTapped(object sender, EventArgs e)
        {
            popupImageView.IsVisible = true;
            popupImageView2.IsVisible = false;
            popupImageView3.IsVisible = false;
            // imgPopup.Source = ImageSource.FromResource("ParkHyderabad.Resources.Android.png"); 
        }
        private void LotTwoImageTapped(object sender, EventArgs e)
        {
            popupImageView2.IsVisible = true;
            popupImageView.IsVisible = false;
            popupImageView3.IsVisible = false;
        }
        private void LotThreeImageTapped(object sender, EventArgs e)
        {
            popupImageView3.IsVisible = true;
            popupImageView.IsVisible = false;
            popupImageView2.IsVisible = false;
        }
        private void ClosePopup1(object sender, EventArgs e)
        {
            popupImageView.IsVisible = false;
        }
        private void ClosePopup2(object sender, EventArgs e)
        {
            popupImageView2.IsVisible = false;
        }
        private void ClosePopup3(object sender, EventArgs e)
        {
            popupImageView3.IsVisible = false;
        }
        public string GenerateRefNo()
        {
            string refNo = "";
            Random ran = new Random();
            string b = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int length = 6;
            string random = "";
            for (int i = 0; i < length; i++)
            {
                int a = ran.Next(26);
                random = random + b.ElementAt(a);
            }
            int d = ran.Next(100000, 999999);

            refNo = $"{random}{d}";
            return refNo;
        }
    }
}