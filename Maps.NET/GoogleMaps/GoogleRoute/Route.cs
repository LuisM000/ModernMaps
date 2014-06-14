using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maps.NET.GoogleMaps.GoogleRoute
{
    class Route
    {
        public enum Mode { driving, walking, bicycling };
        public enum Avoid { none, tolls, highways };

        public static Mode ModeRoute { get; set; }
        public static Avoid AvoidRoute { get; set; }

        public List<Direction> RouteList { get; set; }
        public GeneralDirections GeneralDatesRoute { get; set; } 
        public event EventHandler FinishedReading;

        private WebClient Query;
        private const string basicURL = "https://maps.googleapis.com/maps/api/directions/xml?";
        private const string finalURL = "&sensor=true";

        public void getRoute(Location startPoint,Location endPoint, string language="es")
        {
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "origin=" + startPoint.Latitude.ToString().Replace(",", ".") + "," + startPoint.Longitude.ToString().Replace(",", ".")
                + "&destination=" + endPoint.Latitude.ToString().Replace(",", ".") + "," + endPoint.Longitude.ToString().Replace(",", ".") + routeOptions() + " &language=" + language + finalURL;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("route", url);
        }
        public void getRoute(string startPoint, string endPoint, string language = "es")
        {
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "origin=" + startPoint + "&destination=" + endPoint + routeOptions() + "&language=" + language + finalURL;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("route", url);

        }


        void Query_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                }
                else
                {
                    Stream stream = e.Result;
                    XDocument document = XDocument.Load(stream);
                    RouteList = new List<Direction>();
                    RouteList = (from text in document.Descendants("step")
                                 select new Direction
                                 {
                                     StartGeocoordinate=new Location(Convert.ToDouble(text.Element("start_location").Element("lat").Value),Convert.ToDouble(text.Element("start_location").Element("lng").Value)),
                                     StartLatitude = text.Element("start_location").Element("lat").Value,
                                     StartLongitude = text.Element("start_location").Element("lng").Value,
                                     FinishGeocoordinate = new Location(Convert.ToDouble(text.Element("end_location").Element("lat").Value), Convert.ToDouble(text.Element("end_location").Element("lng").Value)),
                                     FinishLatitude = text.Element("end_location").Element("lat").Value,
                                     FinishLongitude = text.Element("end_location").Element("lat").Value,
                                     EncodePolyline = text.Element("polyline").Element("points").Value,
                                     DecodePolyline = decodePolyline(text.Element("polyline").Element("points").Value),
                                     DurationSeconds = text.Element("duration").Element("value").Value,
                                     DurationText = text.Element("duration").Element("text").Value,
                                     DistanceMeters = text.Element("distance").Element("value").Value,
                                     DistanceText = text.Element("distance").Element("text").Value,
                                     Description = getRemoveTags(text.Element("html_instructions").Value),
                                 }).ToList();
                    if(RouteList.Count>2)
                    {
                        Direction finishDirection = new Direction();
                        finishDirection.Description = "Ha llegado a su destino"; 
                        finishDirection.DistanceMeters = "0";
                        finishDirection.DecodePolyline = new List<Location>() { new Location(RouteList[RouteList.Count - 1].FinishGeocoordinate) };
                        finishDirection.DistanceText = "-"; 
                        finishDirection.DurationSeconds = "0";
                        finishDirection.DurationText = "-"; 
                        finishDirection.Index += 1; 
                        finishDirection.StartGeocoordinate = RouteList[RouteList.Count-1].FinishGeocoordinate;
                        finishDirection.FinishGeocoordinate = RouteList[RouteList.Count - 1].FinishGeocoordinate;

                        RouteList.Add(finishDirection);
                    }
                    GeneralDatesRoute = new GeneralDirections();
                    GeneralDatesRoute = (from text in document.Descendants("route")
                                 select new GeneralDirections
                                 {
                                     Summary = text.Element("summary").Value,
                                     Copyright = text.Element("copyrights").Value,
                                     EncodeOverviewPolyline = text.Element("overview_polyline").Element("points").Value,
                                     DecodeOverviewPolyline = decodePolyline(text.Element("overview_polyline").Element("points").Value),
                                 }).First();
                    GeneralDatesRoute.StartPoint = RouteList[0].StartGeocoordinate;
                    GeneralDatesRoute.EndPoint = RouteList[RouteList.Count - 1].StartGeocoordinate;
                    int seconds = 0;
                    int index = 0;
                    foreach (var item in RouteList)
                    {
                        item.Index = index;
                        GeneralDatesRoute.DistanceMeters += Convert.ToInt64(item.DistanceMeters);
                        seconds += Convert.ToInt16(item.DurationSeconds);
                        index++;
                    }
                    GeneralDatesRoute.Duration = TimeSpan.FromSeconds(seconds);
                    FinishedReading(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }

        private string routeOptions()
        {
            string options="&mode=" + ModeRoute.ToString();
            if (AvoidRoute != Avoid.none) { options += "&avoid=" + AvoidRoute.ToString(); }
            return options;
        }
        public static List<Location> decodePolyline(String encoded)
        {

            List<Location> poly = new List<Location>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                Location p = new Location((double)(((double)lat / 1E5) * 1E6) / 1000000,
                     (double)(((double)lng / 1E5) * 1E6) / 1000000);
                poly.Add(p);
            }
            return poly;

        }
        public static string encodePolyline(List<Location> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;
                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;
            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);
                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);
                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }
        private string getRemoveTags(string item)
        {
            item=item.Replace("><", "> <"); item=item.Replace(">.", ">. ");
            Regex regex = new Regex("<(.|\n)*?>");
            item = regex.Replace(item, string.Empty);
            return item;
        }

    }
}
