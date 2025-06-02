using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicForms.Converters
{
    public class SelectedIndexToBooleanConverter : IValueConverter
    {
        //Converts the SelectedIndex value to a boolean, false if -1, true otherwise.

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (System.Convert.ToInt32(value) > -1);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
