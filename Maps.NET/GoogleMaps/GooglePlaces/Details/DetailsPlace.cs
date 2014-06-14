using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maps.NET.GoogleMaps.GooglePlaces.Details
{
    class DetailsPlace
    {
        public Place PlaceDetails { get; private set; }
        public List<Review> Reviews { get; private set; }
        public List<Photo> Photos { get; set; }
        public event EventHandler FinishedReading;

        private WebClient Query;
        private const string basicURL = "https://maps.googleapis.com/maps/api/place/details/xml?reference=";
        private const string finalURL = "&sensor=true&key=";
        private Location position;

        public void getDetails(string reference, string language="es")
        {
            position = null;
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url=basicURL + reference + "&language=" + language + finalURL + GoogleStaticInfo.Key;
            Query.OpenReadAsync(new Uri(url, UriKind.Absolute));
            GoogleStaticInfo.setRequest("details place", url);
        }

        public void getDetails(string reference, Location actualPosition, string language="es")
        {
            position = actualPosition;
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + reference + "&language=" + language + finalURL + GoogleStaticInfo.Key;
            Query.OpenReadAsync(new Uri(url, UriKind.Absolute));
            GoogleStaticInfo.setRequest("details place", url);
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
                    PlaceDetails = new Place();
                    PlaceDetails = (from text in document.Descendants("result")
                                    select new Place
                                    {
                                        Name = (text.Element("name") != null) ? text.Element("name").Value.ToUpper() : "",
                                        Address = (text.Element("formatted_address") != null) ? text.Element("formatted_address").Value : "",
                                        PhoneNumber = (text.Element("formatted_phone_number") != null) ? text.Element("formatted_phone_number").Value : "",
                                        Rating = (text.Element("rating") != null) ? text.Element("rating").Value : "0",
                                        RatingDecimal = (text.Element("rating") != null) ? (Convert.ToDouble(text.Element("rating").Value) / 5).ToString() : "0",
                                        RatingString = (text.Element("rating") != null) ? text.Element("rating").Value + "/5" : "-/5",
                                        WebGoogle = (text.Element("url").Value != null) ? text.Element("url").Value : "",
                                        Website = (text.Element("website") != null) ? text.Element("website").Value : "",
                                        Activity = (text.Element("icon") != null) ? text.Element("icon").Value : "",
                                        Horary = (text.Element("opening_hours") != null) ? (text.Element("opening_hours").Element("open_now").Value == "open") ? "/Assets/Places/Place/Open.png" : "/Assets/Places/Place/Close.png" : "/Assets/Places/Place/Unknow.png",
                                        HoraryIcon = (text.Element("opening_hours") != null) ? text.Element("opening_hours").Element("open_now").Value : "",
                                        Latitude = text.Element("geometry").Element("location").Element("lat").Value,
                                        Longitude = text.Element("geometry").Element("location").Element("lng").Value
                                    }).First();

                    PlaceDetails.GeoCoordinates=convertoToGeoCoordinate(PlaceDetails.Latitude, PlaceDetails.Longitude);
                    if (position != null) { PlaceDetails.Distance = getDistance(position, PlaceDetails.GeoCoordinates).ToString("0.0"); }
                    PlaceDetails.StaticMap = staticMap(PlaceDetails);
                    Reviews = new List<Review>();
                    Reviews = (from text in document.Descendants("review")
                               select new Review
                               {
                                   Time = (text.Element("time") != null) ? UnixTimeStampToDateTime(text.Element("time").Value) : "",
                                   Text = (text.Element("text") != null) ? text.Element("text").Value : "",
                                   Author = (text.Element("author_name") != null) ? text.Element("author_name").Value : "",
                                   GooglePlusAuthor = (text.Element("author_url") != null) ? text.Element("author_url").Value : "",
                                   Rating = (text.Element("rating") != null) ? text.Element("rating").Value : "",
                                   RatingDecimal = (text.Element("rating") != null) ? (Convert.ToDouble(text.Element("rating").Value) / 5).ToString() : "0",

                               }).ToList();

                    const string initial = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=160&photoreference=";
                    const string initialMax = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=1600&photoreference=";

                    Photos = new List<Photo>();
                    Photos = (from text in document.Descendants("photo")
                              select new Photo
                              {
                                  ThumbailURL = initial + text.Element("photo_reference").Value + finalURL + GoogleStaticInfo.Key,
                                  PhotoReference = initialMax + text.Element("photo_reference").Value + finalURL + GoogleStaticInfo.Key,
                                  Width = text.Element("width").Value,
                                  Height = text.Element("height").Value
                              }).ToList();

                }
                FinishedReading(sender, e);

            }
            catch (Exception)
            {

            }

        }
        private double getDistance(Location p1, Location p2)
        {
            double d = p1.Latitude * 0.017453292519943295;
            double num3 = p1.Longitude * 0.017453292519943295;
            double num4 = p2.Latitude * 0.017453292519943295;
            double num5 = p2.Longitude * 0.017453292519943295;
            double num6 = num5 - num3;
            double num7 = num4 - d;
            double num8 = Math.Pow(Math.Sin(num7 / 2.0), 2.0) + ((Math.Cos(d) * Math.Cos(num4)) * Math.Pow(Math.Sin(num6 / 2.0), 2.0));
            double num9 = 2.0 * Math.Atan2(Math.Sqrt(num8), Math.Sqrt(1.0 - num8));
            return ((6376500.0 * num9) / 1000);
        }
        private Location convertoToGeoCoordinate(string latitude, string longitude)
        {
            double lat, lng;
            double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
            double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lng);
            return new Location(lat, lng);
        }

        //http://stackoverflow.com/questions/249760/how-to-convert-unix-timestamp-to-datetime-and-vice-versa
        private string UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime.Day + "/" + dtDateTime.Month + "/" + dtDateTime.Year;
        }
        
        private string staticMap(Place place)
        {
            string url = "http://maps.google.com/maps/api/staticmap?&" + "center=" + place.Latitude + "," + place.Longitude + "&zoom=14&size=575x303&format=png&maptype=roadmap&language=es&markers=color:red|label:1|" + place.Latitude + "," + place.Longitude + "&sensor=false";
            return url;
        }
       
    }
      
}
