using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LogReader.Utils
{
    public static class Wnd
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void CreateBinding(string propertyname, object source, DependencyProperty dp, DependencyObject dobj)
        {
            Binding asBinding = new Binding();
            asBinding.Source = source;
            asBinding.Path = new PropertyPath(propertyname);
            asBinding.Mode = BindingMode.TwoWay;
            asBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(dobj, dp, asBinding);
        }

        public static bool CanReadDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return false;

            try
            {
                DirectoryInfo dinfo = new DirectoryInfo(directory);
                var dSec = dinfo.GetAccessControl();
                foreach (FileSystemAccessRule rule in dSec.GetAccessRules(true, true, typeof(NTAccount)))
                {
                    if (rule.FileSystemRights == FileSystemRights.Read)
                    {
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
            }

            return false;
        }
    }
}
