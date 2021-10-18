using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Web;
using Plugin.Connectivity;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RazorPayment : ContentPage
    {
        string UserName = string.Empty;
        string Email = string.Empty;
        string PhoneNumber = string.Empty;
        static CustomerParkingSlot obj_CustomerParkingSlot;
        static CustomerVehiclePass obj_CustomerVehiclePass;
        CustomerPaymentLog objinitlog;
        DALCustomer dal_Customer;
        DALPass dal_Pass;
        string transactionID = string.Empty;
        Uri uri = null;
        static bool IsOnline = false;
        public RazorPayment(CustomerParkingSlot customerParkingSlot, CustomerVehiclePass customerVehiclePass)
        {
            InitializeComponent();
            dal_Customer = new DALCustomer();
            dal_Pass = new DALPass();
            obj_CustomerParkingSlot = customerParkingSlot;
            obj_CustomerVehiclePass = customerVehiclePass;
            objinitlog = new CustomerPaymentLog();
            if (App.Current.Properties.ContainsKey("UserName"))
            {
                UserName = Convert.ToString(App.Current.Properties["UserName"]);
            }
            if (App.Current.Properties.ContainsKey("Email"))
            {
                Email = Convert.ToString(App.Current.Properties["Email"]);
            }
            if (App.Current.Properties.ContainsKey("PhoneNumber"))
            {
                PhoneNumber = Convert.ToString(App.Current.Properties["PhoneNumber"]);
            }
            if (obj_CustomerParkingSlot != null && obj_CustomerVehiclePass == null)
            {
                string Source = "http://devrazor.instaparking.in/Payment.aspx?" +
                "name=" + UserName + "" +
                   "&email=" + Email +
                   "&contact=" + PhoneNumber + "" +
                   "&description=" + Convert.ToString(obj_CustomerParkingSlot.LocationParkingLotName) +
                   "&amount=" + Convert.ToString(obj_CustomerParkingSlot.PaidAmount) +
                   "&receiptID=" + 0 + "";

                uri = new Uri(Source, UriKind.RelativeOrAbsolute);
                CookieContainer cookieContainer = new CookieContainer();
                Cookie cookie = new Cookie
                {
                    Name = "CustomerID",
                    Expires = DateTime.Now.AddDays(1),
                    Value = Convert.ToString(obj_CustomerParkingSlot.CustomerID),
                    Domain = uri.Host,
                    Path = "/"
                };
                cookieContainer.Add(uri, cookie);
                webView.Cookies = cookieContainer;
                // webView.Source = new UrlWebViewSource { Url = uri.ToString() };
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    objinitlog.CustomerParkingSlotID = obj_CustomerParkingSlot.CustomerParkingSlotID;
                    objinitlog.CustomerID = obj_CustomerParkingSlot.CustomerID;
                    objinitlog.Name = UserName;
                    objinitlog.PhoneNumber = PhoneNumber;
                    objinitlog.Email = Email;
                    objinitlog.PaidAmount = (obj_CustomerParkingSlot.PaidAmount);
                    objinitlog.LocationParkingLotID = obj_CustomerParkingSlot.LocationParkingLotID;
                    objinitlog.CustomerVehicleID = (obj_CustomerParkingSlot.CustomerVehicleID);
                    objinitlog.VehicleTypeID = (obj_CustomerParkingSlot.VehicleTypeID);
                    objinitlog.IsCheckInPayment = true;
                    objinitlog.CreatedOn = DateTime.Now;
                    objinitlog.ActualStartTime = obj_CustomerParkingSlot.ExpectedStartTime;
                    objinitlog.ParkingBayID = obj_CustomerParkingSlot.ParkingBayID;
                    objinitlog.Duration = obj_CustomerParkingSlot.Duration;
                    objinitlog.CreatedBy = obj_CustomerParkingSlot.CustomerID;
                    objinitlog.PaidDueAmount = obj_CustomerParkingSlot.PaidDueAmount;
                    objinitlog.DueCustomerParkingSlotID = obj_CustomerParkingSlot.DueCustomerParkingSlotID;
                    objinitlog.Comments = "APP Initiated";
                    string result = CreatePaymentTransactionLog(objinitlog);
                    if (!string.IsNullOrEmpty(result))
                    {
                        webView.Source = uri.ToString();
                    }

                }

            }
            else if (obj_CustomerParkingSlot == null && obj_CustomerVehiclePass != null)
            {
                string Source = "http://devrazor.instaparking.in/Payment.aspx?" +
               "name=" + UserName + "" +
               "&email=" + Email +
               "&contact=" + PhoneNumber + "" +
               "&description=" + Convert.ToString(obj_CustomerVehiclePass.StationAccess) +
               "&amount=" + Convert.ToString(obj_CustomerVehiclePass.TotalAmount + obj_CustomerVehiclePass.PaidDueAmount) +
               "&receiptID=" + 0 + "";

                CookieContainer cookieContainer = new CookieContainer();
                Uri uri = new Uri(Source, UriKind.RelativeOrAbsolute);
                Cookie cookie = new Cookie
                {
                    Name = "CustomerID",
                    Expires = DateTime.Now.AddDays(1),
                    Value = Convert.ToString(obj_CustomerVehiclePass.CustomerID),
                    Domain = uri.Host,
                    Path = "/"
                };
                cookieContainer.Add(uri, cookie);
                //  webView.Cookies = cookieContainer;
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    objinitlog.CustomerVehiclePassID = obj_CustomerVehiclePass.CustomerVehiclePassID;
                    objinitlog.CustomerID = obj_CustomerVehiclePass.CustomerID;
                    objinitlog.Name = UserName;
                    objinitlog.PhoneNumber = PhoneNumber;
                    objinitlog.Email = Email;
                    objinitlog.PaidAmount = (obj_CustomerVehiclePass.TotalAmount + obj_CustomerVehiclePass.PaidDueAmount);
                    objinitlog.IsPassPayment = true;
                    objinitlog.LocationID = obj_CustomerVehiclePass.LocationID;
                    objinitlog.ActualStartTime = obj_CustomerVehiclePass.StartDate.ToString("MM/dd/yyyy hh:mm tt");
                    objinitlog.IsMultiLot = obj_CustomerVehiclePass.IsMultiLot;
                    objinitlog.PassPriceID = obj_CustomerVehiclePass.PassPriceID;
                    if (obj_CustomerVehiclePass.IsMultiLot)
                    {
                        if (obj_CustomerVehiclePass.lstLocation.Count > 0)
                        {
                            string locName = string.Empty;
                            string locID = string.Empty;
                            for (var i = 0; i < obj_CustomerVehiclePass.lstLocation.Count; i++)
                            {
                                if (i == 0)
                                {
                                    locName = obj_CustomerVehiclePass.lstLocation[i].LocationName;
                                    locID = Convert.ToString(obj_CustomerVehiclePass.lstLocation[i].LocationID);
                                }
                                else
                                {
                                    locName = locName + "," + obj_CustomerVehiclePass.lstLocation[i].LocationName;
                                    locID = locID + "," + Convert.ToString(obj_CustomerVehiclePass.lstLocation[i].LocationID);
                                }
                            }
                            objinitlog.PassLocationID = locID;
                            objinitlog.PassLocations = locName;
                        }
                    }
                    else
                    {
                        objinitlog.PassLocationID = Convert.ToString(obj_CustomerVehiclePass.lstLocation[0].LocationID);
                        objinitlog.PassLocations = obj_CustomerVehiclePass.lstLocation[0].LocationName;
                    }
                    objinitlog.CustomerVehicleID = (obj_CustomerVehiclePass.CustomerVehicleID);
                    objinitlog.PassTypeID = obj_CustomerVehiclePass.PassTypeID;
                    objinitlog.CreatedOn = DateTime.Now;
                    objinitlog.CreatedBy = obj_CustomerVehiclePass.CustomerID;
                    objinitlog.PaidDueAmount = obj_CustomerVehiclePass.PaidDueAmount;
                    objinitlog.DueCustomerParkingSlotID = obj_CustomerVehiclePass.DueCustomerParkingSlotID;
                    objinitlog.Comments = "APP Initiated";
                    string passresult = CreatePaymentTransactionLog(objinitlog);
                    if (!string.IsNullOrEmpty(passresult))
                    {
                       
                        webView.Source = new UrlWebViewSource { Url = uri.ToString() };
                    }
                }
            }
           
        }
        protected async void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    DisplayAlert("Alert", e.Url.ToString(), "Ok");
                    ShowLoading(true);
                }
                else
                {

                    e.Cancel = true;
                    webView.Source = "";
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - RazorPay", Convert.ToString(ex.Message), "Ok");
            }
        }
        protected async void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            try
            {
                int CustomerID = 0;
                ShowLoading(false);
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                   
                    if (e.Url.Contains("http://devrazor.instaparking.in/PaymentClosePage.aspx"))
                    {
                        
                        await GetTransaction();
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
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            if (obj_CustomerParkingSlot != null && obj_CustomerVehiclePass == null)
                            {
                                if (obj_CustomerParkingSlot.LocationName != null && obj_CustomerParkingSlot.LocationName != "")
                                {
                                    if (obj_CustomerParkingSlot.LocationName.ToUpper() == "CHECKOUT")
                                    {
                                        if (transactionID == "")
                                        {
                                            DisplayAlert("", "Payment Failed", "Ok");
                                            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                                            App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                                            if (App.Current.Properties.ContainsKey("CustomerID"))
                                            {
                                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                            }

                                            await Navigation.PushAsync(new Home(null, CustomerID));
                                            return;
                                        }

                                        obj_CustomerParkingSlot.TransactionID = transactionID;
                                        var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                                        CustomerParkingSlotDetails resultObj = dal_Customer.UpdateCheckOut(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);
                                        if (resultObj.CustomerParkingSlotID != 0)
                                        {
                                            App.Current.Properties["CustomerParkingSlotID"] = "0";
                                            App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                                            Application.Current.SavePropertiesAsync();
                                            DisplayAlert("", "Thank you for parking with us!", "Ok");

                                            if (App.Current.Properties.ContainsKey("CustomerID"))
                                            {
                                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                            }

                                            await Navigation.PushAsync(new Home(null, CustomerID));
                                        }
                                        else
                                        {
                                            DisplayAlert("", "Insert Payment Failed", "Ok");
                                            App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                                            if (App.Current.Properties.ContainsKey("CustomerID"))
                                            {
                                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                            }
                                            await Application.Current.SavePropertiesAsync();
                                            await Navigation.PushAsync(new Home(null, CustomerID));

                                        }
                                    }
                                    else
                                    {
                                        if (transactionID == "")
                                        {
                                            DisplayAlert("", "Payment Failed", "Ok");
                                            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                                            App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                                            objinitlog.TransactionID = "Failed";
                                            objinitlog.UpdatedOn = DateTime.Now;
                                            UpdatePaymentTransactionLog(Convert.ToString(App.Current.Properties["apitoken"]), objinitlog);
                                            if (App.Current.Properties.ContainsKey("CustomerID"))
                                            {
                                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                            }

                                            await Navigation.PushAsync(new Home(null, CustomerID));
                                            return;
                                        }

                                        obj_CustomerParkingSlot.TransactionID = transactionID;
                                        var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                                        CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);
                                        if (resultObj.CustomerParkingSlotID != 0)
                                        {
                                            App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                                            App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                                            await Application.Current.SavePropertiesAsync();
                                            await Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                                        }
                                        else
                                        {
                                            DisplayAlert("", "Insert Payment Failed", "Ok");
                                            App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                                            if (App.Current.Properties.ContainsKey("CustomerID"))
                                            {
                                                CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                            }
                                            await Application.Current.SavePropertiesAsync();

                                            await Navigation.PushAsync(new Home(null, CustomerID));
                                        }
                                    }
                                }
                                else
                                {
                                    if (transactionID == "")
                                    {
                                        DisplayAlert("", "Payment Failed", "Ok");
                                        App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerParkingSlotID);
                                        App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlot.CustomerID);
                                        objinitlog.TransactionID = "Failed";
                                        objinitlog.UpdatedOn = DateTime.Now;
                                        UpdatePaymentTransactionLog(Convert.ToString(App.Current.Properties["apitoken"]), objinitlog);
                                        if (App.Current.Properties.ContainsKey("CustomerID"))
                                        {
                                            CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                        }

                                        await Navigation.PushAsync(new Home(null, CustomerID));

                                        return;
                                    }

                                    obj_CustomerParkingSlot.TransactionID = transactionID;
                                    var json = JsonConvert.SerializeObject(obj_CustomerParkingSlot);
                                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    CustomerParkingSlotDetails resultObj = dal_Customer.InsertCustomerParkingSlot(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);
                                    if (resultObj.CustomerParkingSlotID != 0)
                                    {
                                        App.Current.Properties["CustomerParkingSlotID"] = Convert.ToString(resultObj.CustomerParkingSlotID);
                                        App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                                        await Application.Current.SavePropertiesAsync();
                                        await Navigation.PushAsync(new SessionHomeReceipt(resultObj));
                                    }
                                    else
                                    {
                                        DisplayAlert("", "Insert Payment Failed", "Ok");
                                        App.Current.Properties["CustomerID"] = obj_CustomerParkingSlot.CustomerID;
                                        await Application.Current.SavePropertiesAsync();
                                    }
                                }
                            }
                            else if (obj_CustomerParkingSlot == null && obj_CustomerVehiclePass != null)
                            {
                                if (transactionID == "")
                                {
                                    DisplayAlert("", "Payment Failed", "Ok");
                                    App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerVehiclePass.CustomerID);
                                    objinitlog.TransactionID = "Failed";
                                    objinitlog.UpdatedOn = DateTime.Now;
                                    UpdatePaymentTransactionLog(Convert.ToString(App.Current.Properties["apitoken"]), objinitlog);
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                    }

                                    await Navigation.PushAsync(new Home(null, CustomerID));
                                    return;
                                }

                                obj_CustomerVehiclePass.TransactionID = transactionID;
                                var json = JsonConvert.SerializeObject(obj_CustomerVehiclePass);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                OCustomerVehiclePass resultObj;

                                if (obj_CustomerVehiclePass.IsMultiLot)
                                {
                                    resultObj = dal_Pass.InsertMultiStationCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehiclePass);
                                }
                                else
                                {
                                    resultObj = dal_Pass.InsertCustomerVehiclePass(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehiclePass);
                                }

                                if (resultObj.CustomerVehiclePassID != 0)
                                {
                                    App.Current.Properties["CustomerID"] = obj_CustomerVehiclePass.CustomerID;
                                    await Navigation.PushAsync(new PassReceipt(resultObj));
                                }
                                else
                                {
                                    DisplayAlert("", "Insert Payment Failed", "Ok");
                                    App.Current.Properties["CustomerID"] = obj_CustomerVehiclePass.CustomerID;
                                    await Application.Current.SavePropertiesAsync();
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                    }

                                    await Navigation.PushAsync(new Home(null, CustomerID));
                                }
                            }
                        }
                    }
                    
                }
                else
                {

                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {

                        await Navigation.PushAsync(new Home(null, Convert.ToInt32(App.Current.Properties["CustomerID"])));
                    }
                }


            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - RazorPay", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async Task<string> GetValueFromTextbox(string controlId)
        {
            return await webView.EvaluateJavaScriptAsync($"document.getElementById('{controlId}').value;");
        }
        public async Task GetTransaction()
        {
            try
            {

               
               // transactionID = await webView.EvaluateJavaScriptAsync($"document.getElementById('hdPaymentSuccessID').value;");
                transactionID = await webView.EvaluateJavaScriptAsync($"document.getElementById('hdnPaymentID').value;");
               // DisplayAlert("Alert","Success: "+ transactionID, "Ok");
            }
            catch (Exception ex)
            {
                transactionID = "";
                DisplayAlert("Failed - GetTransaction", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void ShowLoading(bool show)
        {
            StklauoutactivityIndicator.IsVisible = show;

            if (show)
            {
                activity.IsRunning = true;
                activity.IsVisible = true;
                absLayout.BackgroundColor = Xamarin.Forms.Color.FromHex("#1976d3");
            }
            else
            {
                absLayout.BackgroundColor = Xamarin.Forms.Color.Transparent;
                absLayout.Opacity = 1;
                activity.IsRunning = false;
                activity.IsVisible = false;
            }
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        public string CreatePaymentTransactionLog(CustomerPaymentLog objnewlog)
        {
            string apiresult = string.Empty;
            try
            {
                DALCustomerPaymentLog dal_customerpaymentlog = new DALCustomerPaymentLog();
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
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    apiresult = dal_customerpaymentlog.InsertCustomerPaymentLog(Convert.ToString(App.Current.Properties["apitoken"]), objnewlog);
                }


            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - CreatePaymentTransactionLog", Convert.ToString(ex.Message), "Ok");
            }
            return apiresult;
        }
        public void UpdatePaymentTransactionLog(string apitoken, CustomerPaymentLog objnewlog)
        {
            try
            {
                DALCustomerPaymentLog dal_customerpaymentlog = new DALCustomerPaymentLog();
                dal_customerpaymentlog.UpdateCustomerPaymentLog(apitoken, objnewlog);


            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - CreatePaymentTransactionLog", Convert.ToString(ex.Message), "Ok");
            }

        }
        public void DeletePaymentTransactionLog(CustomerPaymentLog objnewlog)
        {
            try
            {
                DALCustomerPaymentLog dal_customerpaymentlog = new DALCustomerPaymentLog();
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
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    dal_customerpaymentlog.DeleteCustomerPaymentLog(Convert.ToString(App.Current.Properties["apitoken"]), objnewlog);
                }

            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - CreatePaymentTransactionLog", Convert.ToString(ex.Message), "Ok");
            }

        }
        protected override bool OnBackButtonPressed()
        {
            if(objinitlog!=null)
            {
                objinitlog.Comments = "Attempt Removed; APP BackPressed";
                DeletePaymentTransactionLog(objinitlog);
            }
            // Do your magic here
            return false;
        }

    }
}