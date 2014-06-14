using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maps.NET.GoogleMaps.GoogleStreet
{
    class StreetView
    {
        private const string basicURL = "http://maps.googleapis.com/maps/api/streetview?";
        private const string finalURL = "&sensor=true";

        public string getImage(string address, Size size, int heading, int pitch, int fov)
        {
            string url = basicURL + "size=" + size.Width.ToString() + "x" + size.Height.ToString()
                + "&location=" + address + "&heading=" + heading.ToString() + "&pitch=" + pitch.ToString() + "&fov=" + fov.ToString() + finalURL;
            GoogleStaticInfo.setRequest("image street view", url);
            return url;
        }
    }
}
