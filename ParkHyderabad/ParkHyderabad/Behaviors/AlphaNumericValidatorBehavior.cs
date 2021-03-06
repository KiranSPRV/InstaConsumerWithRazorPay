using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ParkHyderabad.Behaviors
{
    public class AlphaNumericValidatorBehavior : Behavior<Entry>
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        const string alphanumericRegex = @"^[a-zA-Z0-9]*$";

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            ((Entry)sender).Text = e.NewTextValue.ToUpper();
            ((Entry)sender).TextColor = Color.FromHex("#a3a3a3");
            bool IsValid = false;
            IsValid = (Regex.IsMatch(e.NewTextValue, alphanumericRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

            if (!IsValid)
            {
                ((Entry)sender).Text = e.NewTextValue.Substring(0, e.NewTextValue.Length - 1).ToUpper();
            }            
            if (e.NewTextValue.Length < this.MinLength)
            {
               
            }
            if (e.NewTextValue.Length > this.MaxLength)
            {
                string entryText = e.NewTextValue;  
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}

