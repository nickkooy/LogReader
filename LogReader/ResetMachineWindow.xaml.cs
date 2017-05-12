using FirstFloor.ModernUI.Windows.Controls;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LogReader.Utils;
using LogReader.Content;
using LogReader.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogReader
{
    /// <summary>
    /// Interaction logic for RestMachineWindow.xaml
    /// </summary>
    public partial class ResetMachineWindow : ModernWindow, INotifyPropertyChanged
    {
        ResetMachine resetMachine;
        ObservableCollection<ResetExpr> resetExprsObs;
        public ResetMachineWindow()
        {
            var settings = Properties.Settings.Default;
            resetMachine = new ResetMachine()
            {
                ResetKey = settings.ResetKey,
                PauseKey = settings.PauseKey,
                WindowName = settings.WindowName
            };
            InitializeComponent();
            this.Closed += ResetMachineWindow_Closed;

            resetMachine.OnComplete += ResetMachine_OnComplete;
            resetMachine.OnReset += ResetMachine_OnReset;
            
            //resetExprs.Add(new ResetExpr() { Expression = "shovel_blood & familiar_shopkeeper" });
            resetExprsObs = new ObservableCollection<ResetExpr>(Properties.Settings.Default.ResetExpressions);
            resetExprsObs.CollectionChanged += ResetExprsObs_CollectionChanged;
            foreach (var resetExpr in resetExprsObs)
            {
                resetExpr.PropertyChanged += ResetExpr_PropertyChanged;
            }
            dgResetItems.ItemsSource = resetExprsObs;
            dgResetItems.CellEditEnding += DgResetItems_CellEditEnding;

            //MainWindow.self.XMLTest(resetExprs, @"C:\Users\Nick\Desktop\xmltest\resetexprs.xml");
        }

        private void DgResetItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Cancel)
                return;

            Properties.Settings.Default.Save();
        }

        private void ResetMachineWindow_Closed(object sender, EventArgs e)
        {
            // Save settings
            var settings = Properties.Settings.Default;
            settings.ResetExpressions = resetExprsObs.ToArray();
            settings.Save();

            if (resetMachine != null)
            {
                resetMachine.OnComplete -= ResetMachine_OnComplete;
                resetMachine.OnReset -= ResetMachine_OnReset;
                resetMachine = null;
            }
        }

        private void ResetExprsObs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ResetExpr resetExpr in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    //resetExpr.PropertyChanged += ResetExpr_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ResetExpr resetExpr in e.OldItems)
                {
                    //resetExpr.PropertyChanged -= ResetExpr_PropertyChanged;
                }
            }
            //var settings = Properties.Settings.Default;
            //settings.ResetExpressions = resetExprsObs.ToArray();
            //settings.Save();
        }

        private void ResetExpr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Active")
            //{
            //    Properties.Settings.Default.Save();
            //}
            Properties.Settings.Default.Save();
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
        string rkey, pkey, wname = "";

        private void btnResetOptions_Click(object sender, RoutedEventArgs e)
        {
            rkey = resetMachine.ResetKey;
            pkey = resetMachine.PauseKey;
            wname = resetMachine.WindowName;
            ModernDialog dlg = new ModernDialog
            {
                Title = "Reset Meme Settings",
                Content = new ResetMachineOptions()
            };
            dlg.Buttons = new Button[] { dlg.CancelButton, dlg.OkButton };
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                resetMachine.ResetKey = rkey;
                resetMachine.PauseKey = pkey;
                resetMachine.WindowName = wname;
                Properties.Settings.Default.ResetKey = rkey;
                Properties.Settings.Default.PauseKey = pkey;
                Properties.Settings.Default.WindowName = wname;
                Properties.Settings.Default.Save();
            }
        }


        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            btnStopReset.IsEnabled = true;
            lblResetStatus.Content = "Load a level in Necrodancer to begin.";
            btnReset.IsEnabled = false;
            resetMachine.ItemExprs = new List<ResetExpr>();
            resetMachine.ItemExprs.AddRange(resetExprsObs.Where(rexpr => rexpr.Active));
            resetMachine.Start(MainWindow.self.Reader);
        }
        private void btnStopReset_Click(object sender, RoutedEventArgs e)
        {
            resetMachine.Stop();
            btnStopReset.IsEnabled = false;
            lblResetStatus.Content = "Stopped";
            btnReset.IsEnabled = true;
        }
        private void ResetMachine_OnComplete(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                btnStopReset.IsEnabled = false;
                btnReset.IsEnabled = true;
                //lblResetStatus.Content = "";
            }));
        }
        private void ResetMachine_OnReset(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate ()
            {
                lblResetStatus.Content = $"Reset Count: {(sender as ResetMachine).ResetCount}";
            }));
        }

        public string ResetKey
        {
            get { return rkey; }
            set
            {
                if (value != rkey)
                {
                    rkey = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PauseKey
        {
            get { return pkey; }
            set
            {
                if (value != pkey)
                {
                    pkey = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string WindowName
        {
            get { return wname; }
            set
            {
                if (value != wname)
                {
                    wname = value;
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

        private void resetWnd_Closing(object sender, CancelEventArgs e)
        {
            if (resetMachine.Running)
                resetMachine.Stop();
        }
    }
}
