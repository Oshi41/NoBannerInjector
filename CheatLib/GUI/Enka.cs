using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CheatLib
{
    public partial class Enka : UserControl
    {
        public Enka()
        {
            InitializeComponent();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await webView21.EnsureAsync();
            var uuid = CookieManager.INSTANCE.UUID;
            webView21.Source = new Uri("https://enka.network/u/" + uuid);
            groupBox1.Text = CookieManager.INSTANCE.Nickname + " UUID - " + uuid;
        }
    }
}