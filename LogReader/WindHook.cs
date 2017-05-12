using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace LogReader
{
    class WindowHook
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        public static void SendKeystroke(string keys, string WindowName)
        {

            //Process p = Process.GetProcessesByName(WindowName).FirstOrDefault();
            IntPtr WindowToFind = FindWindow(null, WindowName);

            SetForegroundWindow(WindowToFind);
            SendKeys.SendWait(keys);
        }

        //public static void SendKeystrokeQuiet(string keys, string WindowName)
        //{
        //    //Process p = Process.GetProcessesByName(WindowName).FirstOrDefault();
        //    IntPtr prevWnd = GetForegroundWindow();
        //    IntPtr WindowToFind = FindWindow(null, WindowName);

        //    SetForegroundWindow(WindowToFind);
        //    SendKeys.SendWait(keys);
        //    if (prevWnd != WindowToFind)
        //        SetForegroundWindow(prevWnd);
        //}

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
