using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps.GoogleStaticMaps
{
    class Route
    {
        public enum ColorRoute { standard, black, brown, green, purple, yellow, blue, gray, orange, red, white }

        public List<string> Center { get; set; }
        public int Weight { get; set; }
        public ColorRoute Color { get; set; }
        public ColorRoute FillColor { get; set; }


        public static string getRoute(Route routes)
        {
            string routesReturn = "";
            if (routes != null)
            {
                routesReturn = getRouteString(routes);
            }
            return routesReturn;
        }
        private static string getRouteString(Route route)
        {
            if(route!=null && route.Center!=null)
            {

          
            string weight, color, fillColor, center;
            color = "color:" + route.Color;
            weight = (route.Weight != 0) ? "|weight:" + route.Weight.ToString() : "";
            fillColor = (route.FillColor != ColorRoute.standard) ? "|fillcolor:" + route.FillColor : "";
            center = "";
            foreach (string item in route.Center)
            {
                center += "|" + item;
            }
            return "&path=" + color + weight + fillColor + center;
            }
            else
            {
                return "";
            }
        }


    }
}
