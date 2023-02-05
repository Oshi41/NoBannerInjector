using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace CheatLib
{
    public partial class Achievments : UserControl, IRefresh
    {
        public Achievments()
        {
            InitializeComponent();
            InjectorUtils.EnsureAsync(webView21);
            LanguageSwitcher.RegisterLanguageSwitcher(this);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView21.CoreWebView2.NavigationCompleted += async (sender, args) =>
            {
                var url = webView21.Source.ToString();
                if (url.Contains("genshin-center.com") && url.Contains("achievements"))
                {
                    await InjectorUtils.ExecuteScriptAsync(webView21, "hide_tabs.js");
                }
            };
            webView21.CoreWebView2.WebResourceResponseReceived += (sender, args) => { };
            RefreshControl();
        }

        public async void RefreshControl()
        {
            webView21.Source = new Uri("https://genshin-center.com/" +
                                       Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()
                                       + "/achievements");
            await InjectorUtils.ExecuteScriptAsync(webView21, "hide_tabs.js");
        }
    }
}