using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace HUSauth
{
    class Network
    {
		private static string UserAgent { get; } = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; ASU2JS; rv:11.0) like Gecko | HUSauth";

        public static bool IsAvailable(bool isVerboseOutput)
        {
			string resText = null;

			using (var wc = new WebClient())
			{
				wc.Headers.Add("user-agent", UserAgent);

				const string checkServer = @"http://157.7.201.213/check.html";
				if (isVerboseOutput) Console.WriteLine("trying \"GET\" " + checkServer + " ...");

				for(var i = 1; i <= 5; i++) 
				{
					try
					{
						var resData = wc.OpenRead(checkServer);
						using(var sr = new StreamReader(resData))
						{
							resText = sr.ReadToEnd();
						}
						break;
					}
					catch (WebException)
					{
						if (isVerboseOutput) Console.WriteLine(@"server is not responding");
					}
					catch (Exception)
					{
						if (isVerboseOutput) Console.WriteLine(@"failed to an unknown error");
					}                    
				}
			}

			return resText.Contains(@"<title>CHECK</title>");
        }

        public static bool Authentication(string id, string password, bool isVerboseOutput)
        {
            var nvc = new NameValueCollection
            {
                {"user_id", id},
                {"pass", password},
                {"url", "http://www.wh2.fiberbit.net/canned/chp/index2.html"},
                {"lang", "ja"},
                {"event", "1"},
            };

            string resText = null;

            using (var wc = new WebClient())
            {
				wc.Headers.Add("user-agent", UserAgent);

				const string authServer = @"http://192.168.253.81/cgi-bin/guide.cgi";
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
                        if (isVerboseOutput) Console.WriteLine(@"server is not responding");
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
