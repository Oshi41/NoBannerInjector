using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace example
{
    public partial class AuthForm : Form
    {
        private int stage = 0;

        public AuthForm()
        {
            InitializeComponent();
            Load += async (sender, args) =>
            {
                await webView21.EnsureCoreWebView2Async();
                InitCoreView(null, null);
            };
        }

        private async void OnReload(object sender, EventArgs args)
        {
            switch (stage)
            {
                case 0:
                    webView21.CoreWebView2.Navigate(CookieManager.INSTANCE.AuthUrl);
                    stage = 1;
                    break;

                case 1:
                    var cookies = await webView21.CoreWebView2.CookieManager.GetCookiesAsync("");
                    if (!cookies.Any())
                        break;

                    var manager = CookieManager.INSTANCE;
                    manager.SaveFrom(cookies);

                    webView21.Source =
                        new Uri("https://www.hoyolab.com/accountCenter/postList?id=" + manager.HoyolabUid);
                    stage = 2;
                    break;

                case 2:
                    if (string.IsNullOrEmpty(CookieManager.INSTANCE.UUID))
                        break;
                    stage = 3;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }

        private void InitCoreView(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            CookieManager.INSTANCE.LoadTo(webView21.CoreWebView2.CookieManager);
            webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            OnReload(null, null);
            webView21.CoreWebView2.DOMContentLoaded += OnReload;
            webView21.CoreWebView2.WebResourceResponseReceived += async (o, args) =>
            {
                try
                {
                    var stream = await args.Response.GetContentAsync();
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var jObject = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                            if (args.Request.Uri.Contains("getGameRecordCard"))
                            {
                                CookieManager.INSTANCE.UUID = jObject.SelectToken("data.list[0].game_role_id").Value<string>();
                                CookieManager.INSTANCE.Nickname = jObject.SelectToken("data.list[0].nickname").Value<string>();
                                OnReload(null, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            };
        }
    }
}