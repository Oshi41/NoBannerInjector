using System;
using System.Threading;
using System.Windows.Forms;

namespace CheatLib
{
    public partial class Abyss : UserControl, IRefresh
    {
        public Abyss()
        {
            InitializeComponent();
            InjectorUtils.EnsureAsync(webView21);
            LanguageSwitcher.RegisterLanguageSwitcher(this);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RefreshControl();
        }

        public async void RefreshControl()
        {
            webView21.Source = new Uri("https://act.hoyolab.com/app/community-game-records-sea/index.html" +
                                       "?bbs_presentation_style=fullscreen&bbs_auth_required=true&v=330&gid=2" +
                                       "&utm_source=hoyolab&utm_medium=tools");
            InjectorUtils.ExecuteScriptAsync(webView21, "lang_switch.js",
                x => x.Replace("{0}", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()));
        }
    }
}