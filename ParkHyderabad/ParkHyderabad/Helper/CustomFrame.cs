using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ParkHyderabad.Helper
{
    public class CustomFrame : Frame
    {
        public static new readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CustomFrame), typeof(CornerRadius), typeof(CustomFrame));
        // public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(nameof(ShadowColor), typeof(Color), typeof(CustomFrame), Color.Transparent);
        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(float), typeof(CustomFrame));
        public static readonly BindableProperty ShadowRadiusProperty = BindableProperty.Create(nameof(ShadowRadius), typeof(int), typeof(CustomFrame));
        //public static readonly BindableProperty ShadowOpacityProperty = BindableProperty.Create(nameof(ShadowOpacity), typeof(float), typeof(CustomFrame));
        public CustomFrame()
        {
            // MK Clearing default values (e.g. on iOS it's 5)
            base.CornerRadius = 0;
        }
        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        //public Color ShadowColor
        //{
        //    get { return (Color)GetValue(ShadowColorProperty); }
        //    set { SetValue(ShadowColorProperty, value); }
        //}


        public float BorderWidth
        {
            get { return (float)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }
        public int ShadowRadius
        {
            get { return (int)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }

        //public float ShadowOpacity
        //{
        //    get { return (float)GetValue(ShadowOpacityProperty); }
        //    set { SetValue(ShadowOpacityProperty, value); }
        //}
    }
}
