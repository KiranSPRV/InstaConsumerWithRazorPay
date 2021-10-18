using System.Collections;
using Xamarin.Forms;

namespace ParkHyderabad.CustomXamarinElementsModel
{
    public class SingleColorDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SingleTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {

            ListView lv = container as ListView;
            if (lv != null)
            {
                IList listItem = lv.ItemsSource as IList;

                int idx = listItem.IndexOf(item);
                return SingleTemplate;
            }
            else
            {
                return SingleTemplate;
            }
        }
    }
}

