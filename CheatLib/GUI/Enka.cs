using System;
using System.Threading;
using System.Windows.Forms;

namespace CheatLib
{
    public partial class Enka : UserControl, IRefresh
    {
        public Enka()
        {
            InitializeComponent();
            LanguageSwitcher.RegisterLanguageSwitcher(this);
            InjectorUtils.EnsureAsync(webView21);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshControl();
        }

        public async void RefreshControl()
        {
            var uuid = CookieManager.INSTANCE.UUID;
            webView21.Source = new Uri("https://enka.network/u/" + uuid);
            groupBox1.Text = CookieManager.INSTANCE.Nickname + " UUID - " + uuid;
            InjectorUtils.ExecuteScriptAsync(webView21, "lang_switch.js",
                x => x.Replace("{0}", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()));
        }
    }
}