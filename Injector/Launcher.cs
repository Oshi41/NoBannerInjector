using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Injector
{
    public class Launcher
    {
        private static JObject _settings;
        private static JObject _web;
        private static Process _process;
        private static Mem _memory = new Mem();

        [STAThread]
        public static void Main(string[] args)
        {
            // Run as Administrator
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = true,
                    Verb = "runas"
                });
                return;
            }

            // Loading web config from GitHub
            try
            {
                string data;
#if DEBUG
                data = File.ReadAllText("web.json");
#else
                var url = "https://github.com/Oshi41/Injector/blob/master/Injector/conf.json";
                var request = WebRequest.Create(url);
                request.Method = "GET";

                using (var webResponse = request.GetResponse())
                {
                    using (var webStream = webResponse.GetResponseStream())
                    {
                        using (var reader = new StreamReader(webStream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }
                }
#endif
                _web = JsonConvert.DeserializeObject<JObject>(data);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _web = new JObject();
            }

            // Load main config
            try
            {
                _settings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("cfg.json"));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                File.CreateText("cfg.json").Close();
                _settings = new JObject();
            }

            // Open Genshin process
            var genshPath = _settings.Value<string>("genshin_path");
            if (!File.Exists(genshPath))
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "executable|*.exe",
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _settings["genshin_path"] = genshPath = dialog.FileName;
                    File.WriteAllText("cfg.json", JsonConvert.SerializeObject(_settings, Formatting.Indented));
                }
                else
                    return;
            }

            _process = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(genshPath),
                FileName = Path.Combine(genshPath),
                Arguments = _settings.Value<string>("genshin_arguments"),
            });

            Console.WriteLine("Genshin starting...");
            if (!DisableProtection())
            {
                _process?.Kill();
                return;
            }

            Thread.Sleep(1000);

            if (!InjectLibrary())
            {
                _process?.Kill();
                return;
            }

            Console.WriteLine("Library was injected");
            Console.ReadKey();
            _process?.Kill();
        }

        private static bool DisableProtection()
        {
            var handleName = _web.Value<string>("name");
            var handleExe = _settings.Value<string>("handle_path") ?? "libs/handle.exe";
            if (!File.Exists(handleExe))
            {
                Console.WriteLine("Can't find handle.exe");
                return false;
            }

            while (true)
            {
                var args = GetHandleArgs(handleExe, handleName);
                if (!args.Any())
                {
                    var sec = 1;
                    Console.WriteLine($"Can't find {handleName}, waiting {sec} sec");
                    Thread.Sleep(1000 * sec);
                }
                else
                {
                    var errors = CloseHandles(handleExe, args)
                        .Where(x => !x.EndsWith("Handle closed.\r\n")).ToList();
                    if (errors.Any())
                    {
                        Console.WriteLine(string.Join(Environment.NewLine, errors));
                    }
                    else
                    {
                        Console.WriteLine($"{handleName} was closed, happy hacking!");
                        return true;
                    }
                }
            }
        }

        private static bool InjectLibrary()
        {
            var dllName = _settings.Value<string>("launch.c++.dll") ?? "Bootstrap.dll";

            if (!_memory.OpenProcess(_process.Id, out var reason))
            {
                Console.WriteLine(reason);
                return false;
            }

            if (!_memory.InjectDll(Path.GetFullPath(dllName)))
            {
                Console.WriteLine("Can't inject library");
                return false;
            }

            return true;
        }

        private static List<string> GetHandleArgs(string handleExe, string handleName)
        {
            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = handleExe,
                Arguments = $"/accepteula -nobanner  -a -v {handleName}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
            var args = new List<string>();
            proc.WaitForExit();
            var readToEnd = proc.StandardOutput.ReadToEnd();
            readToEnd.Split('\n')
                .Select(x => x.Split(','))
                .Where(x => x.Length == 5 && int.TryParse(x[1], out _)).ToList().ForEach(x =>
                {
                    args.Add($"-p {x[1]} -c {x[3]} -y");
                });
            return args;
        }

        private static List<string> CloseHandles(string handleExe, List<string> args)
        {
            var res = new List<string>();
            foreach (var arguments in args)
            {
                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = handleExe,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                });
                proc.WaitForExit();
                res.Add(proc.StandardOutput.ReadToEnd());
            }

            return res;
        }
    }
}