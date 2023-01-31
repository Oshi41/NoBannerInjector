using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Web.WebView2.Core;

namespace example
{
    public static class AuthServiceExtensions
    {
        private static List<CoreWebView2Cookie> _cookies = new List<CoreWebView2Cookie>();
        public static string UUID => _cookies.FirstOrDefault(x => x.Name == "account_id")?.Value;
        public static string LT_Uid => _cookies.FirstOrDefault(x => x.Name == "ltuid")?.Value;
        public static string LT_Token => _cookies.FirstOrDefault(x => x.Name == "ltoken")?.Value;
        public static bool HasAuth => new[] { UUID, LT_Uid, LT_Token }.All(x => !string.IsNullOrEmpty(x));

        public const string AuthDomain = "https://www.hoyolab.com";

        public static CoreWebView2CookieManager With(this CoreWebView2CookieManager manager)
        {
            _cookies.ForEach(manager.AddOrUpdateCookie);
            return manager;
        }

        public static void Save(this CoreWebView2CookieManager manager)
        {
            manager?.GetCookiesAsync(AuthDomain)?.ContinueWith(task =>
            {
                _cookies.Clear();
                _cookies.AddRange(task.Result);
            });
        }
    }
}