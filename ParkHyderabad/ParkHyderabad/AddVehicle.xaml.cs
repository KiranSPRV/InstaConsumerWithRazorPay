using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddVehicle : ContentPage
    {
        static int CustomerID;
        static int VehicleTypeID;
        static string VehicleTypeCode = VehicleTypeCodes.TwoWheeler;
        DALVehicle dal_Vehicle;
        decimal Latitude;
        decimal Longitude;
        string From = string.Empty;
        static bool IsOnline = false;

        //Floating Lable Code Start
        int _placeholderFontSize = 14;
        int _titleFontSize = 12;
        int _topMargin = -20;

        public event EventHandler Completed;
        //Floating Lable Code End
        public AddVehicle(int customerID, string from, string vehicleTypeCode)
        {
            InitializeComponent();
            ShowLoading(true);
            CustomerID = customerID;

            VehicleTypeCode = vehicleTypeCode;
            From = from;

            if (string.IsNullOrEmpty(from))
            {
                NavigationPage.SetHasBackButton(this, true);
            }
            else
            {
                NavigationPage.SetHasBackButton(this, false);
            }

            if (VehicleTypeCode == "2W")
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
                VehicleTypeCode = VehicleTypeCodes.TwoWheeler;
                lblMake.Text = "Make (Ex:Honda)";
                lblModel.Text = "Model (Ex:Activa)";
                lblColor.Text = "Color (Ex:Red)";

            }
            else if (VehicleTypeCode == "4W")
            {
                svgTwo.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
                svgFour.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
                VehicleTypeCode = VehicleTypeCodes.FourWheeler;
                lblMake.Text = "Make (Ex:Maruti)";
                lblModel.Text = "Model (Ex:Swift)";
                lblColor.Text = "Color (Ex:Blue)";
            }

            ShowLoading(false);

            //Floating Lable Code Start
            lblMake.TranslationX = lblModel.TranslationX = lblColor.TranslationX = lblRegNumber.TranslationX = 10;
            lblMake.FontSize = lblModel.FontSize = lblColor.FontSize = lblRegNumber.FontSize = _placeholderFontSize;
            lblMake.TextColor = lblModel.TextColor = lblColor.TextColor = lblRegNumber.TextColor = Color.FromHex("#a3a3a3");

            txtMake.Focus();
            //Floating Lable Code End
        }
        private async void btn_AddVehicleClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();

            if (IsOnline)
            {
                ShowLoading(true);
                btnAddVehicle.IsVisible = false;

                if (txtMake.Text == "" || txtMake.Text == null)
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter Brand of your Vehicle", "Ok");
                    return;
                }

                var regexName = new Regex("^[a-zA-Z ]*$");
                if (!regexName.IsMatch(txtMake.Text.Trim()))
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter alphabets for Vehicle Brand", "Ok");
                    return;
                }

                if (txtModel.Text == "" || txtModel.Text == null)
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter Model of your Vehicle", "Ok");
                    return;
                }

                regexName = new Regex("^[a-zA-Z0-9 ]*$");
                if (!regexName.IsMatch(txtModel.Text.Trim()))
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please do not enter special characters for Vehicle Model", "Ok");
                    return;
                }

                if (txtColor.Text == "" || txtColor.Text == null)
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter Color of your Vehicle", "Ok");
                    return;
                }

                regexName = new Regex("^[a-zA-Z ]*$");
                if (!regexName.IsMatch(txtColor.Text.Trim()))
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please do not enter special characters for Vehicle Color", "Ok");
                    return;
                }

                if (txtRegNumber.Text == "" || txtRegNumber.Text == null)
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter Registration Number of your Vehicle", "Ok");
                    return;
                }

                regexName = new Regex("^[a-zA-Z0-9]*$");
                if (!regexName.IsMatch(txtRegNumber.Text.Trim()))
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please enter alphabets and numbers only in Registration Number", "Ok");
                    return;
                }

                if (ValidRegistrationNumber(txtRegNumber.Text.Trim()))
                {
                    if (ValidateRegistrationNumber())
                    {
                        btnAddVehicle.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "This Registration Number already exists!", "Ok");
                    }
                    else
                    {
                        CustomerVehicle objCustomerVehicle = new CustomerVehicle();
                        objCustomerVehicle.CustomerID = CustomerID;
                        objCustomerVehicle.VehicleTypeCode = VehicleTypeCode;
                        objCustomerVehicle.Make = txtMake.Text.Trim();
                        objCustomerVehicle.Model = txtModel.Text.Trim();
                        objCustomerVehicle.Color = txtColor.Text.Trim();
                        objCustomerVehicle.RegistrationNumber = txtRegNumber.Text.Trim();
                        objCustomerVehicle.IsPrimaryVehicle = swtPrimary.IsToggled;

                        swtActive.IsToggled = true;
                        objCustomerVehicle.IsActive = swtActive.IsToggled;

                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            var json = JsonConvert.SerializeObject(objCustomerVehicle);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            dal_Vehicle = new DALVehicle();
                            OCustomerVehicle resultObj = new OCustomerVehicle();

                            await Task.Run(() =>
                            {
                                resultObj = dal_Vehicle.InsertCustomerVehicle(Convert.ToString(App.Current.Properties["apitoken"]), objCustomerVehicle);
                            });

                            if (resultObj.CustomerVehicleID != 0)
                            {
                                var location = await Geolocation.GetLastKnownLocationAsync();
                                if (location != null)
                                {
                                    Latitude = (decimal)location.Latitude;
                                    Longitude = (decimal)location.Longitude;
                                }
                                else
                                {
                                    btnAddVehicle.IsVisible = true;
                                    ShowLoading(false);
                                    DisplayAlert("", "Please turn on device location", "Ok");
                                    return;
                                }

                                App.Current.Properties["Vehicle"] = true;
                                App.Current.Properties["VehicleTypeID"] = resultObj.VehicleTypeID;
                                App.Current.Properties["CustomerID"] = CustomerID;
                                App.Current.Properties["CustomerVehicleID"] = resultObj.CustomerVehicleID;
                                App.Current.Properties["RegistrationNumber"] = objCustomerVehicle.RegistrationNumber;
                                btnAddVehicle.IsVisible = true;
                                await Application.Current.SavePropertiesAsync();
                                //await Navigation.PushAsync(new MasterPage(objLocationParkingLotFilter));
                                await Navigation.PushAsync(new Home(null, CustomerID));
                                ShowLoading(false);
                            }
                        }
                    }
                }
                else
                {
                    btnAddVehicle.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("", "Please provide valid vehicle Registration Number ", "Ok");
                }
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }            
        }
        public bool ValidRegistrationNumber(string registrationNumber)
        {
            bool result = false;

            if (registrationNumber.Length >= 6 & registrationNumber.Length <= 10)
            {
                if (Regex.IsMatch(registrationNumber.Substring(registrationNumber.Length - 4, 4), @"^[0-9]*$"))
                {
                    result = true;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        public bool ValidateRegistrationNumber()
        {
            bool result = false;

            if (App.Current.Properties.ContainsKey("apitoken"))
            {
                CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                obj_CustomerVehicle.CustomerID = CustomerID;
                obj_CustomerVehicle.CustomerVehicleID = 0;
                obj_CustomerVehicle.CustomerVehicleMapperID = 0;
                obj_CustomerVehicle.RegistrationNumber = txtRegNumber.Text.Trim();

                var json = JsonConvert.SerializeObject(obj_CustomerVehicle);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                dal_Vehicle = new DALVehicle();
                OCustomerVehicle resultObj = dal_Vehicle.ValidateRegistrationNumber(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                if (resultObj.CustomerVehicleID != 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
        private async void TwoWheeler_Tapped(object sender, EventArgs e)
        {
            svgTwo.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
            svgFour.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
            VehicleTypeCode = VehicleTypeCodes.TwoWheeler;
            lblMake.Text = "Make (Ex:Honda)";
            lblModel.Text = "Model (Ex:Activa)";
            lblColor.Text = "Color (Ex:Red)";
        }
        private async void FourWheeler_Tapped(object sender, EventArgs e)
        {
            svgTwo.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
            svgFour.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
            VehicleTypeCode = VehicleTypeCodes.FourWheeler;
            lblMake.Text = "Make (Ex:Maruti)";
            lblModel.Text = "Model (Ex:Swift)";
            lblColor.Text = "Color (Ex:Blue)";
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
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
        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(From))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //For Floating Label Start 
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(string), string.Empty, BindingMode.TwoWay, null, null);
        async void txtMake_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle(true);
            }
        }
        async void txtMake_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder(true);
            }
            if (!string.IsNullOrEmpty(txtMake.Text))
            {
                await TransitionToTitle(true);
            }
        }
        void lblMake_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtMake.Focus();
            }
        }
        async Task TransitionToTitle(bool animated)
        {
            if (animated)
            {
                var t1 = lblMake.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblMake.TranslationX = 0;
                lblMake.TranslationY = -30;
                lblMake.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder(bool animated)
        {
            if (animated)
            {
                var t1 = lblMake.TranslateTo(10, 0, 100);
                var t2 = SizeTo(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblMake.TranslationX = 10;
                lblMake.TranslationY = 0;
                lblMake.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblMake.FontSize = input; };
            double startingHeight = lblMake.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblMake.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtMake_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }
        private async void txtModel_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_Model(true);
            }
        }
        async void txtModel_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_Model(true);
            }
            if (!string.IsNullOrEmpty(txtModel.Text))
            {
                await TransitionToTitle_Model(true);
            }
        }
        void lblModel_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtModel.Focus();
            }
        }
        async Task TransitionToTitle_Model(bool animated)
        {
            if (animated)
            {
                var t1 = lblModel.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_Model(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblModel.TranslationX = 0;
                lblModel.TranslationY = -30;
                lblModel.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_Model(bool animated)
        {
            if (animated)
            {
                var t1 = lblModel.TranslateTo(10, 0, 100);
                var t2 = SizeTo_Model(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblModel.TranslationX = 10;
                lblModel.TranslationY = 0;
                lblModel.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_Model(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblModel.FontSize = input; };
            double startingHeight = lblModel.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblModel.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtModel_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }
        async void txtColor_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_Color(true);
            }
        }
        async void txtColor_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_Color(true);
            }
            if (!string.IsNullOrEmpty(txtColor.Text))
            {
                await TransitionToTitle_Color(true);
            }
        }
        void lblColor_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtColor.Focus();
            }
        }
        async Task TransitionToTitle_Color(bool animated)
        {
            if (animated)
            {
                var t1 = lblColor.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_Color(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblColor.TranslationX = 0;
                lblColor.TranslationY = -30;
                lblColor.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_Color(bool animated)
        {
            if (animated)
            {
                var t1 = lblColor.TranslateTo(10, 0, 100);
                var t2 = SizeTo_Color(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblColor.TranslationX = 10;
                lblColor.TranslationY = 0;
                lblColor.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_Color(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblColor.FontSize = input; };
            double startingHeight = lblColor.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblColor.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtColor_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }
        async void txtRegNumber_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_RegNumber(true);
            }
        }
        async void txtRegNumber_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_RegNumber(true);
            }
            if (!string.IsNullOrEmpty(txtRegNumber.Text))
            {
                await TransitionToTitle_RegNumber(true);
            }
        }
        void lblRegNumber_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtRegNumber.Focus();
            }
        }
        async Task TransitionToTitle_RegNumber(bool animated)
        {
            if (animated)
            {
                var t1 = lblRegNumber.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_RegNumber(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblRegNumber.TranslationX = 0;
                lblRegNumber.TranslationY = -30;
                lblRegNumber.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_RegNumber(bool animated)
        {
            if (animated)
            {
                var t1 = lblRegNumber.TranslateTo(10, 0, 100);
                var t2 = SizeTo_RegNumber(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblRegNumber.TranslationX = 10;
                lblRegNumber.TranslationY = 0;
                lblRegNumber.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_RegNumber(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblRegNumber.FontSize = input; };
            double startingHeight = lblRegNumber.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblRegNumber.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtRegNumber_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        //For Floating Label End        
    }
}