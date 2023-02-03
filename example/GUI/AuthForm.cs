﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace example
{
    public partial class AuthForm : Form
    {
        /// <summary>
        /// possible stages
        /// 0 - navigating to auth url
        /// 1 - retrieving cookies and to personal user card
        /// 2 - retrieving Genshin UUID and navigating to Daily check-in
        /// 3 - injecting script to sign-up automatically
        /// 4 - closing window 
        /// </summary>
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

        private void InitCoreView(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var manager = CookieManager.INSTANCE;
            manager.LoadTo(webView21.CoreWebView2.CookieManager);
            webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            OnNextStep(null, null);
            webView21.CoreWebView2.NavigationCompleted += OnNextStep;
            // webView21.CoreWebView2.DOMContentLoaded += OnNextStep;
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
                                manager.UUID = jObject.SelectToken("data.list[0].game_role_id")?.Value<string>();
                                manager.Nickname = jObject.SelectToken("data.list[0].nickname")?.Value<string>();
                                OnNextStep(null, null);
                            }

                            if (stage >= 3 && args.Request.Uri.Contains("info?"))
                            {
                                manager.IsDailySigned = jObject.SelectToken("data.is_sign")?.Value<bool>() == true;
                                if (manager.IsDailySigned)
                                {
                                    stage = 4;
                                    OnNextStep(null, null);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            };
        }

        private async void OnNextStep(object sender = null, EventArgs args = null)
        {
            var manager = CookieManager.INSTANCE;
            switch (stage)
            {
                case 0:
                    webView21.CoreWebView2.Navigate(manager.AuthUrl);
                    stage = 1;
                    break;

                case 1:
                    var cookies = await webView21.CoreWebView2.CookieManager.GetCookiesAsync("");
                    if (!cookies.Any())
                        break;

                    manager.SaveFrom(cookies);

                    webView21.Source =
                        new Uri("https://www.hoyolab.com/accountCenter/postList?id=" + manager.HoyolabUid);
                    stage = 2;
                    break;

                case 2:
                    if (string.IsNullOrEmpty(manager.UUID))
                        break;

                    stage = 3;
                    webView21.Source =
                        new Uri("https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481");
                    break;

                case 3:
                    if (manager.IsDailySigned)
                    {
                        stage = 4;
                        break;
                    }

                    var assembly = Assembly.GetExecutingAssembly();
                    var resource = nameof(example) + ".Scripts.post_daily.js";
                    using (var stream = assembly.GetManifestResourceStream(resource))
                    using (var reader = new StreamReader(stream))
                    {
                        var script = await reader.ReadToEndAsync();
                        try
                        {
                            var result = await webView21.CoreWebView2.ExecuteScriptWithResultAsync(script);
                            _ = result.Exception;
                            Console.WriteLine($"Daily script result: {result}");
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine(e);
                        }
                    }

                    break;

                case 4:
                    Console.WriteLine("Daily signed");
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }
    }
}