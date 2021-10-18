using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Android.Factories;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using AndroidBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;

namespace ParkHyderabad.Droid
{
    public sealed class AccessNativeBitmapConfig : IBitmapDescriptorFactory
    {
        public AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor)
        {
            int resId = 0;

            switch (descriptor.Id)
            {
                case "GREEN":
                    resId = Resource.Drawable.active_lot;
                    break;
                case "ORANGE":
                    resId = Resource.Drawable.going_to_filled_out;
                    break;
                case "RED":
                    resId = Resource.Drawable.filled_lot;
                    break;
                case "GREY":
                    resId = Resource.Drawable.selected_lot;
                    break;
                case "YELLOW":
                    resId = Resource.Drawable.map_current;
                    break;
            }

            return AndroidBitmapDescriptorFactory.FromResource(resId);
        }
    }
}