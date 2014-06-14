using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps.GoogleRoute
{
    class GeneralDirections
    {
        public Location StartPoint { get; set; }
        public Location EndPoint { get; set; }
        public string Summary { get; set; }
        public string Copyright { get; set; }
        public string EncodeOverviewPolyline { get; set; }
        public List<Location> DecodeOverviewPolyline { get; set; }
        public TimeSpan Duration { get; set; }
        public long DistanceMeters { get; set; }
    }
}
