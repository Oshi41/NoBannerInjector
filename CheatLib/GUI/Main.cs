using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CheatLib.GUI
{
    public partial class Main : MaterialForm
    {
        public Main()
        {
            InitializeComponent();
            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }
    }
}