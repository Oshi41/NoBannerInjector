using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CheatLib.Extensions;
using CheatLib.Model;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CheatLib.Controls;

public partial class AuthControl : UserControl
{
    private DateTime _lastCall = DateTime.MaxValue;
    private TimeSpan _await = TimeSpan.FromSeconds(3);

    public AuthControl()
    {
        InitializeComponent();
        InitAsync();
    }

    #region DP

    public static readonly DependencyProperty IsAuthNeededProperty = DependencyProperty.Register(
        nameof(IsAuthNeeded), typeof(bool), typeof(AuthControl), new PropertyMetadata(true));

    public bool IsAuthNeeded
    {
        get { return (bool)GetValue(IsAuthNeededProperty); }
        set { SetValue(IsAuthNeededProperty, value); }
    }

    public static readonly DependencyProperty ShowWebProperty = DependencyProperty.Register(
        nameof(ShowWeb), typeof(bool), typeof(AuthControl), new PropertyMetadata(default(bool)));

    public bool ShowWeb
    {
        get { return (bool)GetValue(ShowWebProperty); }
        set { SetValue(ShowWebProperty, value); }
    }

    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
        nameof(Step), typeof(Steps), typeof(AuthControl), new PropertyMetadata(default(Steps)));

    public Steps Step
    {
        get { return (Steps)GetValue(StepProperty); }
        set { SetValue(StepProperty, value); }
    }

    #endregion

    private async void InitAsync()
    {
        await WebView.EnsureAsync();
        WebView.Source = new Uri(Cookies.HoyolabUrl);
        WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        WebView.CoreWebView2.WebResourceResponseReceived += OnReqReceived;
        WebView.CoreWebView2.NavigationCompleted += OnNextStep;

        OnNextStep(null, null);
        Loop();
    }

    private async void OnReqReceived(object sender, CoreWebView2WebResourceResponseReceivedEventArgs args)
    {
        try
        {
            if (!args.Response.Headers.GetHeader("content-type").Contains("json"))
                return;

            var stream = await args.Response.GetContentAsync();
            var str = await new StreamReader(stream).ReadToEndAsync();
            if (string.IsNullOrEmpty(str)) return;
            var json = JsonConvert.DeserializeObject<JObject>(str);
            var Url = args.Request.Uri;

            // get needed data
            if (Url.Contains("getGameRecordCard"))
            {
                var id_token = json.SelectToken("data.list[0].game_role_id");
                if (id_token != null)
                {
                    Cookies.UUID = id_token?.Value<string>();
                    Cookies.Nickname = json.SelectToken("data.list[0].nickname")?.Value<string>();
                    OnNextStep(null, null);
                }
            }

            // entered credentials
            if (Step == Steps.OnAuth && Url.Contains("user/full")
                                     && json.SelectToken("data.user_info.nickname") != null)
            {
                OnNextStep(null, null);
            }

            // check is signed
            if (Step == Steps.OnSign && Url.Contains("info?"))
            {
                if (json.SelectToken("data.is_sign")?.Value<bool>() == true)
                {
                    OnNextStep("already_signed", null);
                }
            }
        }
        catch (Exception e)
        {
            if (e is COMException ex)
            {
                Trace.WriteLine(ex);
            }
            else
            {
                Console.WriteLine("Error during json sniffing");
                Console.WriteLine(e);
            }
        }
    }

    private async void OnNextStep(object sender, EventArgs e)
    {
        _lastCall = DateTime.Now;
        switch (Step)
        {
            case Steps.ToAuth:
                Navigate(Cookies.HoyolabUrl);
                Step = Steps.OnAuth;
                return;

            case Steps.OnAuth:
                await WebView.LoadCookiesAsync(Cookies.HoyolabUrl);
                if (new[] { Cookies.LT_Token, Cookies.LT_UID }.Any(string.IsNullOrEmpty))
                    break;

                Navigate("https://www.hoyolab.com/accountCenter/postList?id=" + Cookies.HoyolabUid);
                Step = Steps.OnUserCard;
                return;

            case Steps.OnUserCard:
                if (string.IsNullOrEmpty(Cookies.UUID))
                    return;

                Navigate("https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481");
                Step = Steps.OnSign;
                return;

            case Steps.OnSign:
                switch (sender)
                {
                    case "already_signed":
                        Step = Steps.Success;
                        OnNextStep(null, null);
                        return;

                    default:
                        await WebView.ExecuteAsync("post_daily.js");
                        return;
                }

            case Steps.Success:
                IsAuthNeeded = false;
                return;
        }
    }

    private void Navigate(string url)
    {
        WebView.Source = new Uri(url);
    }

    private async Task Loop()
    {
        while (true)
        {
            if (DateTime.Now > _lastCall + _await)
            {
                ShowWeb = true;
                return;
            }

            await Task.Delay(500);
        }
    }
}

public enum Steps
{
    ToAuth,
    OnAuth,

    OnUserCard,
    OnSign,

    Success,
}