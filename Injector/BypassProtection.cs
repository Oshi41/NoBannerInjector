using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Injector
{
    public class BypassProtection
    {
        public static void Run(string handle = "libs/handle.exe", string name = "mhyprot2")
        {
            if (!File.Exists(handle))
                throw new Exception("handle.exe not founded");

            var args = new List<string>();
            using (new Logger($"handle lookup"))
            {
                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = handle,
                    Arguments = $"/accepteula -nobanner  -a -v {name}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                });
                proc.WaitForExit();
                var readToEnd = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(readToEnd);
                readToEnd.Split('\n')
                    .Select(x => x.Split(','))
                    .Where(x => x.Length == 5 && int.TryParse(x[1], out _)).ToList().ForEach(x =>
                    {
                        args.Add($"-p {x[1]} -a {x[3]} -y");
                    });
            }

            if (!args.Any())
                Console.WriteLine($"Didn't find the {name} handle");

            using (new Logger($"closing {args.Count} handles"))
            {
                foreach (var arguments in args)
                {
                    var proc = Process.Start(new ProcessStartInfo
                    {
                        FileName = handle,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    });
                    proc.WaitForExit();
                }
            }

            Console.WriteLine($"{name} is closed, happy hacking!");
        }

        private class Logger : IDisposable
        {
            private readonly string _msg;
            private readonly DateTime _time;

            public Logger(string msg)
            {
                _msg = msg;
                _time = DateTime.Now;
                Console.WriteLine($"starting {msg}...");
            }

            public void Dispose()
            {
                var seconds = (DateTime.Now - _time).TotalSeconds;
                Console.WriteLine($"ending {_msg}m took {seconds:F} seconds");
            }
        }
    }
}