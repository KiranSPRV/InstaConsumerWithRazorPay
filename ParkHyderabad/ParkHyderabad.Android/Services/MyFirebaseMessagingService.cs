using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using System;
using System.Text.RegularExpressions;

namespace ParkHyderabad.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public const string PRIMARY_CHANNEL = "default";

        /*
         
        // Tried Code

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            var body = message.GetNotification().Body;
            Log.Debug(TAG, "Notification Message Body: " + body);
            SendNotification(message);
        }

        void SendNotification(RemoteMessage message)
        {
            NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
            var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
            int id = new Random(seed).Next(000000000, 999999999);
            var intent = new Intent(this, typeof(MainActivity));
            var fullScreenPendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.CancelCurrent);
            NotificationCompat.Builder notification;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var chan1 = new NotificationChannel(PRIMARY_CHANNEL,
                 new Java.Lang.String("Primary"), NotificationImportance.High);
                chan1.LightColor = Color.Green;
                //manager.CreateNotificationChannel(chan1);
                notification = new NotificationCompat.Builder(this, PRIMARY_CHANNEL);
                notification.SetFullScreenIntent(fullScreenPendingIntent, true);
            }
            else
            {
                notification = new NotificationCompat.Builder(this);
            }

            // set the vibration patterns for the channels
            long[] urgentVibrationPattern = { 100, 30, 100, 30, 100, 200, 200, 30, 200, 30, 200, 200, 100, 30, 100, 30, 100, 100, 30, 100, 30, 100, 200, 200, 30, 200, 30, 200, 200, 100, 30, 100, 30, 100 };

            // Creating an Audio Attribute
            var alarmAttributes = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Speech)
                .SetUsage(AudioUsageKind.Notification).Build();

            // Create the uri for the alarm file

            var recordingFileDestinationPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, AppConstants.NotificationSound);
            Android.Net.Uri urgentAlarmUri = Android.Net.Uri.Parse(recordingFileDestinationPath);

            notification.SetContentIntent(fullScreenPendingIntent)
                     .SetContentTitle(message.GetNotification().Title)
                     .SetContentText(message.GetNotification().Body)
                     .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.logo))
                     .SetSmallIcon(Resource.Drawable.logo)
                     .SetStyle((new NotificationCompat.BigTextStyle()))
                     .SetPriority(NotificationCompat.PriorityMax)
                     .SetColor(0x9c6114)
                     .SetSound(urgentAlarmUri, (int)alarmAttributes) // Loke                        
                     .SetVibrate(urgentVibrationPattern) // Loke
                     .SetSound(Android.Net.Uri.Parse("android.resource://InstaConsumer.Droid/Assets/air_horn_big.mp3"))
                     .SetAutoCancel(true);
            manager.Notify(id, notification.Build());


            //var notificationBuilder = new NotificationCompat.Builder(this)
            //    .SetSmallIcon(Resource.Drawable.logo)
            //    .SetContentTitle("FCM Message")
            //    .SetContentText(messageBody)
            //    .SetAutoCancel(true)
            //    .SetContentIntent(pendingIntent);

            //var notificationManager = NotificationManagerCompat.From(this);
            //notificationManager.Notify(0, notificationBuilder.Build());
        }

        */

        /*

                // Previous Code
                public override void OnMessageReceived(RemoteMessage message)
                {
                    Log.Debug(TAG, "From: " + message.From);
                    var body = message.GetNotification().Body;
                    Log.Debug(TAG, "Notification Message Body: " + body);
                    SendNotification(body);
                }

                void SendNotification(string messageBody)
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    intent.AddFlags(ActivityFlags.ClearTop);

                    var pendingIntent = PendingIntent.GetActivity(this, 0, intent,
                        PendingIntentFlags.OneShot);

                    var notificationBuilder = new NotificationCompat.Builder(this)
                        .SetSmallIcon(Resource.Drawable.logo)
                        .SetContentTitle("FCM Message")
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetContentIntent(pendingIntent);

                    var notificationManager = NotificationManagerCompat.From(this);
                    notificationManager.Notify(0, notificationBuilder.Build());
                }

                    */
        /*
        // Remote Fore Ground

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            SendNotification(message.GetNotification().Body, message.Data);
        }
        private void SendNotification(string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity)); intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }
            var pendingIntent = PendingIntent.GetActivity(this, MainActivity.NOTIFICATION_ID, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID).SetSmallIcon(Resource.Drawable.logo).SetContentTitle("FCM Message").SetContentText(messageBody).SetAutoCancel(true).SetContentIntent(pendingIntent).SetPriority(NotificationCompat.PriorityMax);
            var notificationManager = NotificationManagerCompat.From(this); notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
        */

        // Internal Fore Ground
        public override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                Log.Debug(TAG, "From: " + message.From);                
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotifications(message);
            }
            catch (Exception ex)
            {
            }

        }
        public void SendNotifications(RemoteMessage message)
        {
            try
            {
                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
                int id = new Random(seed).Next(000000000, 999999999);
                var push = new Intent();
                push.AddFlags(ActivityFlags.ClearTop);
                var fullScreenPendingIntent = PendingIntent.GetActivity(this, MainActivity.NOTIFICATION_ID,
               push, PendingIntentFlags.UpdateCurrent);
                NotificationCompat.Builder notification;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var chan1 = new NotificationChannel(PRIMARY_CHANNEL,
                     new Java.Lang.String("Primary"), NotificationImportance.High);
                    chan1.LightColor = Color.Green;
                    chan1.LockscreenVisibility = NotificationVisibility.Public;
                    chan1.SetShowBadge(true);
                    manager.CreateNotificationChannel(chan1);
                    notification = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID);
                    notification.SetFullScreenIntent(fullScreenPendingIntent, true);
                }
                else
                {
                    notification = new NotificationCompat.Builder(this);
                }

                // set the vibration patterns for the channels
                long[] urgentVibrationPattern = { 100, 30, 100, 30, 100, 200, 200, 30, 200, 30, 200, 200, 100, 30, 100, 30, 100, 100, 30, 100, 30, 100, 200, 200, 30, 200, 30, 200, 200, 100, 30, 100, 30, 100 };

                // Creating an Audio Attribute
                var alarmAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Speech)
                    .SetUsage(AudioUsageKind.Notification).Build();

                // Create the uri for the alarm file

                var recordingFileDestinationPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.Path, AppConstants.NotificationSound);
                Android.Net.Uri urgentAlarmUri = Android.Net.Uri.Parse(recordingFileDestinationPath);

                notification.SetContentIntent(fullScreenPendingIntent)
                         .SetContentTitle(message.GetNotification().Title)
                         .SetContentText(message.GetNotification().Body)                        
                         .SetSmallIcon(Resource.Drawable.logo)
                         //.SetStyle((new NotificationCompat.BigTextStyle()))
                         .SetStyle(new NotificationCompat.BigTextStyle().BigText(message.GetNotification().Body))
                         .SetAutoCancel(true)                        
                         .SetPriority((int)NotificationPriority.High)
                         .SetVisibility((int)NotificationVisibility.Public)
                         .SetDefaults(NotificationCompat.DefaultVibrate | NotificationCompat.DefaultSound)                        
                         //.SetVibrate(new long[0])
                         //.SetVibrate(new long[] { 300, 300, 300 })
                         .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification));
                         //.SetPriority(NotificationCompat.PriorityMax)
                         //.SetColor(0x9c6114)
                         //.SetSound(urgentAlarmUri, (int)alarmAttributes) // Loke                        
                         //.SetVibrate(urgentVibrationPattern) // Loke
                         //.SetSound(Android.Net.Uri.Parse("android.resource://InstaConsumer.Droid/Assets/air_horn_big.mp3"))
                        
                manager.Notify(id, notification.Build());               
            }
            catch (Exception ex)
            {
            }
        }
    }
}