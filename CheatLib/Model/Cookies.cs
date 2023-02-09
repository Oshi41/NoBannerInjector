using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Policy;
using System.Windows;

namespace CheatLib.Model;

public class Cookies
{
    public const string HoyolabUrl = "https://www.hoyolab.com";
    private const string _other_domain = "https://www.not-hoyolab.com";

    public static string LT_UID
    {
        get => FindValue("ltuid");
        set => AddValue("ltuid", value);
    }

    public static string LT_Token
    {
        get => FindValue("ltoken");
        set => AddValue("ltoken", value);
    }

    public static string HoyolabUid
    {
        get => FindValue("account_id");
        set => AddValue("account_id", value);
    }

    public static string Nickname
    {
        get => FindValue("nickname", _other_domain);
        set => AddValue("nickname", value, _other_domain);
    }

    public static string UUID
    {
        get => FindValue("uuid", _other_domain);
        set => AddValue("uuid", value, _other_domain);
    }

    public static bool IsDailySigned
    {
        get => FindValue("is_signed", _other_domain) != "1";
        set => AddValue("is_signed", value ? "1" : "0", _other_domain, TimeSpan.FromHours(1));
    }

    private static string FindValue(string name, string domain = HoyolabUrl)
    {
        var uri = new Uri(domain);
        var cookies_header = Application_GetCookies(uri);
        var container = new CookieContainer();
        // https://stackoverflow.com/questions/32956693/cookiecontainer-setcookies-only-sets-the-first-one
        container.SetCookies(uri, cookies_header.Replace(";", ","));
        return container.GetCookies(uri).OfType<Cookie>().Where(x => !x.Expired).FirstOrDefault(x => x.Name == name)
            ?.Value;
    }

    private static void AddValue(string name, string value, string domain = HoyolabUrl, TimeSpan? expires = null)
    {
        var uri = new Uri(domain);
        var container = new CookieContainer();
        container.SetCookies(uri, Application_GetCookies(uri) ?? "");
        var cookie = new Cookie(name, value);
        if (expires != null)
            cookie.Expires = DateTime.Now + expires.Value;
        container.Add(uri, cookie);
        Application_SetCookies(uri, container);
    }

    public static string Application_GetCookies(Uri uri)
    {
        try
        {
            return Application.GetCookie(uri);
        }
        catch (Exception e)
        {
            Trace.WriteLine("No cookies present");
            Trace.WriteLine(e);
            return "";
        }
    }

    public static void Application_SetCookies(Uri uri, CookieContainer container)
    {
        try
        {
            foreach (var cookie in container.GetCookies(uri).OfType<Cookie>().Where(x => !x.Expired))
            {
                Application.SetCookie(uri, cookie.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot set cookies");
            Console.WriteLine(e);
        }
    }
}