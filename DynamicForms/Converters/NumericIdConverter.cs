using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicForms.Converters
{
    internal class NumericIdConverter : IValueConverter
    {
        private readonly string _newIdIndicator = "(New)";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = (int)value;
            if (id == 0)
                return _newIdIndicator;
            else
            {
                string format = (string)parameter;
                if (!string.IsNullOrWhiteSpace(format))
                    return id.ToString(format);
                else
                    return id.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string id = (string)value;
            if (id == _newIdIndicator)
                return 0;
            return int.Parse(id);
        }
    }
}
