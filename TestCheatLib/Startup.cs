using System;
using System.Windows.Forms;

namespace TestCheatLib
{
    public class Startup
    {
        [STAThread]
        public static int Main(string arg)
        {
            MessageBox.Show("Here");
            return 0;
        }
    }
}