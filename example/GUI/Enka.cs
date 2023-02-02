using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace example
{
    public partial class Enka : UserControl
    {
        public Enka()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var uuid = CookieManager.INSTANCE.UUID;
            webView21.Source = new Uri("https://enka.network/u/" + uuid);
            groupBox1.Text = CookieManager.INSTANCE.Nickname + " UUID - " + uuid;
        }
    }
}