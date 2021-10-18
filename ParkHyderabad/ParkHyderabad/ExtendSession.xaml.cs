using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendSession : ContentPage
    {
        DALLocationParkingLots dal_LocationParkingLot;
        DALCustomer dal_Customer;
        DALVehicle dal_Vehicle;
        DALPass dal_Pass;
        CustomerParkingSlotDetails objCustomerParkingSlotDetails;
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
        static int Duration;
        DateTime currentTime = DateTime.Now;
        decimal Latitude;
        decimal Longitude;
        OCustomerVehiclePass objOCustomerVehiclePass;
        static string CheckedInLocationName;
        static string CheckedInLotName;
        static DateTime ExpectedEndTime_date = DateTime.Now;
        static bool IsFullDay = false;
        static bool stkTimingFlag = false;

        static bool IsOnline = false;

        public ExtendSession(CustomerParkingSlotDetails customerParkingSlotDetails)
        {
            InitializeComponent();
            ShowLoading(true);
            objCustomerParkingSlotDetails = customerParkingSlotDetails;
            VehicleTypeID = customerParkingSlotDetails.VehicleTypeID;
            CustomerID = customerParkingSlotDetails.CustomerID;
            CustomerVehicleID = customerParkingSlotDetails.CustomerVehicleID;
            CustomerParkingSlotID = customerParkingSlotDetails.CustomerParkingSlotID;
            LocationParkingLotID = customerParkingSlotDetails.LocationParkingLotID;
            LocationParkingLotName = customerParkingSlotDetails.LocationParkingLotName;
            dal_LocationParkingLot = new DALLocationParkingLots();
            dal_Customer = new DALCustomer();
            dal_Vehicle = new DALVehicle();
            dal_Pass = new DALPass();

            objCustomerParkingSlotDetails.LotOpenCloseTime = ("(" +
               objCustomerParkingSlotDetails.LotOpeningTime + " - " +
               objCustomerParkingSlotDetails.LotClosingTime + ")").ToLower();

            if (VehicleTypeID == Convert.ToInt32(VehicleTypeCodes.TwoWheelerTypeID))
            {
                imgVehicle.Source = "resource://ParkHyderabad.Resources." + VehicleTypeCodes.TwoWheelerGreenSpots;
            }
            else if (VehicleTypeID == Convert.ToInt32(VehicleTypeCodes.FourWheelerTypeID))
            {
                imgVehicle.Source = "resource://ParkHyderabad.Resources." + VehicleTypeCodes.FourWheelerGreenSpots;
            }

            this.BindingContext = objCustomerParkingSlotDetails;

            VehicleTypeID = customerParkingSlotDetails.VehicleTypeID;
            CustomerID = customerParkingSlotDetails.CustomerID;
            CustomerVehicleID = customerParkingSlotDetails.CustomerVehicleID;
            RegistrationNumber = customerParkingSlotDetails.RegistrationNumber;
            CustomerParkingSlotID = customerParkingSlotDetails.CustomerParkingSlotID;
            LocationID = customerParkingSlotDetails.LocationID;
            LocationParkingLotID = customerParkingSlotDetails.LocationParkingLotID;
            LocationParkingLotName = customerParkingSlotDetails.LocationParkingLotName;
            dal_LocationParkingLot = new DALLocationParkingLots();
            dal_Customer = new DALCustomer();
            dal_Vehicle = new DALVehicle();
            dal_Pass = new DALPass();

            ParkingBayRange = customerParkingSlotDetails.ParkingBayRange;
            ParkingBayID = customerParkingSlotDetails.ParkingBayID;
            Duration = 1;
            IsFullDay = false;
            ShowLoading(false);
        }
        protected override void OnAppearing()
        {
            GetParkingFee(LocationParkingLotID, Duration);
        }
        private void btn_OneClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(1);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

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

            btnFullDay.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnFullDay.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            GetParkingFee(LocationParkingLotID, Duration);
        }
        private void btn_TwoClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(2);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

            btnTwo.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
            btnTwo.TextColor = Xamarin.Forms.Color.FromHex("#FFFFFF");

            btnThree.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
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
        private void btn_ThreeClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(3);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

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
        private void btn_FourClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(4);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

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
        private void btn_FiveClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(5);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

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
        private void btn_SixClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(6);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

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
        private void btn_FullDayClicked(object sender, EventArgs e)
        {
            Duration = Convert.ToInt32(7);

            btnOne.BackgroundColor = Xamarin.Forms.Color.FromHex("#FFFFFF");
            btnOne.TextColor = Xamarin.Forms.Color.FromHex("#010101");

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
        public void GetParkingFee(int locationParkingLotID, int duration)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    ParkingDetails objParkingDetails = new ParkingDetails();
                    objParkingDetails.LocationParkingLotID = locationParkingLotID;
                    objParkingDetails.VehicleTypeID = VehicleTypeID;
                    objParkingDetails.ParkingHours = duration;
                    objParkingDetails.Amount = Convert.ToDecimal(objCustomerParkingSlotDetails.PriceAmount);

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

                            if (objCustomerParkingSlotDetails.To >= LotCloseTime)
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
                                stkTiming.IsVisible = duration_double <= 1 ? false : true;
                                stkTimingFlag = stkTiming.IsVisible;
                                duration = stkTimingFlag ? duration : Duration;
                            }

                            ExpectedEndTime_date = objCustomerParkingSlotDetails.To.AddHours(duration) <= LotCloseTime ? objCustomerParkingSlotDetails.To.AddHours(duration) : LotCloseTime;
                            lblExpiryMessage.Text = ExpectedEndTime_date.ToString("hh:mm tt");
                            lblParkingFee.Text = Convert.ToString(resultObj.Price.ToString("N2"));
                            Duration = duration;
                            IsFullDay = resultObj.IsFullDay;

                            stkExpiry.IsVisible = true;
                            stkPrice.IsVisible = true;
                            btnPay.IsVisible = true;
                        }
                        else
                        {
                            btnPay.IsVisible = false;
                            lblParkingFee.Text = "0";
                            lblExpiryMessage.Text = Convert.ToString(currentTime.AddHours(Duration).ToString("hh:mm tt"));
                            DisplayAlert("Result - GetParkingFee", "Pricing and Lot timings not available", "Ok");
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
                btnPay.IsVisible = false;
                lblParkingFee.Text = "0";
                DisplayAlert("Failed - GetParkingFee", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void btn_PayClicked(object sender, EventArgs e)
        {
            ShowLoading(true);
            btnPay.IsVisible = false;

            try
            {
                double duration_double = 0;
                currentTime = DateTime.Now;

                DateTime LotOpenTime = objCustomerParkingSlotDetails.LotOpenTime;
                DateTime LotCloseTime = objCustomerParkingSlotDetails.LotCloseTime;

                TimeSpan tss = LotCloseTime - objCustomerParkingSlotDetails.To;
                duration_double = tss.TotalHours;

                if (Duration <= Math.Ceiling(duration_double))
                {
                    TimeSpan ts = LotCloseTime - currentTime;
                    duration_double = ts.TotalHours;
                    stkTiming.IsVisible = duration_double <= 1 ? false : true;
                    stkTimingFlag = stkTiming.IsVisible;

                    if (Duration <= Math.Ceiling(duration_double))
                    {
                        ExpectedEndTime_date = objCustomerParkingSlotDetails.To.AddHours(Duration) <= LotCloseTime ? objCustomerParkingSlotDetails.To.AddHours(Duration) : LotCloseTime;
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
                        btnPay.IsVisible = false;
                        btnCheckIn.IsVisible = false;
                        ShowLoading(false);
                        await DisplayAlert("", "Parking Lot closes at " + Convert.ToString(Convert.ToDateTime(LotCloseTime).ToString("hh:mm tt")).ToLower() + " today. Please select proper timings.", "Ok");
                        return;
                    }
                }
                else
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
                    obj_CustomerParkingSlot.ExpectedStartTime = objCustomerParkingSlotDetails.To.ToString("MM/dd/yyyy hh:mm tt");
                    obj_CustomerParkingSlot.ExpectedEndTime = ExpectedEndTime_date.ToString("MM/dd/yyyy hh:mm tt");
                }

                obj_CustomerParkingSlot.ActualStartTime = objCustomerParkingSlotDetails.From.ToString("MM/dd/yyyy hh:mm tt");
                obj_CustomerParkingSlot.ActualEndTime = ExpectedEndTime_date.ToString("MM/dd/yyyy hh:mm tt");
                obj_CustomerParkingSlot.ExtendAmount = Convert.ToDecimal(lblParkingFee.Text);

                obj_CustomerParkingSlot.Duration = Convert.ToString(Duration);
                obj_CustomerParkingSlot.Amount = Convert.ToDecimal(lblParkingFee.Text);
                obj_CustomerParkingSlot.TransactionID = "0";
                obj_CustomerParkingSlot.StatusID = Convert.ToInt32(App.Current.Properties["StatusID"]);
                obj_CustomerParkingSlot.CustomerVehicleID = CustomerVehicleID;
                obj_CustomerParkingSlot.ApplicationTypeID = Convert.ToInt32(App.Current.Properties["ApplicationTypeID"]);
                obj_CustomerParkingSlot.PaidAmount = Convert.ToDecimal(lblParkingFee.Text);
                obj_CustomerParkingSlot.ParkingBayID = ParkingBayID;
                btnPay.IsVisible = true;
                ShowLoading(false);
                //21062021 
                 await Navigation.PushAsync(new RazorPayment(obj_CustomerParkingSlot, null));
                //RazorPayload payload = new RazorPayload();
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
                //{
                //    string transactionID = string.Empty;
                //    transactionID = paymentData.PaymentId;

                //    if (obj_CustomerParkingSlot.LocationName != null && obj_CustomerParkingSlot.LocationName != "")
                //    {
                //        if (obj_CustomerParkingSlot.LocationName.ToUpper() == "CHECKOUT")
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
                //            CustomerParkingSlotDetails resultObj = dal_Customer.UpdateCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                //            if (resultObj.CustomerParkingSlotID != 0)
                //            {
                //                App.Current.Properties["CustomerParkingSlotID"] = "0";
                //                App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                //                Application.Current.SavePropertiesAsync();
                //                DisplayAlert("", "Thank you for parking with us!", "Ok");
                //                int CustomerID = 0;
                //                if (App.Current.Properties.ContainsKey("CustomerID"))
                //                {
                //                    CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                //                }

                //                Navigation.PushAsync(new Home(null, CustomerID));
                //            }
                //            else
                //            {
                //                DisplayAlert("", "Insert Payment Failed", "Ok");
                //                App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                //                Application.Current.SavePropertiesAsync();
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
                //                App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                //                Application.Current.SavePropertiesAsync();
                //                Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                //            }
                //            else
                //            {
                //                DisplayAlert("", "Insert Payment Failed", "Ok");
                //                App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                //                Application.Current.SavePropertiesAsync();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (transactionID == "")
                //        {
                //            DisplayAlert("", "Payment Failed", "Ok");
                //            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                //            App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                //            return;
                //        }

                //        obj_CustomerParkingSlot.TransactionID = transactionID;
                //        var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                //        var content = new StringContent(json, Encoding.UTF8, "application/json");
                //        CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                //        if (resultObj.CustomerParkingSlotID != 0)
                //        {
                //            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                //            App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                //            Application.Current.SavePropertiesAsync();
                //            Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                //        }
                //        else
                //        {
                //            DisplayAlert("", "Insert Payment Failed", "Ok");
                //            App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                //            Application.Current.SavePropertiesAsync();
                //        }
                //    }
                //    MessagingCenter.Unsubscribe<PaymentDataOutput>(this, "PaySuccess");
                //});
            }
            catch (Exception ex)
            {
                btnPay.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("Failed - Pay", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void GetCustomerVehiclePassDetails(int locationID, int customerVehicleID, int locationParkingLotID)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
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

                            DisplayAlert("", "Vehicle number " + RegistrationNumber + " has a valid pass", "Ok");
                            btnCheckIn.IsVisible = true;

                            stkChoosePark.IsVisible = false;
                            gridTiming.IsVisible = false;
                            stkExpiry.IsVisible = false;
                            stkPrice.IsVisible = false;
                            btnPay.IsVisible = false;
                            stkPrice.IsVisible = false;
                        }
                        else
                        {
                            btnCheckIn.IsVisible = false;

                            stkChoosePark.IsVisible = true;
                            gridTiming.IsVisible = true;
                            stkExpiry.IsVisible = true;
                            stkPrice.IsVisible = true;
                            btnPay.IsVisible = true;
                            lblParkingFee.Text = "0";
                            GetParkingFee(LocationParkingLotID, Duration);
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
                btnPay.IsVisible = false;
                btnCheckIn.IsVisible = false;

                stkChoosePark.IsVisible = false;
                gridTiming.IsVisible = false;
                stkExpiry.IsVisible = false;
                stkPrice.IsVisible = false;
                lblParkingFee.Text = "0";
                DisplayAlert("Failed - GetCustomerVehiclePassDetails", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void btn_CheckInClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();

            if (IsOnline)
            {
                ShowLoading(true);
                btnCheckIn.IsVisible = false;

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
                    obj_CustomerParkingSlot.ExpectedStartTime = objCustomerParkingSlotDetails.To.ToString("MM/dd/yyyy hh:mm tt");
                    TimeSpan ts = LotCloseTime - objCustomerParkingSlotDetails.To;
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

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                    if (resultObj.CustomerParkingSlotID != 0)
                    {
                        btnCheckIn.IsVisible = true;
                        ShowLoading(false);
                        App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                        await Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                    }
                    else
                    {
                        btnCheckIn.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Payment Failed", "Ok");
                    }
                }
                else
                {
                    btnCheckIn.IsVisible = true;
                    ShowLoading(false);
                }
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
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