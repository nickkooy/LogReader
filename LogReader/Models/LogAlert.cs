using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ComponentModel;
using LogReader.Models.Converters;
using System.Runtime.CompilerServices;

namespace LogReader
{
    //[TypeConverter(typeof(LogAlertConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class LogAlert : INotifyPropertyChanged
    {
        string trigger;
        string alertText;
        bool active = true;

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                if (active != value)
                {
                    active = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Trigger
        {
            get { return trigger; }
            set
            {
                if (trigger != value)
                {
                    trigger = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string AlertText
        {
            get { return alertText; }
            set
            {
                if (alertText != value)
                {
                    alertText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
