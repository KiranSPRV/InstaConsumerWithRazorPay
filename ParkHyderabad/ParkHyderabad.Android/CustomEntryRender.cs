using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using ParkHyderabad.Helper;
using ParkHyderabad.Droid;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using System;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace ParkHyderabad.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var view = (CustomEntry)Element;
                if (view.IsCurvedCornersEnabled)
                {
                    // creating gradient drawable for the curved background  
                    var _gradientBackground = new GradientDrawable();
                    _gradientBackground.SetShape(ShapeType.Line);
                    _gradientBackground.SetColor(view.BackgroundColor.ToAndroid());
                    //Control.SetPadding(0, 0, 0, 0);                    

                    // Thickness of the stroke line  
                    //_gradientBackground.SetStroke(view.BorderWidth, view.BorderColor.ToAndroid());

                    var blueHex = ColorConverters.FromHex("#D4D5D9");
                    Xamarin.Forms.Color xfColour = blueHex;
                    Android.Graphics.Color greyColor = xfColour.ToAndroid();
                    _gradientBackground.SetStroke(view.BorderWidth, greyColor);


                    // Radius for the curves  
                    //_gradientBackground.SetCornerRadius(
                    //    DpToPixels(this.Context, Convert.ToSingle(view.CornerRadius)));

                    // set the background of the   
                    //Control.SetBackground(_gradientBackground);
                }
                // Set padding for the internal text from border  
                //Control.SetPadding(
                //    (int)DpToPixels(this.Context, Convert.ToSingle(12)), Control.PaddingTop,
                //    (int)DpToPixels(this.Context, Convert.ToSingle(12)), Control.PaddingBottom);
            }
            if (Control == null || e.NewElement == null) return;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
            else
                Control.Background.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);

            //IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
            //IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "mCursorDrawableRes", "I");
            //JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.my_cursor);
        }
        public static float DpToPixels(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}