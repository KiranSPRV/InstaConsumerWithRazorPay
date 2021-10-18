using FFImageLoading.Svg.Forms;
using ParkHyderabad.DAL;
using ParkHyderabad.Helper;
using ParkHyderabad.Model;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        static int LocationID;
        static int CustomerID;
        static int CustomerVehicleID;
        static int PrimaryVehicleType;
        static int VehicleTypeID;
        static string PreviousIcon = "";
        DALLocationParkingLots dal_LocationParkingLot;
        static string RegistrationNumber;
        static string VehicleImage;
        DALVehicle dal_Vehicle;
        DALCustomer dal_Customer;
        static float Latitude;
        static float Longitude;
        private bool IsOnline = false;
        static string FilterTwoWheelerTypeCode = "";
        static string FilterFourWheelerTypeCode = "";
        List<Model.APIOutPutModel.Location> lst_LocationParkingLot = new List<Model.APIOutPutModel.Location>();
        static int LocationParkingLotID;
        static LocationParkingLotDetails resultObj = new LocationParkingLotDetails();
        static LocationParkingLotFilter objLocationParkingLotFilter;
        List<LocationParkingLotService> lst_LocationParkingLotService = new List<LocationParkingLotService>();
        static OCustomerVehicle obj_OCustomerVehicle = new OCustomerVehicle();
        List<OCustomerVehicle> obj_listCustomerVehicle = new List<OCustomerVehicle>();
        List<LocationParkingLotDetails> obj_LocationParkingLotDetails = new List<LocationParkingLotDetails>();
        public List<Contact> AllContacts { get; set; }
        public DateTime currentTime = new DateTime();
        Model.APIOutPutModel.Location locationsearchfill = new Model.APIOutPutModel.Location();
        public Home(LocationParkingLotFilter locationParkingLotFilter, int customerID)
        {
            InitializeComponent();
            ShowLoading(true);
            dal_LocationParkingLot = new DALLocationParkingLots();
            dal_Vehicle = new DALVehicle();
            dal_Customer = new DALCustomer();


            App.Current.Properties["BaseURL"] = "http://devcusapi.instaparking.in/";
            App.Current.Properties["PaymentTypeID"] = 2;
            App.Current.Properties["ApplicationTypeID"] = 1;
            App.Current.Properties["PassApplicationTypeID"] = 4;
            App.Current.Properties["StatusID"] = 4;

            CustomerID = customerID;

            if (Latitude != 0 && Longitude != 0)
                CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Position(Latitude, Longitude), Distance.FromKilometers(5)));

            CustomerAppGoogleMap.UiSettings.ZoomControlsEnabled = true;

            if (locationParkingLotFilter == null)
            {
                objLocationParkingLotFilter = new LocationParkingLotFilter();
                objLocationParkingLotFilter.CustomerID = CustomerID;
                objLocationParkingLotFilter.LocationParkingLotID = 0;
                objLocationParkingLotFilter.TwoWheelerTypeCode = VehicleTypeCodes.TwoWheeler;
                objLocationParkingLotFilter.FourWheelerTypeCode = "";
                objLocationParkingLotFilter.Distance = "5";
                objLocationParkingLotFilter.Latitude = Convert.ToDecimal(Latitude);
                objLocationParkingLotFilter.Longitude = Convert.ToDecimal(Longitude);
                objLocationParkingLotFilter.CLatitude = 0;
                objLocationParkingLotFilter.CLongitude = 0;
                objLocationParkingLotFilter.CoveredParking = "";
                objLocationParkingLotFilter.Handicapped = "";
                objLocationParkingLotFilter.EvCharging = "";
                objLocationParkingLotFilter.Mechanic = "";
                objLocationParkingLotFilter.CarWash = "";
                objLocationParkingLotFilter.BikeWash = "";
            }
            else
            {

                objLocationParkingLotFilter = locationParkingLotFilter;
                objLocationParkingLotFilter.CLatitude = 0;
                objLocationParkingLotFilter.CLongitude = 0;

            }

            if (objLocationParkingLotFilter.FourWheelerTypeCode == "" && objLocationParkingLotFilter.TwoWheelerTypeCode == VehicleTypeCodes.TwoWheeler)
            {
                VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;
            }
            else if (objLocationParkingLotFilter.TwoWheelerTypeCode == "" && objLocationParkingLotFilter.FourWheelerTypeCode == VehicleTypeCodes.FourWheeler)
            {
                VehicleTypeID = VehicleTypeCodes.FourWheelerTypeID;
            }

            //if (location.LocationName == null || location.LocationName == "")
            //{
            GetTheFlow();
            //}
            /*
            Device.StartTimer(TimeSpan.FromSeconds(60), (Func<bool>)(() =>
            {
                currentTime = DateTime.Now;
                lblTiming.Text = "Today, " + currentTime.ToString("hh:mm tt", CultureInfo.CreateSpecificCulture("en-US")) +
                " - " + currentTime.AddHours(2).ToString("hh:mm tt", CultureInfo.CreateSpecificCulture("en-US")) + "(2 hrs)";
                return true; // True = Repeat again, False = Stop the timer
            }));
            */

            //if (location.LocationName != null && location.LocationName != "")
            //{
            //    //txtSearchBar.Text = location.LocationName;
            //    txtSearchBar.Text = Convert.ToString(App.Current.Properties["SearchLocation"]);
            //    locationsearchfill.LocationName = location.LocationName;
            //    locationsearchfill.Latitude = location.Latitude;
            //    locationsearchfill.Longitude = location.Longitude;
            //    //lstParkingLots.IsVisible = false;
            //    LoadSelectedLocation(location.Latitude, location.Longitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
            //   // LoadLocationByVehicleType(location.Latitude, location.Longitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
            //    //CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
            //    //    new Position(Convert.ToDouble(location.Latitude),Convert.ToDouble(location.Longitude)), Distance.FromKilometers(1)));
            //    //txtSearchBar.Text = location.LocationName;
            //}
            ShowLoading(false);
        }
        private async Task GetLocationPermission()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();

                if (location != null)
                {
                    Latitude = (float)location.Latitude;
                    Longitude = (float)location.Longitude;

                    if (Latitude != 0 && Longitude != 0)
                        CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(Latitude, Longitude), Distance.FromKilometers(5)));
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed - GetLocationPermission", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void GetTheFlow()
        {
            IsOnline = VerifyInternet();

            await GetLocationPermission();

            if (IsOnline)
            {
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
            }
            else
            {
                DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }

            if (App.Current.Properties.ContainsKey("CustomerParkingSlotID"))
            {
                if (Convert.ToString(App.Current.Properties["CustomerParkingSlotID"]) == "0")
                {
                    //LoadCurrentLocation();

                    if (objLocationParkingLotFilter.SelectedLatitude != 0 && objLocationParkingLotFilter.SelectedLongitude != 0)
                    {
                        //objLocationParkingLotFilter = locationParkingLotFilter;
                        // objLocationParkingLotFilter.CLatitude = locationParkingLotFilter.SelectedLatitude;
                        // objLocationParkingLotFilter.CLongitude = locationParkingLotFilter.SelectedLongitude;
                        LoadSelectedLocation(objLocationParkingLotFilter.SelectedLatitude, objLocationParkingLotFilter.SelectedLongitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
                    }
                    else
                    {
                        LoadCurrentLocation();
                    }


                }
                else
                {
                    GetCustomerParkingSlotByID(Convert.ToInt32(App.Current.Properties["CustomerParkingSlotID"]));
                    return;
                }
            }
            else
            {
                if (objLocationParkingLotFilter.SelectedLatitude != 0 && objLocationParkingLotFilter.SelectedLongitude != 0)
                {
                    //objLocationParkingLotFilter = locationParkingLotFilter;
                    // objLocationParkingLotFilter.CLatitude = locationParkingLotFilter.SelectedLatitude;
                    // objLocationParkingLotFilter.CLongitude = locationParkingLotFilter.SelectedLongitude;
                    LoadSelectedLocation(objLocationParkingLotFilter.SelectedLatitude, objLocationParkingLotFilter.SelectedLongitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
                }
                else
                {
                    LoadCurrentLocation();
                }
            }

            #region Image

            if (App.Current.Properties.ContainsKey("ProfileImage"))
            {
                byte[] imgCameraByteData = null;
                ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])));

                imgCameraByteData = App.Current.Properties["ProfileImage"] == null ? null :
                    new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])).ToArray();

                if (imgCameraByteData != null)
                {
                    Stream stream = new MemoryStream(imgCameraByteData);
                    svgHomeProfileImage.Source = ImageSource.FromStream(() => { return stream; });
                }
                else
                {
                    svgHomeProfileImage.Source = "female.png";
                }
            }
            #endregion
        }
        public bool VerifyInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        public async void LoadCurrentLocation()
        {
            try
            {
                ShowLoading(true);
                IsOnline = VerifyInternet();
                var location = await Geolocation.GetLastKnownLocationAsync();

                objLocationParkingLotFilter.Latitude = Convert.ToDecimal(location.Latitude);
                objLocationParkingLotFilter.Longitude = Convert.ToDecimal(location.Longitude);

                if (IsOnline)
                {
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

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLot> resultObj = new List<LocationParkingLot>();
                        await Task.Run(() =>
                        {
                            resultObj = dal_LocationParkingLot.GetLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);
                        });

                        CustomerAppGoogleMap.Pins.Clear();
                        PreviousIcon = "";
                        Pin obj_pin = new Pin();

                        if (resultObj.Count > 0)
                        {
                            imgCurrentLoc.Source = "resource://ParkHyderabad.Resources.current_location_blue.svg";

                            if (objLocationParkingLotFilter.LocationParkingLotID != 0)
                            {
                                for (int i = 0; i < resultObj.Count; i++)
                                {
                                    obj_pin = new Pin();
                                    obj_pin.ZIndex = Convert.ToInt32(resultObj[i].LocationParkingLotID);
                                    obj_pin.Label = resultObj[i].LocationParkingLotName;
                                    obj_pin.Type = PinType.Place;
                                    obj_pin.Position = new Position(Convert.ToDouble(resultObj[i].Lattitude), Convert.ToDouble(resultObj[i].Longitude));

                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj[i].CapacityColour).ToUpper());
                                    }
                                    else if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREEN")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "ORANGE")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "RED")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREY")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
                                        }
                                    }

                                    CustomerAppGoogleMap.Pins.Add(obj_pin);
                                }

                                CustomerAppGoogleMap.PinClicked += OnPinClicked;

                                CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                                new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));

                                //GetLotParkingDetails(objLocationParkingLotFilter.LocationParkingLotID, false);
                            }
                            else
                            {
                                for (int i = 0; i < resultObj.Count; i++)
                                {
                                    obj_pin = new Pin();
                                    obj_pin.ZIndex = Convert.ToInt32(resultObj[i].LocationParkingLotID);
                                    obj_pin.Label = resultObj[i].LocationParkingLotName;
                                    obj_pin.Type = PinType.Place;
                                    obj_pin.Position = new Position(Convert.ToDouble(resultObj[i].Lattitude), Convert.ToDouble(resultObj[i].Longitude));

                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj[i].CapacityColour).ToUpper());
                                    }
                                    else if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREEN")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "ORANGE")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "RED")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
                                        }
                                        else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREY")
                                        {
                                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
                                        }
                                    }

                                    CustomerAppGoogleMap.Pins.Add(obj_pin);
                                }

                                CustomerAppGoogleMap.PinClicked += OnPinClicked;

                                CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                                new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));

                                if (location != null)
                                {
                                    obj_pin = new Pin();
                                    obj_pin.ZIndex = 0;
                                    obj_pin.Label = "Current Location";
                                    obj_pin.Type = PinType.Place;
                                    obj_pin.Position = new Position(location.Latitude, location.Longitude);

                                    if (Device.RuntimePlatform == Device.Android)
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("YELLOW");
                                    }
                                    else if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("map_current");
                                    }

                                    CustomerAppGoogleMap.Pins.Add(obj_pin);

                                    CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                                    new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));
                                }
                                else
                                {
                                    DependencyService.Get<IAppSettingsHelper>().OpenAppSettings();
                                }
                            }
                        }
                        else
                        {
                            if (location != null)
                            {
                                obj_pin = new Pin();
                                obj_pin.ZIndex = 0;
                                obj_pin.Label = "Current Location";
                                obj_pin.Type = PinType.Place;
                                obj_pin.Position = new Position(location.Latitude, location.Longitude);

                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    obj_pin.Icon = BitmapDescriptorFactory.FromBundle("YELLOW");
                                }
                                else if (Device.RuntimePlatform == Device.iOS)
                                {
                                    obj_pin.Icon = BitmapDescriptorFactory.FromBundle("map_current");
                                }

                                CustomerAppGoogleMap.Pins.Add(obj_pin);

                                CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                                new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));
                            }
                            else
                            {
                                DependencyService.Get<IAppSettingsHelper>().OpenAppSettings();
                            }
                        }

                        /*
                        if (objLocationParkingLotFilter.TwoWheelerTypeCode == "2W" && objLocationParkingLotFilter.FourWheelerTypeCode == "4W")
                        {
                            await DisplayAlert("", "Two And Four Wheeler Lots only", "Ok");
                        }
                        else if (objLocationParkingLotFilter.TwoWheelerTypeCode == "2W" && objLocationParkingLotFilter.FourWheelerTypeCode != "4W")
                        {
                            await DisplayAlert("", "Two Wheeler Lots only", "Ok");
                        }
                        else if (objLocationParkingLotFilter.TwoWheelerTypeCode != "2W" && objLocationParkingLotFilter.FourWheelerTypeCode == "4W")
                        {
                            await DisplayAlert("", "Four Wheeler Lots only", "Ok");
                        }
                        */
                        GetListOfLocations();
                        //GetListViewLotsByFilter

                        json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        content = new StringContent(json, Encoding.UTF8, "application/json");

                        List<LocationParkingLotDetails> result = new List<LocationParkingLotDetails>();

                        await Task.Run(() =>
                        {
                            result = dal_LocationParkingLot.GetListViewLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);
                        });

                        if (result.Count > 0)
                        {
                            for (int j = 0; j < resultObj.Count; j++)
                            {
                                if (App.Current.Properties.ContainsKey("CustomerID"))
                                {
                                    if (Convert.ToString(App.Current.Properties["CustomerID"]) == "0")
                                    {
                                        result[j].CustomerExists = false;
                                        result[j].CustomerNotExists = true;
                                    }
                                    else
                                    {
                                        result[j].CustomerExists = true;
                                        result[j].CustomerNotExists = false;
                                    }
                                }
                                else
                                {
                                    result[j].CustomerExists = false;
                                    result[j].CustomerNotExists = true;
                                }
                            }

                            lstParkingLotFilter.ItemsSource = null;
                            lstParkingLotFilter.ItemsSource = result;
                            obj_LocationParkingLotDetails = result;
                            stkListView.IsVisible = true;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                            ShowLoading(false);
                        }
                        else
                        {
                            lstParkingLotFilter.ItemsSource = null;
                            stkListView.IsVisible = false;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                            ShowLoading(false);
                            DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
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


                #region Image

                if (App.Current.Properties.ContainsKey("ProfileImage"))
                {
                    byte[] imgCameraByteData = null;
                    ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])));

                    imgCameraByteData = App.Current.Properties["ProfileImage"] == null ? null :
                        new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])).ToArray();

                    if (imgCameraByteData != null)
                    {
                        Stream stream = new MemoryStream(imgCameraByteData);
                        svgHomeProfileImage.Source = ImageSource.FromStream(() => { return stream; });
                    }
                    else
                    {
                        svgHomeProfileImage.Source = "female.png";
                    }
                }
                #endregion


                //int s = 1;
                //if (s == 1)
                //{
                //    IsLott
                //}
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                await DisplayAlert("Failed - LoadCurrentLocation", Convert.ToString(ex.Message), "Ok");
            }
        }
        private void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            ShowLoading(true);

            if (!e.Handled)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    for (int i = 0; i < CustomerAppGoogleMap.Pins.Count; i++)
                    {
                        if (CustomerAppGoogleMap.Pins[i].Icon.BundleName.ToUpper() == "GREY")
                        {
                            if (PreviousIcon != "")
                            {
                                CustomerAppGoogleMap.Pins[i].Icon = BitmapDescriptorFactory.FromBundle(PreviousIcon);
                            }
                        }
                    }

                    if (e.Pin.Icon.BundleName.ToUpper() == "RED")
                    {
                        //DisplayAlert("", "Sorry, '" + Convert.ToString(e.Pin.Label) + "' lot is full. Please select another lot to park your vehicle", "Ok");

                        if (obj_LocationParkingLotDetails.Count > 1)
                        {
                            LocationParkingLotDetails locobj = obj_LocationParkingLotDetails.FirstOrDefault(x => x.LocationParkingLotID == Convert.ToInt32(e.Pin.ZIndex));
                            lstParkingLotFilter.ScrollTo(locobj);
                        }
                    }
                    else if (e.Pin.Icon.BundleName.ToUpper() == "GREEN" || e.Pin.Icon.BundleName.ToUpper() == "ORANGE")
                    {
                        PreviousIcon = e.Pin.Icon.BundleName.ToUpper();
                        e.Pin.Icon = BitmapDescriptorFactory.FromBundle("GREY");

                        if (obj_LocationParkingLotDetails.Count > 1)
                        {
                            LocationParkingLotDetails locobj = obj_LocationParkingLotDetails.FirstOrDefault(x => x.LocationParkingLotID == Convert.ToInt32(e.Pin.ZIndex));
                            lstParkingLotFilter.ScrollTo(locobj);
                        }
                        //GetLotParkingDetails(Convert.ToInt32(e.Pin.ZIndex), false);
                    }
                }
                else if (Device.RuntimePlatform == Device.iOS)
                {
                    for (int i = 0; i < CustomerAppGoogleMap.Pins.Count; i++)
                    {
                        if (CustomerAppGoogleMap.Pins[i].Icon.BundleName == "selected_lot")
                        {
                            if (PreviousIcon != "")
                            {
                                CustomerAppGoogleMap.Pins[i].Icon = BitmapDescriptorFactory.FromBundle(PreviousIcon);
                            }
                        }
                    }

                    if (e.Pin.Icon.BundleName == "filled_lot")
                    {
                        //DisplayAlert("", "Sorry, '" + Convert.ToString(e.Pin.Label) + "' lot is full. Please select another lot to park your vehicle", "Ok");

                        if (obj_LocationParkingLotDetails.Count > 1)
                        {
                            LocationParkingLotDetails locobj = obj_LocationParkingLotDetails.FirstOrDefault(x => x.LocationParkingLotID == Convert.ToInt32(e.Pin.ZIndex));
                            lstParkingLotFilter.ScrollTo(locobj);
                        }
                    }
                    else if (e.Pin.Icon.BundleName == "active_lot" || e.Pin.Icon.BundleName == "going_to_filled_out")
                    {
                        PreviousIcon = e.Pin.Icon.BundleName;
                        e.Pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");

                        if (obj_LocationParkingLotDetails.Count > 1)
                        {
                            LocationParkingLotDetails locobj = obj_LocationParkingLotDetails.FirstOrDefault(x => x.LocationParkingLotID == Convert.ToInt32(e.Pin.ZIndex));
                            lstParkingLotFilter.ScrollTo(locobj);
                        }

                        //GetLotParkingDetails(Convert.ToInt32(e.Pin.ZIndex), false);
                    }
                }

                e.Handled = true;
            }

            ShowLoading(false);
        }
        //private void GetLotParkingDetails(int LocationParkingLotID, bool FromListView)
        //{
        //    try
        //    {
        //        LocationParkingLot objLocationParkingLot = new LocationParkingLot();
        //        objLocationParkingLot.LocationParkingLotID = Convert.ToInt32(LocationParkingLotID);

        //        if (App.Current.Properties.ContainsKey("apitoken"))
        //        {
        //            var json = JsonConvert.SerializeObject(objLocationParkingLot);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");
        //            resultObj = dal_LocationParkingLot.GetLotParkingDetails(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLot);

        //            if (resultObj.LocationParkingLotID != 0)
        //            {
        //                StackLayout stk_Two = new StackLayout
        //                {
        //                    Orientation = StackOrientation.Horizontal,
        //                    Spacing = 5,
        //                    Children =
        //                    {
        //                        new Image
        //                        {
        //                            Source = "bike_black.png",
        //                            HeightRequest = 20,
        //                            BackgroundColor = Color.Transparent,
        //                            VerticalOptions = LayoutOptions.Center
        //                        },
        //                        new StackLayout
        //                        {
        //                            Spacing = 0,
        //                            HorizontalOptions = LayoutOptions.FillAndExpand,
        //                            Children =
        //                            {
        //                                 new Label
        //                                 {
        //                                    Text =  Convert.ToString(resultObj.TwoWheelerCount),
        //                                    TextColor = Color.FromHex("#F7941D"),
        //                                    FontSize = 12,
        //                                    FontFamily = "ProximaNovaBold.otf#ProximaNova Bold"
        //                                 },
        //                                 new Label
        //                                 {
        //                                    Text =  "Spots Left",
        //                                    TextColor = Color.FromHex("#000000"),
        //                                    FontSize = 11,
        //                                    FontFamily = "ProximaNovaRegular.otf#ProximaNovaRegular"
        //                                 }
        //                            }
        //                        }
        //                    }
        //                };

        //                StackLayout stk_Four = new StackLayout
        //                {
        //                    Orientation = StackOrientation.Horizontal,
        //                    Spacing = 5,
        //                    Children =
        //                    {
        //                        new Image
        //                        {
        //                            Source = "car_black.png",
        //                            HeightRequest = 20,
        //                            BackgroundColor = Color.Transparent,
        //                            VerticalOptions = LayoutOptions.Center
        //                        },
        //                        new StackLayout
        //                        {
        //                            Spacing = 0,
        //                            HorizontalOptions = LayoutOptions.FillAndExpand,
        //                            Children =
        //                            {
        //                                 new Label
        //                                 {
        //                                    Text =  Convert.ToString(resultObj.FourWheelerCount),
        //                                    TextColor = Color.FromHex("#F7941D"),
        //                                    FontSize = 12,
        //                                    FontFamily = "ProximaNovaBold.otf#ProximaNovaBold"
        //                                 },
        //                                 new Label
        //                                 {
        //                                    Text =  "Spots Left",
        //                                    TextColor = Color.FromHex("#000000"),
        //                                    FontSize = 11,
        //                                    FontFamily = "ProximaNovaRegular.otf#ProximaNovaRegular"
        //                                 }
        //                            }
        //                        }
        //                    }
        //                };

        //                LocationID = resultObj.LocationID;
        //                App.Current.Properties["LocationID"] = resultObj.LocationID;
        //                App.Current.Properties["LocationParkingLotID"] = LocationParkingLotID;
        //                App.Current.Properties["LocationParkingLotName"] = resultObj.LocationParkingLotName;
        //                App.Current.Properties["LotCloseTime"] = resultObj.LotCloseTime;

        //                LocationParkingLotService obj_locationParkingLotService;

        //                if (resultObj.LocationParkingLotService.Count > 0)
        //                {
        //                    lst_LocationParkingLotService = new List<LocationParkingLotService>();

        //                    for (int i = 0; i < resultObj.LocationParkingLotService.Count; i++)
        //                    {
        //                        obj_locationParkingLotService = new LocationParkingLotService();
        //                        obj_locationParkingLotService.ServiceTypeName = resultObj.LocationParkingLotService[i].ServiceTypeName;
        //                        lst_LocationParkingLotService.Add(obj_locationParkingLotService);
        //                    }

        //                    //if (lst_LocationParkingLotService.Count > 0)
        //                    //    cvLotService.ItemsSource = lst_LocationParkingLotService;
        //                    //else
        //                    //    cvLotService.ItemsSource = null;
        //                }

        //                if (FromListView)
        //                {
        //                    Pin obj_pin = new Pin();
        //                    obj_pin.ZIndex = resultObj.LocationParkingLotID;
        //                    obj_pin.Label = resultObj.LocationParkingLotName;
        //                    obj_pin.Type = PinType.Place;
        //                    obj_pin.Position = new Position(Convert.ToDouble(resultObj.Lattitude), Convert.ToDouble(resultObj.Longitude));

        //                    if (Device.RuntimePlatform == Device.Android)
        //                    {
        //                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj.CapacityColour).ToUpper());
        //                    }
        //                    else if (Device.RuntimePlatform == Device.iOS)
        //                    {
        //                        if (Convert.ToString(resultObj.CapacityColour).ToUpper() == "GREEN")
        //                        {
        //                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
        //                        }
        //                        else if (Convert.ToString(resultObj.CapacityColour).ToUpper() == "ORANGE")
        //                        {
        //                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
        //                        }
        //                        else if (Convert.ToString(resultObj.CapacityColour).ToUpper() == "RED")
        //                        {
        //                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
        //                        }
        //                        else if (Convert.ToString(resultObj.CapacityColour).ToUpper() == "GREY")
        //                        {
        //                            obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
        //                        }
        //                    }

        //                    CustomerAppGoogleMap.Pins.Add(obj_pin);

        //                    CustomerAppGoogleMap.PinClicked += OnPinClicked;

        //                    CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
        //                    new Position(Convert.ToDouble(resultObj.Lattitude), Convert.ToDouble(resultObj.Longitude)), Distance.FromKilometers(1)));
        //                    objLocationParkingLotFilter.Distance = "1";
        //                    objLocationParkingLotFilter.Latitude = resultObj.Lattitude;
        //                    objLocationParkingLotFilter.Longitude = resultObj.Longitude;

        //                    if (resultObj.CapacityColour.ToUpper() == "RED")
        //                    {
        //                        DisplayAlert("", "Sorry, '" + Convert.ToString(resultObj.LocationParkingLotName) + "' lot is full. Please select another lot to park your vehicle", "Ok");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                LocationID = 0;
        //                lst_LocationParkingLotService.Clear();
        //                DisplayAlert("", "Lot details not available  ", "Ok");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("Failed - GetLotParkingDetails", Convert.ToString(ex.Message), "Ok");
        //    }
        //}
        private async void btn_BuyAPassClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                ShowLoading(true);

                PrimaryVehicleType = 0;
                CustomerVehicleID = 0;

                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    PrimaryVehicleType = VehicleTypeID;
                    LocationParkingLotDetails locationParkingLotDetails = ((Button)sender).BindingContext as LocationParkingLotDetails;

                    int TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                    int FourWheelerCount = locationParkingLotDetails.FourWheelerCount;

                    obj_OCustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                    obj_OCustomerVehicle.VehicleTypeID = PrimaryVehicleType;
                    obj_OCustomerVehicle.LocationID = locationParkingLotDetails.LocationID;


                    BuyAPass pageBuyAPass = null;
                    await Task.Run(() =>
                    {
                        pageBuyAPass = new BuyAPass(obj_OCustomerVehicle);

                    });
                    await Navigation.PushAsync(pageBuyAPass);

                    ShowLoading(false);
                    /*
                    CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                    obj_CustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                    obj_listCustomerVehicle = dal_Vehicle.GetListOfCustomerVehicle(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                    if (obj_listCustomerVehicle.Count > 0)
                    {
                        PrimaryVehicleType = VehicleTypeID;
                        LocationParkingLotDetails locationParkingLotDetails = ((Button)sender).BindingContext as LocationParkingLotDetails;

                        int TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                        int FourWheelerCount = locationParkingLotDetails.FourWheelerCount;

                        obj_OCustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        obj_OCustomerVehicle.VehicleTypeID = PrimaryVehicleType;
                        obj_OCustomerVehicle.LocationID = locationParkingLotDetails.LocationID;

                        ShowLoading(false);
                        await Navigation.PushAsync(new BuyAPass(obj_OCustomerVehicle));
                    }
                    else
                    {
                        await Navigation.PushAsync(new AddVehicle(Convert.ToInt32(App.Current.Properties["CustomerID"]), null, VehicleTypeCodes.TwoWheeler));
                    }
                    */
                }
                else
                {
                    DisplayAlert("", "Customer does not exists", "Ok");
                    ShowLoading(false);
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                ShowLoading(false);
                return;
            }
        }
        private async void btn_BookASpotClicked(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                ShowLoading(true);

                PrimaryVehicleType = 0;
                CustomerVehicleID = 0;

                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    PrimaryVehicleType = VehicleTypeID;

                    LocationParkingLotDetails locationParkingLotDetails = ((Button)sender).BindingContext as LocationParkingLotDetails;

                    int TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                    int FourWheelerCount = locationParkingLotDetails.FourWheelerCount;

                    if (locationParkingLotDetails.TwoWheelerVisibility == true)
                    {
                        if ((PrimaryVehicleType == VehicleTypeCodes.TwoWheelerTypeID && TwoWheelerCount <= 0))
                        {
                            ShowLoading(false);
                            await DisplayAlert("", "Sorry, '" + locationParkingLotDetails.LocationParkingLotName + "' lot is full. Please select another lot to park your vehicle", "Ok");
                            return;
                        }
                    }

                    if (locationParkingLotDetails.FourWheelerVisibility == true)
                    {
                        if ((PrimaryVehicleType == VehicleTypeCodes.FourWheelerTypeID && FourWheelerCount <= 0))
                        {
                            ShowLoading(false);
                            await DisplayAlert("", "Sorry, '" + locationParkingLotDetails.LocationParkingLotName + "' lot is full. Please select another lot to park your vehicle", "Ok");
                            return;
                        }
                    }

                    DateTime currentTime = DateTime.Now;
                    DateTime ExpectedEndTime_date = currentTime.AddHours(2);
                    CustomerParkingSlotDetails customerParkingSlotDetails = new CustomerParkingSlotDetails();

                    customerParkingSlotDetails.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                    customerParkingSlotDetails.VehicleTypeID = PrimaryVehicleType;
                    customerParkingSlotDetails.LocationID = locationParkingLotDetails.LocationID;
                    customerParkingSlotDetails.CustomerVehicleID = CustomerVehicleID;
                    customerParkingSlotDetails.LocationParkingLotID = locationParkingLotDetails.LocationParkingLotID;
                    customerParkingSlotDetails.LocationParkingLotName = locationParkingLotDetails.LocationParkingLotName;
                    customerParkingSlotDetails.CustomerParkingSlotID = 0;

                    customerParkingSlotDetails.DisabledParking = locationParkingLotDetails.DisabledParking;
                    customerParkingSlotDetails.CoveredParking = locationParkingLotDetails.CoveredParking;
                    customerParkingSlotDetails.Mechanic = locationParkingLotDetails.Mechanic;
                    customerParkingSlotDetails.EvCharging = locationParkingLotDetails.EvCharging;
                    customerParkingSlotDetails.BikeWash = locationParkingLotDetails.BikeWash;
                    customerParkingSlotDetails.CarWash = locationParkingLotDetails.CarWash;

                    customerParkingSlotDetails.Address = locationParkingLotDetails.Address;

                    customerParkingSlotDetails.TwoWheelerVisibility = locationParkingLotDetails.TwoWheelerVisibility;
                    customerParkingSlotDetails.FourWheelerVisibility = locationParkingLotDetails.FourWheelerVisibility;
                    customerParkingSlotDetails.TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                    customerParkingSlotDetails.FourWheelerCount = locationParkingLotDetails.FourWheelerCount;
                    customerParkingSlotDetails.Distance = locationParkingLotDetails.Distance;

                    customerParkingSlotDetails.LotImageName = locationParkingLotDetails.LotImageName;
                    customerParkingSlotDetails.LotImageName2 = locationParkingLotDetails.LotImageName2;
                    customerParkingSlotDetails.LotImageName3 = locationParkingLotDetails.LotImageName3;

                    customerParkingSlotDetails.LotOpeningTime = locationParkingLotDetails.LotOpenTime;
                    customerParkingSlotDetails.LotClosingTime = locationParkingLotDetails.LotCloseTime;

                    customerParkingSlotDetails.ParkingBayID = 0;
                    customerParkingSlotDetails.ParkingBayRange = "";

                    ShowLoading(false);
                    ShowLoading(true);
                    StartSession pageStartSession = null;
                    await Task.Run(() =>
                    {
                        pageStartSession = new StartSession(customerParkingSlotDetails);
                    });
                    await Navigation.PushAsync(pageStartSession);
                    ShowLoading(false);

                    /*
                    CustomerVehicle obj_CustomerVehicle = new CustomerVehicle();
                    obj_CustomerVehicle.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                    obj_listCustomerVehicle = dal_Vehicle.GetListOfCustomerVehicle(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerVehicle);

                    if (obj_listCustomerVehicle.Count > 0)
                    {
                        PrimaryVehicleType = VehicleTypeID;

                        LocationParkingLotDetails locationParkingLotDetails = ((Button)sender).BindingContext as LocationParkingLotDetails;

                        int TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                        int FourWheelerCount = locationParkingLotDetails.FourWheelerCount;

                        if (locationParkingLotDetails.TwoWheelerVisibility == true)
                        {
                            if ((PrimaryVehicleType == VehicleTypeCodes.TwoWheelerTypeID && TwoWheelerCount <= 0))
                            {
                                ShowLoading(false);
                                await DisplayAlert("", "Sorry, '" + locationParkingLotDetails.LocationParkingLotName + "' lot is full. Please select another lot to park your vehicle", "Ok");
                                return;
                            }
                        }

                        if (locationParkingLotDetails.FourWheelerVisibility == true)
                        {
                            if ((PrimaryVehicleType == VehicleTypeCodes.FourWheelerTypeID && FourWheelerCount <= 0))
                            {
                                ShowLoading(false);
                                await DisplayAlert("", "Sorry, '" + locationParkingLotDetails.LocationParkingLotName + "' lot is full. Please select another lot to park your vehicle", "Ok");
                                return;
                            }
                        }

                        DateTime currentTime = DateTime.Now;
                        DateTime ExpectedEndTime_date = currentTime.AddHours(2);
                        CustomerParkingSlotDetails customerParkingSlotDetails = new CustomerParkingSlotDetails();

                        customerParkingSlotDetails.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                        customerParkingSlotDetails.VehicleTypeID = PrimaryVehicleType;
                        customerParkingSlotDetails.LocationID = locationParkingLotDetails.LocationID;
                        customerParkingSlotDetails.CustomerVehicleID = CustomerVehicleID;
                        customerParkingSlotDetails.LocationParkingLotID = locationParkingLotDetails.LocationParkingLotID;
                        customerParkingSlotDetails.LocationParkingLotName = locationParkingLotDetails.LocationParkingLotName;
                        customerParkingSlotDetails.CustomerParkingSlotID = 0;

                        customerParkingSlotDetails.DisabledParking = locationParkingLotDetails.DisabledParking;
                        customerParkingSlotDetails.CoveredParking = locationParkingLotDetails.CoveredParking;
                        customerParkingSlotDetails.Mechanic = locationParkingLotDetails.Mechanic;
                        customerParkingSlotDetails.EvCharging = locationParkingLotDetails.EvCharging;
                        customerParkingSlotDetails.BikeWash = locationParkingLotDetails.BikeWash;
                        customerParkingSlotDetails.CarWash = locationParkingLotDetails.CarWash;

                        customerParkingSlotDetails.Address = locationParkingLotDetails.Address;

                        customerParkingSlotDetails.TwoWheelerVisibility = locationParkingLotDetails.TwoWheelerVisibility;
                        customerParkingSlotDetails.FourWheelerVisibility = locationParkingLotDetails.FourWheelerVisibility;
                        customerParkingSlotDetails.TwoWheelerCount = locationParkingLotDetails.TwoWheelerCount;
                        customerParkingSlotDetails.FourWheelerCount = locationParkingLotDetails.FourWheelerCount;
                        customerParkingSlotDetails.Distance = locationParkingLotDetails.Distance;

                        customerParkingSlotDetails.LotImageName = locationParkingLotDetails.LotImageName;
                        customerParkingSlotDetails.LotImageName2 = locationParkingLotDetails.LotImageName2;
                        customerParkingSlotDetails.LotImageName3 = locationParkingLotDetails.LotImageName3;

                        customerParkingSlotDetails.LotOpeningTime = locationParkingLotDetails.LotOpenTime;
                        customerParkingSlotDetails.LotClosingTime = locationParkingLotDetails.LotCloseTime;

                        customerParkingSlotDetails.ParkingBayID = 0;
                        customerParkingSlotDetails.ParkingBayRange = "";

                        ShowLoading(false);
                        await Navigation.PushAsync(new StartSession(customerParkingSlotDetails));
                    }
                    else
                    {
                        await Navigation.PushAsync(new AddVehicle(Convert.ToInt32(App.Current.Properties["CustomerID"]), null, VehicleTypeCodes.TwoWheeler));
                    }
                    */
                }
                else
                {
                    DisplayAlert("", "Customer does not exists", "Ok");
                    ShowLoading(false);
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                ShowLoading(false);
                return;
            }
        }
        private async void btn_SignUpStartParkingClicked(object sender, EventArgs e)
        {
            App.Current.Properties["CustomerID"] = "0";
            App.Current.Properties.Remove("CustomerID");
            await Navigation.PushAsync(new VerifyMobileNumber());
        }
        private async void btn_AddVehicleClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddVehicle(CustomerID, null, VehicleTypeCodes.TwoWheeler));
        }
        private async void imgbtn_CurrentLocationClicked(object sender, EventArgs e)
        {
            try
            {
                ScaleTo2x(imgCurrentLoc);

                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    ShowLoading(true);


                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location != null)
                    {
                        Latitude = (float)location.Latitude;
                        Longitude = (float)location.Longitude;
                    }
                    else
                    {
                        ShowLoading(false);
                        await DisplayAlert("", "Please turn on device location", "Ok");
                        return;
                    }


                    List<CustomPin> list_customPin = new List<CustomPin>();

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

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {

                        objLocationParkingLotFilter.CLatitude = 0;
                        objLocationParkingLotFilter.CLongitude = 0;

                        objLocationParkingLotFilter.LocationParkingLotID = 0;
                        //objLocationParkingLotFilter.TwoWheelerTypeCode = VehicleTypeCodes.TwoWheeler;
                        //objLocationParkingLotFilter.FourWheelerTypeCode = "";
                        objLocationParkingLotFilter.Distance = "5";
                        objLocationParkingLotFilter.Latitude = Convert.ToDecimal(Latitude);
                        objLocationParkingLotFilter.Longitude = Convert.ToDecimal(Longitude);
                        objLocationParkingLotFilter.CoveredParking = "";
                        objLocationParkingLotFilter.Handicapped = "";
                        objLocationParkingLotFilter.EvCharging = "";
                        objLocationParkingLotFilter.Mechanic = "";
                        objLocationParkingLotFilter.CarWash = "";
                        objLocationParkingLotFilter.BikeWash = "";

                        CustomerAppGoogleMap.Pins.Clear();
                        txtSearchBar.Text = "";
                        txtSearchBar.Placeholder = "Search parking spots at";
                        txtSearchBar.PlaceholderColor = Color.FromHex("#a3a3a3");
                        var json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLot> resultObj = new List<LocationParkingLot>();

                        await Task.Run(() =>
                        {
                            resultObj = dal_LocationParkingLot.GetLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);
                        });

                        CustomerAppGoogleMap.Pins.Clear();
                        PreviousIcon = "";

                        imgCurrentLoc.Source = "current_location_blue.svg";

                        if (resultObj.Count > 0)
                        {
                            Pin obj_pin = new Pin();

                            for (int i = 0; i < resultObj.Count; i++)
                            {
                                obj_pin = new Pin();
                                obj_pin.ZIndex = Convert.ToInt32(resultObj[i].LocationParkingLotID);
                                obj_pin.Label = resultObj[i].LocationParkingLotName;
                                obj_pin.Type = PinType.Place;
                                obj_pin.Position = new Position(Convert.ToDouble(resultObj[i].Lattitude), Convert.ToDouble(resultObj[i].Longitude));

                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj[i].CapacityColour).ToUpper());
                                }
                                else if (Device.RuntimePlatform == Device.iOS)
                                {
                                    if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREEN")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "ORANGE")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "RED")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREY")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
                                    }
                                }

                                CustomerAppGoogleMap.Pins.Add(obj_pin);
                            }

                            CustomerAppGoogleMap.PinClicked += OnPinClicked;

                            CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));

                            //GetListViewLotsByFilter

                            json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                            content = new StringContent(json, Encoding.UTF8, "application/json");
                            List<LocationParkingLotDetails> result = new List<LocationParkingLotDetails>();

                            await Task.Run(() =>
                            {

                                result = dal_LocationParkingLot.GetListViewLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);
                            });

                            if (result.Count > 0)
                            {
                                for (int j = 0; j < resultObj.Count; j++)
                                {
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        if (Convert.ToString(App.Current.Properties["CustomerID"]) == "0")
                                        {
                                            result[j].CustomerExists = false;
                                            result[j].CustomerNotExists = true;
                                        }
                                        else
                                        {
                                            result[j].CustomerExists = true;
                                            result[j].CustomerNotExists = false;
                                        }
                                    }
                                    else
                                    {
                                        result[j].CustomerExists = false;
                                        result[j].CustomerNotExists = true;
                                    }
                                }

                                lstParkingLotFilter.ItemsSource = null;
                                lstParkingLotFilter.ItemsSource = result;
                                obj_LocationParkingLotDetails = result;
                                stkListView.IsVisible = true;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                                ScaleTo3x(imgCurrentLoc);
                            }
                            else
                            {
                                lstParkingLotFilter.ItemsSource = null;
                                stkListView.IsVisible = false;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                                ScaleTo3x(imgCurrentLoc);
                                DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
                            }
                        }
                        else
                        {
                            lstParkingLotFilter.ItemsSource = null;
                            stkListView.IsVisible = false;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                            ScaleTo3x(imgCurrentLoc);
                            DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
                        }


                        if (location != null)
                        {
                            Pin obj_pin = new Pin();
                            obj_pin.ZIndex = 0;
                            obj_pin.Label = "Current Location";
                            obj_pin.Type = PinType.Place;
                            obj_pin.Position = new Position(location.Latitude, location.Longitude);

                            if (Device.RuntimePlatform == Device.Android)
                            {
                                obj_pin.Icon = BitmapDescriptorFactory.FromBundle("YELLOW");
                            }
                            else if (Device.RuntimePlatform == Device.iOS)
                            {
                                obj_pin.Icon = BitmapDescriptorFactory.FromBundle("map_current");
                            }

                            CustomerAppGoogleMap.Pins.Add(obj_pin);

                            CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(Convert.ToDouble(objLocationParkingLotFilter.Latitude), Convert.ToDouble(objLocationParkingLotFilter.Longitude)), Distance.FromKilometers(Convert.ToInt32(objLocationParkingLotFilter.Distance))));
                        }
                        else
                        {
                            DependencyService.Get<IAppSettingsHelper>().OpenAppSettings();
                        }
                    }
                    else
                    {
                        ShowLoading(false);
                        ScaleTo3x(imgCurrentLoc);
                        await DisplayAlert("", "Token does not exists", "Ok");
                    }
                }
                else
                {
                    ScaleTo3x(imgCurrentLoc);
                    await DisplayAlert("", "Please check your network connectivity", "Ok");
                    return;
                }

                ShowLoading(false);

            }
            catch (Exception ex)
            {
                ShowLoading(false);
                await DisplayAlert("Failed - LoadLotsBasedOnCurrentLocation", Convert.ToString(ex.Message), "Ok");
            }
        }

        public void ScaleTo2x(SvgCachedImage image)
        {
            var animation = new Animation(v => image.Scale = v, 1, 1.25);
            animation.Commit(this, "ScaleIt", length: 500, easing: Easing.Linear,
                finished: (v, c) => image.Scale = 1, repeat: () => true);
        }
        public void ScaleTo3x(SvgCachedImage image)
        {
            var animation = new Animation(v => image.Scale = v, 1, 1.25);
            animation.Commit(this, "ScaleIt", length: 500, easing: Easing.Linear,
                finished: (v, c) => image.Scale = 1, repeat: () => false);
        }

        public async void GetCustomerParkingSlotByID(int customerParkingSlotID)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
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

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        CustomerParkingSlot obj_CustomerParkingSlot = new CustomerParkingSlot();
                        obj_CustomerParkingSlot.CustomerParkingSlotID = Convert.ToInt32(customerParkingSlotID);
                        obj_CustomerParkingSlot.CurrentTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                        CustomerParkingSlotDetails obj_CustomerParkingSlotDetails = new CustomerParkingSlotDetails();
                        obj_CustomerParkingSlotDetails = dal_Customer.GetCustomerParkingSlotByID(Convert.ToString(App.Current.Properties["apitoken"]), obj_CustomerParkingSlot);

                        if (obj_CustomerParkingSlotDetails.CustomerParkingSlotID > 0)
                        {
                            App.Current.Properties["CustomerID"] = Convert.ToString(obj_CustomerParkingSlotDetails.CustomerID);
                            App.Current.Properties["UserName"] = Convert.ToString(obj_CustomerParkingSlotDetails.CustomerName);
                            App.Current.Properties["PhoneNumber"] = Convert.ToString(obj_CustomerParkingSlotDetails.CustomerPhoneNumber);
                            App.Current.Properties["Email"] = Convert.ToString(obj_CustomerParkingSlotDetails.CustomerEmail);
                            App.Current.Properties["ProfileImage"] = Convert.ToString(obj_CustomerParkingSlotDetails.CustomerProfileImage);
                            await Navigation.PushAsync(new SessionHomeReceipt(obj_CustomerParkingSlotDetails));
                        }
                        else
                        {
                            if (App.Current.Properties.ContainsKey("CustomerParkingSlotID"))
                            {
                                App.Current.Properties["CustomerParkingSlotID"] = "0";
                            }
                            LoadCurrentLocation();
                        }
                    }
                    else
                    {
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
                await DisplayAlert("Alert - GetCustomerParkingSlotByID", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void GetListOfParkingLots()
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(null);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLot> resultObj = dal_LocationParkingLot.GetListOfParkingLots(Convert.ToString(App.Current.Properties["apitoken"]));

                        if (resultObj.Count > 0)
                        {
                            // lstParkingLots.ItemsSource = resultObj;
                        }
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
                DisplayAlert("Failed - GetListOfParkingLots", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void GetListOfLocations()
        {
            try
            {
                IsOnline = VerifyInternet();
                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        List<Model.APIOutPutModel.Location> obj_listLocation = new List<Model.APIOutPutModel.Location>();
                        obj_listLocation = dal_LocationParkingLot.GetListOfLocations(Convert.ToString(App.Current.Properties["apitoken"]));

                        if (obj_listLocation.Count > 0)
                        {
                            // lstParkingLots.ItemsSource = obj_listLocation;
                            lst_LocationParkingLot = obj_listLocation;
                        }
                        else
                        {
                            DisplayAlert("", "Locations not found!", "Ok");
                        }
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
                DisplayAlert("Failed - GetListOfLocations", Convert.ToString(ex.Message), "Ok");
            }
        }
        //private void lstParkingLots_OnItemTapped(Object sender, ItemTappedEventArgs e)
        //{
        //    Model.APIOutPutModel.Location location = (Model.APIOutPutModel.Location)e.Item;
        //    txtSearchBar.Text = location.LocationName;
        //    lstParkingLots.IsVisible = false;
        //    ((ListView)sender).SelectedItem = null;

        //    //LoadSelectedLocation(location.Latitude, location.Longitude, "", "");
        //    LoadSelectedLocation(location.Latitude, location.Longitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
        //}
        //private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    lstParkingLots.IsVisible = true;
        //    lstParkingLots.BeginRefresh();

        //    try
        //    {
        //        var dataEmpty = lst_LocationParkingLot.Where(i => i.LocationName.ToLower().StartsWith(e.NewTextValue.ToLower()));
        //        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        //            lstParkingLots.IsVisible = false;
        //        else
        //            lstParkingLots.ItemsSource = lst_LocationParkingLot.Where(i => i.LocationName.ToLower().StartsWith(e.NewTextValue.ToLower()));
        //    }
        //    catch (Exception ex)
        //    {
        //        lstParkingLots.IsVisible = false;

        //    }
        //    lstParkingLots.EndRefresh();
        //}
        public async void LoadSelectedLocation(decimal latitude, decimal longitude, string twoWheelerTypeCode, string fourWheelerTypeCode)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    List<CustomPin> list_customPin = new List<CustomPin>();

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

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();

                        objLocationParkingLotFilter.CLatitude = Convert.ToDecimal(location.Latitude);
                        objLocationParkingLotFilter.CLongitude = Convert.ToDecimal(location.Longitude);

                        //if (latitude != 0 && longitude != 0 && latitude != null && longitude != null)
                        //{
                        // objLocationParkingLotFilter.CLatitude = Convert.ToDecimal(latitude);
                        // objLocationParkingLotFilter.CLongitude = Convert.ToDecimal(longitude);
                        //}
                        objLocationParkingLotFilter.LocationParkingLotID = 0;
                        objLocationParkingLotFilter.TwoWheelerTypeCode = twoWheelerTypeCode;
                        objLocationParkingLotFilter.FourWheelerTypeCode = fourWheelerTypeCode;
                        objLocationParkingLotFilter.Distance = "1";
                        objLocationParkingLotFilter.Latitude = Convert.ToDecimal(latitude);
                        objLocationParkingLotFilter.Longitude = Convert.ToDecimal(longitude);
                        objLocationParkingLotFilter.CoveredParking = "";
                        objLocationParkingLotFilter.Handicapped = "";
                        objLocationParkingLotFilter.EvCharging = "";
                        objLocationParkingLotFilter.Mechanic = "";
                        objLocationParkingLotFilter.CarWash = "";
                        objLocationParkingLotFilter.BikeWash = "";

                        var json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLot> resultObj = dal_LocationParkingLot.GetLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);

                        if (resultObj.Count > 0)
                        {
                            imgCurrentLoc.Source = "resource://ParkHyderabad.Resources.current_location_lightgrey.svg";

                            CustomerAppGoogleMap.Pins.Clear();
                            PreviousIcon = "";
                            Pin obj_pin = new Pin();

                            for (int i = 0; i < resultObj.Count; i++)
                            {
                                obj_pin = new Pin();
                                obj_pin.ZIndex = Convert.ToInt32(resultObj[i].LocationParkingLotID);
                                obj_pin.Label = resultObj[i].LocationParkingLotName;
                                obj_pin.Type = PinType.Place;
                                obj_pin.Position = new Position(Convert.ToDouble(resultObj[i].Lattitude), Convert.ToDouble(resultObj[i].Longitude));

                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj[i].CapacityColour).ToUpper());
                                }
                                else if (Device.RuntimePlatform == Device.iOS)
                                {
                                    if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREEN")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "ORANGE")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "RED")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREY")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
                                    }
                                }

                                CustomerAppGoogleMap.Pins.Add(obj_pin);
                            }

                            CustomerAppGoogleMap.PinClicked += OnPinClicked;

                            CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(Convert.ToDouble(latitude), Convert.ToDouble(longitude)), Distance.FromKilometers(1)));
                            objLocationParkingLotFilter.Distance = "1";

                            //GetListViewLotsByFilter

                            json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                            content = new StringContent(json, Encoding.UTF8, "application/json");
                            List<LocationParkingLotDetails> result = dal_LocationParkingLot.GetListViewLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);

                            if (result.Count > 0)
                            {
                                for (int j = 0; j < resultObj.Count; j++)
                                {
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        if (Convert.ToString(App.Current.Properties["CustomerID"]) == "0")
                                        {
                                            result[j].CustomerExists = false;
                                            result[j].CustomerNotExists = true;
                                        }
                                        else
                                        {
                                            result[j].CustomerExists = true;
                                            result[j].CustomerNotExists = false;
                                        }
                                    }
                                    else
                                    {
                                        result[j].CustomerExists = false;
                                        result[j].CustomerNotExists = true;
                                    }
                                }

                                lstParkingLotFilter.ItemsSource = null;
                                lstParkingLotFilter.ItemsSource = result;
                                obj_LocationParkingLotDetails = result;
                                stkListView.IsVisible = true;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                                txtSearchBar.Placeholder = objLocationParkingLotFilter.LocationName;
                                txtSearchBar.PlaceholderColor = Color.FromHex("#4b4c51");
                                if (objLocationParkingLotFilter.TwoWheelerTypeCode == "2W")
                                {
                                    svgTwo.Source = "resource://ParkHyderabad.Resources.bikeselect.svg";
                                    svgFour.Source = "resource://ParkHyderabad.Resources.carunselect.svg";
                                }
                                else if (objLocationParkingLotFilter.FourWheelerTypeCode == "4W")
                                {
                                    svgTwo.Source = "resource://ParkHyderabad.Resources.bikeunselect.svg";
                                    svgFour.Source = "resource://ParkHyderabad.Resources.carselect.svg";
                                }
                            }
                            else
                            {
                                lstParkingLotFilter.ItemsSource = null;
                                stkListView.IsVisible = false;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                                DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
                            }
                        }
                        else
                        {
                            DisplayAlert("", "Please select another location to park your vehicle! Thank you", "Ok");
                            CustomerAppGoogleMap.Pins.Clear();
                            lstParkingLotFilter.ItemsSource = null;
                            stkListView.IsVisible = false;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Token does not exists", "Ok");
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
                DisplayAlert("Failed - LoadSelectedLocation", Convert.ToString(ex.Message), "Ok");
            }
        }
        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                var FCMToken = Application.Current.Properties.Keys.Contains("RefreshedToken");
                if (FCMToken)
                {
                    var FCMTockenValue = Application.Current.Properties["RefreshedToken"].ToString();
                    FCMBody body = new FCMBody();
                    FCMNotification notification = new FCMNotification();
                    notification.title = "Xamarin Forms FCM Notifications";
                    notification.body = "Sample For FCM Push Notifications in Xamairn Forms";
                    FCMData data = new FCMData();
                    data.key1 = "";
                    data.key2 = "";
                    data.key3 = "";
                    data.key4 = "";
                    body.registration_ids = new[] { FCMTockenValue };
                    body.notification = notification;
                    body.data = data;
                    var isSuccessCall = SendNotification(body).Result;
                    if (isSuccessCall)
                    {
                        DisplayAlert("Alert", "Notifications Send Successfully", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Alert", "Notifications Send Failed", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task<bool> SendNotification(FCMBody fcmBody)
        {
            try
            {
                var httpContent = JsonConvert.SerializeObject(fcmBody);
                var client = new HttpClient();
                var authorization = string.Format("key={0}", "AAAAW-3fgRU:APA91bGPQh__2jjPxUjQ5UmnCXJ8lAAwMrXqQGNtiEfwUv7n90wXQqsh73PChpC-GqrbKYyv6mzVo74t0iwWboQmU0MZpot32pOLB2OdxY6ZrnW55Bme3Qyf-w478-tZ4vx9rzdfzbPq");

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
                var stringContent = new StringContent(httpContent);
                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string uri = "https://fcm.googleapis.com/fcm/send";
                var response = await client.PostAsync(uri, stringContent).ConfigureAwait(false);
                var result = response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (TaskCanceledException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
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
        private async void TwoWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    svgTwo.Source = "resource://ParkHyderabad.Resources.bikeselect.svg";
                    svgFour.Source = "resource://ParkHyderabad.Resources.carunselect.svg";
                    VehicleTypeID = VehicleTypeCodes.TwoWheelerTypeID;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        objLocationParkingLotFilter.TwoWheelerTypeCode = VehicleTypeCodes.TwoWheeler;
                        objLocationParkingLotFilter.FourWheelerTypeCode = "";
                        LoadLocationByVehicleType(objLocationParkingLotFilter.Latitude, objLocationParkingLotFilter.Longitude,
                            objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
                    }
                    else
                    {
                        DisplayAlert("", "Token does not exists", "Ok");
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
                DisplayAlert("Failed - GetListViewLotsByFilter", Convert.ToString(ex.Message), "Ok");
            }
        }
        private async void FourWheeler_Tapped(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    svgTwo.Source = "resource://ParkHyderabad.Resources.bikeunselect.svg";
                    svgFour.Source = "resource://ParkHyderabad.Resources.carselect.svg";
                    VehicleTypeID = VehicleTypeCodes.FourWheelerTypeID;

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        objLocationParkingLotFilter.TwoWheelerTypeCode = "";
                        objLocationParkingLotFilter.FourWheelerTypeCode = VehicleTypeCodes.FourWheeler;
                        LoadLocationByVehicleType(objLocationParkingLotFilter.Latitude, objLocationParkingLotFilter.Longitude,
                            objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
                    }
                    else
                    {
                        DisplayAlert("", "Token does not exists", "Ok");
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
                DisplayAlert("Failed - GetListViewLotsByFilter", Convert.ToString(ex.Message), "Ok");
            }
        }
        public void LoadLocationByVehicleType(decimal latitude, decimal longitude, string twoWheelerTypeCode, string fourWheelerTypeCode)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    List<CustomPin> list_customPin = new List<CustomPin>();

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

                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        objLocationParkingLotFilter.LocationParkingLotID = 0;
                        objLocationParkingLotFilter.TwoWheelerTypeCode = twoWheelerTypeCode;
                        objLocationParkingLotFilter.FourWheelerTypeCode = fourWheelerTypeCode;
                        objLocationParkingLotFilter.Latitude = Convert.ToDecimal(latitude);
                        objLocationParkingLotFilter.Longitude = Convert.ToDecimal(longitude);

                        var json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLot> resultObj = dal_LocationParkingLot.GetLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);

                        if (resultObj.Count > 0)
                        {
                            CustomerAppGoogleMap.Pins.Clear();
                            PreviousIcon = "";
                            Pin obj_pin = new Pin();

                            for (int i = 0; i < resultObj.Count; i++)
                            {
                                obj_pin = new Pin();
                                obj_pin.ZIndex = Convert.ToInt32(resultObj[i].LocationParkingLotID);
                                obj_pin.Label = resultObj[i].LocationParkingLotName;
                                obj_pin.Type = PinType.Place;
                                obj_pin.Position = new Position(Convert.ToDouble(resultObj[i].Lattitude), Convert.ToDouble(resultObj[i].Longitude));

                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    obj_pin.Icon = BitmapDescriptorFactory.FromBundle(Convert.ToString(resultObj[i].CapacityColour).ToUpper());
                                }
                                else if (Device.RuntimePlatform == Device.iOS)
                                {
                                    if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREEN")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("active_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "ORANGE")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("going_to_filled_out");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "RED")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("filled_lot");
                                    }
                                    else if (Convert.ToString(resultObj[i].CapacityColour).ToUpper() == "GREY")
                                    {
                                        obj_pin.Icon = BitmapDescriptorFactory.FromBundle("selected_lot");
                                    }
                                }

                                CustomerAppGoogleMap.Pins.Add(obj_pin);
                            }

                            CustomerAppGoogleMap.PinClicked += OnPinClicked;

                            CustomerAppGoogleMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                            new Position(Convert.ToDouble(latitude), Convert.ToDouble(longitude)), Distance.FromKilometers(Convert.ToDouble(objLocationParkingLotFilter.Distance))));

                            //GetListViewLotsByFilter

                            json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                            content = new StringContent(json, Encoding.UTF8, "application/json");
                            List<LocationParkingLotDetails> result = dal_LocationParkingLot.GetListViewLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);

                            if (result.Count > 0)
                            {
                                for (int j = 0; j < resultObj.Count; j++)
                                {
                                    if (App.Current.Properties.ContainsKey("CustomerID"))
                                    {
                                        if (Convert.ToString(App.Current.Properties["CustomerID"]) == "0")
                                        {
                                            result[j].CustomerExists = false;
                                            result[j].CustomerNotExists = true;
                                        }
                                        else
                                        {
                                            result[j].CustomerExists = true;
                                            result[j].CustomerNotExists = false;
                                        }
                                    }
                                    else
                                    {
                                        result[j].CustomerExists = false;
                                        result[j].CustomerNotExists = true;
                                    }
                                }

                                lstParkingLotFilter.ItemsSource = null;
                                lstParkingLotFilter.ItemsSource = result;
                                obj_LocationParkingLotDetails = result;
                                stkListView.IsVisible = true;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                            }
                            else
                            {
                                lstParkingLotFilter.ItemsSource = null;
                                stkListView.IsVisible = false;
                                imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                                DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
                            }
                        }
                        else
                        {
                            DisplayAlert("", "Please select another location to park your vehicle! Thank you", "Ok");
                            CustomerAppGoogleMap.Pins.Clear();
                            lstParkingLotFilter.ItemsSource = null;
                            stkListView.IsVisible = false;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Token does not exists", "Ok");
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
                DisplayAlert("Failed - LoadSelectedLocation", Convert.ToString(ex.Message), "Ok");
            }
        }
        private void imgbtn_DownArrowTapped(object sender, EventArgs e)
        {
            if (stkListView.IsVisible)
            {
                stkListView.IsVisible = false;
                imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
            }
            else
            {
                stkListView.IsVisible = true;
                imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
            }
        }
        private async void GetListViewLotsByFilter(LocationParkingLotFilter objLocationParkingLotFilter)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("apitoken"))
                    {
                        var json = JsonConvert.SerializeObject(objLocationParkingLotFilter);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        List<LocationParkingLotDetails> result = dal_LocationParkingLot.GetListViewLotsByFilter(Convert.ToString(App.Current.Properties["apitoken"]), objLocationParkingLotFilter);

                        if (result.Count > 0)
                        {
                            lstParkingLotFilter.ItemsSource = null;
                            lstParkingLotFilter.ItemsSource = result;
                            obj_LocationParkingLotDetails = result;
                            stkListView.IsVisible = true;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                        }
                        else
                        {
                            lstParkingLotFilter.ItemsSource = null;
                            stkListView.IsVisible = false;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
                            DisplayAlert("", "No Parking lots near by.Please 'Search' for another location.", "Ok");
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Token does not exists", "Ok");
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
                DisplayAlert("Failed - GetListViewLotsByFilter", Convert.ToString(ex.Message), "Ok");
            }
        }

        // Menu
        private async void OpenPopup(object sender, EventArgs e)
        {
            if (App.Current.Properties.ContainsKey("CustomerID"))
            {
                if (Convert.ToInt32(App.Current.Properties["CustomerID"]) != 0)
                {
                    lblCustomerName.Text = Convert.ToString(App.Current.Properties["UserName"]);

                    byte[] imgCameraByteData = null;
                    ImageSource.FromStream(() => new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])));

                    imgCameraByteData = App.Current.Properties["ProfileImage"] == null ? null :
                        new MemoryStream(ByteArrayCompressionUtility.Decompress((byte[])App.Current.Properties["ProfileImage"])).ToArray();

                    if (imgCameraByteData != null)
                    {
                        Stream stream = new MemoryStream(imgCameraByteData);
                        svgCustomerImage.Source = ImageSource.FromStream(() => { return stream; });
                    }
                    else
                    {
                        svgCustomerImage.Source = "female.png";
                    }
                }
                else
                {
                    lblCustomerName.Text = "XXXXXXXXXX";
                }
            }
            else
            {
                lblCustomerName.Text = "XXXXXXXXXX";
            }
            popupMenuView.IsVisible = true;
            stkListView.IsVisible = false;
            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
        }
        private async void ClosePopup(object sender, EventArgs e)
        {
            popupMenuView.IsVisible = false;
            stkListView.IsVisible = true;
            imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
        }
        private async void lblMyVehicles_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            //Navigation.PushAsync(new MyVehicles());

                            MyVehicles myVehicles = null;
                            StklauoutactivityIndicator.IsVisible = true;
                            await Task.Run(() =>
                            {
                                myVehicles = new MyVehicles();
                            });

                            await Navigation.PushAsync(myVehicles);
                            StklauoutactivityIndicator.IsVisible = false;
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void lblMyPasses_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            Navigation.PushAsync(new MyPasses());


                            //MyPasses myPasses = null;
                            //StklauoutactivityIndicator.IsVisible = true;
                            //await Task.Run(() =>
                            //{
                            //    myPasses = new MyPasses();
                            //});
                            //await Navigation.PushAsync(myPasses);

                            StklauoutactivityIndicator.IsVisible = false;

                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void lblHistory_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            Navigation.PushAsync(new History());

                            /*
                            History history = null;
                            StklauoutactivityIndicator.IsVisible = true;
                            await Task.Run(() =>
                            {
                                history = new History();
                            });

                            await Navigation.PushAsync(history);
                            StklauoutactivityIndicator.IsVisible = false;
                            */
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void lblSupport_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                           Navigation.PushAsync(new InstaSupport());
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void lblTermsAndCondition_Tapped(object sender, EventArgs e)
        {
            //Navigation.PushAsync(new TermsAndConditions());

            TermsAndConditions termsAndConditions = null;
            StklauoutactivityIndicator.IsVisible = true;
            await Task.Run(() =>
            {
                termsAndConditions = new TermsAndConditions();
            });

            await Navigation.PushAsync(termsAndConditions);
            StklauoutactivityIndicator.IsVisible = false;
        }
        private async void lblOfferMySpace_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            Navigation.PushAsync(new OfferMySpace());

                            /*
                            OfferMySpace offerMySpace = null;
                            StklauoutactivityIndicator.IsVisible = true;
                            await Task.Run(() =>
                            {
                                offerMySpace = new OfferMySpace();
                            });

                            await Navigation.PushAsync(offerMySpace);
                            StklauoutactivityIndicator.IsVisible = false;
                            */
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }
        private async void lblNotification_Tapped(object sender, EventArgs e)
        {
            IsOnline = VerifyInternet();
            if (IsOnline)
            {
                if (App.Current.Properties.ContainsKey("CustomerID"))
                {
                    if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                           Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                    {
                        if (App.Current.Properties.ContainsKey("apitoken"))
                        {
                            //Navigation.PushAsync(new Notification(Convert.ToInt32(App.Current.Properties["CustomerID"])));

                            Notification notification = null;
                            StklauoutactivityIndicator.IsVisible = true;
                            ShowLoading(true);
                            await Task.Run(() =>
                            {
                                notification = new Notification(Convert.ToInt32(App.Current.Properties["CustomerID"]));
                            });
                            ShowLoading(false);
                            await Navigation.PushAsync(notification);
                            StklauoutactivityIndicator.IsVisible = false;
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                    }
                }
                else
                {
                    DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                }
            }
            else
            {
                await DisplayAlert("", "Please check your network connectivity", "Ok");
                return;
            }
        }

        // Filters
        private async void OpenFilterPopup(object sender, EventArgs e)
        {
            popupFilterView.IsVisible = true;
            stkListView.IsVisible = false;
            xScaleSlider.Value = 5;
            lblSliderValue.Text = "5 km";
            imgArrow.Source = "resource://ParkHyderabad.Resources.uparrow.svg";
        }
        private async void CloseFilterPopup(object sender, EventArgs e)
        {
            popupFilterView.IsVisible = false;
            stkListView.IsVisible = true;
            imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
        }
        private void sliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            var newStep = Math.Round(args.NewValue / VehicleTypeCodes.StepValue);
            double SliderMain_Value = newStep * VehicleTypeCodes.StepValue;

            lblSliderValue.Text = SliderMain_Value.ToString() + " km";
        }
        /*
        private async void FilterTwoWheeler_Tapped(object sender, EventArgs e)
        {
            svgFilterTwoWhlr.Source = "resource://ParkHyderabad.Resources.blue_circle_bike.svg";
            svgFilterFourWhlr.Source = "resource://ParkHyderabad.Resources.grey_circle_car.svg";
            FilterTwoWheelerTypeCode = "2W";
            FilterFourWheelerTypeCode = "";
        }
        private async void FilterFourWheeler_Tapped(object sender, EventArgs e)
        {
            svgFilterTwoWhlr.Source = "resource://ParkHyderabad.Resources.grey_circle_bike.svg";
            svgFilterFourWhlr.Source = "resource://ParkHyderabad.Resources.blue_circle_car.svg";
            FilterTwoWheelerTypeCode = "";
            FilterFourWheelerTypeCode = "4W";
        }
        */
        private async void chk_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var resultbox = (CheckBox)sender;

            if (resultbox.IsChecked)
            {
                resultbox.Color = Color.FromHex("#009f76");
            }
            else
            {
                resultbox.Color = Color.FromHex("#a3a3a3");
            }
        }
        private async void btn_ApplyClicked(object sender, EventArgs e)
        {
            try
            {
                ShowLoading(true);
                if (VerifyInternet())
                {
                    btnApply.IsVisible = false;
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    if (location != null)
                    {
                        Latitude = (float)location.Latitude;
                        Longitude = (float)location.Longitude;
                    }
                    else
                    {
                        ShowLoading(false);
                        DisplayAlert("", "Please turn on device location", "Ok");
                        return;
                    }

                    //LocationParkingLotFilter objLocationParkingLotFilter = new LocationParkingLotFilter();

                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {
                        objLocationParkingLotFilter.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                    }
                    else
                    {
                        objLocationParkingLotFilter.CustomerID = 0;
                    }

                    objLocationParkingLotFilter.LocationParkingLotID = 0;
                    //objLocationParkingLotFilter.TwoWheelerTypeCode = FilterTwoWheelerTypeCode;
                    //objLocationParkingLotFilter.FourWheelerTypeCode = FilterFourWheelerTypeCode;
                    objLocationParkingLotFilter.Distance = lblSliderValue.Text == "0 km" ? "5" : lblSliderValue.Text.Replace("km", "").Trim();
                    objLocationParkingLotFilter.Latitude = Convert.ToDecimal(Latitude);
                    objLocationParkingLotFilter.Longitude = Convert.ToDecimal(Longitude);
                    objLocationParkingLotFilter.CLatitude = 0;
                    objLocationParkingLotFilter.CLongitude = 0;
                    objLocationParkingLotFilter.CoveredParking = chkCoveredParking.IsChecked ? VehicleTypeCodes.CoveredParking : "";
                    objLocationParkingLotFilter.Handicapped = chkDisabledParking.IsChecked ? VehicleTypeCodes.Handicaped : "";
                    objLocationParkingLotFilter.EvCharging = chkEVCharging.IsChecked ? VehicleTypeCodes.EvCharging : "";
                    objLocationParkingLotFilter.Mechanic = chkMechanic.IsChecked ? VehicleTypeCodes.Mechanic : "";
                    objLocationParkingLotFilter.CarWash = chkCarWash.IsChecked ? VehicleTypeCodes.CarWash : "";
                    objLocationParkingLotFilter.BikeWash = chkBikeWash.IsChecked ? VehicleTypeCodes.BikeWash : "";

                    btnApply.IsVisible = true;

                    LoadCurrentLocation();

                    popupFilterView.IsVisible = false;
                    stkListView.IsVisible = true;
                    imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";

                    //await Navigation.PushAsync(new Home(objLocationParkingLotFilter, objLocationParkingLotFilter.CustomerID));
                }
                else
                {
                    ShowLoading(false);
                    DisplayAlert("", "Please check your network connectivity", "Ok");
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                DisplayAlert("Failed - ApplyClicked", Convert.ToString(ex.Message), "Ok");
            }
        }

        // MyProfile
        private async void svgCustomerImage_Tapped(object sender, EventArgs e)
        {
            try
            {
                IsOnline = VerifyInternet();

                if (IsOnline)
                {
                    if (App.Current.Properties.ContainsKey("CustomerID"))
                    {
                        if (Convert.ToString(App.Current.Properties["CustomerID"]) != "" &&
                            Convert.ToString(App.Current.Properties["CustomerID"]) != "0")
                        {
                            if (App.Current.Properties.ContainsKey("apitoken"))
                            {
                                Customer objCustomer = new Customer();
                                objCustomer.CustomerID = Convert.ToInt32(App.Current.Properties["CustomerID"]);
                                var json = JsonConvert.SerializeObject(objCustomer);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                dal_Customer = new DALCustomer();
                                OCustomer resultObj = new OCustomer();

                                await Task.Run(() =>
                                {
                                    resultObj = dal_Customer.GetCustomerByID(Convert.ToString(App.Current.Properties["apitoken"]), objCustomer);
                                });

                                if (resultObj.CustomerID != 0)
                                {
                                    await Navigation.PushAsync(new MyProfile(resultObj));
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                            popupMenuView.IsVisible = false;
                            stkListView.IsVisible = true;
                            imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
                        }
                    }
                    else
                    {
                        await DisplayAlert("", "Please Sign Up with ParkHyderabad", "Ok");
                        popupMenuView.IsVisible = false;
                        stkListView.IsVisible = true;
                        imgArrow.Source = "resource://ParkHyderabad.Resources.downarrow.svg";
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
                await DisplayAlert("Failed - Profile", Convert.ToString(ex.Message), "Ok");
            }
        }


        private async void SearchBarFocussed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchLocation(objLocationParkingLotFilter));
        }

        private void svgtwGesture_Tapped(object sender, EventArgs e)
        {

        }

        private void svgfwGesture_Tapped(object sender, EventArgs e)
        {

        }

        private async void gestureFavorites_Tapped(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "Coming soon", "OK");
        }
        //private async void SearchBarFocussed(object sender, EventArgs e)
        //{
        //    if ((txtSearchBar.Text == "" || txtSearchBar.Text==null)&&(locationsearchfill.LocationName=="" || locationsearchfill.LocationName==null))
        //    { 
        //        await Navigation.PushAsync(new SearchLocation());
        //    }
        //    else
        //    {
        //        txtSearchBar.Text = locationsearchfill.LocationName;
        //        LoadSelectedLocation(locationsearchfill.Latitude, locationsearchfill.Longitude, objLocationParkingLotFilter.TwoWheelerTypeCode, objLocationParkingLotFilter.FourWheelerTypeCode);
        //    }
        //}
    }
}