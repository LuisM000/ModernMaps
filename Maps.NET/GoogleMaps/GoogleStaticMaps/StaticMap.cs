using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maps.NET.GoogleMaps.GoogleStaticMaps
{   
    class StaticMap
    {
        public static Size SizeMap { get; set; }
        public static Scale ScaleMap { get; set; }
        public static Format FormatMap { get; set; }
        public static MapType Maptype { get; set; }

        public enum Scale { NORMAL=1,ADVANCED=2 };
        public enum Format { PNG, PNG32, GIF, JPG, JPG_BASELINE };
        public enum MapType {ROADMAP,SATELLITE,TERRAIN,HYBRID };

        private const string basicURL = "https://maps.googleapis.com/maps/api/staticmap?sensor=false";
        private const string style1 = "&style=feature:road.highway%7Celement:geometry%7Chue:0xff0022%7Csaturation:60%7Clightness:-20&style=feature:road.arterial%7Celement:geometry%7Chue:0x2200ff%7Clightness:-40:visibility:simplified%7Csaturation:30&style=feature:road.local%7Chue:0xf6ff00%7Csaturation:60%7Cgamma:0.7%7Cvisibility:simplified&style=feature:water%7Celement:geometry%7Csaturation:40%7Clightness:40&style=road.highway%7Celement:labels%7Cvisibility:on%7Csaturation:98&style=feature:administrative.locality%7Celement:labels%7Chue:0x0022ff%7Csaturation:50%7Clightness:-10%7Cgamma:0.9&style=feature:transit.line%7Celement:geometry%7Chue:0xff0000%7Cvisibility:on%7Clightness:-70";
        private const string simpleMarker = "&markers=color:red%7C";
        
        public string getBasicMap(string center,int zoom=17,bool marker=true)
        {
            string markerMap = (marker) ? simpleMarker + center : "";
            string url = basicURL + "&center=" + center + "&zoom=" + zoom + getBasicParameters() + markerMap;
            GoogleStaticInfo.setRequest("static map (basic)",url);
            return url;
            
        }
        public string getBasicMap(Location center, int zoom = 17,bool marker=true)
        {
            string markerMap = (marker) ? simpleMarker + locationToString(center) : "";
            string url = basicURL + "&center=" + locationToString(center) + "&zoom=" + zoom + getBasicParameters() + markerMap;
            GoogleStaticInfo.setRequest("static map (basic)", url);
            return url;
        }
        public string getRouteMap(string encodePolyline,Location startPoint,Location EndPoint,Size size)
        {
            string url = basicURL + sizeToString(size) + "&path=color:0x0000ffff|weight:5%7Cenc:" + encodePolyline
                + "&markers=color:green%7Clabel:S%7C" + locationToString(startPoint) + "&markers=color:red%7Clabel:F%7C" + locationToString(EndPoint);
            GoogleStaticInfo.setRequest("static map (Route)", url);
            return url;
        }

        public string getMap(string center, int zoom, List<Marker> markers=null,Route routes=null,
            List<string> visibilityZones=null,List<Style> styles=null)
        {
            string markersString = Marker.getMarkers(markers);
            string routesString = Route.getRoute(routes);
            string visibilityZonesString = getVisibilityZones(visibilityZones);
            string stylesString = Style.getStyle(styles);
            string centerString =(visibilityZonesString=="")? "&center=" + center:"";
            string zoomString = (visibilityZonesString=="")? "&zoom=" + zoom:"";
            string url = basicURL + centerString + zoomString + visibilityZonesString + getBasicParameters() + markersString + routesString + stylesString;
            GoogleStaticInfo.setRequest("static map (advanced)", url);
            return url;
        }

        private string getBasicParameters()
        {
            return sizeToString(SizeMap) +   getScale() + "&format=" + FormatMap.ToString().ToLower().Replace("_","-") + "&maptype=" + Maptype.ToString().ToLower();
        }
      
        private string sizeToString(Size size)
        {
            Size sizeCheck=checkSize(size);
            return "&size=" + sizeCheck.Width + "x" + sizeCheck.Height;
        }
        private string getScale()
        {
            int scale = (ScaleMap == Scale.NORMAL || ScaleMap==0) ? 1 : 2;
            return "&scale=" + scale;
        }
        private Size checkSize(Size size)
        {
            Size sizeReturn = new Size();
            sizeReturn.Width = (size.Width <= 0) ? 512 : size.Width;
            sizeReturn.Height = (size.Height <= 0) ? 512 : size.Height;
            return sizeReturn;
        }
        private string locationToString(Location location)
        {
            return location.Latitude.ToString() + "," + location.Longitude.ToString();
        }
        private string getVisibilityZones(List<string> zones)
        {
            string zonesReturn = "";
            if(zones!=null && zones.Count>0)
            {
                zonesReturn = "&visible=";
                foreach (var item in zones)
                {
                    zonesReturn += item + "|";
                }
            }
            return zonesReturn;
        }
    }
}
