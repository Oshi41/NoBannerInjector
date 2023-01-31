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
            webView21.Source = new Uri("https://enka.network/u/" + AuthServiceExtensions.UUID);
            groupBox1.Text = "Player UUID - " + AuthServiceExtensions.UUID;
        }
    }
}