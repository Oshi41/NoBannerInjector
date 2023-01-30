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
            
            Reload();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var url = "https://enka.network/u/" + comboBox1.SelectedItem;
            webView21.Source = new Uri(url);
        }

        public void Reload()
        {
            var uuids = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "..", "LocalLow", "miHoYo", "Genshin Impact", "UidInfo.txt")
            }.Where(File.Exists).SelectMany(File.ReadAllLines).ToArray();
            this.comboBox1.Items.AddRange(uuids);
            
            if (uuids.Length > 0)
                comboBox1.SelectedIndex = 0;
        }
    }
}