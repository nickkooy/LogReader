using LogReader.Models.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogReader.Models
{
    //[TypeConverter(typeof(ResetExprConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class ResetExpr : INotifyPropertyChanged
    {
        bool active = true;
        string expression;
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
        public string Expression
        {
            get
            {
                return expression;
            }
            set
            {
                if (expression != value)
                {
                    expression = value;
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
