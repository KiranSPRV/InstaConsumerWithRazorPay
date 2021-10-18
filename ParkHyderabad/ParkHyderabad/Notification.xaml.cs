using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Notification : ContentPage
    {
        DALCustomer dal_Customer;
        static bool IsOnline = false;
        public Notification(int CustomerID)
        {
            InitializeComponent();
            dal_Customer = new DALCustomer();
            GetNotificationsByCustomerID(CustomerID);
        }
        public void GetNotificationsByCustomerID(int CustomerID)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        if (App.Current.Properties.ContainsKey("CustomerID"))
                        {
                            Customer obj_Customer = new Customer();
                            obj_Customer.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                            List<ParkHyderabad.Model.APIOutPutModel.Notification> obj_listNotification = new List<ParkHyderabad.Model.APIOutPutModel.Notification>();
                            obj_listNotification = dal_Customer.GetNotificationsByCustomerID(Convert.ToString(App.Current.Properties["apitoken"]), obj_Customer);

                            if (obj_listNotification.Count > 0)
                            {
                                if (obj_listNotification[0].NotificationID != 0)
                                {
                                    lstNotification.ItemsSource = obj_listNotification;
                                }
                                else
                                {
                                    lstNotification.ItemsSource = null;
                                    DisplayAlert("", "No new notifications", "Ok");
                                }
                            }
                        }
                        else
                        {
                            DisplayAlert("Failed", "Profile not found!", "Ok");
                        }
                    }
                    else
                    {
                        DisplayAlert("Failed", "Token does not exists", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetNotificationsByCustomerID", Convert.ToString(ex.Message), "Ok");
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