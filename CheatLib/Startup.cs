using System;
using System.Windows.Forms;
using CheatLib.GUI;

namespace CheatLib
{
    public class Startup
    {
        [STAThread]
        public static int Main(string arg)
        {
            Application.Run(new Main());
            return 0;
        }
    }
}