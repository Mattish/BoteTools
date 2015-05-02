using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BoteCore.Network
{
    public static class ApplicationInfoRequest
    {
        private static readonly Uri ApplicationInfoUri = new Uri("somePath");
        private static readonly int TimeoutInMilliseconds = 5000;

        private async static Task<string> GetLatestApplicationInfoFromRepo()
        {
            try
            {
                var request = WebRequest.CreateHttp(ApplicationInfoUri);
                request.Timeout = TimeoutInMilliseconds;
                var response = await request.GetResponseAsync();
                if (response == null)
                    return null;
                var stream = response.GetResponseStream();
                string str = "";
                using (var br = new StreamReader(stream))
                {
                    str = br.ReadToEnd();
                }
                return str;

            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}