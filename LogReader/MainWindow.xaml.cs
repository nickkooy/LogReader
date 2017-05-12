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
using FirstFloor.ModernUI.Presentation;
using LogReader.Expressions;
using LogReader.Models;
using System.Xml.Serialization;

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
        ObservableCollection<LogAlert> alertsObs;
        ObservableCollection<LogFormat> formatsObs;
        bool autoscroll = true;
        bool ignoregold = true;
        bool onlyItems = true;
        bool clearLevel = true;
        bool spawnItemsOnly = true;
        bool showTimestamp = false;
        bool creatingLevel = false;
        LReader reader = null;
        GridLength sidePanelWidth;

        //https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx
        ResetMachineWindow resetWnd;

        public static MainWindow self
        {
            get;
            private set;
        }
        public MainWindow()
		{
            self = this;
            this.Closed += MainWindow_Closed;
            var settings = Properties.Settings.Default;
            autoscroll = settings.AutoScroll;
            ignoregold = settings.IgnoreGold;
            onlyItems = settings.OnlyItems;
            clearLevel = settings.ClearLevel;
            spawnItemsOnly = settings.LevelCreateItemsOnly;
            showTimestamp = settings.ShowTimestamp;
            sidePanelWidth = settings.SidePanelWidth;
            ndDir = settings.NDDir;
            InitResetWnd();

            AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
			NDDirectory = DefaultSteamDirectory;
            //NDDirectory = TestDir;
			InitializeComponent();

            CreateBinding("AutoScroll", CheckBox.IsCheckedProperty, chkAutoScroll);
            CreateBinding("ShowOnlyItems", CheckBox.IsCheckedProperty, chkShowOnlyItems);
            CreateBinding("IgnoreGoldDrops", CheckBox.IsCheckedProperty, chkIgnoreGoldDrops);
            CreateBinding("ClearOnNewLevel", CheckBox.IsCheckedProperty, chkClearLevel);
            CreateBinding("SpawnItemsOnly", CheckBox.IsCheckedProperty, chkSpawnItemsOnly);
            CreateBinding("ShowTimestamp", CheckBox.IsCheckedProperty, chkWriteTimestamp);
            CreateBinding("LevelExit", CheckBox.IsCheckedProperty, chkLevelExit);
            CreateBinding("SidePanelWidth", ColumnDefinition.WidthProperty, sidePanel);
            //alerts.Add(new LogAlert() { Trigger = "shovel_blood", AlertText = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!BLOOD SHOVEL!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" });
            //alerts.Add(new LogAlert() { Trigger = "familiar_shopkeeper", AlertText = "$$$$$ BABY FREDDY $$$$$" });
            ////alerts.Add(new LogAlert() { Trigger = "ring_becoming", AlertText = "BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING BECOMING " });
            alertsObs = new ObservableCollection<LogAlert>(Properties.Settings.Default.Alerts);
            foreach (var alert in alertsObs)
            {
                alert.PropertyChanged += LogAlert_PropertyChanged;
            }
            alertsObs.CollectionChanged += AlertsObs_CollectionChanged;
            //formats.Add(new LogFormat() { Find = @"ITEM NEW: (-?\d+), (-?\d+) itemType: (\w+) entityNum: \d+", Replace = "$1\t$2\t$3" });
            formatsObs = new ObservableCollection<LogFormat>(Properties.Settings.Default.Formats);
            foreach (var format in formatsObs)
            {
                format.PropertyChanged += LogFormat_PropertyChanged;
            }
            formatsObs.CollectionChanged += FormatsObs_CollectionChanged;
            dgAlerts.ItemsSource = alertsObs;
            dgFormats.ItemsSource = formatsObs;
            //dgFormats.ColumnWidth = DataGridLength.SizeToHeader;
            //XMLTest(alerts, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LogAlerts", "alerts.xml"));
            //XMLTest(formats, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LogAlerts", "formats.xml"));
            //XMLTest(alerts, Path.Combine(@"C:\Users\Nick\Desktop\xmltest\", "alerts.xml"));
            //XMLTest(formats, Path.Combine(@"C:\Users\Nick\Desktop\xmltest\", "formats.xml"));


            // Start the reader
            if (reader == null)
            {
                reader = new LReader(NDDirectory);
                reader.OnFileError += Reader_OnFileError;
                reader.StartRead();
                reader.OnLogEvent += Reader_OnLogEvent;
            }
        }

        private void Reader_OnFileError(object sender, LogReaderErrorArgs e)
        {
            tbStatus.Clear();
            tbStatus.Text = e.ErrorMessage;
        }

        void InitResetWnd()
        {
            resetWnd = new ResetMachineWindow();
            resetWnd.Closed += ResetWnd_Closed;
            resetWnd.Closing += ResetWnd_Closing;
            resetWnd.IsVisibleChanged += ResetWnd_IsVisibleChanged;
        }

        private void ResetWnd_Closed(object sender, EventArgs e)
        {
            NotifyPropertyChanged("IsResetWindowOpenBtnEnabled");
            resetWnd = null;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (resetWnd != null)
            {
                resetWnd.Closed -= ResetWnd_Closed;
                resetWnd.Closing -= ResetWnd_Closing;
                resetWnd.IsVisibleChanged -= ResetWnd_IsVisibleChanged;
                resetWnd.Close();
            }
            Properties.Settings.Default.Save();
        }

        private void FormatsObs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LogFormat logFormat in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    logFormat.PropertyChanged += LogFormat_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LogFormat logFormat in e.OldItems)
                {
                    logFormat.PropertyChanged -= LogFormat_PropertyChanged;
                }
            }
            var settings = Properties.Settings.Default;
            settings.Formats = formatsObs.ToArray();
            settings.Save();
        }

        private void LogFormat_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void AlertsObs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LogAlert logAlert in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    logAlert.PropertyChanged += LogAlert_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LogAlert logAlert in e.OldItems)
                {
                    logAlert.PropertyChanged -= LogAlert_PropertyChanged;
                }
            }
            var settings = Properties.Settings.Default;
            settings.Alerts = alertsObs.ToArray();
            settings.Save();
        }

        private void LogAlert_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public void XMLTest(object obj, string file)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            using (var fs = File.Create(file))
            using (TextWriter writer = new StreamWriter(fs))
            {
                ser.Serialize(writer, obj);
            }

        }

        private void ResetWnd_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("IsResetWindowOpenBtnEnabled");
        }

        private void ResetWnd_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            resetWnd.Hide();
        }

        public bool IsResetWindowOpenBtnEnabled
        {
            get { return resetWnd == null || !resetWnd.IsVisible; }
        }

        public ResetMachineWindow ResetWindow
        {
            get { return resetWnd; }
        }

        void CreateBinding(string propertyname, DependencyProperty dp, DependencyObject dobj)
        {
            Utils.Wnd.CreateBinding(propertyname, this, dp, dobj);
        }
        public bool? AutoScroll
        {
            get { return autoscroll; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (autoscroll != val)
                {
                    autoscroll = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.AutoScroll = val;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool? LevelExit
        {
            get;
            set;
        }

        public bool? IgnoreGoldDrops
        {
            get { return ignoregold; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (ignoregold != val)
                {
                    ignoregold = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.IgnoreGold = val;
                    Properties.Settings.Default.Save();
                }
            }
        }
        public bool? ShowOnlyItems
        {
            get { return onlyItems; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (onlyItems != val)
                {
                    onlyItems = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.OnlyItems = val;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool? ClearOnNewLevel
        {
            get { return clearLevel; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (clearLevel != val)
                {
                    clearLevel = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.ClearLevel = val;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool? SpawnItemsOnly
        {
            get { return spawnItemsOnly; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (spawnItemsOnly != val)
                {
                    spawnItemsOnly = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.LevelCreateItemsOnly = val;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool? ShowTimestamp
        {
            get { return showTimestamp; }
            set
            {
                bool val = value.HasValue && value.Value;
                if (showTimestamp != value)
                {
                    showTimestamp = val;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.ShowTimestamp = val;
                    Properties.Settings.Default.Save();
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
                    Properties.Settings.Default.NDDir = value;
                    Properties.Settings.Default.Save();
                }
			}
		}
        
        public double TextDisplayWidth
        {
            get { return alertPanel.ActualWidth; }
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

        void _WriteItemLine(DateTime timestamp, string textline)
        {
            foreach (LogFormat fmt in formatsObs.Where(f => f.Active))
            {
                textline = Regex.Replace(textline, Utils.Text.UnEscapeStr(fmt.Find), Utils.Text.UnEscapeStr(fmt.Replace));
            }
            if (showTimestamp)
                tbStatus.AppendText($"{timestamp}: {textline}{Environment.NewLine}");
            else
                tbStatus.AppendText(textline + Environment.NewLine);
        }

        bool CanWriteItems
        {
            get {
                bool sonly = (!SpawnItemsOnly.HasValue || !SpawnItemsOnly.Value);
                return      // Not spawn only items
                        sonly ||
                          // Spawn only items and creatingLevel
                          ((SpawnItemsOnly ?? false) && creatingLevel);
            }
        }

        private void Reader_OnLogEvent(object sender, NecroLogs.OnLogEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate () {
                
                if (LevelExit ?? false)
                {
                    if (e.Line.Text.StartsWith("PLACEEXIT: "))
                    {
                        _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                        return;
                    }
                }

                if (ClearOnNewLevel ?? false)
                {
                    if (e.Line.Text.StartsWith("NEWLEVEL:"))
                    {
                        tbStatus.Clear();
                    }
                }

                if (e.Line.Text.StartsWith("NEWLEVEL:"))
                    creatingLevel = true;

                if (Regex.IsMatch(e.Line.Text, "CREATEMAP ZONE\\d+: Finished!") || e.Line.Text.StartsWith("Level generation completed"))
                    creatingLevel = false;

                if (ShowOnlyItems ?? false)
                {
                    if (e.Line.Text.StartsWith("ITEM NEW:") && CanWriteItems)
                    {
                        if (IgnoreGoldDrops ?? true)
                        {
                            if (!e.Line.Text.Contains("itemType: resource_coin"))
                            {
                                _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                            }

                        }
                        else
                        {
                            _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                        }
                    }
                }
                else
                {
                    if (e.Line.Text.StartsWith("ITEM NEW:"))
                    {
                        if (CanWriteItems)
                        {
                            if (IgnoreGoldDrops ?? true)
                            {
                                if (!e.Line.Text.Contains("itemType: resource_coin"))
                                    _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                            }
                            else
                            {
                                _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                            }
                        }
                    }
                    else
                    {
                        _WriteItemLine(e.Line.Timestamp, e.Line.Text);
                    }
                    
                }

                if (CanWriteItems)
                {
                    foreach (var alert in alertsObs.Where(a => a.Active))
                    {
                        Regex r = new Regex(alert.Trigger);
                        if (r.IsMatch(e.Line.Text))
                        {
                            tbStatus.AppendText(alert.AlertText != null ? alert.AlertText : "");
                            tbStatus.AppendText(Environment.NewLine);
                        }
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

        public LReader Reader
        {
            get
            {
                return reader;
            }
        }

        public GridLength SidePanelWidth
        {
            get
            {
                return sidePanelWidth;
            }

            set
            {
                if (sidePanelWidth != value)
                {
                    sidePanelWidth = value;
                    NotifyPropertyChanged();
                    Properties.Settings.Default.SidePanelWidth = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

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
        
        
        private void btnRMSetAdd_Click(object sender, RoutedEventArgs e)
        {
            //tabResetMachine.Items.Add()
        }

        void _CreateResetWindow()
        {
            if (resetWnd == null)
                InitResetWnd();

            resetWnd.Show();
        }

        private void btnResetWindowToggle_Click(object sender, RoutedEventArgs e)
        {
            if (resetWnd == null)
                InitResetWnd();
            resetWnd.Show();
        }
    }
}
