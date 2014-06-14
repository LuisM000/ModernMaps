using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.BingMaps.BingRoute
{
    class Direction
    {
        public string Index { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CompassDirection { get; set; }
        public string CompassDirectionImage { get; set; }
        public string ManeuverType { get; set; }
        public double Distance { get; set; }
        public long TimeSeconds { get; set; }
        public TimeSpan Time { get; set; }

        public string ImageHints { get; set; }
        public List<ItineraryHint> Hints { get; set; }
        public string ImageWarnings { get; set; }
        public List<ItineraryWarning> Warnings { get; set; }
    }

    class ItineraryHint
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }
    class ItineraryWarning
    {
        public string Severity { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
