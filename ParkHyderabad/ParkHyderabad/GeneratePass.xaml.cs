using ParkHyderabad.DAL;
using ParkHyderabad.Model;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneratePass : ContentPage
    {
        string PassType = string.Empty;
        static int price;
        static string LocationName;
        static bool IsMultiLot = false;
        static OCustomerVehicle objCustomerVehicle;
        static PassPrice objpassPrice;
        static DateTime StartDate;
        static DateTime ExpiryDate;
        DALPass dal_Pass;
        List<Location> objLocation;
        List<Location> lst_Location;
        Location objLoc = new Location();
        bool IsMulti;

        static bool IsOnline = false;

        public GeneratePass(PassPrice obj_passPrice, List<Location> location, OCustomerVehicle obj_CustomerVehicle, bool isMulti)
        {
            InitializeComponent();
            ShowLoading(true);

            lst_Location = new List<Location>();

            objCustomerVehicle = obj_CustomerVehicle;
            objLocation = location;
            objpassPrice = obj_passPrice;

            lblPriceDisplay.Text = "₹ " + Convert.ToString(Convert.ToDouble(objpassPrice.Price + objpassPrice.DueAmount)) + " /-";

            this.BindingContext = objpassPrice;

            PassType = obj_passPrice.PassTypeName.ToUpper();

            dal_Pass = new DALPass();
            IsMulti = isMulti;
            if (isMulti)
            {
                string str = string.Empty;
                for (int i = 0; i < location.Count; i++)
                {
                    str = str + location[i].LocationName + ", ";

                    objLoc = new Location();
                    objLoc.LocationNumber = i + 1;
                    objLoc.LocationName = location[i].LocationName;
                    lst_Location.Add(objLoc);
                }
                LocationName = str.Substring(0, str.Length - 2).Trim();
            }
            else
            {
                LocationName = location[0].LocationName;

                objLoc = new Location();
                objLoc.LocationNumber = 1;
                objLoc.LocationName = location[0].LocationName;
                lst_Location.Add(objLoc);
            }

            lstLocation.ItemsSource = lst_Location;
            lblModel.Text = obj_CustomerVehicle.Model;
            lblRegistrationNumber.Text = obj_CustomerVehicle.RegistrationNumber;
            imgVehicle.Source = obj_CustomerVehicle.VehicleImage;

            DateTime startDate = DateTime.Now;

            if (obj_passPrice.PassCode.ToUpper() == "DP")
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
                StartDate = startDate;
                ExpiryDate = startDate;
            }
            else if (obj_passPrice.PassCode.ToUpper() == "2W WP" || obj_passPrice.PassCode.ToUpper() == "4W WP")
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
                StartDate = startDate;
                ExpiryDate = startDate.AddDays((Convert.ToInt32(obj_passPrice.Duration) - 1));
            }
            else if (obj_passPrice.PassCode.ToUpper() == "2W ALL STATION" || obj_passPrice.PassCode.ToUpper() == "4W ALL STATION")
            {
                IsMultiLot = true;
                lblLineStation.IsVisible = false;
                lblStation.IsVisible = false;
                lblAllStation.IsVisible = true;
                lstLocation.IsVisible = false;
                StartDate = startDate;
                ExpiryDate = startDate.AddDays((Convert.ToInt32(obj_passPrice.Duration) - 1));
            }
            else
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
                StartDate = startDate;
                ExpiryDate = startDate.AddDays((Convert.ToInt32(obj_passPrice.Duration) - 1));
            }

            if (obj_passPrice.PassTypeCode == "EP")// Event Pass
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;

                obj_passPrice.Duration = (obj_passPrice.Duration == null || obj_passPrice.Duration == "0" || obj_passPrice.Duration == "") ? "0" : obj_passPrice.Duration;

                // EP Start Date Validation
                if (DateTime.Now.Date >= Convert.ToDateTime(obj_passPrice.StartDate).Date && DateTime.Now.Date <= Convert.ToDateTime(obj_passPrice.EndDate).Date)
                {
                    obj_passPrice.StartDate = DateTime.Now.Date;
                }

                StartDate = Convert.ToDateTime(obj_passPrice.StartDate);
                ExpiryDate = Convert.ToDateTime(obj_passPrice.EndDate);
            }

            lblFrom.Text = StartDate.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
            lblTo.Text = ExpiryDate.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
            string fromdatesplit = lblFrom.Text.Split(',')[0];
            string fromtimesplit = lblFrom.Text.Split(',')[1];
            lblFrom.Text = fromdatesplit;
            lblFromTime.Text = "06:00 AM";
            string toDatesplit = lblTo.Text.Split(',')[0];
            string toTimesplit = lblTo.Text.Split(',')[1];
            lblTo.Text = toDatesplit;
            lblToTime.Text = "10:00 PM";

            price = obj_passPrice.Price;

            ShowLoading(false);

        }
        private async void btn_PayClicked(object sender, EventArgs e)
        {
            ShowLoading(true);

            try
            {
                CustomerVehiclePass obj_CustomerVehiclePass = new CustomerVehiclePass();
                obj_CustomerVehiclePass.CustomerVehicleID = objCustomerVehicle.CustomerVehicleID;
                obj_CustomerVehiclePass.CustomerID = objCustomerVehicle.CustomerID;
                obj_CustomerVehiclePass.PaymentTypeID = Convert.ToInt32(App.Current.Properties["PaymentTypeID"]);
                obj_CustomerVehiclePass.IsMultiLot = IsMulti;
                obj_CustomerVehiclePass.StartDate = StartDate;
                obj_CustomerVehiclePass.ExpiryDate = ExpiryDate;
                obj_CustomerVehiclePass.IssuedCard = false;
                obj_CustomerVehiclePass.CardNumber = null;
                obj_CustomerVehiclePass.Amount = price;
                obj_CustomerVehiclePass.CardAmount = 0;

                obj_CustomerVehiclePass.TotalAmount = price;

                obj_CustomerVehiclePass.TransactionID = "";
                obj_CustomerVehiclePass.StatusID = 1;
                obj_CustomerVehiclePass.PassTypeID = objpassPrice.PassTypeID;
                obj_CustomerVehiclePass.PassPriceID = objpassPrice.PassPriceID;
                obj_CustomerVehiclePass.lstLocation = objLocation;
                obj_CustomerVehiclePass.StationAccess = objpassPrice.PassTypeName + " " + objpassPrice.StationAccess;

                if (objpassPrice.DueAmount > 0)
                {
                    obj_CustomerVehiclePass.DueCustomerParkingSlotID = objpassPrice.DueCustomerParkingSlotID;
                    obj_CustomerVehiclePass.PaidDueAmount = objpassPrice.DueAmount;
                    obj_CustomerVehiclePass.IsDueAmountPaid = true;
                    obj_CustomerVehiclePass.DueAmountPaidOn = DateTime.Now.Date.ToString("MM/dd/yyyy hh:mm tt"); ;
                    //obj_CustomerVehiclePass.TotalAmount = price + objpassPrice.DueAmount;
                }
                else
                {
                    obj_CustomerVehiclePass.DueCustomerParkingSlotID = 0;
                    obj_CustomerVehiclePass.PaidDueAmount = 0;
                    obj_CustomerVehiclePass.IsDueAmountPaid = false;
                    obj_CustomerVehiclePass.DueAmountPaidOn = DateTime.Now.Date.ToString("MM/dd/yyyy hh:mm tt"); ;
                    //obj_CustomerVehiclePass.TotalAmount = price;
                }

                ShowLoading(false);
                ShowLoading(true);
                RazorPayment pageRazorPayment = null;
                await Task.Run(() =>
                {
                    pageRazorPayment = new RazorPayment(null, obj_CustomerVehiclePass);
                });

                await Navigation.PushAsync(pageRazorPayment);
                ShowLoading(false);


                ////19062021 Tried code razorpay Start
                //RazorPayload payload = new RazorPayload();
                //payload.amount = Convert.ToInt32(obj_CustomerVehiclePass.TotalAmount + obj_CustomerVehiclePass.PaidDueAmount) * 100;
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
                //userdetials_Razor.Description = Convert.ToString(obj_CustomerVehiclePass.StationAccess);
                //userdetials_Razor.PayFor = "Pass";
                //MessagingCenter.Send<RazorPayload, RazorUserDetails>(payload, "PayNow", userdetials_Razor);

                //MessagingCenter.Subscribe<PassPaymentData>(this, "PaySuccess", (passpaymetData) =>
                //{
                //    if (passpaymetData.PaymentId != "" && passpaymetData.PaymentId != null)
                //    {
                //        obj_CustomerVehiclePass.TransactionID = passpaymetData.PaymentId;
                //        var json = JsonConvert.SerializeObject(obj_CustomerVehiclePass);
                //        var content = new StringContent(json, Encoding.UTF8, "application/json");
                //        OCustomerVehiclePass resultObj;

                //        if (obj_CustomerVehiclePass.IsMultiLot)
                //        {
                //            resultObj = dal_Pass.InsertMultiStationCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehiclePass);
                //        }
                //        else
                //        {
                //            resultObj = dal_Pass.InsertCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehiclePass);
                //        }

                //        if (resultObj.CustomerVehiclePassID != 0)
                //        {
                //            App.Current.Properties["CustomerID"] = obj_CustomerVehiclePass.CustomerID;
                //            Navigation.PushAsync(new PassReceipt(resultObj));
                //        }
                //        else
                //        {
                //            DisplayAlert("", "Insert Payment Failed", "Ok");
                //            App.Current.Properties["CustomerID"] = obj_CustomerVehiclePass.CustomerID;
                //             Application.Current.SavePropertiesAsync();
                //        }
                //    }
                //    else
                //    {
                //        DisplayAlert("", "Payment Failed", "Ok");
                //        App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerVehiclePass.CustomerID);
                //        return;
                //    }
                //    MessagingCenter.Unsubscribe<PassPaymentData>(this, "PaySuccess");
                //});
                ////19062021 Tried code razorpay End
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                DisplayAlert("Failed", Convert.ToString(ex.Message), "Ok");
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