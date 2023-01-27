using System;
using System.Runtime.InteropServices;
using System.Threading;
using RGiesecke.DllExport;

namespace CheatLib
{
    public static class Program
    {
        const int DLL_PROCESS_ATTACH = 0;
        const int DLL_THREAD_ATTACH = 1;
        const int DLL_THREAD_DETACH = 2;
        const int DLL_PROCESS_DETACH = 3;
        
        [DllExport("CheatLib", CallingConvention.Winapi)]
        public static bool DllMain(int hModule, int ul_reason_for_call, IntPtr lpReserved)
        {
            switch (ul_reason_for_call)
            {
                case DLL_PROCESS_ATTACH:
                    new Thread(() => ProcAttach(hModule, lpReserved)).Start();
                    break;
                case DLL_THREAD_ATTACH:
                    ThreadAttach(hModule, lpReserved);
                    break;
                case DLL_THREAD_DETACH:
                    ThreadDetach(hModule, lpReserved);
                    break;
                case DLL_PROCESS_DETACH:
                    ProcDetach(hModule, lpReserved);
                    break;
            }
 
            return true;
        }
        
        private static void ProcAttach(int hModule, IntPtr lpReserved)
        {
            Console.WriteLine("Attached to process");
            
            // all the main activity are here
            while (IntPtr.Zero == PInvoke.GetModuleHandle("UserAssembly.dll"))
            {
                Console.WriteLine("UserAssembly.dll isn't initialized, waiting for 2 sec.");
                Thread.Sleep(2000);
            }
            Console.WriteLine("Waiting 10sec for loading game library");
            Thread.Sleep(10 * 1000);
        }

        private static void ProcDetach(int hModule, IntPtr lpReserved)
        {
            
        }

        private static void ThreadDetach(int hModule, IntPtr lpReserved)
        {
            
        }

        private static void ThreadAttach(int hModule, IntPtr lpReserved)
        {
            
        }
    }
}