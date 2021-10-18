using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ParkHyderabad.Helper;
using ParkHyderabad.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ParkHyderabad.Controls;

[assembly: ExportRenderer(typeof(FloatingLableforOfferSpace), typeof(OfferSpaceEntryRenderer))]
namespace ParkHyderabad.Droid
{
    public class OfferSpaceEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);

            else
                Control.Background.SetColorFilter(Android.Graphics.Color.Gray, PorterDuff.Mode.SrcAtop);

            //IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
            //IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "mCursorDrawableRes", "I");
            //JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.my_cursor);
            //Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Gray);
        }
    }
}