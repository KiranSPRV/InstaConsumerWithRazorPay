using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class History : ContentPage
    {
        DALCustomer dal_Customer;
        static CustomerPaymentHistoryDetails obj_cphd;
        private bool IsOnline = false;
        public History()
        {
            InitializeComponent();
            ShowLoading(true);
            dal_Customer = new DALCustomer();
            GetLotParkingDetailsByCustomerID();
            ShowLoading(false);
        }
        private async void GetLotParkingDetailsByCustomerID()
        {
            try
            {
                if (App.Current.Properties.ContainsKey("apitoken"))
                {
                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {
                        Customer obj_Customer = new Customer();
                        obj_Customer.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        List<CustomerPaymentHistoryDetails> lst_objCustomerParkingSlotDetails = new List<CustomerPaymentHistoryDetails>();
                        List<CustomerPaymentHistoryDetails> lst_objCustomerParkingSlotDetailsnew = new List<CustomerPaymentHistoryDetails>();

                        lstLotParking.ItemsSource = null;
                        ShowLoading(true);
                        await Task.Run(() =>
                        {
                            
                            lst_objCustomerParkingSlotDetails = dal_Customer.GetLotParkingDetailsByCustomerID(Convert.ToString(App.Current.Properties["apitoken"]), obj_Customer);

                            if (lst_objCustomerParkingSlotDetails.Count > 0)
                            {
                                for (int j = 0; j < lst_objCustomerParkingSlotDetails.Count; j++)
                                {
                                    if (lst_objCustomerParkingSlotDetails[j].InvoiceType == "CHECKIN")
                                    {
                                        lst_objCustomerParkingSlotDetails[j].LocationLotName = lst_objCustomerParkingSlotDetails[j].LocationParkingLotName + ", Bay " + lst_objCustomerParkingSlotDetails[j].ParkingBayRange;
                                        lst_objCustomerParkingSlotDetails[j].FromAndTo = lst_objCustomerParkingSlotDetails[j].FromAndTo + " (" + lst_objCustomerParkingSlotDetails[j].Duration + " hrs)";
                                        lst_objCustomerParkingSlotDetails[j].HistoryType = "Parking";
                                        lst_objCustomerParkingSlotDetails[j].PassValidityVisible = false;
                                        if (lst_objCustomerParkingSlotDetails[j].ApplicationTypeID == 4)
                                        {
                                            lst_objCustomerParkingSlotDetails[j].PassFlag = true;
                                            lst_objCustomerParkingSlotDetails[j].PaidSectionVisible = false;
                                            lst_objCustomerParkingSlotDetails[j].DueClampSectionVisible = false;
                                            lst_objCustomerParkingSlotDetails[j].FeeDueClampAmount = "";
                                        }
                                        else
                                        {
                                            lst_objCustomerParkingSlotDetails[j].PassFlag = false;
                                            lst_objCustomerParkingSlotDetails[j].PaidSectionVisible = true;

                                            if (lst_objCustomerParkingSlotDetails[j].IsClamp == true || Convert.ToDouble(lst_objCustomerParkingSlotDetails[j].PaidDueAmount) > 0)
                                            {
                                                lst_objCustomerParkingSlotDetails[j].DueClampSectionVisible = true;
                                            }
                                            else
                                            {
                                                lst_objCustomerParkingSlotDetails[j].DueClampSectionVisible = false;
                                                //lst_objCustomerParkingSlotDetails[j].FeeDueClampAmount = "";
                                            }
                                        }
                                        lst_objCustomerParkingSlotDetails[j].PaymentTypeName = "Via " + lst_objCustomerParkingSlotDetails[j].PaymentTypeName;
                                    }
                                    else if (lst_objCustomerParkingSlotDetails[j].InvoiceType == "PASS")
                                    {
                                        if (lst_objCustomerParkingSlotDetails[j].IsMultiLot == true)
                                        {
                                            string multilocname = "";
                                            if (lst_objCustomerParkingSlotDetails[j].PrimaryLocationParkingLotID == 0)
                                            {
                                                for (int z = 0; z < lst_objCustomerParkingSlotDetails.Count; z++)
                                                {
                                                    if (lst_objCustomerParkingSlotDetails[z].PrimaryLocationParkingLotID != 0)
                                                    {
                                                        if (lst_objCustomerParkingSlotDetails[z].PrimaryLocationParkingLotID == lst_objCustomerParkingSlotDetails[j].CustomerVehiclePassID)
                                                        {
                                                            multilocname = multilocname + "," + lst_objCustomerParkingSlotDetails[z].LocationLotName;
                                                        }
                                                    }
                                                }
                                            }
                                            if (multilocname.Length > 0)
                                            {
                                                lst_objCustomerParkingSlotDetails[j].HistoryType = lst_objCustomerParkingSlotDetails[j].PassTypeName + " - " + lst_objCustomerParkingSlotDetails[j].SingleOrMulti + " (" + multilocname.Substring(1) + ")";
                                            }
                                            else
                                            {
                                                lst_objCustomerParkingSlotDetails[j].HistoryType = lst_objCustomerParkingSlotDetails[j].PassTypeName + " - " + lst_objCustomerParkingSlotDetails[j].SingleOrMulti;// + " (" + multilocname + ")";
                                            }
                                        }
                                        else
                                        {
                                            lst_objCustomerParkingSlotDetails[j].HistoryType = lst_objCustomerParkingSlotDetails[j].PassTypeName + " - " + lst_objCustomerParkingSlotDetails[j].SingleOrMulti;// + " (" + lst_objCustomerParkingSlotDetails[j].CardTypeName + ")";
                                        }

                                        lst_objCustomerParkingSlotDetails[j].LocationLotName = lst_objCustomerParkingSlotDetails[j].LocationName;
                                        lst_objCustomerParkingSlotDetails[j].FromAndTo = lst_objCustomerParkingSlotDetails[j].FromDisplay;
                                        lst_objCustomerParkingSlotDetails[j].PassValidityVisible = true;
                                        lst_objCustomerParkingSlotDetails[j].PassValidity = "Validity : " + lst_objCustomerParkingSlotDetails[j].FromDisplay + " to " + lst_objCustomerParkingSlotDetails[j].ToDisplay;
                                        lst_objCustomerParkingSlotDetails[j].PassFlag = false;
                                        lst_objCustomerParkingSlotDetails[j].PaidSectionVisible = true;
                                        lst_objCustomerParkingSlotDetails[j].PaymentTypeName = "Via " + lst_objCustomerParkingSlotDetails[j].PaymentTypeName;
                                        lst_objCustomerParkingSlotDetails[j].DueClampSectionVisible = false;
                                        lst_objCustomerParkingSlotDetails[j].FeeDueClampAmount = "";
                                    }
                                }

                                for (int b = 0; b < lst_objCustomerParkingSlotDetails.Count; b++)
                                {
                                    if (lst_objCustomerParkingSlotDetails[b].PrimaryLocationParkingLotID == 0)
                                    {
                                        lst_objCustomerParkingSlotDetailsnew.Add(lst_objCustomerParkingSlotDetails[b]);
                                    }
                                }
                            }
                        });

                        if (lst_objCustomerParkingSlotDetailsnew.Count > 0)
                        {
                            lstLotParking.ItemsSource = lst_objCustomerParkingSlotDetailsnew;
                            ShowLoading(false);                           
                        }
                        else
                        {
                            lstLotParking.ItemsSource = null;
                            ShowLoading(false);
                            DisplayAlert("", "Invoices not found", "Ok");
                        }
                        ShowLoading(false);
                    }
                    else
                    {
                        ShowLoading(false);
                        await DisplayAlert("", "Profile not found!", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                await DisplayAlert("Failed - GetLotParkingDetailsByCustomerID", Convert.ToString(ex.Message), "Ok");
            }
        }
        private void lstLotParking_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            ((ListView)sender).SelectedItem = null;
        }
        private void lstLotParking_Refreshing(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    GetLotParkingDetailsByCustomerID();
                    lstLotParking.IsRefreshing = false;
                }
                else
                {
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                    lstLotParking.IsRefreshing = false;
                }
            }
            catch (Exception ex)
            {

            }
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
    }
}
