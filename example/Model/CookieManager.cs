using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Community.CsharpSqlite;
using GenshinInfo.Managers;
using GenshinInfo.Services;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace example
{
    public class CookieManager
    {
        public static CookieManager INSTANCE = new CookieManager();

        private CookieContainer _cookies = new CookieContainer();
        private string _url = "https://www.hoyolab.com";
        private string _filePath = "cookies.txt";
        private string _authUrlCheck = "https://bbs-api-os.hoyolab.com/community/notification/wapi/getUserUnreadCount";
        private GenshinInfoManager _genshinInfoManager;

        private CookieManager()
        {
            Load();
            Application.ApplicationExit += (sender, args) => Save();
        }

        #region Properties

        public string LT_Uid => Find("ltuid")?.Value;
        public string LT_Token => Find("ltoken")?.Value;
        public string HoyolabUid => Find("account_id")?.Value;

        public string Nickname
        {
            get => Find("_nickname")?.Value;
            set => _cookies.Add(new Uri(_url), new Cookie("_nickname", value));
        }

        public string UUID
        {
            get => Find("_genshin_account_id")?.Value;
            set => _cookies.Add(new Uri(_url), new Cookie("_genshin_account_id", value));
        }

        public string AuthUrl => _url;
        public string CookieHeader => _cookies.GetCookieHeader(new Uri(_url));

        public GenshinInfoManager GenshinInfoManager
        {
            get
            {
                var present = new[] { UUID, LT_Uid, LT_Token }.Any(string.IsNullOrEmpty);
                var exist = _genshinInfoManager != null;
                if (present != exist)
                {
                    _genshinInfoManager = present
                        ? new GenshinInfoManager(UUID, LT_Uid, LT_Token)
                        : null;
                }

                return _genshinInfoManager;
            }
            private set => _genshinInfoManager = value;
        }

        #endregion

        public Cookie Find(string name)
        {
            return _cookies.GetCookies(new Uri(_url)).OfType<Cookie>()
                .FirstOrDefault(x => !x.Expired && x.Name == name);
        }

        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                File.CreateText(_filePath).Close();
                return;
            }

            foreach (var s in File.ReadAllText(_filePath).Split(';'))
                _cookies.SetCookies(new Uri(_url), s);
        }

        public void LoadTo(CoreWebView2CookieManager from)
        {
            foreach (Cookie cookie in _cookies.GetCookies(new Uri(_url)))
            {
                var copy = from.CreateCookieWithSystemNetCookie(cookie);
                copy.Expires = cookie.Expires;
                copy.IsHttpOnly = cookie.HttpOnly;
                copy.IsSecure = cookie.Secure;
                from.AddOrUpdateCookie(copy);
            }
        }

        public void Save()
        {
            var cookieString = _cookies.GetCookieHeader(new Uri(_url));
            File.WriteAllText(_filePath, cookieString);
        }

        public void SaveFrom(List<CoreWebView2Cookie> from)
        {
            foreach (var cookie in from)
            {
                var copy = new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain)
                {
                    Secure = cookie.IsSecure,
                    Expires = cookie.Expires,
                    HttpOnly = cookie.IsHttpOnly
                };
                _cookies.Add(copy);
            }
        }

        public async Task<bool> CheckAuthenticationAsync()
        {
            if (new[] { LT_Uid, LT_Token }.Any(string.IsNullOrEmpty))
            {
                GenshinInfoManager = null;
                return false;
            }

            if (string.IsNullOrEmpty(UUID) && !string.IsNullOrEmpty(HoyolabUid))
            {
                dynamic json = null;
                try
                {
                    var url = "https://bbs-api-os.hoyolab.com/game_record/card/wapi/getGameRecordCard?uid=" +
                              HoyolabUid;
                    var resp = await RequestExtensions.GetApi(url);
                    json = JsonConvert.DeserializeObject<dynamic>(await resp.Content.ReadAsStringAsync());
                    var uuid = json.data.list[0].game_role_id.ToString();
                    if (string.IsNullOrEmpty(uuid))
                        return false;
                    _cookies.Add(new Cookie("_genshin_account_id", uuid.ToString(), "/", _url));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Resp: " + json?.ToString());
                    return false;
                }
            }
            else
            {
                try
                {
                    return await this.GenshinInfoManager.CheckLogin();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return false;
                }
            }


            return true;
        }

        public void AddCookie(string name, string val)
        {
            _cookies.Add(new Cookie(name, val, "/", _url));
        }
    }
}