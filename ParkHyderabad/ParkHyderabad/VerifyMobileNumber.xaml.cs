using ParkHyderabad.DAL;
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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerifyMobileNumber : ContentPage
    {
        DALCustomer dal_Customer;
        static bool IsOnline = false;
        public VerifyMobileNumber()
        {
            InitializeComponent();
        }

        private async void btn_ProceedClicked(object sender, EventArgs e)
        {
            btnProceed.IsVisible = false;
            ShowLoading(true);
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (txtPhoneNumber.Text != "" && txtPhoneNumber.Text != null)
                {
                    if (txtPhoneNumber.Text.Length != 10)
                    {
                        btnProceed.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Please enter valid Phone Number", "Ok");
                        return;
                    }

                    Customer objCustomer = new Customer();
                    objCustomer.PhoneNumber = txtPhoneNumber.Text;

                    // Remove Code

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

                    // Remove Code

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objCustomer);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        dal_Customer = new DALCustomer();
                        OCustomer resultObj = dal_Customer.ValidateCustomer(Convert.ToString(App.Current.Properties["apitoken"]), objCustomer);

                        if (resultObj.CustomerID != 0)
                        {
                            btnProceed.IsVisible = true;
                            ShowLoading(false);
                            await Navigation.PushAsync(new VerifyOTP(txtPhoneNumber.Text, resultObj.CustomerID, "EXISTING", resultObj.Name));
                        }
                        else
                        {
                            btnProceed.IsVisible = true;
                            ShowLoading(false);
                            await Navigation.PushAsync(new SignUp(resultObj.CustomerID, txtPhoneNumber.Text));
                        }
                    }
                    else
                    {
                        btnProceed.IsVisible = true;
                        ShowLoading(false);
                    }
                }
                else
                {
                    btnProceed.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter your Phone Number", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
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