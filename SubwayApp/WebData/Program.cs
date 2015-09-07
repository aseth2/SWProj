using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.IO;

namespace WebData
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebClient client = new WebClient();
            //client.Credentials = new NetworkCredential("287561ps", "2bSecure");
            //String downloadedString = client.DownloadString("https://liveiq.subway.com/Reporting/Sales/WeeklySales");
            //System.IO.File.WriteAllText(@"C:\Users\Public\WriteText.txt", downloadedString);
            Button5_Click();
        }
        
        public static string ExtractViewState(string s)
        {
            string viewStateNameDelimiter = "__VIEWSTATE";
            string valueDelimiter = "value=\"";

            int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
            int viewStateValuePosition = s.IndexOf(
                  valueDelimiter, viewStateNamePosition
               );

            int viewStateStartPosition = viewStateValuePosition +
                                         valueDelimiter.Length;
            int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);
            
            return HttpUtility.UrlEncodeUnicode(
                     s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                     )
                  );
        }

        public static void Button5_Click()
        {
            // first, request the login form to get the viewstate value
            HttpWebRequest webRequest = WebRequest.Create("https://partners.subway.com/portal/Default.aspx?tabid=58&returnurl=%2fportal%2fTabID%2f174%2fSSOREDIR%2fDefault.aspx%3fSSOURL%3d%252Fportal%252Fsaml.aspx%253Fappid%253Dliveiq%2526Target%253dhttp%25253a%25252f%25252fliveiq.subway.com%25252fReporting%25252fSales%25252fWeeklySales") as HttpWebRequest;
            StreamReader responseReader = new StreamReader(
                  webRequest.GetResponse().GetResponseStream()
               );
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            // extract the viewstate value and build out POST data
            string viewState = ExtractViewState(responseData);
            string postData =
                  String.Format(
                     "__VIEWSTATE={0}&UsernameTextBox={1}&PasswordTextBox={2}&LoginButton=Login",
                     viewState, "287561ps", "2bSecure"
                  );

            // have a cookie container ready to receive the forms auth cookie
            CookieContainer cookies = new CookieContainer();

            // now post to the login form
            webRequest = WebRequest.Create("https://partners.subway.com/portal/Default.aspx?tabid=58&returnurl=%2fportal%2fTabID%2f174%2fSSOREDIR%2fDefault.aspx%3fSSOURL%3d%252Fportal%252Fsaml.aspx%253Fappid%253Dliveiq%2526Target%253dhttp%25253a%25252f%25252fliveiq.subway.com%25252fReporting%25252fSales%25252fWeeklySales") as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = cookies;

            // write the form values into the request message
            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postData);
            requestWriter.Close();

            // we don't need the contents of the response, just the cookie it issues
            webRequest.GetResponse().Close();

            // now we can send out cookie along with a request for the protected page
            webRequest = WebRequest.Create("https://liveiq.subway.com/Reporting/Sales/WeeklySales") as HttpWebRequest;
            webRequest.CookieContainer = cookies;
            responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

            // and read the response
            responseData = responseReader.ReadToEnd();
            responseReader.Close();

            HttpContext.Current.Response.Write(responseData);
        }
    }
}
