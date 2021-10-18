using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassReceipt : ContentPage
    {
        static OCustomerVehiclePass obj_CustomerVehiclePass;
        Location objLoc = new Location();
        List<Location> lst_Location = new List<Location>();
        public PassReceipt(OCustomerVehiclePass objCustomerVehiclePass)
        {
            InitializeComponent();
            ShowLoading(true);

            App.Current.Properties["UserName"] = objCustomerVehiclePass.CustomerName;
            App.Current.Properties["PhoneNumber"] = objCustomerVehiclePass.CustomerPhoneNumber;
            App.Current.Properties["Email"] = objCustomerVehiclePass.CustomerEmail;
            App.Current.Properties["CustomerID"] = objCustomerVehiclePass.CustomerID;
            App.Current.Properties["ProfileImage"] = objCustomerVehiclePass.CustomerProfileImage;

            if (Convert.ToDecimal(objCustomerVehiclePass.CardAmount) != 0)
            {
                DisplayAlert("", "please collect " + objCustomerVehiclePass.TagType + " from ParkHyderabad lot", "Ok");
                lblNFCDisplay.IsVisible = true;
            }

            obj_CustomerVehiclePass = objCustomerVehiclePass;

            lblPassTypeName.Text = obj_CustomerVehiclePass.PassTypeName.Replace("Pass", "").Trim();
            lblStationAccess.Text = obj_CustomerVehiclePass.SingleOrMulti;
            lblPriceDisplay.Text = obj_CustomerVehiclePass.Amount;

            if (obj_CustomerVehiclePass.IsMultiLot)
            {
                string[] str = obj_CustomerVehiclePass.LocationName.Split(',');

                for (int i = 0; i < str.Length; i++)
                {
                    objLoc = new Location();
                    objLoc.LocationNumber = i + 1;
                    objLoc.LocationName = str[i];
                    lst_Location.Add(objLoc);
                }
            }
            else
            {
                objLoc = new Location();
                objLoc.LocationNumber = 1;
                objLoc.LocationName = obj_CustomerVehiclePass.LocationName;
                lst_Location.Add(objLoc);
            }

            lstLocation.ItemsSource = lst_Location;

            if (obj_CustomerVehiclePass.PassCode.ToUpper() == "DP")
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
            }
            else if (obj_CustomerVehiclePass.PassCode.ToUpper() == "2W WP" || obj_CustomerVehiclePass.PassCode.ToUpper() == "4W WP")
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
            }
            else if (obj_CustomerVehiclePass.PassCode.ToUpper() == "2W ALL STATION" || obj_CustomerVehiclePass.PassCode.ToUpper() == "4W ALL STATION")
            {                
                lblLineStation.IsVisible = false;
                lblStation.IsVisible = false;
                lblAllStation.IsVisible = true;
                lstLocation.IsVisible = false;
            }
            else
            {
                lblLineStation.IsVisible = true;
                lblStation.IsVisible = true;
                lblAllStation.IsVisible = false;
                lstLocation.IsVisible = true;
            }

            lblFrom.Text = objCustomerVehiclePass.StartDate.ToString("d MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            lblTo.Text = objCustomerVehiclePass.ExpiryDate.ToString("d MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            lblModel.Text = objCustomerVehiclePass.Model;
            lblRegistrationNumber.Text = objCustomerVehiclePass.RegistrationNumber;
            lblParkingFee.Text = objCustomerVehiclePass.TotalAmount;
            //lblPaymentType.Text = "( Including " + objCustomerVehiclePass.TagType + " )";
            OrderID.Text = "ID : #" + objCustomerVehiclePass.CustomerVehiclePassID;
            imgVehicle.Source = objCustomerVehiclePass.VehicleImage;
            ShowLoading(false);
        }
        private async void btn_DoneClicked(object sender, EventArgs e)
        {
            try
            {
                int CustomerID = 0;
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                }

                await Navigation.PushAsync(new Home(null, CustomerID));
            }
            catch (Exception ex)
            {

            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
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
    }
}