using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace NoBannerInjector
{
    internal class Launcher
    {
        private static Mem _memory;
        private static Process _genshinProc;
        private static Ini _legacyConf;
        private static string _libPath;
        private static JObject _settings;

        [STAThread]
        public static void Main(string[] args)
        {
            if (!IsAdministrator)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = true,
                    Verb = "runas"
                });
                return;
            }

            _memory = new Mem();
            InitLegacyConfig();
            Inject(args);
            LoadConfig();
            RemoveBanner();
        }

        private static bool IsAdministrator =>
            new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        private static void InitLegacyConfig()
        {
            var file = "./cfg.ini";
            if (!File.Exists(file))
            {
                File.CreateText(file).Close();
            }

            _legacyConf = new Ini(file);
            var path = _legacyConf.GetValue("GenshinPath", "Inject");
            if (!File.Exists(path))
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "*.exe|executable",
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _legacyConf.WriteValue("GenshinPath", "Inject", dialog.FileName);
                    _legacyConf.Save();
                }
            }

            path = _legacyConf.GetValue("InitializationDelayMS", "System");
            if (!int.TryParse(path, out var number))
            {
                _legacyConf.WriteValue("InitializationDelayMS", "System", "15000");
                _legacyConf.Save();
            }

            _legacyConf.Load();
        }

        private static void Inject(string[] args)
        {
            var file = _legacyConf.GetValue("GenshinPath", "Inject");
            var info = new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(file),
                FileName = file,
                Arguments = _legacyConf.GetValue("GenshinCommandLine", "Inject"),
            };
            _genshinProc = Process.Start(info);
            Console.WriteLine("Genshin starting...");
            Thread.Sleep(1000);
            if (!_memory.OpenProcess(_genshinProc.Id, out var reason))
            {
                throw new Exception("Error during connecting to Genshin process: " + reason);
            }

            var dll = args.ElementAtOrDefault(0) ?? "CLibrary.dll";
            _libPath = Path.Combine(dll);
            if (!_memory.InjectDll(_libPath))
            {
                throw new Exception("Can't inject DLL");
            }

            Thread.Sleep(2000);
            Console.WriteLine("Injected");
        }

        private static void LoadConfig()
        {
            string data;
#if DEBUG
            data = File.ReadAllText("conf.json");
#else
            var url = "https://github.com/Oshi41/NoBannerInjector/blob/master/NoBannerInjector/conf.json";
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
            _settings = JsonConvert.DeserializeObject<JObject>(data);
        }

        private static void RemoveBanner()
        {
            var pattern = _settings.Value<string>("pattern");
            while (true)
            {
                Thread.Sleep(500);
                var scan = _memory.AoBScan(pattern, true, true).Result?.Take(1).ToList();
                if (!scan.Any())
                    continue;
                _memory.WriteMemory(scan[0].ToString("X"), "int", "0");
                Console.WriteLine("Banner removed");
                return;
            }
        }
    }
}