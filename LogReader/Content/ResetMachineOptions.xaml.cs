using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogReader.Utils;

namespace LogReader.Content
{
    /// <summary>
    /// Interaction logic for ResetMachineOptions.xaml
    /// </summary>
    public partial class ResetMachineOptions : UserControl
    {
        public ResetMachineOptions()
        {
            InitializeComponent();
            Wnd.CreateBinding("ResetKey", MainWindow.self.ResetWindow, TextBox.TextProperty, txtResetKey);
            Wnd.CreateBinding("PauseKey", MainWindow.self.ResetWindow, TextBox.TextProperty, txtPauseKey);
            Wnd.CreateBinding("WindowName", MainWindow.self.ResetWindow, TextBox.TextProperty, txtWindowName);
        }
    }
}
