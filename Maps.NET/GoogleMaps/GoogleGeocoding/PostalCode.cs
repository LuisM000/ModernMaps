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
    class PostalCode
    {
        public PostalCod resultPostal;
        public event EventHandler FinishedReading;
        private WebClient Query;

        private const string basicURL = "https://maps.googleapis.com/maps/api/geocode/xml?";
        private const string finalURL = "&sensor=true";

        public void getPostalCode(string address, string region = "es", string language = "es")
        {
            Query = new WebClient();
            Query.OpenReadCompleted += Query_OpenReadCompleted;
            string url = basicURL + "address=" + address + "&region=" + region + "&language=" + language +
               finalURL;
            Query.OpenReadAsync(new Uri(url), UriKind.Absolute);
            GoogleStaticInfo.setRequest("postal code", url);
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
                    resultPostal = new PostalCod();
                    var code = (from text in document.Descendants("result").Elements("address_component")
                                   where text.Element("type").Value == "postal_code"
                                   select text).FirstOrDefault();
                    if(code!=null)
                    {
                        resultPostal.PostalCode = ((XElement)code.FirstNode).Value;
                        resultPostal.FormatedAddress = ((XElement)code.FirstNode.Parent.Parent).Element("formatted_address").Value;
                    }
                    FinishedReading(sender, e);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
