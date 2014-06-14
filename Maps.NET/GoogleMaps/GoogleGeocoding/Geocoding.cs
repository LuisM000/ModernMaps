using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Maps.NET.GoogleMaps.GoogleGeocoding
{
    class Geocoding
    {
        public GoogleGeocoding.Geoco resultGeocoding;
        public event EventHandler FinishedReading;
        private WebClient Query;

        private const string basicURL = "https://maps.googleapis.com/maps/api/geocode/xml?";
        private const string finalURL = "&sensor=true";

        public void getGeocoding(string address, string region="es", string language="es")
        {
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "address=" + address + "&region=" + region + "&language=" + language +
               finalURL;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("direct geocoding", url);
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
                    resultGeocoding = new GoogleGeocoding.Geoco();
                    resultGeocoding = (from text in document.Descendants("result")
                                       select new GoogleGeocoding.Geoco
                                 {
                                     FormatedAddress = text.Element("formatted_address").Value,
                                     Latitude = text.Element("geometry").Element("location").Element("lat").Value,
                                     Longitude = text.Element("geometry").Element("location").Element("lng").Value
                                 }).First();

                    FinishedReading(sender, e);

                }
            }
            catch (Exception)
            {
            }
        }
    }
}
