using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.NET.GoogleMaps
{
    class Request
    {
        public DateTime Time { get; set; }
        public string Service { get; set; }
        public string Url { get; set; }
    }
    class GoogleStaticInfo
    {
        public static string Key { get; set; }
        public static List<Request> Request {get;private set;}

        public static void setRequest(string service,string url)
        {
            if (Request == null) { Request = new List<Request>(); }
            Request request = new Request();
            request.Service = service; request.Url = url; request.Time = DateTime.Now;
            Request.Add(request);

        }
    }
}
