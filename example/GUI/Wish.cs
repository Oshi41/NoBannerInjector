using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenshinInfo.Managers;
using Microsoft.Web.WebView2.Core;

namespace example
{
    public partial class Wish : UserControl
    {
        const string _domain = "https://www.hoyolab.com";
        private readonly string[] _must_have = { "ltoken", "ltuid" };

        public Wish()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            webView21.Source = new Uri(_domain);
            webView21.ContentLoading += OnSourceChanged;
        }

        private async void OnSourceChanged(object sender, EventArgs e)
        {
            var haveAuth = await HaveAuth();
            webView21.Visible = haveAuth;

            if (!haveAuth)
                webView21.Source = new Uri(_domain);
        }

        private async Task<bool> HaveAuth()
        {
            var manager = webView21.CoreWebView2.CookieManager;
            var cookies = (await manager.GetCookiesAsync(_domain))
                .Select(x => x.Name).ToList();
            return _must_have.All(x => cookies.Contains(x));
        }
    }
}