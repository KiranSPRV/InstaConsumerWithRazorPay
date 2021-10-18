using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
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
    public partial class MyProfile : ContentPage
    {
        DALCustomer dal_Customer;
        static int CustomerID;
        static string Gender_static;
        string fileName = string.Empty;
        byte[] imgCameraByteData = null;

        //Floating Lable Code Start
        int _placeholderFontSize = 14;
        int _titleFontSize = 12;
        int _topMargin = -20;
        public event EventHandler Completed;
        static bool IsOnline = false;
        //Floating Lable Code End
        public MyProfile(OCustomer customer)
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Customer = new DALCustomer();
            CustomerID = customer.CustomerID;
            Gender_static = customer.Gender;
            txtName.Text = customer.Name;
            txtPhoneNumber.Text = customer.PhoneNumber;
            txtEmail.Text = customer.Email;
            imgProfile.Source = customer.ProfileImage == null ? "profilepic.png" :
            ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress(customer.ProfileImage)));

            imgCameraByteData = customer.ProfileImage == null ? null :
                new MemoryStream(ByteArrayCompressionUtility.Decompress(customer.ProfileImage)).ToArray();

            if (customer.YearOfBirth != 0)
            {
                txtYearOfBirth.Text = Convert.ToString(customer.YearOfBirth);
            }
            else
            {
                txtYearOfBirth.Text = "";
            }
            ShowLoading(false);

            //Floating Lable Code Start
            lblName.TranslationX = lblPhoneNumber.TranslationX = lblEmail.TranslationX = lblYear.TranslationX = 10;
            lblName.FontSize = lblPhoneNumber.FontSize = lblEmail.FontSize = lblYear.FontSize = _placeholderFontSize;
            lblName.TextColor = lblPhoneNumber.TextColor = lblEmail.TextColor = lblYear.TextColor = Color.Gray;

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
                TransitionToPlaceholder_YearofBirth(true);
            }
            else
            {
                TransitionToTitle_YearofBirth(true);
            }

            txtName.Focus();
            //Floating Lable Code End
        }
        private async void btn_UpdateClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();

            if (IsOnline)
            {
                btnUpdate.IsVisible = false;
                ShowLoading(true);

                try
                {
                    if (txtName.Text == "" || txtName.Text == null)
                    {
                        btnUpdate.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Please enter your Name", "Ok");
                        return;
                    }

                    var regexName = new Regex("^[a-z A-Z]*$");
                    if (!regexName.IsMatch(txtName.Text.Trim()))
                    {
                        btnUpdate.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Please enter only alphabets for Name", "Ok");
                        return;
                    }

                    if (txtPhoneNumber.Text == "" || txtPhoneNumber.Text == null)
                    {
                        btnUpdate.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Please enter your Phone Number", "Ok");
                        return;
                    }

                    if (txtEmail.Text == "" || txtEmail.Text == null)
                    {
                        btnUpdate.IsVisible = true;
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
                                btnUpdate.IsVisible = true;
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

                        if (imgCameraByteData != null)
                        {
                            objCustomer.ProfileImage = ByteArrayCompressionUtility.Compress(imgCameraByteData);
                        }

                        if (App.Current.Properties.ContainsKey("RefreshedToken"))
                        {
                            btnUpdate.IsVisible = true;
                            objCustomer.DeviceID = Convert.ToString(App.Current.Properties["RefreshedToken"]);
                        }
                        else
                        {
                            btnUpdate.IsVisible = true;
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
                                btnUpdate.IsVisible = true;
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
                            OCustomer resultObj = new OCustomer();
                            await Task.Run(() =>
                            {
                                resultObj = dal_Customer.UpdateCustomer(Convert.ToString(App.Current.Properties["apitoken"]), objCustomer);
                            });
                            if (resultObj.CustomerID != 0)
                            {
                                App.Current.Properties["UserName"] = txtName.Text.Trim();
                                App.Current.Properties["PhoneNumber"] = txtPhoneNumber.Text.Trim();
                                App.Current.Properties["Email"] = txtEmail.Text.Trim();
                                App.Current.Properties["CustomerID"] = resultObj.CustomerID;
                                App.Current.Properties["ProfileImage"] = resultObj.ProfileImage;
                                await Application.Current.SavePropertiesAsync();
                                int CustomerID = resultObj.CustomerID;
                                App.Current.Properties["CustomerID"] = CustomerID;

                                btnUpdate.IsVisible = true;
                                ShowLoading(false);
                                await Navigation.PushAsync(new Home(null, CustomerID));
                            }
                            else
                            {
                                btnUpdate.IsVisible = true;
                                ShowLoading(false);
                                DisplayAlert("Failed - UpdateCustomer", "Update Customer Failed", "Ok");
                            }
                        }
                        else
                        {
                            btnUpdate.IsVisible = true;
                            ShowLoading(false);
                        }
                    }
                    else
                    {
                        btnUpdate.IsVisible = true;
                        ShowLoading(false);
                        DisplayAlert("", "Name must be min of 3 and max of 20 characters", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    btnUpdate.IsVisible = true;
                    ShowLoading(false);
                    DisplayAlert("Failed - ", "Failed To Update Details", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
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
        private async void img_ProfileClicked(object sender, EventArgs e)
        {
            stkCameraGallery.IsVisible = true;
        }
        private async void img_CameraClicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    stkCameraGallery.IsVisible = false;
                    ShowLoading(false);
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Custom,
                    Directory = "ProfilePics",
                    SaveToAlbum = false,
                    CustomPhotoSize = 20,
                    CompressionQuality = 92
                });

                if (file == null)
                    return;

                if (file != null)
                {
                    if (file.Path.Contains("/"))
                    {
                        try
                        {
                            Stream objfilestream = file.GetStream();
                            string[] getfilename = file.Path.Split('/');
                            fileName = getfilename[getfilename.Length - 1];
                            var memoryStream = new MemoryStream();
                            file.GetStream().CopyTo(memoryStream);
                            imgCameraByteData = memoryStream.ToArray();
                            imgProfile.Source = file.Path;
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            stkCameraGallery.IsVisible = false;
            ShowLoading(false);
        }
        private async void img_GalleryClicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                stkCameraGallery.IsVisible = false;
                ShowLoading(false);
                return;
            }
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Custom,
                    CustomPhotoSize = 20, // Resize to 20% of original
                    CompressionQuality = 92
                });

                if (file == null)
                    return;

                if (file != null)
                {
                    if (file.Path.Contains("/"))
                    {
                        try
                        {
                            Stream objfilestream = file.GetStream();
                            string[] getfilename = file.Path.Split('/');
                            fileName = getfilename[getfilename.Length - 1];
                            var memoryStream = new MemoryStream();
                            file.GetStream().CopyTo(memoryStream);
                            imgCameraByteData = memoryStream.ToArray();
                            imgProfile.Source = ImageSource.FromStream(() => objfilestream);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("No Camera", "Unable to open:" + ex.Message, "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            stkCameraGallery.IsVisible = false;
            ShowLoading(false);
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



        async void txtYearOfBirth_Handle_Focused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToTitle_YearofBirth(true);
            }
        }
        async void txtYearOfBirth_Handle_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                await TransitionToPlaceholder_YearofBirth(true);
            }
            if (!string.IsNullOrEmpty(txtYearOfBirth.Text))
            {
                await TransitionToTitle_YearofBirth(true);
            }
        }
        void lblYear_Handle_Tapped(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                txtYearOfBirth.Focus();
            }
        }
        async Task TransitionToTitle_YearofBirth(bool animated)
        {
            if (animated)
            {
                var t1 = lblYear.TranslateTo(2, _topMargin, 100);
                var t2 = SizeTo_YearofBirth(_titleFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblYear.TranslationX = 0;
                lblYear.TranslationY = -30;
                lblYear.FontSize = 14;
            }
        }
        async Task TransitionToPlaceholder_YearofBirth(bool animated)
        {
            if (animated)
            {
                var t1 = lblYear.TranslateTo(10, 0, 100);
                var t2 = SizeTo_YearofBirth(_placeholderFontSize);
                await Task.WhenAll(t1, t2);
            }
            else
            {
                lblYear.TranslationX = 10;
                lblYear.TranslationY = 0;
                lblYear.FontSize = _placeholderFontSize;
            }
        }
        Task SizeTo_YearofBirth(int fontSize)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // setup information for animation
            Action<double> callback = input => { lblYear.FontSize = input; };
            double startingHeight = lblYear.FontSize;
            double endingHeight = fontSize;
            uint rate = 5;
            uint length = 100;
            Easing easing = Easing.Linear;

            // now start animation with all the setup information
            lblYear.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));

            return taskCompletionSource.Task;
        }
        void txtYearOfBirth_Handle_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, e);
        }


        //For Floating Label End
    }
}