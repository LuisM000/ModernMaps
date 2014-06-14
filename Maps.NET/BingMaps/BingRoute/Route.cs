using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maps.NET.BingMaps.BingRoute
{
    class Route
    {
        public static BingServices.TravelMode Mode { get; set; }
        public static BingServices.RouteOptimization RouteOptimization { get; set; }
        public static BingServices.TrafficUsage TrafficUsage { get; set; }
        public static BingServices.DistanceUnit DistanceUnit { get; set; }
        public List<Direction> listDirections { get; set; }

        public Route()
        {
        }

        /// <summary>
        /// Selected static properties
        /// </summary>
        /// <param name="mode">0=Driving ; 1=Walking</param>
        /// <param name="routeOptimization">0=Minimize time ; 1=Minimize distance</param>
        /// <param name="trafficUsage">0=None ; 1=Traffic usage based time ; 2=Traffic usage based time and route</param>
        /// <param name="distanceUnit">0=kilometers ; 1=miles</param>
        public Route(BingServices.TravelMode mode, BingServices.RouteOptimization routeOptimization, BingServices.TrafficUsage trafficUsage, BingServices.DistanceUnit distanceUnit)
        {
            Mode = mode; RouteOptimization = routeOptimization; TrafficUsage = trafficUsage; DistanceUnit = distanceUnit;
        }

        public async Task<BingServices.RouteResponse> getRoute(Location coordinatesStart, Location coordinatesFinish, string language="es")
        {
            using (BingServices.RouteServiceClient client = new BingServices.RouteServiceClient("CustomBinding_IRouteService"))
            {
                BingServices.RouteRequest request = new BingServices.RouteRequest();
                request.Culture = language;
                request.Credentials = new Credentials() { ApplicationId = "ArACHo6f4ItiZJM3pI4Nv3wIVAfBI_QMSWJhuid8hDyoX92PLoom745lNypmlJXP" };
                request.Waypoints = new BingServices.Waypoint[2];
                request.Waypoints[0] = ConvertResultToWayPoint(coordinatesStart);
                request.Waypoints[1] = ConvertResultToWayPoint(coordinatesFinish);
                request.Options = options();
                request.UserProfile = new BingServices.UserProfile();
                request.UserProfile.DistanceUnit = DistanceUnit;
                BingServices.RouteResult RouteResult = new BingServices.RouteResult();
                Task<BingServices.RouteResponse> taskRoute = getDirections(client, request);
                BingServices.RouteResponse resultRoute = await taskRoute;
                setDirections(resultRoute.Result);
                return resultRoute;
            }
        }

        private BingServices.RouteOptions options()
        {
            BingServices.RouteOptions options = new BingServices.RouteOptions();
            options.Mode = Mode;
            options.Optimization = RouteOptimization;
            if (TrafficUsage == BingServices.TrafficUsage.TrafficBasedRouteAndTime) { options.Optimization = BingServices.RouteOptimization.MinimizeTime; }
            options.TrafficUsage = TrafficUsage;
            options.RoutePathType = BingServices.RoutePathType.Points;
            return options;
        }

        private async Task<BingServices.RouteResponse> getDirections(BingServices.RouteServiceClient client, BingServices.RouteRequest request)
        {
            BingServices.RouteResponse resultAsync = await client.CalculateRouteAsync(request);
            return resultAsync;
        }

        private void setDirections(BingServices.RouteResult route)
        {
            listDirections = new List<Direction>();
            Direction direction;
            int i=0;
            foreach (BingServices.ItineraryItem item in route.Legs[0].Itinerary)
            {
                direction = new Direction();
                direction.Index = i.ToString();
                direction.Description = getRemoveTags(item.Text);
                direction.Latitude = item.Location.Latitude.ToString("0.0000");
                direction.Longitude = item.Location.Longitude.ToString("0.0000");
                direction.Location = new Location(item.Location.Latitude, item.Location.Longitude);
                direction.CompassDirection = item.CompassDirection;
                direction.CompassDirectionImage = compassImage(item.CompassDirection);
                direction.ManeuverType = item.ManeuverType.ToString();
                direction.Distance = item.Summary.Distance;
                direction.TimeSeconds = item.Summary.TimeInSeconds;
                direction.Time = TimeSpan.FromSeconds(item.Summary.TimeInSeconds);
                direction.ImageHints = (item.Hints.Length > 0) ? "/Assets/Route/Hint.png" : "/Assets/Route/Wrong.png";
                direction.ImageWarnings = (item.Warnings.Length > 0) ? "/Assets/Route/Warning.png" : "/Assets/Route/Wrong.png";
                direction.Warnings = getWarnings(item.Warnings);
                direction.Hints = getHints(item.Hints);
                listDirections.Add(direction);
                i++;
            }
        }
        private List<ItineraryWarning> getWarnings(BingServices.ItineraryItemWarning[] warnings)
        {
            List<ItineraryWarning> listWarnings = new List<ItineraryWarning>();
            ItineraryWarning warning;
            foreach (var item in warnings)
            {
                warning = new ItineraryWarning();
                warning.Severity = item.Severity.ToString();
                warning.Type = item.WarningType.ToString();
                warning.Description = getRemoveTags(item.Text);
                listWarnings.Add(warning);
            }
            return listWarnings;
        }
        private List<ItineraryHint> getHints(BingServices.ItineraryItemHint[] hints)
        {
            List<ItineraryHint> listHints = new List<ItineraryHint>();
            ItineraryHint hint;
            foreach (var item in hints)
            {
                hint = new ItineraryHint();
                hint.Type = item.HintType.ToString();
                hint.Description = getRemoveTags(item.Text);
                listHints.Add(hint);
            }
            return listHints;
        }
        private string compassImage(string textCompass)
        {
            string compassReturn="";
            switch (textCompass)
            {

                case "norte":
                    compassReturn = "/Assets/Route/Compass/North.png";
                    break;
                case "noreste":
                    compassReturn = "/Assets/Route/Compass/NorthEast.png";
                    break;
                case "este":
                    compassReturn = "/Assets/Route/Compass/East.png";
                    break;
                case "sureste":
                    compassReturn = "/Assets/Route/Compass/SouthEast.png";
                    break;
                case "sur":
                    compassReturn = "/Assets/Route/Compass/South.png";
                    break;
                case "suroeste":
                    compassReturn = "/Assets/Route/Compass/SouthWest.png";
                    break;
                case "oeste":
                    compassReturn = "/Assets/Route/Compass/West.png";
                    break;
                case "noroeste":
                    compassReturn = "/Assets/Route/Compass/NorthWest.png";
                    break; 
                default:
                    break;
            }
            return compassReturn;
        }
        private BingServices.Waypoint ConvertResultToWayPoint(Location result)
        {
            BingServices.Waypoint waypoint = new BingServices.Waypoint();
            BingServices.Location location = new BingServices.Location();
            location.Latitude = result.Latitude;
            location.Longitude = result.Longitude;
            waypoint.Location = location;
            return waypoint;
        }
        public static string getRemoveTags(string item)
        {
            item=item.Replace("><", "> <");
            Regex regex = new Regex("<(.|\n)*?>");
            item = regex.Replace(item, string.Empty);
            return item;
        }

     

       
    }
}
