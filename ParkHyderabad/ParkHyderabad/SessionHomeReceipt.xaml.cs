//using Android.OS;
using ParkHyderabad.DAL;
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
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SessionHomeReceipt : ContentPage
    {
        static CustomerParkingSlotDetails objCustomerParkingSlotDetails;
        DALCustomer dal_Customer;
        DALLocationParkingLots dal_LocationParkingLot;
        static DateTime currentTime = DateTime.Now;
        static DateTime ActualEndTime = DateTime.Now;
        static string SupervisorPhoneNumber = string.Empty;
        static bool IsClamp = false;
        static bool IsOnline = false;
        public SessionHomeReceipt(CustomerParkingSlotDetails customerParkingSlotDetails)
        {           

            this.IsBusy = true;
            InitializeComponent();
            dal_Customer = new DALCustomer();
            dal_LocationParkingLot = new DALLocationParkingLots();

            if (customerParkingSlotDetails != null)
            {
                objCustomerParkingSlotDetails = customerParkingSlotDetails;
                lblLotName.Text = customerParkingSlotDetails.ApplicationTypeID == Convert.ToInt32(App.Current.Properties["PassApplicationTypeID"]) ? customerParkingSlotDetails.LocationParkingLotName + " - PASS CHECK IN" : customerParkingSlotDetails.LocationParkingLotName;
                lblAddress.Text = customerParkingSlotDetails.Address;
                lblFrom.Text = customerParkingSlotDetails.From.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
                lblTo.Text = customerParkingSlotDetails.To.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
                string fromdatesplit = lblFrom.Text.Split(',')[0];
                string fromtimesplit = lblFrom.Text.Split(',')[1];
                lblFrom.Text = fromdatesplit;
                lblFromTime.Text = fromtimesplit;
                string toDatesplit = lblTo.Text.Split(',')[0];
                string toTimesplit = lblTo.Text.Split(',')[1];
                lblTo.Text = toDatesplit;
                lblToTime.Text = toTimesplit;
                lblRegistrationNumber.Text = customerParkingSlotDetails.RegistrationNumber;
                lblModel.Text = customerParkingSlotDetails.Model;

                App.Current.Properties["CustomerParkingSlotID"] = customerParkingSlotDetails.CustomerParkingSlotID;
                App.Current.Properties["UserName"] = customerParkingSlotDetails.CustomerName;
                App.Current.Properties["PhoneNumber"] = customerParkingSlotDetails.CustomerPhoneNumber;
                App.Current.Properties["Email"] = customerParkingSlotDetails.CustomerEmail;
                App.Current.Properties["CustomerID"] = customerParkingSlotDetails.CustomerID;
                App.Current.Properties["ProfileImage"] = customerParkingSlotDetails.CustomerProfileImage;

                if (customerParkingSlotDetails.ApplicationTypeID == Convert.ToInt32(App.Current.Properties["PassApplicationTypeID"]))
                {
                    stkParkingFee.IsVisible = false;
                    lblLotName.HeightRequest = 45;
                }
                else
                {
                    stkParkingFee.IsVisible = true;
                   // lblLotName.HeightRequest = 10;
                    lblParkingFee.Text = customerParkingSlotDetails.PaidAmount;

                    TimeSpan t_duration = (customerParkingSlotDetails.To - customerParkingSlotDetails.From);
                    int d_duration = Convert.ToInt32(Math.Round(t_duration.TotalHours));

                    lblPaymentType.Text = " Paid for " + d_duration + " hour (Via " + customerParkingSlotDetails.PaymentTypeName + ")";
                }

                OrderID.Text = "ID : #" + customerParkingSlotDetails.CustomerParkingSlotID;
                imgVehicle.Source = customerParkingSlotDetails.VehicleImage;
                currentTime = DateTime.Now;
                ActualEndTime = Convert.ToDateTime(customerParkingSlotDetails.To);

                DisabledParking.Source = customerParkingSlotDetails.DisabledParking;
                EvCharging.Source = customerParkingSlotDetails.EvCharging;
                CoveredParking.Source = customerParkingSlotDetails.CoveredParking;
                BikeWash.Source = customerParkingSlotDetails.BikeWash;
                CarWash.Source = customerParkingSlotDetails.CarWash;
                Mechanic.Source = customerParkingSlotDetails.Mechanic;

                lblWarning.Text = "'" + Convert.ToString(customerParkingSlotDetails.ViolationWarningCount) + "' Warning(s) Completed";
                lblOverStayFee.Text = customerParkingSlotDetails.Price == 0 ? "0" : Convert.ToString(decimal.Truncate(customerParkingSlotDetails.Price));
                lblClampFee.Text = customerParkingSlotDetails.ClampFee == 0 ? "0" : Convert.ToString(decimal.Truncate(customerParkingSlotDetails.ClampFee));
                lblTotal.Text = Convert.ToDecimal(customerParkingSlotDetails.Price) + Convert.ToDecimal(customerParkingSlotDetails.ClampFee) == 0 ? "0" :
                    Convert.ToString(decimal.Truncate((Convert.ToDecimal(customerParkingSlotDetails.Price) + Convert.ToDecimal(customerParkingSlotDetails.ClampFee))));
                lblClampReason.Text = customerParkingSlotDetails.ClampReason;

                if (customerParkingSlotDetails.IsClamp)
                {
                    stkSupervisor.IsVisible = true;
                    stkClampWarning.IsVisible = true;
                    IsClamp = customerParkingSlotDetails.IsClamp;
                    SupervisorPhoneNumber = customerParkingSlotDetails.PhoneNumber;
                    lblSupervisor.Text = customerParkingSlotDetails.SupervisorName;
                    lblPhoneNumber.Text = customerParkingSlotDetails.PhoneNumber;
                }

                if (ActualEndTime < currentTime)
                {
                    lblSpotExpiry.Text = "Spot Expired At";
                    lblExpiryTime.Text = customerParkingSlotDetails.To.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
                    lblExpiryTime.FontSize = 20;
                }
                else
                {
                    TimeSpan duration = ActualEndTime - currentTime;
                    long durationTicks = Math.Abs(duration.Ticks / TimeSpan.TicksPerMillisecond);
                    long hours = durationTicks / (1000 * 60 * 60);
                    long minutes = (durationTicks - (hours * 60 * 60 * 1000)) / (1000 * 60);

                    lblExpiryTime.Text = hours.ToString("00 h") + " : " + minutes.ToString("00 m");
                }

                Device.StartTimer(TimeSpan.FromSeconds(60), (Func<bool>)(() =>
                {
                    currentTime = DateTime.Now;
                    if (ActualEndTime < currentTime)
                    {
                        lblSpotExpiry.Text = "Spot Expired At";
                        lblExpiryTime.Text = customerParkingSlotDetails.To.ToString("d MMM yyyy, hh:mm tt", CultureInfo.CreateSpecificCulture("en-US"));
                        lblExpiryTime.FontSize = 20;
                        return false;
                    }
                    else
                    {
                        TimeSpan duration = ActualEndTime - currentTime;
                        long durationTicks = Math.Abs(duration.Ticks / TimeSpan.TicksPerMillisecond);
                        long hours = durationTicks / (1000 * 60 * 60);
                        long minutes = (durationTicks - (hours * 60 * 60 * 1000)) / (1000 * 60);

                        lblExpiryTime.Text = hours.ToString("00 h") + " : " + minutes.ToString("00 m");
                    }
                    return true; // True = Repeat again, False = Stop the timer
                }));

                GenerateAPIToken();
                this.IsBusy = false;
            }
        }
        private async void stk_ExtendTimeTapped(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ds;
                int days;
                currentTime = DateTime.Now;
                ds = (currentTime.Date - Convert.ToDateTime(objCustomerParkingSlotDetails.To).Date);
                days = Convert.ToInt32(ds.Days);

                ParkingFee objParkingFee = new ParkingFee();

                objCustomerParkingSlotDetails.LotOpenTime = Convert.ToDateTime(objCustomerParkingSlotDetails.LotOpeningTime);
                objCustomerParkingSlotDetails.LotCloseTime = Convert.ToDateTime(objCustomerParkingSlotDetails.LotClosingTime);

                ShowLoading(true);
                await Task.Run(() =>
                {
                    objParkingFee = ValidateVehicleClamped();
                });

                if (objParkingFee.CustomerParkingSlotID != 0 && objParkingFee.StatusID != 5 && objParkingFee.StatusID != 6)
                {
                    lblWarning.Text = "'" + Convert.ToString(objParkingFee.ViolationWarningCount) + "' Warning(s) Completed";
                    lblOverStayFee.Text = objParkingFee.Price == 0 ? "0" : Convert.ToString(decimal.Truncate(objParkingFee.Price));
                    lblClampFee.Text = objParkingFee.ClampFee == 0 ? "0" : Convert.ToString(decimal.Truncate(objParkingFee.ClampFee));
                    lblTotal.Text = Convert.ToDecimal(objParkingFee.Price) + Convert.ToDecimal(objParkingFee.ClampFee) == 0 ? "0" :
                        Convert.ToString(decimal.Truncate((Convert.ToDecimal(objParkingFee.Price) + Convert.ToDecimal(objParkingFee.ClampFee))));
                    lblClampReason.Text = objParkingFee.ClampReason;

                    if (objParkingFee.IsClamp)
                    {
                        stkSupervisor.IsVisible = true;
                        stkClampWarning.IsVisible = true;
                        IsClamp = objParkingFee.IsClamp;
                        SupervisorPhoneNumber = objParkingFee.PhoneNumber;
                        lblSupervisor.Text = objParkingFee.SupervisorName;
                        lblPhoneNumber.Text = objParkingFee.PhoneNumber;
                        ShowLoading(false);
                        StklauoutactivityIndicator.IsVisible = false;
                        await DisplayAlert("", "Sorry! Your vehicle is clamped. Please visit the Parking Lot", "Ok");
                        return;
                    }
                    else
                    {
                        if (objCustomerParkingSlotDetails.ApplicationTypeID == Convert.ToInt32(App.Current.Properties["PassApplicationTypeID"]))
                        {
                            ShowLoading(false);
                            StklauoutactivityIndicator.IsVisible = false;
                            await DisplayAlert("", "You have a valid pass! extension of time is not required", "Ok");
                            return;
                        }
                        else
                        {
                            if (days <= 0)
                            {
                               
                                if (currentTime >= objCustomerParkingSlotDetails.LotCloseTime)
                                {
                                    ShowLoading(false);
                                    StklauoutactivityIndicator.IsVisible = false;
                                    await DisplayAlert("", "Please visit the Parking Lot! You cannot Extend Time for this Vehicle", "Ok");
                                }                               
                                else
                                {
                                    ShowLoading(false);
                                    StklauoutactivityIndicator.IsVisible = false;
                                    await Navigation.PushAsync(new ExtendSession(objCustomerParkingSlotDetails));
                                }
                            }
                            else
                            {
                                ShowLoading(false);
                                StklauoutactivityIndicator.IsVisible = false;
                                await DisplayAlert("", "Please visit the Parking Lot! You cannot Extend Time for this Vehicle", "Ok");
                            }
                        }
                    }
                }
                else
                {
                    if (objParkingFee.StatusID == 5 || objParkingFee.StatusID == 6)
                    {
                        lblOverStayFee.Text = "0";
                        lblClampFee.Text = "0";
                        lblTotal.Text = "0";
                        ShowLoading(false);
                        StklauoutactivityIndicator.IsVisible = false;
                        App.Current.Properties["CustomerParkingSlotID"] = "0";
                        Application.Current.SavePropertiesAsync();
                        await DisplayAlert("", "Your Vehicle has been Checked Out!", "Ok");
                        int CustomerID = 0;
                        if (App.Current.Properties.ContainsKey("CustomerID"))
                        {
                            CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        }

                        await Navigation.PushAsync(new Home(null, CustomerID));
                    }
                    else
                    {
                        //await DisplayAlert("Status -  CustomerParkingSlotID : ", Convert.ToString(objParkingFee.StatusID) + " - " + Convert.ToString(objParkingFee.CustomerParkingSlotID), "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void stk_CheckOutTapped(object sender, EventArgs e)
        {
            try
            {

                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    ParkingFee objParkingFee = new ParkingFee();
                    this.IsBusy = true;
                    DateTime ActualEndTime = Convert.ToDateTime(objCustomerParkingSlotDetails.To);

                    TimeSpan ds;
                    int days;
                    currentTime = DateTime.Now;
                    ds = (currentTime.Date - Convert.ToDateTime(objCustomerParkingSlotDetails.To).Date);
                    days = Convert.ToInt32(ds.Days);

                    ShowLoading(true);

                    await Task.Run(() =>
                    {
                        objParkingFee = ValidateVehicleClamped();
                    });

                    if (objParkingFee.CustomerParkingSlotID != 0 && objParkingFee.StatusID != 5 && objParkingFee.StatusID != 6)
                    {
                        lblWarning.Text = "'" + Convert.ToString(objParkingFee.ViolationWarningCount) + "' Warning(s) Completed";
                        lblOverStayFee.Text = objParkingFee.Price == 0 ? "0" : Convert.ToString(decimal.Truncate(objParkingFee.Price));
                        lblClampFee.Text = objParkingFee.ClampFee == 0 ? "0" : Convert.ToString(decimal.Truncate(objParkingFee.ClampFee));
                        lblTotal.Text = Convert.ToDecimal(objParkingFee.Price) + Convert.ToDecimal(objParkingFee.ClampFee) == 0 ? "0" :
                            Convert.ToString(decimal.Truncate((Convert.ToDecimal(objParkingFee.Price) + Convert.ToDecimal(objParkingFee.ClampFee))));
                        lblClampReason.Text = objParkingFee.ClampReason;

                        if (objParkingFee.IsClamp)
                        {
                            stkSupervisor.IsVisible = true;
                            stkClampWarning.IsVisible = true;
                            IsClamp = objParkingFee.IsClamp;
                            SupervisorPhoneNumber = objParkingFee.PhoneNumber;
                            lblSupervisor.Text = objParkingFee.SupervisorName;
                            lblPhoneNumber.Text = objParkingFee.PhoneNumber;
                            ShowLoading(false);
                            StklauoutactivityIndicator.IsVisible = false;
                            await DisplayAlert("", "Sorry! Your vehicle is clamped. Please visit the Parking Lot", "Ok");
                            return;
                        }
                        else
                        {
                            if (days <= 0)
                            {
                                if (objParkingFee.Price != 0)
                                {
                                    if (ActualEndTime < currentTime)
                                    {
                                        var confirmed = await DisplayAlert("", "Please confirm that you want to Check Out your Vehicle " + lblRegistrationNumber.Text + "", "Yes", "No");
                                        if (confirmed)
                                        {
                                            CustomerParkingSlot obj_CustomerParkingSlot = new CustomerParkingSlot();
                                            obj_CustomerParkingSlot.CustomerParkingSlotID = objCustomerParkingSlotDetails.CustomerParkingSlotID;
                                            obj_CustomerParkingSlot.ExpectedStartTime = objCustomerParkingSlotDetails.From.ToString("MM/dd/yyyy hh:mm tt");
                                            obj_CustomerParkingSlot.ActualEndTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                                            obj_CustomerParkingSlot.CustomerID = objCustomerParkingSlotDetails.CustomerID;
                                            obj_CustomerParkingSlot.LocationName = "CHECKOUT";
                                            obj_CustomerParkingSlot.LocationParkingLotName = objCustomerParkingSlotDetails.LocationParkingLotName;
                                            obj_CustomerParkingSlot.PaidAmount = Convert.ToDecimal(objParkingFee.Price == 0 ? "0" : Convert.ToString(decimal.Truncate(objParkingFee.Price)));

                                            obj_CustomerParkingSlot.Duration = Convert.ToString(objParkingFee.Hours);
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
                                        else
                                        {
                                            ShowLoading(false);
                                            StklauoutactivityIndicator.IsVisible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    var confirmed = await DisplayAlert("", "Please confirm that you want to Check Out your Vehicle " + lblRegistrationNumber.Text + "", "Yes", "No");
                                    if (confirmed)
                                    {
                                        CustomerParkingSlot obj_CustomerParkingSlot = new CustomerParkingSlot();
                                        obj_CustomerParkingSlot.CustomerParkingSlotID = objCustomerParkingSlotDetails.CustomerParkingSlotID;
                                        obj_CustomerParkingSlot.ExpectedStartTime = objCustomerParkingSlotDetails.From.ToString("MM/dd/yyyy hh:mm tt");
                                        obj_CustomerParkingSlot.ActualEndTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                                        obj_CustomerParkingSlot.CustomerID = objCustomerParkingSlotDetails.CustomerID;

                                        TimeSpan ts = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(objCustomerParkingSlotDetails.From);
                                        obj_CustomerParkingSlot.Duration = Convert.ToString(ts.Hours);

                                        obj_CustomerParkingSlot.PaidAmount = 0;

                                        if (App.Current.Properties.ContainsKey("apitoken"))
                                        {
                                            var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                                            CustomerParkingSlotDetails resultObj = dal_Customer.UpdateCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                                            if (resultObj.CustomerParkingSlotID != 0)
                                            {
                                                App.Current.Properties["CustomerParkingSlotID"] = "0";
                                                Application.Current.SavePropertiesAsync();
                                                ShowLoading(false);
                                                StklauoutactivityIndicator.IsVisible = false;
                                                await DisplayAlert("", "Thank you for parking with us!", "Ok");
                                                int CustomerID = 0;
                                                if (App.Current.Properties.ContainsKey("CustomerID"))
                                                {
                                                    CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                                }

                                                await Navigation.PushAsync(new Home(null, CustomerID));
                                            }
                                            else
                                            {
                                                ShowLoading(false);
                                                StklauoutactivityIndicator.IsVisible = false;
                                                await DisplayAlert("", "Check Out failed", "Ok");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ShowLoading(false);
                                        StklauoutactivityIndicator.IsVisible = false;
                                    }
                                }
                            }
                            else
                            {
                                ShowLoading(false);
                                StklauoutactivityIndicator.IsVisible = false;
                                await DisplayAlert("", "This Vehicle cannot be Checked Out. Please visit the Parking Lot.", "Ok");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (objParkingFee.StatusID == 5 || objParkingFee.StatusID == 6)
                        {
                            lblOverStayFee.Text = "0";
                            lblClampFee.Text = "0";
                            lblTotal.Text = "0";
                            ShowLoading(false);
                            StklauoutactivityIndicator.IsVisible = false;
                            App.Current.Properties["CustomerParkingSlotID"] = "0";
                            Application.Current.SavePropertiesAsync();
                            await DisplayAlert("", "Your Vehicle has been Checked Out!", "Ok");

                            int CustomerID = 0;
                            if (App.Current.Properties.ContainsKey("CustomerID"))
                            {
                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                            }

                            await Navigation.PushAsync(new Home(null, CustomerID));
                        }
                        else
                        {
                            //await DisplayAlert("Status -  CustomerParkingSlotID : ", Convert.ToString(objParkingFee.StatusID) + " - " + Convert.ToString(objParkingFee.CustomerParkingSlotID), "Ok");
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
                ShowLoading(false);
                StklauoutactivityIndicator.IsVisible = false;
                await DisplayAlert("Failed - CheckOut", Convert.ToString(ex.Message), "Ok");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public ParkingFee ValidateVehicleClamped()
        {
            ParkingFee resultObj = new ParkingFee();
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(Convert.ToString(App.Current.Properties["BaseURL"]));
                        // We want the response to be JSON.
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        List<System.Collections.Generic.KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
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

                    ParkingDetails objParkingDetails = new ParkingDetails();
                    objParkingDetails.CustomerParkingSlotID = objCustomerParkingSlotDetails.CustomerParkingSlotID;
                    objParkingDetails.ActualEndTime = objCustomerParkingSlotDetails.To.ToString("MM/dd/yyyy hh:mm tt");
                    objParkingDetails.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                    objParkingDetails.LocationParkingLotID = objCustomerParkingSlotDetails.LocationParkingLotID;
                    objParkingDetails.VehicleTypeID = objCustomerParkingSlotDetails.VehicleTypeID;
                    objParkingDetails.Amount = Convert.ToDecimal(objCustomerParkingSlotDetails.PriceAmount);

                    TimeSpan ts = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(objCustomerParkingSlotDetails.To);
                    int duration = ts.Hours;
                    objParkingDetails.ParkingHours = duration;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objParkingDetails);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        resultObj = dal_LocationParkingLot.ValidateVehicleClamped(Convert.ToString(App.Current.Properties["apitoken"]), objParkingDetails);
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                lblOverStayFee.Text = "0";
                lblClampFee.Text = "0";
                lblTotal.Text = "0";
                DisplayAlert("Failed - ValidateVehicleClamped", Convert.ToString(ex.Message), "Ok");
            }
            return resultObj;
        }
        private async void imgbtn_FilterClicked(object sender, EventArgs e)
        {
            return;
        }
        private async void lblMakeACall(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SupervisorPhoneNumber))
            {
                if (IsClamp)
                {
                    await Call(SupervisorPhoneNumber);
                }
            }
        }
        public async Task Call(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }

            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.  
            }
            catch (Exception ex)
            {
                // Other error has occurred.  
            }
        }
        public void GenerateAPIToken()
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