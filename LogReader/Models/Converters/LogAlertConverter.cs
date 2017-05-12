using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader.Models.Converters
{
    public class LogAlertConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = ((string)value).Split(new char[] { ',' });
                LogAlert alert = new LogAlert() { Trigger = parts[0], AlertText = parts[1] };
                return alert;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                LogAlert alert = value as LogAlert;
                return $"{alert.Trigger},{alert.AlertText}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
