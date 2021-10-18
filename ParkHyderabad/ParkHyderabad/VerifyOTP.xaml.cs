using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerifyOTP : ContentPage
    {
        DALCustomer dal_Customer;
        static string PhoneNumber;
        static int CustomerID;
        static int OTPVerificationID;
        static string random_number;
        DALVehicle dal_Vehicle;
        decimal Latitude;
        decimal Longitude;
        string NewORExisting;
        string Name;
        static bool IsOnline = false;
        Model.APIOutPutModel.Location locationsearchfill = new Model.APIOutPutModel.Location();

        private Random _random = new Random();
        public VerifyOTP(string phoneNumber, int customerID, string newOrExisting, string name)
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Vehicle = new DALVehicle();
            PhoneNumber = phoneNumber;
            CustomerID = customerID;
            NewORExisting = newOrExisting;
            Name = name;
            lblDisplayText.Text = "Please enter OTP sent to +91 " + phoneNumber;
            SendOTP(phoneNumber, customerID);
            InsertOTP();
            ShowLoading(false);
        }
        public string GenerateRandomNo()
        {
            return _random.Next(0, 9999).ToString("D4");
        }
        private async void SendOTP(string phoneNumber, int customerID)
        {
            try
            {
                random_number = GenerateRandomNo();
                string msg = "Your One Time Password (OTP) for InstaParking is " + random_number + " .";
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync("https://www.smsstriker.com/API/sms.php?username=sprvtechnology&password=128391&from=INSPRK&to=" + phoneNumber + "&msg=" + msg + "&type=1&template_id=1407161131763549672");

                string resultobj = response.Replace("{", "").Replace("}", "").Trim();
                string[] result_arr = null;
                if (resultobj != null && resultobj != "")
                {
                    result_arr = resultobj.Split(',');
                }

                Dictionary<string, string> obj_dic = new Dictionary<string, string>();
                for (int i = 0; i < result_arr.Length; i++)
                {
                    obj_dic.Add(result_arr[i].Split(':')[0].Trim(), result_arr[i].Split(':')[1].Trim());
                }

                if (obj_dic.ContainsKey("Ack"))
                {
                    string result = string.Empty;
                    obj_dic.TryGetValue("Ack", out result);
                    if (result != "Messages has been sent")
                    {
                        DisplayAlert("", result + " - " + phoneNumber, "Ok");
                    }
                    else
                    {
                        DisplayAlert("", "OTP has been sent to " + phoneNumber, "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - SendOTP", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void InsertOTP()
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                OTPVerification objOTP = new OTPVerification();
                objOTP.PhoneNumber = PhoneNumber;
                objOTP.OTP = random_number;
                objOTP.StartTime = DateTime.Now;
                objOTP.ExpiryTime = DateTime.Now.AddMinutes(5);
                objOTP.Status = Convert.ToInt32(1);
                objOTP.CreatedBy = CustomerID;

                if (App.Current.Properties.ContainsKey("RefreshedToken"))
                {
                    objOTP.DeviceID = Convert.ToString(App.Current.Properties["RefreshedToken"]).Trim();
                }
                else
                {
                    objOTP.DeviceID = "";
                }

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objOTP);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    dal_Customer = new DALCustomer();
                    OTPVerification resultObj = dal_Customer.InsertOTP(Convert.ToString(App.Current.Properties["apitoken"]), objOTP);

                    if (resultObj.OTPVerificationID != 0)
                    {
                        OTPVerificationID = resultObj.OTPVerificationID;
                    }
                    else
                    {
                        DisplayAlert("", "Failed to generate OTP", "Ok");
                    }
                }
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void btn_VerifyOTPClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                btnVerify.IsVisible = false;
                ShowLoading(true);

                if ((OTP1.Text != "" && OTP1.Text != null) && (OTP2.Text != "" && OTP2.Text != null) &&
                    (OTP3.Text != "" && OTP3.Text != null) && (OTP4.Text != "" && OTP4.Text != null))
                {
                    OTPVerification objOTP = new OTPVerification();
                    objOTP.OTPVerificationID = OTPVerificationID;
                    objOTP.PhoneNumber = PhoneNumber;
                    objOTP.OTP = OTP1.Text + OTP2.Text + OTP3.Text + OTP4.Text;
                    objOTP.ExpiryTime = DateTime.Now;
                    objOTP.CreatedBy = CustomerID;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objOTP);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        dal_Customer = new DALCustomer();
                        OTPVerification resultObj = dal_Customer.VerifyOTP(Convert.ToString(App.Current.Properties["apitoken"]), objOTP);

                        if (resultObj.OTPVerificationID != 0)
                        {
                            CustomerID = resultObj.CustomerID;
                            App.Current.Properties["CustomerID"] = Convert.ToString(resultObj.CustomerID);
                            App.Current.Properties["UserName"] = Convert.ToString(resultObj.Name);
                            App.Current.Properties["PhoneNumber"] = Convert.ToString(PhoneNumber);
                            App.Current.Properties["Email"] = Convert.ToString(resultObj.Email);
                            App.Current.Properties["ProfileImage"] = resultObj.ProfileImage;
                            ShowLoading(false);
                            //GetListOfCustomerVehicles();

                            if (NewORExisting == "EXISTING")
                            {
                                DisplayAlert("", "Welcome back " + Name + "! Thank you for using ParkHyderabad", "Ok");
                            }
                            //await Navigation.PushAsync(new MasterPage(objLocationParkingLotFilter));

                            await Navigation.PushAsync(new Home(null, CustomerID));
                        }
                        else
                        {
                            btnVerify.IsVisible = true;
                            ShowLoading(false);
                            DisplayAlert("", "Please enter valid OTP", "Ok");
                        }
                    }
                    else
                    {
                        btnVerify.IsVisible = true;
                        ShowLoading(false);
                    }
                }
                else
                {
                    btnVerify.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter OTP sent to " + PhoneNumber + "", "Ok");
                }
            }
            else
            {
               await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void skt_ResendOTPClicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                random_number = GenerateRandomNo();
                string msg = "Your One Time Password (OTP) for ParkHyderabad is " + random_number + " .";
                InsertOTP();
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync("https://www.smsstriker.com/API/sms.php?username=sprvtechnology&password=128391&from=INSPRK&to=" + PhoneNumber + "&msg=" + msg + "&type=1&template_id=1407161131763549672");

                string resultobj = response.Replace("{", "").Replace("}", "").Trim();
                string[] result_arr = null;
                if (resultobj != null && resultobj != "")
                {
                    result_arr = resultobj.Split(',');
                }

                Dictionary<string, string> obj_dic = new Dictionary<string, string>();
                for (int i = 0; i < result_arr.Length; i++)
                {
                    obj_dic.Add(result_arr[i].Split(':')[0].Trim(), result_arr[i].Split(':')[1].Trim());
                }

                if (obj_dic.ContainsKey("Ack"))
                {
                    string result = string.Empty;
                    obj_dic.TryGetValue("Ack", out result);
                    if (result != "Messages has been sent")
                    {
                        ShowLoading(false);
                        DisplayAlert("", result + " - " + PhoneNumber, "Ok");
                    }
                    else
                    {
                        ShowLoading(false);
                        DisplayAlert("", "OTP has been sent to " + PhoneNumber, "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
            }
        }
        public async void GetListOfCustomerVehicles()
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    ShowLoading(true);
                    CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                    obj_CustomerVehicle.CustomerID = Convert.ToInt32(CustomerID);
                    List<OCustomerVehicle> obj_listCustomerVehicle = new List<OCustomerVehicle>();
                    obj_listCustomerVehicle = dal_Vehicle.GetListOfCustomerVehicle(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                    if (obj_listCustomerVehicle.Count > 0)
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();
                        if (location != null)
                        {
                            Latitude = (decimal)location.Latitude;
                            Longitude = (decimal)location.Longitude;
                        }
                        else
                        {
                            DisplayAlert("", "Please turn on device location", "Ok");
                            ShowLoading(false);
                            return;
                        }

                        App.Current.Properties["Vehicle"] = true;
                        App.Current.Properties["VehicleTypeID"] = obj_listCustomerVehicle[0].VehicleTypeID;
                        App.Current.Properties["CustomerVehicleID"] = obj_listCustomerVehicle[0].CustomerVehicleID;
                        App.Current.Properties["RegistrationNumber"] = obj_listCustomerVehicle[0].RegistrationNumber;
                        await Application.Current.SavePropertiesAsync();
                        ShowLoading(false);
                        if (NewORExisting == "EXISTING")
                        {
                            DisplayAlert("", "Welcome back " + Name + "! Thank you for using ParkHyderabad", "Ok");
                        }
                        //await Navigation.PushAsync(new MasterPage(objLocationParkingLotFilter));
                        await Navigation.PushAsync(new Home(null, CustomerID));
                    }
                    else
                    {
                        ShowLoading(false);
                        await Navigation.PushAsync(new AddVehicle(CustomerID, "VERIFY", VehicleTypeCodes.TwoWheeler));
                    }
                }
                else {
                    await DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                DisplayAlert("Failed - GetListOfCustomerVehicles", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void OTP1_TextChanged(object sender, EventArgs e)
        {
            if (OTP1.Text != null && OTP1.Text != "")
            {
                if (OTP1.Text.Trim() != "")
                {
                    OTP2.Focus();
                }
            }
        }
        private async void OTP2_TextChanged(object sender, EventArgs e)
        {
            if (OTP2.Text != null && OTP2.Text != "")
            {
                if (OTP2.Text.Trim() != "")
                {
                    OTP3.Focus();
                }
            }
        }
        private async void OTP3_TextChanged(object sender, EventArgs e)
        {
            if (OTP3.Text != null && OTP3.Text != "")
            {
                if (OTP3.Text.Trim() != "")
                {
                    OTP4.Focus();
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
    }
}