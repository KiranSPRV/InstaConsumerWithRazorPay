using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using InstaWebAPI.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstaSupport : ContentPage
    {
        DALSupport dal_Support;
        DALCustomer dal_Customer;
        static OCompanyInfo obj_OCompanyInfo = new OCompanyInfo();

        //Floating Lable Code Start
        int _placeholderFontSize = 14;
        int _titleFontSize = 12;
        int _topMargin = -20;
        public event EventHandler Completed;

        static bool IsOnline = false;

        //Floating Lable Code End
        public InstaSupport()
        {
            InitializeComponent();
            
            txtPhoneNumber.Text = Convert.ToString(App.Current.Properties["PhoneNumber"]);
            dal_Support = new DALSupport();
            dal_Customer = new DALCustomer();
            GetCompanyInfo();
           

            

            //Floating Lable Code Start
            lblName.TranslationX = lblPhoneNumber.TranslationX = lblEmail.TranslationX = lblMessage.TranslationX = 10;
            lblName.FontSize = lblPhoneNumber.FontSize = lblEmail.FontSize = lblMessage.FontSize = _placeholderFontSize;
            lblName.TextColor = lblPhoneNumber.TextColor = lblEmail.TextColor = lblMessage.TextColor = Color.White;
            txtPhoneNumber.IsReadOnly = true;
            txtEmail.IsReadOnly = true;

            if (string.IsNullOrEmpty(txtName.Text))
            {
                TransitionToPlaceholder(true);
            }
            else
            {
                TransitionToTitle(true);
            }
            if (string.IsNullOrEmpty(txtPhoneNumber.Text))
            {
                TransitionToPlaceholder_PhoneNo(true);
            }
            else
            {
                TransitionToTitle_PhoneNo(true);
            }
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                TransitionToPlaceholder_Email(true);
            }
            else
            {
                TransitionToTitle_Email(true);
            }
            if (string.IsNullOrEmpty(txtMessage.Text))
            {
                TransitionToPlaceholder_Message(true);
            }
            else
            {
                TransitionToTitle_Message(true);
            }

            txtName.Focus();
            //Floating Lable Code End
        }
        private async void GetCompanyInfo()
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        Customer objCustomer = new Customer();
                        objCustomer.PhoneNumber = txtPhoneNumber.Text;
                        var json = JsonConvert.SerializeObject(objCustomer);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        OCompanyInfo resultObj = new OCompanyInfo();
                        ShowLoading(true);
                        await Task.Run(() =>
                        //Device.BeginInvokeOnMainThread(() =>
                        {
                            resultObj = dal_Customer.GetCompanyInfo(Convert.ToString(App.Current.Properties["apitoken"]), objCustomer);
                        });

                        if (resultObj.AccountName != null)
                        {
                            obj_OCompanyInfo = resultObj;
                            txtName.Text = obj_OCompanyInfo.CustomerName;
                            txtEmail.Text = obj_OCompanyInfo.CustomerEmail;
                            ShowLoading(false);

                            #region floatcode
                            if (string.IsNullOrEmpty(txtName.Text))
                            {
                                TransitionToPlaceholder(true);
                            }
                            else
                            {
                                TransitionToTitle(true);
                            }
                            if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                            {
                                TransitionToPlaceholder_PhoneNo(true);
                            }
                            else
                            {
                                TransitionToTitle_PhoneNo(true);
                            }
                            if (string.IsNullOrEmpty(txtEmail.Text))
                            {
                                TransitionToPlaceholder_Email(true);
                            }
                            else
                            {
                                TransitionToTitle_Email(true);
                            }
                            if (string.IsNullOrEmpty(txtMessage.Text))
                            {
                                TransitionToPlaceholder_Message(true);
                            }
                            else
                            {
                                TransitionToTitle_Message(true);
                            }
                            #endregion
                        }
                        else
                        {
                            ShowLoading(false);
                            await DisplayAlert("", "Company details not available", "Ok");
                        }
                    }
                    else
                    {
                        ShowLoading(false);
                        await DisplayAlert("", "Token does not exists", "Ok");
                    }
                }
                else
                {
                    ShowLoading(false);
                    await DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                await DisplayAlert("Failed - GetCompanyInfo", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void btn_SubmitClicked(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    ShowLoading(true);
                    btnSubmit.IsVisible = false;

                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {
                        if (Convert.ToInt32(App.Current.Properties["CustomerID"]) != 0)
                        {
                            if (txtName.Text == "" || txtName.Text == null)
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter your Name", "Ok");
                                return;
                            }

                            if (txtName.Text.Trim() == "")
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter your Name", "Ok");
                                return;
                            }

                            txtName.Text = txtName.Text.Trim();

                            if (txtName.Text.Length >= 3 && txtName.Text.Length <= 20)
                            {

                            }
                            else
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Name must be min of 3 and max of 20 characters", "Ok");
                                return;
                            }

                            var regexName = new Regex("^[a-zA-Z ]*$");
                            if (!regexName.IsMatch(txtName.Text.Trim()))
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter alphabets for Name", "Ok");
                                return;
                            }

                            if (txtPhoneNumber.Text == "" || txtPhoneNumber.Text == null)
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter your Phone Number", "Ok");
                                return;
                            }

                            if (txtEmail.Text == "" || txtEmail.Text == null)
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter your Email", "Ok");
                                return;
                            }

                            if (!RegexUtilities.IsEmailValid(txtEmail.Text.Trim()))
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter valid Email", "Ok");
                                return;
                            }

                            if (txtMessage.Text == "" || txtMessage.Text == null)
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter Message", "Ok");
                                return;
                            }

                            if (txtMessage.Text.Trim() == "")
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter Message", "Ok");
                                return;
                            }

                            txtMessage.Text = txtMessage.Text.Trim();

                            if (txtMessage.Text.Length >= 10 && txtMessage.Text.Length <= 200)
                            {

                            }
                            else
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Message must be min of 10 and max of 200 characters", "Ok");
                                return;
                            }

                            CSupport objOfferMySpace = new CSupport();
                            objOfferMySpace.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                            objOfferMySpace.Name = txtName.Text.Trim();
                            objOfferMySpace.Email = txtEmail.Text.Trim();
                            objOfferMySpace.PhoneNumber = txtPhoneNumber.Text.Trim();
                            objOfferMySpace.Message = txtMessage.Text.Trim();

                            if (App.Current.Properties.ContainsKey("apitoken"))
                            {
                                var json = JsonConvert.SerializeObject(objOfferMySpace);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                OSupport resultObj = new OSupport();

                                await Task.Run(() =>
                                {
                                   
                                    resultObj = dal_Support.InsertSupport(Convert.ToString(App.Current.Properties["apitoken"]), objOfferMySpace);
                                });

                                if (resultObj.SupportRequestID != 0)
                                {
                                    /*
                                    await Task.Run(() =>
                                    {
                                        SendEmail();
                                    });
                                    */
                                    int CustomerID = 0;
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                    }

                                    ShowLoading(false);
                                    await DisplayAlert("", "Thank you for contacting Support. Our team will get back to you soon!", "Ok");
                                    btnSubmit.IsVisible = true;
                                    await Navigation.PushAsync(new Home(null,CustomerID));
                                }
                                else
                                {
                                    btnSubmit.IsVisible = true;
                                    ShowLoading(false);
                                    DisplayAlert("Failed - InsertSupport", "Insert Failed", "Ok");
                                    return;
                                }
                            }
                            else
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                            }
                        }
                        else
                        {
                            btnSubmit.IsVisible = true;
                            ShowLoading(false);
                            DisplayAlert("", "Profile not found!", "Ok");
                        }
                    }
                    else
                    {
                        btnSubmit.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Customer Key does not exists", "Ok");
                    }

                    ShowLoading(false);
                }
                else {
                   await DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }

            }
            catch (Exception ex)
            {
                btnSubmit.IsVisible = true;
                ShowLoading(false);
            }
        }
        private async void imgbtn_MakeACall(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(obj_OCompanyInfo.SupportContactNumber))
            {
                await Call(obj_OCompanyInfo.SupportContactNumber);
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
        public void SendEmail()
        {
            try
            {
                ShowLoading(true);

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(Convert.ToString(txtEmail.Text));
                mail.To.Add(Convert.ToString(obj_OCompanyInfo.SupportEmailID));
                mail.Subject = "Support Request";
                mail.Body = " Name : " + txtName.Text + " \n PhoneNumber : " + txtPhoneNumber.Text + " \n Email : " + txtEmail.Text + " \n Message : " + txtMessage.Text + "";

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("lokeshwarib@aveniras.com", "Lokes@29");

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed", ex.Message, "OK");
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

        //For Floating Label Start 
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(string), string.Empty, BindingMode.TwoWay, null, null);


        async void txtName_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle(true);
            }
        }
        async void txtName_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder(true);
            }
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                await TransitionToTitle(true);
            }
        }
        void lblName_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtName.Focus();
            }
        }
        async Task TransitionToTitle(bool animated)
        {
            if (animated)
            {
                var t1 = lblName.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblName.TranslationX = 0;
                lblName.TranslationY = -30;
                lblName.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder(bool animated)
        {
            if (animated)
            {
                var t1 = lblName.TranslateTo(10, 0, 100);
                var t2 = SizeTo(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblName.TranslationX = 10;
                lblName.TranslationY = 0;
                lblName.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblName.FontSize = input; };
            double startingHeight = lblName.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblName.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtName_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }


        private async void txtPhoneNumber_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_PhoneNo(true);
            }
        }
        async void txtPhoneNumber_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_PhoneNo(true);
            }
            if (!string.IsNullOrEmpty(txtPhoneNumber.Text))
            {
                await TransitionToTitle_PhoneNo(true);
            }
        }
        void lblPhoneNumber_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtPhoneNumber.Focus();
            }
        }
        async Task TransitionToTitle_PhoneNo(bool animated)
        {
            if (animated)
            {
                var t1 = lblPhoneNumber.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_PhoneNo(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblPhoneNumber.TranslationX = 0;
                lblPhoneNumber.TranslationY = -30;
                lblPhoneNumber.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_PhoneNo(bool animated)
        {
            if (animated)
            {
                var t1 = lblPhoneNumber.TranslateTo(10, 0, 100);
                var t2 = SizeTo_PhoneNo(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblPhoneNumber.TranslationX = 10;
                lblPhoneNumber.TranslationY = 0;
                lblPhoneNumber.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_PhoneNo(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblPhoneNumber.FontSize = input; };
            double startingHeight = lblPhoneNumber.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblPhoneNumber.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtPhoneNumber_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }



        async void txtEmail_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_Email(true);
            }
        }
        async void txtEmail_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_Email(true);
            }
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                await TransitionToTitle_Email(true);
            }
        }
        void lblEmail_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtEmail.Focus();
            }
        }
        async Task TransitionToTitle_Email(bool animated)
        {
            if (animated)
            {
                var t1 = lblEmail.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_Email(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblEmail.TranslationX = 0;
                lblEmail.TranslationY = -30;
                lblEmail.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_Email(bool animated)
        {
            if (animated)
            {
                var t1 = lblEmail.TranslateTo(10, 0, 100);
                var t2 = SizeTo_Email(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblEmail.TranslationX = 10;
                lblEmail.TranslationY = 0;
                lblEmail.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_Email(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblEmail.FontSize = input; };
            double startingHeight = lblEmail.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblEmail.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtEmail_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }



        async void txtMessage_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_Message(true);
            }
        }
        async void txtMessage_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_Message(true);
            }
            if (!string.IsNullOrEmpty(txtMessage.Text))
            {
                await TransitionToTitle_Message(true);
            }
        }
        void lblMessage_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtMessage.Focus();
            }
        }
        async Task TransitionToTitle_Message(bool animated)
        {
            if (animated)
            {
                var t1 = lblMessage.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_Message(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblMessage.TranslationX = 0;
                lblMessage.TranslationY = -30;
                lblMessage.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_Message(bool animated)
        {
            if (animated)
            {
                var t1 = lblMessage.TranslateTo(10, 0, 100);
                var t2 = SizeTo_Message(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblMessage.TranslationX = 10;
                lblMessage.TranslationY = 0;
                lblMessage.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_Message(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblMessage.FontSize = input; };
            double startingHeight = lblMessage.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblMessage.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtMessage_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        private async void guestureChat_Tapped(object sender, EventArgs e)
        {
           await DisplayAlert("Alert", "Coming soon", "OK");
        }


        //For Floating Label End
    }
}