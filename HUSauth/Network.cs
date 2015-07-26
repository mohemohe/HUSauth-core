using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HUSauth
{
    class Network
    {
        public static bool IsAvailable(bool isVerboseOutput)
        {
            bool result = false;

            using (var ping = new Ping())
            {
                if (isVerboseOutput) Console.WriteLine(@"ping to randgrid.ghippos.net...");

                for (int i = 1; i <= 5; i++)
                {
                    if (isVerboseOutput) Console.Write(i + ": ");
                    var reply = ping.Send(@"randgrid.ghippos.net", 500);

                    if (reply?.Status == IPStatus.Success)
                    {
                        if (isVerboseOutput) Console.WriteLine(@"receive pong");
                        result = true;
                        break;
                    }
                    else
                    {
                        if (isVerboseOutput) Console.WriteLine(@"error");
                    }
                }
            }

            return result;
        }

        public static bool Authentication(string id, string password, bool isVerboseOutput)
        {
            var nvc = new NameValueCollection
            {
                {"user_id", id},
                {"pass", password},
                {"url", "http://randgrid.ghippos.net/check.html"},
                {"lang", "ja"},
                {"event", "1"},
            };

            string resText = null;

            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; ASU2JS; rv:11.0) like Gecko | HUSauth");

                string authServer = @"http://gonet.localhost/cgi-bin/guide.cgi";
                if (isVerboseOutput) Console.WriteLine(@"authenticate to " + authServer);

                for(var i = 1; i <= 5; i++) 
                {
                    if (isVerboseOutput) Console.WriteLine(@"trying to authentication...");
                    try
                    {
                        var resData = wc.UploadValues(authServer, nvc);
                        resText = Encoding.UTF8.GetString(resData?.ToArray());
                        break;
                    }
                    catch (WebException)
                    {
                        if (isVerboseOutput) Console.WriteLine(@"authentication server is not responding");
                    }
                    catch (Exception)
                    {
                        if (isVerboseOutput) Console.WriteLine(@"failed to an unknown error");
                    }                    
                }
            }

            if (string.IsNullOrEmpty(resText))
            {
                if (isVerboseOutput) Console.WriteLine(@"incorrect response from the authentication server");
                return false;
            }

            return true;
        }
    }
}
