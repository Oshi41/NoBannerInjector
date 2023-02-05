using System;
using System.Windows.Forms;

namespace CheatLib
{
    static class Startup
    {
        /// <summary>
        /// Enter point from DLL loading
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [STAThread]
        public static int Main(string arg)
        {
            Run();
            return 0;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Run()
        {
            LanguageSwitcher.LoadLanguage();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, args) =>
            {
                Console.Error.WriteLine(args.Exception);
                MessageBox.Show("Exception");
            };

            if (!CookieManager.INSTANCE.CheckAuthenticationAsync().Result)
            {
                var form = new AuthForm();
                var dialogResult = form.ShowDialog();
                if (dialogResult != DialogResult.OK && form.DialogResult != DialogResult.OK)
                    return;
            }

            Application.Run(new Form1());
        }
    }
}