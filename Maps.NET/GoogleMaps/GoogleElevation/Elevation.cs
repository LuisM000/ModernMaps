using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maps.NET.GoogleMaps.GoogleElevation
{
    class Elevation
    {
        public List<Elevat> resultElevation;
        public event EventHandler FinishedReading;
        private WebClient Query;

        private const string basicURL = "http://maps.googleapis.com/maps/api/elevation/xml?";
        private const string finalURL = "&sensor=true";

        public void getElevation(List<Location> coordinates)
        {
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string locations="";
            foreach (var item in coordinates)
            {
                locations += item.Latitude.ToString("0.0000").Replace(",", ".") + "," + item.Longitude.ToString("0.0000").Replace(",", ".") + "|";
            }
            locations=locations.Substring(0, locations.Length - 1);
            string url = basicURL + "locations=" + locations + finalURL;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("elevation (coordinates)", url);
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
                    resultElevation = new List<Elevat>();
                    resultElevation = (from text in document.Descendants("result")
                                              select new Elevat
                                              {
                                                  Elevation=text.Element("elevation").Value,
												  Resolution=text.Element("resolution").Value,
												  Latitude= text.Element("location").Element("lat").Value,
                                                  Longitude = text.Element("location").Element("lng").Value
                                              }).ToList();

                    FinishedReading(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }


    }
}
