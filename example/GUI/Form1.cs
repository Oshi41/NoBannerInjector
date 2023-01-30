using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HookManager;
using MaterialSkin;
using MaterialSkin.Controls;

namespace example
{
    public partial class Form1 : MaterialForm
    {
        private readonly SystemHotKey _manager;

        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            _manager = new SystemHotKey(this.Handle);
            _manager.AddHotKey(0, 0, Keys.F1, () =>
            {
                var opactity = this.Opacity > 0 ? 0 : 100;
                this.Opacity = opactity;
                if (opactity > 0)
                {
                    this.Focus();
                    this.TopLevel = true;
                }
            });
            
            
            
        }

        private void materialComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}