using Android.Content;
using Android.Widget;
using ParkHyderabad.Droid;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomSearchBarRender))]
namespace ParkHyderabad.Droid
{
    public class CustomSearchBarRender : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                //plate.SetBackgroundColor(Android.Graphics.Color.White);

                var blueHex = ColorConverters.FromHex("#FFFFFF");
                Xamarin.Forms.Color xfColour = blueHex;
                Android.Graphics.Color whiteColor = xfColour.ToAndroid();
                plate.SetBackgroundColor(whiteColor);

                var searchView = Control;
                searchView.Iconified = true;
                searchView.Left = 0;
                searchView.SetIconifiedByDefault(false);
                // (Resource.Id.search_mag_icon); is wrong / Xammie bug
                int searchIconId = Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
                var icon = searchView.FindViewById(searchIconId);
                (icon as ImageView).SetImageResource(Resource.Drawable.search);
            }

            //if (e.NewElement != null)
            //{
            //    var searchView = base.Control as SearchView;
            //    //Get the Id for your search icon
            //    int searchIconId = Context.Resources.GetIdentifier("android:id/search", null, null);
            //    ImageView searchViewIcon = (ImageView)searchView.FindViewById<ImageView>(searchIconId);
            //    ViewGroup linearLayoutSearchView = (ViewGroup)searchViewIcon.Parent;
            //    searchViewIcon.SetAdjustViewBounds(true);
            //    //Remove the search icon from the view group and add it once again to place it at the end of the view group elements
            //    linearLayoutSearchView.RemoveView(searchViewIcon);
            //    linearLayoutSearchView.AddView(searchViewIcon);
            //}
        }
    }
}