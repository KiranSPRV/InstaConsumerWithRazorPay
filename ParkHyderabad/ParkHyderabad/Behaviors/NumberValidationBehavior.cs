using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ParkHyderabad.Behaviors
{
    public class NumberValidationBehavior : Behavior<Entry>
    {
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        const string numaricRegex = @"^[0-9]*$";
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            ((Entry)sender).TextColor = Color.FromHex("#010101");
            bool isValid = (Regex.IsMatch(e.NewTextValue, numaricRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

            if (isValid)
            {
                if (e.NewTextValue.Length < this.MinLength)
                {
                    ((Entry)sender).TextColor = Color.Red;
                }
                if (e.NewTextValue.Length > this.MaxLength)
                {
                    string entryText = e.NewTextValue;
                    entryText = entryText.Remove(entryText.Length - 1); // remove last char
                    ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength);
                }
            }
            else
            {
                ((Entry)sender).Text = e.NewTextValue.Substring(0, e.NewTextValue.Length - 1).ToUpper();
            }
        }
    }
}
