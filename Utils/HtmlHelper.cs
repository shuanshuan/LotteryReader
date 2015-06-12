using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace LotteryReader.Utils
{
    public class HtmlHelper
    {


        public static string HtmlRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = new WebProxy();
            WebResponse response = request.GetResponse();
            Stream cs = response.GetResponseStream();
            StreamReader sr = new StreamReader(cs);
            string content = sr.ReadToEnd();
            return content;
        }




    }
}
