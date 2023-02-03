using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CheatLib
{
    public static class RequestExtensions
    {
        public static async Task<HttpResponseMessage> GetApi(this string url)
        {
            using (var client = FillHeaders(new HttpClient()))
            {
                var resp = await client.GetAsync(url);
                return resp;
            }
        }

        public static HttpClient FillHeaders(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Cookie", CookieManager.INSTANCE.CookieHeader);
            client.DefaultRequestHeaders.Add("x-rpc-app_version", "1.5.0");
            client.DefaultRequestHeaders.Add("x-rpc-client_type", "4");
            client.DefaultRequestHeaders.Add("DS", GenerateDS());
            return client;
        }

        public static string GenerateDS()
        {
            long num1 = DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000L;
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(
                string.Format("salt={0}&t={1}&r={2}", (object)"6cqshh5dhw73bzxn20oexa9k516chk7s", (object)num1,
                    (object)"abcdef")));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num2 in hash)
                stringBuilder.Append(num2.ToString("x2"));
            return string.Format("{0},{1},{2}", (object)num1, (object)"abcdef", (object)stringBuilder);
        }
    }
}