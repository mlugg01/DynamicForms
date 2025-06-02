using DynamicForms.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace DynamicForms
{
    internal static class Utilities
    {
        public static bool StringsAreEqual(string str1, string str2)
        {
            return str1?.Trim().Equals(str2?.Trim(), StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public static LCI.Controls.DateTimeTextBox.DateTimeButtonTypes GetDateTimeButtonType(string configButtonType)
        {
            switch (configButtonType)
            {
                case DateControlButtonTypes.Now:
                    return LCI.Controls.DateTimeTextBox.DateTimeButtonTypes.Now;

                case DateControlButtonTypes.Today:
                    return LCI.Controls.DateTimeTextBox.DateTimeButtonTypes.Today;

                default:
                    return LCI.Controls.DateTimeTextBox.DateTimeButtonTypes.None;
            }
        }

        public static LCI.Controls.DateTimeTextBox.DropDownModes GetDateTimeDropDownMode(string configDropDownMode)
        {
            switch (configDropDownMode)
            {
                case DateControlDropDownTypes.DateOnly:
                    return LCI.Controls.DateTimeTextBox.DropDownModes.DateOnly;

                case DateControlDropDownTypes.DateAndTime:
                    return LCI.Controls.DateTimeTextBox.DropDownModes.DateTime;

                default:
                    return LCI.Controls.DateTimeTextBox.DropDownModes.None;
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }

        public static void DataGridSingleClickEdit(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grdItems = (DataGrid)sender;
                grdItems.BeginEdit(e);

                Control loControl = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (loControl != null)
                {
                    loControl.Focus();
                }
            }
        }

        public static T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            //INSTANT C# NOTE: The ending condition of VB 'For' loops is tested only on entry to the loop. Instant C# has created a temporary variable in order to use the initial value of VisualTreeHelper.GetChildrenCount(prop) for every iteration:
            int childrenCount = VisualTreeHelper.GetChildrenCount(prop);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild((prop), i);
                if (child == null)
                {
                    continue;
                }

                T castedProp = child as T;
                if (castedProp != null)
                {
                    return castedProp;
                }

                castedProp = GetFirstChildByType<T>(child);
                if (castedProp != null)
                {
                    return castedProp;
                }
            }
            return null;
        }

    }
}
