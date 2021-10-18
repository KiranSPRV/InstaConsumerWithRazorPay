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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfferMySpace : ContentPage
    {
        DALOfferMySpace dal_Offer;
        DALCustomer dal_Customer;
        static OCompanyInfo obj_OCompanyInfo = new OCompanyInfo();

        //Floating Lable Code Start
        int _placeholderFontSize = 14;
        int _titleFontSize = 12;
        int _topMargin = -20;
        public event EventHandler Completed;
        static bool IsOnline = false;
        //Floating Lable Code End
        public OfferMySpace()
        {
            InitializeComponent();
            ShowLoading(true);
            txtPhoneNumber.Text = Convert.ToString(App.Current.Properties["PhoneNumber"]);
            dal_Offer = new DALOfferMySpace();
            ShowLoading(false);
            dal_Customer = new DALCustomer();
            GetCompanyInfo();

            //Floating Lable Code Start
            lblName.TranslationX = lblPhoneNumber.TranslationX = lblEmail.TranslationX = lblOther.TranslationX = 10;
            lblName.FontSize = lblPhoneNumber.FontSize = lblEmail.FontSize = lblOther.FontSize = _placeholderFontSize;
            lblName.TextColor = lblPhoneNumber.TextColor = lblEmail.TextColor = lblOther.TextColor = Color.White;
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
            if (string.IsNullOrEmpty(txtOtherDetails.Text))
            {
                TransitionToPlaceholder_OtherDetails(true);
            }
            else
            {
                TransitionToTitle_OtherDetails(true);
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
                            if (string.IsNullOrEmpty(txtOtherDetails.Text))
                            {
                                TransitionToPlaceholder_OtherDetails(true);
                            }
                            else
                            {
                                TransitionToTitle_OtherDetails(true);
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
            btnSubmit.IsVisible = false;
            ShowLoading(true);

            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
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
                                DisplayAlert("", "Please enter only alphabets for Name", "Ok");
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

                            if (!RegexUtilities.IsEmailValid(txtEmail.Text))
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter valid Email", "Ok");
                                return;
                            }

                            if (txtOtherDetails.Text == "" || txtOtherDetails.Text == null)
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter other details", "Ok");
                                return;
                            }

                            if (txtOtherDetails.Text.Trim() == "")
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Please enter other details", "Ok");
                                return;
                            }

                            txtOtherDetails.Text = txtOtherDetails.Text.Trim();

                            if (txtOtherDetails.Text.Length >= 10 && txtOtherDetails.Text.Length <= 200)
                            {

                            }
                            else
                            {
                                btnSubmit.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("", "Other details must be min of 10 and max of 200 characters", "Ok");
                                return;
                            }

                            COfferMySpace objOfferMySpace = new COfferMySpace();
                            objOfferMySpace.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                            objOfferMySpace.Name = txtName.Text.Trim();
                            objOfferMySpace.Email = txtEmail.Text.Trim();
                            objOfferMySpace.PhoneNumber = txtPhoneNumber.Text.Trim();
                            objOfferMySpace.Other = txtOtherDetails.Text.Trim();

                            if (App.Current.Properties.ContainsKey("apitoken"))
                            {
                                var json = JsonConvert.SerializeObject(objOfferMySpace);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                OOfferMySpace resultObj = new OOfferMySpace();

                                await Task.Run(() =>
                                {
                                   
                                    resultObj = dal_Offer.InsertOfferMySpace(Convert.ToString(App.Current.Properties["apitoken"]), objOfferMySpace);
                                });

                                if (resultObj.OfferMySpaceID != 0)
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
                                    await DisplayAlert("", "Your Request has been accepted. Our team will get back to you soon!", "Ok");
                                    btnSubmit.IsVisible = true;
                                    await Navigation.PushAsync(new Home(null,CustomerID));
                                }
                                else
                                {
                                    btnSubmit.IsVisible = true;
                                    ShowLoading(false);
                                    DisplayAlert("Failed - InsertOfferMySpace", "Insert Failed", "Ok");
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
                }
                else
                {
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
        public bool IsEmailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public void SendEmail()
        {
            try
            {

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(Convert.ToString(txtEmail.Text));
                mail.To.Add(Convert.ToString(obj_OCompanyInfo.SupportEmailID));
                mail.Subject = "Offer My Space";
                mail.Body = " Name : " + txtName.Text + " \n PhoneNumber : " + txtPhoneNumber.Text + " \n Email : " + txtEmail.Text + " \n Message : " + txtOtherDetails.Text + "";

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



        async void txtOtherDetails_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_OtherDetails(true);
            }
        }
        async void txtOtherDetails_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_OtherDetails(true);
            }
            if (!string.IsNullOrEmpty(txtOtherDetails.Text))
            {
                await TransitionToTitle_OtherDetails(true);
            }
        }
        void lblOther_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtOtherDetails.Focus();
            }
        }
        async Task TransitionToTitle_OtherDetails(bool animated)
        {
            if (animated)
            {
                var t1 = lblOther.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_OtherDetails(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblOther.TranslationX = 0;
                lblOther.TranslationY = -30;
                lblOther.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_OtherDetails(bool animated)
        {
            if (animated)
            {
                var t1 = lblOther.TranslateTo(10, 0, 100);
                var t2 = SizeTo_OtherDetails(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblOther.TranslationX = 10;
                lblOther.TranslationY = 0;
                lblOther.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_OtherDetails(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblOther.FontSize = input; };
            double startingHeight = lblOther.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblOther.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtOtherDetails_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }


        //For Floating Label End
    }
}