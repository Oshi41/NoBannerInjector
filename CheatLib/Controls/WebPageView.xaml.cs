using System;
using System.Windows;
using System.Windows.Controls;
using CheatLib.Extensions;

namespace CheatLib.Controls;

public partial class WebPageView : UserControl
{
    public WebPageView()
    {
        InitializeComponent();
        InitAsync();
    }

    private async void InitAsync()
    {
        await WebView2.EnsureAsync();
        Navigate(Address);
    }

    public static readonly DependencyProperty AddressProperty = DependencyProperty.Register(
        nameof(Address), typeof(string), typeof(WebPageView), new PropertyMetadata(default(string), OnAddressChanged));

    private static void OnAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebPageView view)
        {
            view.Navigate(e.NewValue.ToString());
        }
    }

    public string Address
    {
        get { return (string)GetValue(AddressProperty); }
        set
        {
            SetValue(AddressProperty, value);
            Navigate(value);
        }
    }

    public static readonly DependencyProperty ChangeLangScriptProperty = DependencyProperty.Register(
        nameof(ChangeLangScript), typeof(string), typeof(WebPageView),
        new PropertyMetadata(default(string), OnLangScriptChanged));

    private static void OnLangScriptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WebPageView view)
        {
            view.ChangeLang(e.NewValue.ToString());
        }
    }

    public string ChangeLangScript
    {
        get { return (string)GetValue(ChangeLangScriptProperty); }
        set
        {
            SetValue(ChangeLangScriptProperty, value);
            ChangeLang(value);
        }
    }

    private async void Navigate(string addr)
    {
        if (WebView2.CoreWebView2 == null)
            return;

        if (string.IsNullOrEmpty(addr))
            addr = "about:blank";
        WebView2.CoreWebView2.Navigate(addr);
        ChangeLang(ChangeLangScript);
    }

    private async void ChangeLang(string js)
    {
        if (WebView2.CoreWebView2 == null || string.IsNullOrEmpty(js))
            return;

        await WebView2.ExecuteScriptAsync(js);
    }
}