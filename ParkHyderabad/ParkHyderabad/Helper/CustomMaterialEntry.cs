using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ParkHyderabad.Helper
{
    public class CustomMaterialEntry : Entry
    {
        public static readonly BindableProperty MyHighlightColorProperty = BindableProperty.Create(nameof(MyHighlightColor), typeof(Color), typeof(CustomMaterialEntry));

        public static readonly BindableProperty MyHandleColorProperty = BindableProperty.Create(nameof(MyHandleColor), typeof(Color), typeof(CustomMaterialEntry));

        public static readonly BindableProperty MyTintColorProperty = BindableProperty.Create(nameof(MyTintColor), typeof(Color), typeof(CustomMaterialEntry));

        public Color MyHighlightColor
        {
            get => (Color)GetValue(MyHighlightColorProperty);
            set => SetValue(MyHighlightColorProperty, value);
        }

        public Color MyHandleColor
        {
            get => (Color)GetValue(MyHandleColorProperty);
            set => SetValue(MyHandleColorProperty, value);
        }

        public Color MyTintColor
        {
            get => (Color)GetValue(MyTintColorProperty);
            set => SetValue(MyTintColorProperty, value);
        }
    }
}
