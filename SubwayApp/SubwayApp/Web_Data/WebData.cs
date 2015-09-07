using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SubwayApp.Web_Data
{
    public class WebData
    {
        public static void main(String[] args)
        {
            WebClient client = new WebClient();
            String downloadedString = client.DownloadString("http://www.google.com");
            Console.Write(downloadedString);
        }
    }
}