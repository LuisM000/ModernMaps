using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps.GoogleRoute
{
    class Direction
    {
        public int Index { get; set; }
        public Location StartGeocoordinate { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public Location FinishGeocoordinate { get; set; }
        public string FinishLatitude { get; set; }
        public string FinishLongitude { get; set; }
        public string EncodePolyline { get; set; }
        public List<Location> DecodePolyline { get; set; }
        public string DurationSeconds { get; set; }
        public string DurationText { get; set; }
        public string DistanceMeters { get; set; }
        public string DistanceText { get; set; }
        public string Description { get; set; }
    }
}
