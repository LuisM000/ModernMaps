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

namespace Maps.NET.GoogleMaps.GooglePlaces
{

    class Places
    {
        public List<Place> PlaceList { get; set; }
        public List<Autocomplete> AutocompleteList { get; set; }
        public Token NextResults { get; private set; }
        public static SortBy sortBy { get; set; }
        public static int distance = 5000;

        public event EventHandler FinishedReading;
        public event EventHandler FinishedReadingAutocomplete;
        public enum TypesPlaces
        {
            all, accounting, airport, amusement_park, aquarium, art_gallery, atm, bakery, bank, bar, beauty_salon, bicycle_store, book_store, bowling_alley, bus_station, cafe, campground, car_dealer, car_rental, car_repair, car_wash, casino, cemetery, church, city_hall, clothing_store, convenience_store, courthouse, dentist, department_store, doctor, electrician, electronics_store, embassy, establishment, finance, fire_station, florist, food, funeral_home, furniture_store, gas_station, general_contractor, grocery_or_supermarket, gym, hair_care, hardware_store, health, hindu_temple, home_goods_store, hospital, insurance_agency, jewelry_store, laundry, lawyer, library, liquor_store, local_government_office, locksmith, lodging, meal_delivery, meal_takeaway, mosque, movie_rental, movie_theater, moving_company, museum, night_club, painter, park, parking, pet_store, pharmacy, physiotherapist, place_of_worship, plumber, police, post_office, real_estate_agency, restaurant, roofing_contractor, rv_park, school, shoe_store, shopping_mall, spa, stadium, storage, store, subway_station, synagogue, taxi_stand, train_station, travel_agency, university, veterinary_care, zoo
        }
        public enum SortBy { None, Distance, Rating, Opening }
        public enum TypeSearch { Keyword, Name }

        private WebClient Query;
        private const string basicURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/xml?";
        private const string basicURLcomplete = "https://maps.googleapis.com/maps/api/place/autocomplete/xml?";
        private const string finalURL = "&sensor=true&key=";
        private Location coordinates;

        public Places()
        {
            PlaceList = new List<Place>();
            NextResults = new Token();
        }

        public void getPlaces(Location coordinates, List<TypesPlaces> typePlace, string language, string input = "", TypeSearch type = TypeSearch.Keyword)
        {
            this.coordinates = coordinates;
            string types = "";
            if (typePlace[0] != TypesPlaces.all)
            {
                foreach (var item in typePlace)
                {
                    types += item.ToString() + "|";
                }
            }
            if (input != "")
            {
                input = "&" + type.ToString().ToLower() + "=" + input;
            }

            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "location=" + coordinates.Latitude.ToString().Replace(",", ".") + "," + coordinates.Longitude.ToString().Replace(",", ".") +
               "&radius=" + distance.ToString() + "&types=" + types + input + "&language=" + language + finalURL + GoogleStaticInfo.Key;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("places (coordinates)", url);
        }
        public void getPlaces(Location coordinates, string token)
        {
            this.coordinates = coordinates;
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "pagetoken=" + token + finalURL + GoogleStaticInfo.Key;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("places (token)", url);
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
                    PlaceList = new List<Place>();
                    PlaceList = (from text in document.Descendants("result")
                                 select new Place
                                 {
                                     Name = text.Element("name").Value,
                                     Rating = (text.Element("rating") != null) ? text.Element("rating").Value : "0",
                                     RatingDecimal = (text.Element("rating") != null) ? (Convert.ToDouble(text.Element("rating").Value)/5).ToString() : "0",
                                     RatingString = (text.Element("rating") != null) ? text.Element("rating").Value + "/5" : "-/5",
                                     Activity = text.Element("icon").Value,
                                     Horary = (text.Element("opening_hours") != null) ? text.Element("opening_hours").Value : " unknown horary",
                                     HoraryIcon = (text.Element("opening_hours") != null) ? ((text.Element("opening_hours").Value == "true") ? "/Assets/Places/Place/Open.png" : "/Assets/Places/Place/Close.png") : "/Assets/Places/Place/Unknow.png",
                                     Latitude = text.Element("geometry").Element("location").Element("lat").Value,
                                     Longitude = text.Element("geometry").Element("location").Element("lng").Value,
                                     Vicinity = (text.Element("vicinity") != null) ? text.Element("vicinity").Value : "",
                                     PlaceReference = text.Element("reference").Value
                                 }).ToList();

                    NextResults = new Token();
                    NextResults = (from text in document.Descendants("PlaceSearchResponse")
                                   select new Token
                                   {
                                       NextPage = (text.Element("next_page_token") != null) ? text.Element("next_page_token").Value : "no_token",
                                   }).First();

                    double lat, lng, distance;
                    foreach (var item in PlaceList)
                    {
                        double.TryParse(item.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                        double.TryParse(item.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lng);
                        item.GeoCoordinates = new Location(lat, lng);
                        distance = getDistance(this.coordinates, item.GeoCoordinates);
                        item.Distance = distance.ToString("0.0");
                    }
                    PlaceList = sort(PlaceList);
                    FinishedReading(sender, e);

                }
            }
            catch (Exception)
            {
            }

        }
        
