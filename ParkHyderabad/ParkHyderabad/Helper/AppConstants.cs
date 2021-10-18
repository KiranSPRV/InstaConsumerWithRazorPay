using System;
using System.Collections.Generic;
using System.Text;

namespace ParkHyderabad
{
    public static class AppConstants
    {
        public static string NotificationChannelName { get; set; } = "XamarinNotifyChannel";
        public static string NotificationHubName { get; set; } = "InstaConsumerHub";
        public static string ListenConnectionString { get; set; } = "Endpoint=sb://instaconsumerhubnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=q33XLGrV2Yspe/WwjzvdCv/JAEyHNTsNXuT/zMS3A9E=";
        public static string DebugTag { get; set; } = "XamarinNotify";
        public static string[] SubscriptionTags { get; set; } = { "default" };
        public static string FCMTemplateBody { get; set; } = "{\"data\":{\"message\":\"$(messageParam)\"}}";
        public static string APNTemplateBody { get; set; } = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";
        public static string NotificationSound { get; set; } = "android.resource://InstaConsumer.Droid/Assets/air_horn_big.mp3";
    }
}
