using LogReader.Models.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    //[TypeConverter(typeof(LogAlertConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class LogFormat : INotifyPropertyChanged
    {
        bool active = true;
        string find;
        string replace;

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

        public string Find
        {
            get
            {
                return find;
            }

            set
            {
                if (find != value)
                {
                    find = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Replace
        {
            get
            {
                return replace;
            }

            set
            {
                if (replace != value)
                {
                    replace = value;
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
