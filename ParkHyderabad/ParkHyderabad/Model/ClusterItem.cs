using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps.Clustering;
using Xamarin.Forms.GoogleMaps;

namespace ParkHyderabad.Model
{
    public class ClusterItem
    {
        public Position position;
        public string title;
        public string snippet;

        public ClusterItem(double lat, double lng, string title, string snippet)
        {
            position = new Position(lat, lng);
            this.title = title;
            this.snippet = snippet;
        }

        public Position getPosition()
        {
            return position;
        }
        public string getTitle()
        {
            return title;
        }
        public string getSnippet()
        {
            return snippet;
        }
    }
}
