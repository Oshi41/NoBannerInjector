using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CheatLib
{
    public static class Utils
    {
        public static bool CreateFile(string path)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.CreateText(path).Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static async Task EnsureAsync(this WebView2 view2)
        {
            var env = await CoreWebView2Environment.CreateAsync(userDataFolder: Relative("data", "browser"));
            await view2.EnsureCoreWebView2Async(env);
        }

        public static string Relative(params string[] path)
        {
            var args = path.ToList();
            args.Insert(0, "..");
            args.Insert(0, Assembly.GetExecutingAssembly().Location);
            return Path.Combine(args.ToArray());
        }
    }
}