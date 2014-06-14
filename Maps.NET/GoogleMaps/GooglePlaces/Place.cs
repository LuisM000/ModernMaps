using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps.GooglePlaces
{
    class Place
    {
        public string Name { get; set; }
        public string Rating { get; set; }
        public string RatingDecimal { get; set; }
        public string RatingString { get; set; }
        public string Activity { get; set; }
        public string Horary { get; set; }
        public string HoraryIcon { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Vicinity { get; set; }
        public string PlaceReference { get; set; }
        public Location GeoCoordinates { get; set; }
        public string Distance { get; set; }
    }
    class Token
    {
        public string NextPage { get; set; }
    }
}