        public void getAutocomplete(string input, Location coordinates,bool location=false, string language="es")
        {
            Query = new WebClient();
            Query.OpenReadCompleted +=QueryAutocomplete_OpenReadCompleted;
            string locationA = "";
            if (location) { locationA = "&location=" + coordinates.Latitude.ToString().Replace(",", ".") + "," + coordinates.Longitude.ToString().Replace(",", "."); }
            string url = basicURLcomplete + "&input=" + input + locationA + "&radius=" + distance + "&types=establishment" + "&language=" + language + finalURL + GoogleStaticInfo.Key;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("autocomplete", url);
        }

        void QueryAutocomplete_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
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
                    AutocompleteList = new List<Autocomplete>();
                    AutocompleteList = (from text in document.Descendants("prediction")
                                        select new Autocomplete
                                        {
                                            Type = text.Element("description").Value,
                                            Reference = text.Element("reference").Value,
                                            Description = text.Element("description").Value,
                                            Term = text.Element("term").Element("value").Value
                                        }).ToList();
                    FinishedReadingAutocomplete(sender, e);

                }
            }
            catch (Exception)
            {
            }

        }


        public List<Place> sort(List<Place> PlaceList)
        {
            List<Place> PlaceListTemporal = PlaceList;
            switch (sortBy)
            {
                case SortBy.Distance:
                    PlaceListTemporal = sortDistance(PlaceList);
                    break;
                case SortBy.Rating:
                    PlaceListTemporal = sortRating(PlaceList);
                    break;
                case SortBy.Opening:
                    PlaceListTemporal = sortOpening(PlaceList);
                    break;
                case SortBy.None:
                    PlaceListTemporal = PlaceList;
                    break;
                default:
                    PlaceListTemporal = PlaceList;
                    break;
            }
            return PlaceListTemporal;
        }
        public static List<Place> sortDistance(List<Place> PlaceList)
        {
            return PlaceList.OrderBy(resultPlace => Convert.ToDouble(resultPlace.Distance.Replace(" km", ""))).ToList();
        }

        public static List<Place> sortRating(List<Place> PlaceList)
        {
            PlaceList = PlaceList.OrderBy(resultPlace => resultPlace.Rating).ToList();
            PlaceList.Reverse(0, PlaceList.Count);
            return PlaceList;
        }

        public static List<Place> sortOpening(List<Place> PlaceList)
        {
            PlaceList = PlaceList.OrderBy(resultPlace => resultPlace.Horary).ToList();
            PlaceList.Reverse(0, PlaceList.Count);
            return PlaceList;
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
            return ((6376500.0 * num9)/1000);
        }

        private double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calculate distance in meters.
            distance = radius * c;
            return distance; // distance in meters
        }
    }
}
