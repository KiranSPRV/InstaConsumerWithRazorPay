using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using ParkHyderabad.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Webkit.WebSettings;

[assembly: ExportRenderer(typeof(Xamarin.Forms.WebView), typeof(MyWebviewRender))]
namespace ParkHyderabad.Droid
{
    public class MyWebviewRender : WebViewRenderer
    {
        public MyWebviewRender(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            Control.Settings.JavaScriptEnabled = true;
            Control.Settings.SetAppCacheEnabled(true);
            Control.Settings.CacheMode = Android.Webkit.CacheModes.Normal;
            Control.Settings.SetRenderPriority(RenderPriority.High);
            Control.Settings.DomStorageEnabled = true;
            Control.Settings.BlockNetworkImage = true;
            Control.SetWebChromeClient(new MyWebChromeClient());
        }

        
    }
    internal class MyWebChromeClient : WebChromeClient
    {
        public override void OnProgressChanged(Android.Webkit.WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);
            if (newProgress == 100)
            {
                view.Settings.BlockNetworkImage = false;
            }
            else
            {
                
            }
        }
    }
}