using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Community.CsharpSqlite;
using HookManager;
using Memory;

namespace example
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Debug();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, args) => Console.Error.WriteLine(args.Exception);


            if (!CookieManager.INSTANCE.CheckAuthenticationAsync().Result)
            {
                var form = new AuthForm();
                var dialogResult = form.ShowDialog();
                if (dialogResult != DialogResult.OK && form.DialogResult != DialogResult.OK)
                    return;
            }

            Application.Run(new Form1());
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        private delegate dynamic Delegate();

        [Conditional("RELEASE")]
        static void Debug()
        {
            var ptr = GetFunc("mscorlib.dll", "System.Console.WriteLine");
        }

        private static Mem _mem = new Mem();

        private static IntPtr GetFunc(string module, string name)
        {
            if (_mem.mProc != null)
            {
                var lib =
                    @"C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact_Data\Native\UserAssembly.dll";
                if (!_mem.OpenProcess(Process.GetCurrentProcess().Id))
                    return IntPtr.Zero;
                if (!_mem.InjectDll(lib))
                    return IntPtr.Zero;
            }

            var modulePtr = Imps.GetModuleHandle(module);
            if (modulePtr == IntPtr.Zero)
                return IntPtr.Zero;
            if (!string.IsNullOrEmpty(name))
            {
                var procAddress = GetProcAddress(modulePtr, name);
                return procAddress;
            }

            return IntPtr.Zero;
        }
    }
}