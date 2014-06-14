using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Maps.NET.GoogleMaps.GoogleStaticMaps
{
    class Marker
    {
        public enum SizeMarker { normal, tiny, mid, small }
        public enum ColorMarker { standard, black, brown, green, purple, yellow, blue, gray, orange, red, white }

        public string Center { get; set; }
        public SizeMarker Size { get; set; }
        public ColorMarker Color { get; set; }
        public string Label { get; set; }

        public static string getMarkers(List<Marker> markers)
        {
            string markersReturn = "";
            if(markers!=null && markers.Count>0)
            {
                foreach (var item in markers)
                {
                    markersReturn += getMarker(item);
                }
            }
            return markersReturn;
        }
        private static string getMarker(Marker marker)
        {
            string color, label, size, center;
            color = (marker.Color != ColorMarker.standard) ? "color:" + marker.Color : "";
            label = (marker.Label != null) ? "|label:" + marker.Label : "";
            size = (marker.Size != Marker.SizeMarker.normal) ? "|size:" + marker.Size : "";
            center="|" + marker.Center;
            return "&markers=" + color + label + size + center;
        }
    }
}
