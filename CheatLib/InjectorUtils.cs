using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace CheatLib
{
    public static class InjectorUtils
    {
        public static async Task<string> ReadFile(string path, Func<Task<string>> onFileCreate = null)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                if (!File.Exists(path))
                {
                    var txt = "";
                    if (onFileCreate != null)
                        txt = await onFileCreate.Invoke();

                    File.WriteAllText(path, txt);
                }

                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
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

        public static string Relative(params string[] path)
        {
            var args = path.ToList();
            args.Insert(0, "..");
            args.Insert(0, Assembly.GetExecutingAssembly().Location);
            return Path.Combine(args.ToArray());
        }

        /// <param name="view2"></param>
        /// <param name="scriptName">Name of script+ext inside Scripts folder</param>
        /// <returns></returns>
        public static async Task<bool> ExecuteScriptAsync(this WebView2 view2, string scriptName,
            Func<string, string> transformer = null)
        {
            if (view2.CoreWebView2 == null)
                return false;
            
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"{nameof(CheatLib)}.Scripts.{scriptName}"))
            using (var reader = new StreamReader(stream))
            {
                var script = await reader.ReadToEndAsync();
                try
                {
                    if (transformer != null)
                        script = transformer(script);
                    var result = await view2.CoreWebView2.ExecuteScriptAsync(script);
                    return true;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    return false;
                }
            }
        }
        
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, 
            Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while(stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach(var child in childSelector(next))
                    stack.Push(child);
            }
        }
    }
}