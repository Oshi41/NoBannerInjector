using System;
using System.Runtime.InteropServices;

namespace CheatLib
{
    public class PInvoke
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}