using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ParkHyderabad.Helper
{
    public class FloatingLableforOfferSpace : Entry
    {       
        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(string), string.Empty, BindingMode.TwoWay, null);
        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(int), 0, BindingMode.TwoWay, null);
        public static readonly BindableProperty CharacterSpacingProperty = BindableProperty.Create("CharacterSpacing", typeof(decimal), typeof(decimal), Convert.ToDecimal(0), BindingMode.TwoWay, null);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }
        public decimal CharacterSpacing
        {
            get { return (decimal)GetValue(CharacterSpacingProperty); }
            set { SetValue(CharacterSpacingProperty, value); }
        }
    }
}
