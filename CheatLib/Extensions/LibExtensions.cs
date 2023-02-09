using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CheatLib.Model;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace CheatLib.Extensions;

public static class LibExtensions
{
    public static string Relative(params string[] path)
    {
        var args = path.ToList();
        args.Insert(0, "..");
        args.Insert(0, Assembly.GetExecutingAssembly().Location);
        return Path.Combine(args.ToArray());
    }

    public static async Task EnsureAsync(this WebView2 view2)
    {
        view2.CreationProperties = new CoreWebView2CreationProperties
        {
            Language = Thread.CurrentThread.CurrentUICulture.Name
        };
        var env = await CoreWebView2Environment.CreateAsync(userDataFolder: Relative("data", "browser"));
        await view2.EnsureCoreWebView2Async(env);
    }

    public static async Task LoadCookiesAsync(this WebView2 view2, string url)
    {
        var cookies = await view2.CoreWebView2.CookieManager.GetCookiesAsync(url);
        var uri = new Uri(url);
        var container = new CookieContainer();
        container.SetCookies(uri, Cookies.Application_GetCookies(uri) ?? "");
        foreach (var cookie in cookies.Select(x => x.ToSystemNetCookie()))
            container.Add(uri, cookie);
        Cookies.Application_SetCookies(uri, container);
    }

    /// <param name="view2"></param>
    /// <param name="scriptName">Name of script+ext inside Scripts folder</param>
    /// <returns></returns>
    public static async Task<bool> ExecuteAsync(this WebView2 view2, string scriptName,
        Func<string, string> transformer = null)
    {
        if (view2.CoreWebView2 == null)
            return false;

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{nameof(CheatLib)}.Scripts.{scriptName}");
        try
        {
            using var reader = new StreamReader(stream);
            var script = await reader.ReadToEndAsync();
            if (transformer != null)
                script = transformer(script);
            await view2.ExecuteScriptAsync(script);
            return true;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return false;
        }
    }

    public static string ScriptText(string scriptName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream($"{nameof(CheatLib)}.Scripts.{scriptName}");
        try
        {
            using var reader = new StreamReader(stream);
            var script = reader.ReadToEnd();
            return script;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return "";
        }
    }
}