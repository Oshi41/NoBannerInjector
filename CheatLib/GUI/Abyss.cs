using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CheatLib
{
    public partial class Abyss : UserControl
    {
        public Abyss()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetPage();
        }

        private Uri GetLink()
        {
            var link = "https://act.hoyolab.com/app/community-game-records-sea/m.html";
            var query = new Dictionary<string, string>
            {
                { "bbs_presentation_style", "fullscreen" },
                { "bbs_auth_required", "true" },
                { "gid", "2" },
                { "user_id", CookieManager.INSTANCE.HoyolabUid },
                { "utm_source", "hoyolab" },
                { "utm_medium", "gamecard" },
                { "bbs_theme_device", "1" },
                { "lang", Thread.CurrentThread.CurrentUICulture.ToString().ToLower() }
            };
            var queryString = string.Join("&", query.Select(x => $"{Uri.EscapeDataString(x.Key)}=${Uri.EscapeDataString(x.Value)}"));
            return new Uri($"{link}?{queryString}");
        }

        private async void SetPage()
        {
            await webView21.EnsureAsync();
            webView21.CoreWebView2.Navigate("https://act.hoyolab.com/app/community-game-records-sea/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&v=330&gid=2&utm_source=hoyolab&utm_medium=tools");
        }
    }
}