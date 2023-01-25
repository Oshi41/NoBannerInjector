using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NoBannerInjector
{
    internal class LauncherProgram
    {
        public static void Main(string[] args)
        {
            
        }

        private static String FindInjectorPath()
        {
            var name = "injector.exe";
            if (File.Exists(name))
                return Path.Combine(name);
            name = "*.exe";
            var current = Assembly.GetEntryAssembly().Location;
            var file = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), name, SearchOption.TopDirectoryOnly)
                .FirstOrDefault(x => !current.EndsWith(x));
            return file == null ? throw new Exception("No injector founded") : Path.Combine(file);
        }
    }
}