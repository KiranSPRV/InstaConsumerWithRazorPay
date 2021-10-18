using ParkHyderabad.DAL;
using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ParkHyderabad
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPasses : ContentPage
    {
        public AboutPasses()
        {
            InitializeComponent();
        }

        private async void lbl_TermsAndConditionsTapped(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new TermsAndConditions());
        }
    }
}