using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ParkHyderabad.Behaviors
{
    public class EmailValidatorBehavior : Behavior<Entry>
    {

        public int MaxLength { get; set; }
        public int MinLength { get; set; }       

        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            ((Entry)sender).TextColor = Color.FromHex("#010101");
            bool IsValid = false;
            IsValid = (Regex.IsMatch(e.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

            if (IsValid)
            {
                ((Entry)sender).Text = e.NewTextValue.ToUpper();
            }
            else
            {
                ((Entry)sender).Text = e.NewTextValue.Substring(0, e.NewTextValue.Length - 1).ToUpper();
            }
          
            if (e.NewTextValue.Length < this.MinLength)
            {
                ((Entry)sender).TextColor = Color.Red;
            }
            if (e.NewTextValue.Length > this.MaxLength)
            {
                string entryText = e.NewTextValue;
                entryText = entryText.Remove(entryText.Length - 1); // remove last char
                ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength).ToUpper();
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
