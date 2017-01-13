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
using FirstFloor.ModernUI.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace LogReader
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ModernWindow, INotifyPropertyChanged
	{
		const string DefaultSteamDirectory = @"C:\program files (x86)\steam\steamapps\common\Crypt of the Necrodancer";
		const string TestDir = @"C:\TFS\LogReader\Tests";
		const string TestFile = "1.txt";

		string ndDir;
        bool? onlyItems = true;
        List<LogAlert> alerts = new List<LogAlert>();
        ObservableCollection<LogAlert> alertsObs;
        bool? autoscroll = true;
        bool? ignoregold = true;
        LReader reader = null;

        public MainWindow()
		{
			NDDirectory = DefaultSteamDirectory;
			//NDDirectory = TestDir;

			InitializeComponent();
            CreateBinding("AutoScroll", CheckBox.IsCheckedProperty, chkAutoScroll);
            CreateBinding("ShowOnlyItems", CheckBox.IsCheckedProperty, chkAutoScroll);
            CreateBinding("IgnoreGoldDrops", CheckBox.IsCheckedProperty, chkIgnoreGoldDrops);
            alerts.Add(new LogAlert() { Trigger = "ring_becoming", AlertText = "BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING " });
            alertsObs = new ObservableCollection<LogAlert>(alerts);
            dgAlerts.ItemsSource = alertsObs;

            // Start the reader
            if (reader == null)
            {
                reader = new LReader(NDDirectory);
                reader.StartRead();
                reader.OnLogEvent += Reader_OnLogEvent;
            }
        }

        void CreateBinding(string propertyname, DependencyProperty dp, DependencyObject dobj)
        {
            Binding asBinding = new Binding();
            asBinding.Source = this;
            asBinding.Path = new PropertyPath(propertyname);
            asBinding.Mode = BindingMode.TwoWay;
            asBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(dobj, dp, asBinding);
        }
        public bool? AutoScroll
        {
            get { return autoscroll; }
            set
            {
                if (autoscroll != value)
                {
                    autoscroll = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool? IgnoreGoldDrops
        {
            get { return ignoregold; }
            set
            {
                if (ignoregold != value)
                {
                    ignoregold = value;
                    NotifyPropertyChanged();
                }
            }
        }

		public string NDDirectory
		{
			get { return ndDir; }
			set
			{
				if (value != ndDir)
				{
					ndDir = value;
                    if (reader != null)
                    {
                        reader.NecrodancerDir = value;
                        reader.StartRead();
                    }
					NotifyPropertyChanged();
				}
			}
		}

        public double TextDisplayWidth
        {
            get { return alertPanel.ActualWidth; }
        }
        public bool? ShowOnlyItems
        {
            get { return onlyItems; }
            set {
                if (onlyItems != value)
                {
                    onlyItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void btnOpenLog_Click(object sender, RoutedEventArgs e)
		{
			if (CommonFileDialog.IsPlatformSupported)
			{
				CmnFileDialog();
			}
			else
			{
				WinFormsDialog();
			}
		}

		void CmnFileDialog()
		{
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.IsFolderPicker = true;
				dialog.DefaultDirectory = NDDirectory;
				dialog.Title = "Select Necrodancer Install Folder";
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
				{
					NDDirectory = dialog.FileName;
				}
			}
		}

		void WinFormsDialog()
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				dialog.ShowNewFolderButton = false;
				dialog.SelectedPath = NDDirectory;
				var result = dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
				{
					NDDirectory = dialog.SelectedPath;
				}
			}
		}

		private void btnOpenReader_Click(object sender, RoutedEventArgs e)
		{
            
			//reader.Main();
		}

        private void Reader_OnLogEvent(object sender, NecroLogs.OnLogEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate () {
                if (ShowOnlyItems.HasValue && ShowOnlyItems.Value)
                {
                    if (e.Line.Text.StartsWith("ITEM NEW:"))
                    {
                        if (IgnoreGoldDrops ?? true)
                        {
                            if (!e.Line.Text.Contains("itemType: resource_coin"))
                                tbStatus.AppendText(string.Format("{0}: {1}\r\n", e.Line.Timestamp, e.Line.Text));

                        }
                        else
                        {
                            tbStatus.AppendText(string.Format("{0}: {1}\r\n", e.Line.Timestamp, e.Line.Text));
                        }
                    }
                }
                else
                {
                    if (IgnoreGoldDrops ?? true)
                    {
                        if (!e.Line.Text.Contains("itemType: resource_coin"))
                            tbStatus.AppendText(string.Format("{0}: {1}\r\n", e.Line.Timestamp, e.Line.Text));

                    }
                    else
                    {
                        tbStatus.AppendText(string.Format("{0}: {1}\r\n", e.Line.Timestamp, e.Line.Text));
                    }
                }

                foreach (var alert in alertsObs)
                {
                    Regex r = new Regex(alert.Trigger);
                    if (r.IsMatch(e.Line.Text))
                    {
                        tbStatus.AppendText(alert.AlertText);
                        tbStatus.AppendText("\r\n");
                    }
                }

                if (AutoScroll ?? true)
                {
                    //tbStatus.Focus();
                    tbStatus.CaretIndex = tbStatus.Text.Length;
                    tbStatus.ScrollToEnd();

                    svStatus.ScrollToVerticalOffset(svStatus.ExtentHeight - svStatus.ViewportHeight);
                }
            }));
        }

		PlayerObj Player { get; set; }

		public class PlayerObj
		{
			public List<Item> Inventory { get; private set; }

			public PlayerObj() { Inventory = new List<Item>();}

		}

		public class Item
		{

		}

        private void btnAddAlert_Click(object sender, RoutedEventArgs e)
        {
            alertsObs.Add(new LogAlert() { AlertText = "", Trigger = "" });
        }

        private void btnRemoveAlert_Click(object sender, RoutedEventArgs e)
        {
            object vm = this.DataContext;
            var button = sender as Button;
            object item = button.DataContext;
        }
    }
}
