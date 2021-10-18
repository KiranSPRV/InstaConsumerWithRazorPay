using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
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
    [DesignTimeVisible(true)]
    public partial class SignUp : ContentPage
    {
        DALCustomer dal_Customer;
        static string Gender_static = "Male";
        static string PhoneNumber;
        static int CustomerID;

        //Floating Lable Code Start
        int _placeholderFontSize = 14;
        int _titleFontSize = 12;
        int _topMargin = -20;
        public event EventHandler Completed;

        public SignUp(int customerID, string phoneNumber)
        {
            InitializeComponent();
            ShowLoading(true);
            PhoneNumber = phoneNumber;
            CustomerID = customerID;
            txtPhoneNumber.Text = phoneNumber;
            Gender_static = "Male";
            ShowLoading(false);

            //Floating Lable Code Start
            lblName.TranslationX = lblPhoneNumber.TranslationX = lblEmail.TranslationX = lblYearOfBirth.TranslationX = 10;
            lblName.FontSize = lblPhoneNumber.FontSize = lblEmail.FontSize = lblYearOfBirth.FontSize = _placeholderFontSize;
            lblName.TextColor = lblPhoneNumber.TextColor =  txtPhoneNumber.TextColor = lblEmail.TextColor = lblYearOfBirth.TextColor = Color.FromHex("#FFFFFF");

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
            if (string.IsNullOrEmpty(txtYearOfBirth.Text))
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
        private async void imgbtn_MaleClicked(object sender, EventArgs e)
        {
            imgBtnMale.Source = "resource://ParkHyderabad.Resources.maleselect.svg";
            imgBtnFemale.Source = "resource://ParkHyderabad.Resources.femaleunselect.svg";
            imgBtnTrans.Source = "resource://ParkHyderabad.Resources.otherunselect.svg";
            Gender_static = "Male";
        }
        private async void imgbtn_FemaleClicked(object sender, EventArgs e)
        {
            imgBtnMale.Source = "resource://ParkHyderabad.Resources.maleunselect.svg";
            imgBtnFemale.Source = "resource://ParkHyderabad.Resources.femaleselect.svg";
            imgBtnTrans.Source = "resource://ParkHyderabad.Resources.otherunselect.svg";
            Gender_static = "Female";
        }
        private async void imgbtn_TransgenderClicked(object sender, EventArgs e)
        {
            imgBtnMale.Source = "resource://ParkHyderabad.Resources.maleunselect.svg";
            imgBtnFemale.Source = "resource://ParkHyderabad.Resources.femaleunselect.svg";
            imgBtnTrans.Source = "resource://ParkHyderabad.Resources.otherselect.svg";
            Gender_static = "Transgender";
        }
        private async void btn_SendOTPClicked(object sender, EventArgs e)
        {
            btnSendOTP.IsVisible = false;
            ShowLoading(true);

            if (txtName.Text == "" || txtName.Text == null)
            {
                btnSendOTP.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("", "Please enter your Name", "Ok");
                return;
            }

            var regexName = new Regex("^[a-z A-Z]*$");
            if (!regexName.IsMatch(txtName.Text.Trim()))
            {
                btnSendOTP.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("", "Please enter only alphabets for Name", "Ok");
                return;
            }

            if (txtPhoneNumber.Text == "" || txtPhoneNumber.Text == null)
            {
                btnSendOTP.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("", "Please enter your Phone Number", "Ok");
                return;
            }

            if (txtEmail.Text == "" || txtEmail.Text == null)
            {
                btnSendOTP.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("", "Please enter your Email", "Ok");
                return;
            }

            if (txtName.Text.Length >= 3 && txtName.Text.Length <= 20)
            {
                if (txtEmail.Text != "" && txtEmail.Text != null)
                {
                    if (!RegexUtilities.IsEmailValid(txtEmail.Text.Trim()))
                    {
                        btnSendOTP.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Please enter valid Email", "Ok");
                        return;
                    }
                }

                Customer objCustomer = new Customer();
                objCustomer.CustomerID = CustomerID;
                objCustomer.Name = txtName.Text.Trim();
                objCustomer.Email = txtEmail.Text == null || txtEmail.Text == "" ? "" : txtEmail.Text.Trim();
                objCustomer.PhoneNumber = txtPhoneNumber.Text.Trim();
                objCustomer.Gender = Gender_static;

                if (App.Current.Properties.ContainsKey("RefreshedToken"))
                {
                    btnSendOTP.IsVisible = true;
                    objCustomer.DeviceID = Convert.ToString(App.Current.Properties["RefreshedToken"]);                    
                }
                else
                {
                    btnSendOTP.IsVisible = true;
                    objCustomer.DeviceID = "";
                }

                if (txtYearOfBirth.Text != "" && txtYearOfBirth.Text != null)
                {
                    if (Convert.ToInt32(txtYearOfBirth.Text.Trim()) > 1900)
                    {
                        DateTime dateOfBirth = new DateTime(Convert.ToInt32(txtYearOfBirth.Text.Trim()), DateTime.Now.Month, 1);
                        objCustomer.DateOfBirth = dateOfBirth;
                        objCustomer.Age = Convert.ToInt32(DateTime.Now.Year) - Convert.ToInt32(txtYearOfBirth.Text.Trim());
                    }
                    else
                    {
                        btnSendOTP.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Only people born after 1900 please! Enter your Year of Birth", "Ok");
                        return;
                    }
                }
                else
                {
                    objCustomer.DateOfBirth = null;
                    objCustomer.Age = 0;
                }

                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    var json = JsonConvert.SerializeObject(objCustomer);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    dal_Customer = new DALCustomer();
                    OCustomer resultObj = dal_Customer.InsertCustomer(Convert.ToString(App.Current.Properties["apitoken"]), objCustomer);

                    if (resultObj.CustomerID != 0)
                    {                       
                        btnSendOTP.IsVisible = true;
                        ShowLoading(false);
                        await Application.Current.SavePropertiesAsync();
                        await Navigation.PushAsync(new VerifyOTP(txtPhoneNumber.Text, resultObj.CustomerID, "NEW", txtName.Text));
                    }
                    else
                    {
                        btnSendOTP.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("Failed - InsertCustomer", "Insert customer failed", "Ok");
                    }
                }
                else
                {
                    btnSendOTP.IsVisible = true;
                    ShowLoading(false);
                }
            }
            else
            {
                btnSendOTP.IsVisible = true;
                ShowLoading(false);
                DisplayAlert("", "Name must be min of 3 and max of 20 characters", "Ok");
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



        async void txtYearOfBirth_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_Message(true);
            }
        }
        async void txtYearOfBirth_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_Message(true);
            }
            if (!string.IsNullOrEmpty(txtYearOfBirth.Text))
            {
                await TransitionToTitle_Message(true);
            }
        }
        void lblYearOfBirth_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtYearOfBirth.Focus();
            }
        }
        async Task TransitionToTitle_Message(bool animated)
        {
            if (animated)
            {
                var t1 = lblYearOfBirth.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_Message(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblYearOfBirth.TranslationX = 0;
                lblYearOfBirth.TranslationY = -30;
                lblYearOfBirth.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_Message(bool animated)
        {
            if (animated)
            {
                var t1 = lblYearOfBirth.TranslateTo(10, 0, 100);
                var t2 = SizeTo_Message(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblYearOfBirth.TranslationX = 10;
                lblYearOfBirth.TranslationY = 0;
                lblYearOfBirth.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_Message(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblYearOfBirth.FontSize = input; };
            double startingHeight = lblYearOfBirth.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblYearOfBirth.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtYearOfBirth_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }


        //For Floating Label End

    }
}
