using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;


namespace LogReader.Models.Converters
{
    public class ResetExprConverter : TypeConverter
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
                ResetExpr expr = new ResetExpr() { Active = bool.Parse(parts[0]), Expression = parts.Length > 1 ? parts[1] : ""};
                return expr;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                ResetExpr expr = value as ResetExpr;
                return $"{expr.Active},{expr.Expression}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
