using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;

namespace LogReader.Models.Converters
{
    public class LogFormatConverter : TypeConverter
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
                LogFormat format = new LogFormat() { Active = bool.Parse(parts[0]), Find = parts[1], Replace = parts.Length > 2 ? parts[2] : "" };
                return format;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                LogFormat format = value as LogFormat;
                return $"{format.Active},{format.Find},{format.Replace}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
