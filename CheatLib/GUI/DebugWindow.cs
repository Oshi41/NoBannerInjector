using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using IronPython.Modules;
using Memory;

namespace CheatLib
{
    public partial class DebugWindow : UserControl
    {
        private IntPtr _module;
        private IntPtr _function;

        public DebugWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.Parent?.Parent is TabControl control)
            {
                var index = control.SelectedIndex;
                control.SelectedIndexChanged += (sender, args) =>
                {
                    if (index == control.SelectedIndex)
                        Refresh();
                };
            }
        }

        public void Refresh()
        {
            comboBox1.Items.Clear();

            var set = new Dictionary<string, ProcessModule>();

            var currentProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in currentProcess.Modules)
            {
                if (!set.ContainsKey(module.ModuleName))
                    set.Add(module.ModuleName, module);
            }

            comboBox1.Items.AddRange(set.Values.OrderBy(x => x.ModuleName).Distinct().ToArray());
        }

        private void SearchForModule(object sender = null, EventArgs e = null)
        {
            var module = comboBox1.SelectedItem as ProcessModule;
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchForModule();
            }
        }

        #region Win32

        [Flags]
        enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("psapi.dll")]
        static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("psapi")]
        private static extern bool EnumProcesses(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In] [Out]
            IntPtr[] processIds,
            UInt32 arraySizeBytes, [MarshalAs(UnmanagedType.U4)] out UInt32 bytesCopied);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, IntPtr dwProcessId);


        static string PrintProcessName(IntPtr process)
        {
            var sName = "";
            var bFound = false;
            var hProcess = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VMRead,
                false, process);
            if (hProcess != IntPtr.Zero)
            {
                var szProcessName = new StringBuilder(260);
                var hMod = IntPtr.Zero;
                uint cbNeeded = 0;
                Imps.EnumProcessModules(hProcess, hMod, (uint)Marshal.SizeOf(typeof(IntPtr)), out cbNeeded);
                if (GetModuleBaseName(hProcess, hMod, szProcessName, szProcessName.Capacity) > 0)
                {
                    sName = szProcessName.ToString();
                    bFound = true;
                }

                // Close the process handle
                Imps.CloseHandle(hProcess);
            }

            if (!bFound)
            {
                sName = "<unknown>";
            }

            return sName;
        }

        public static void Testy()
        {
            UInt32 arraySize = 9000;
            UInt32 arrayBytesSize = arraySize * sizeof(UInt32);
            IntPtr[] processIds = new IntPtr[arraySize];
            UInt32 bytesCopied;

            bool success = EnumProcesses(processIds, arrayBytesSize, out bytesCopied);

            Console.WriteLine("success={0}", success);
            Console.WriteLine("bytesCopied={0}", bytesCopied);

            if (!success)
            {
                Console.WriteLine("Boo!");
                return;
            }

            if (0 == bytesCopied)
            {
                Console.WriteLine("Nobody home!");
                return;
            }

            UInt32 numIdsCopied = bytesCopied >> 2;
            ;

            if (0 != (bytesCopied & 3))
            {
                UInt32 partialDwordBytes = bytesCopied & 3;

                Console.WriteLine(
                    "EnumProcesses copied {0} and {1}/4th DWORDS...  Please ask it for the other {2}/4th DWORD",
                    numIdsCopied, partialDwordBytes, 4 - partialDwordBytes);
                return;
            }

            for (UInt32 index = 0; index < numIdsCopied; index++)
            {
                string sName = PrintProcessName(processIds[index]);
                IntPtr PID = processIds[index];
                Console.WriteLine("Name '" + sName + "' PID '" + PID + "'");
            }
        }

        #endregion
    }
}