using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Common;
//using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;

using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using Firebase;
using Firebase.Analytics;
using ImageCircle.Forms.Plugin.Droid;
using ParkHyderabad.Model;
using Newtonsoft.Json;
using Org.Json;
using Plugin.FirebasePushNotification;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Android;
using Xamd.ImageCarousel.Forms.Plugin.Droid;
using Android.Views;

namespace ParkHyderabad.Droid
{
    [Activity(Label = "ParkHyderabad", NoHistory = false, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        static string PaymentFrom = "";

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private FirebaseAnalytics firebaseAnalytics;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new AccessNativeBitmapConfig()
            };

            global::Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental");

            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig);
            await CrossMedia.Current.Initialize();
            ImageCircleRenderer.Init();
            ImageCarouselRenderer.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            CachedImageRenderer.Init(true);
            var ignore = typeof(SvgCachedImage);

            Delegate.SetLocalNightMode(AppCompatDelegate.ModeNightNo);
            FirebaseOptions options = new FirebaseOptions.Builder()
                .SetApplicationId("1:7420635500:android:b118660f87151c2ef06b49")
                .SetApiKey("AIzaSyCN_LKGvFz-HSNIuZ6FxLkd2f4bTSmeHSY")
                .SetGcmSenderId("7420635500")
                .Build();


            //commented on 16062021 Start
            bool hasBeenInitialized = false;
            IList<FirebaseApp> firebaseApps = FirebaseApp.GetApps(Android.App.Application.Context);
            foreach (FirebaseApp app in firebaseApps)
            {
                if (app.Name.Equals(FirebaseApp.DefaultAppName))
                {
                    hasBeenInitialized = true;
                    FirebaseApp firebaseApp = app;
                }
            }

            if (!hasBeenInitialized)
            {
                FirebaseApp firebaseApp = FirebaseApp.InitializeApp(Android.App.Application.Context, options);
            }
            //commented on 16062021 End

            //SetContentView(Resource.Layout.MasterPage);
            IsPlayServicesAvailable(); //You can use this method to check if play services are available.
            CreateNotificationChannel();

            LoadApplication(new App());
            FirebasePushNotificationManager.ProcessIntent(this, Intent);



            //Razor pay
            //MessagingCenter.Subscribe<RazorPayload, RazorUserDetails>(this, "PayNow", (payload, userDetails) =>
            //{
            //    //For QA
            //    //string username = "rzp_test_qkjiy6t4w5jy5s";
            //    //string password = "6nwsqv7qnmctnyqzasjn30dm";
            //    string username = "rzp_test_QkjiY6T4w5Jy5s";
            //    string password = "6Nwsqv7QnmcTnyQzaSjn30Dm";

            //    //For UAT
            //    //string username = "rzp_live_VicdCzsJwEwVl0";
            //    //string password = "oPzSb1YOeQB3koL3j8zu0b2d";

            //    PayViaRazor(payload, username, password, userDetails);

            //});
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this); if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                    string test = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    //This device is not supported           
                    Finish(); // Kill the activity if you want.         
                }
                return false;
            }
            else
            {
                //Google Play Services is available.         
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, new Java.Lang.String("Primary"), NotificationImportance.High)
            {
                Description = "Firebase Cloud Messages appear in this channel"
            };

            channel.EnableVibration(true);
            //channel.SetVibrationPattern(vibratePattern);
            channel.LockscreenVisibility = NotificationVisibility.Public;
            channel.SetShowBadge(true);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }

        //public void OnPaymentError(int p0, string p1, PaymentData p2)
        //{
        //    //throw new System.NotImplementedException();
        //}

        //public void OnPaymentSuccess(string p0, PaymentData p1)
        //{
        //    if (PaymentFrom == "Parking")
        //    {
        //        PaymentDataOutput paymetData = new PaymentDataOutput();
        //        paymetData.OrderId = p1.OrderId;
        //        paymetData.PaymentId = p1.PaymentId;
        //        paymetData.Signature = p1.Signature;
        //        paymetData.UserContact = p1.UserContact;
        //        paymetData.UserEmail = p1.UserEmail;
        //        MessagingCenter.Send<PaymentDataOutput>(paymetData, "PaySuccess");
        //    }
        //    else
        //    {
        //        PassPaymentData passpaymetData = new PassPaymentData();
        //        passpaymetData.OrderId = p1.OrderId;
        //        passpaymetData.PaymentId = p1.PaymentId;
        //        passpaymetData.Signature = p1.Signature;
        //        passpaymetData.UserContact = p1.UserContact;
        //        passpaymetData.UserEmail = p1.UserEmail;
        //        MessagingCenter.Send<PassPaymentData>(passpaymetData, "PaySuccess");
        //    }
        //}

        //public async void PayViaRazor(RazorPayload payload, string username, string password, RazorUserDetails userDetails)
        //{
        //    PaymentFrom = userDetails.PayFor;
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.razorpay.com/v1/orders"))
        //        {
        //            var plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        //            var basicAuthKey = Convert.ToBase64String(plainTextBytes);

        //            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {basicAuthKey}");

        //            string jsonData = JsonConvert.SerializeObject(payload);

        //            request.Content = new StringContent(jsonData);
        //            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        //            var response = await httpClient.SendAsync(request);
        //            string jsonResp = await response.Content.ReadAsStringAsync();
        //            RazorResp resp = JsonConvert.DeserializeObject<RazorResp>(jsonResp);

        //            if (!string.IsNullOrEmpty(resp.id))
        //            {
        //                ///checkout
        //                Checkout chekout = new Checkout();
        //                chekout.SetImage(0);

        //                chekout.SetKeyID(username);
        //                try
        //                {
        //                    JSONObject options = new JSONObject();
        //                    options.Put("name", "Insta Parking");
        //                    options.Put("description", userDetails.Description);
        //                    options.Put("image", "https://razorpay.com/favicon.png");
        //                    options.Put("order_id", resp.id);
        //                    options.Put("theme.color", "#3399cc");
        //                    options.Put("currency", "INR");
        //                    options.Put("amount", payload.amount);
        //                    options.Put("prefill.email", userDetails.Email);
        //                    options.Put("prefill.contact", userDetails.PhoneNumber);
        //                    chekout.Open(this, options);
        //                }
        //                catch (Exception e)
        //                {
        //                    //Log.e(TAG, "Error in starting Razorpay Checkout", e);
        //                }
        //            }
        //            else
        //            {
        //                Toast.MakeText(this, "payment error", ToastLength.Short).Show();
        //            }
        //        }
        //    }
        //}
    }
}