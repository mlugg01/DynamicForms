using DynamicForms.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace DynamicForms.Controls
{
    /// <summary>
    /// Interaction logic for DataGridEditControl.xaml
    /// </summary>
    public partial class DataGridEditControl : UserControl
    {
        private LCI.Windows.MultiDataTracker<ExpandoObject> _dataTracker;

        

        private struct CreateDataGridColumnResult
        {
            public enum ResultStates
            {
                Error,
                ColumnNotCreated,
                ColumnCreated
            }

            public ResultStates ColumnCreateState { get; set; }
            public DataGridColumn Column { get; set; }
        }

        #region Events

        public event EventHandler DataChanged = new EventHandler((e, a) => { });

        #endregion

        #region Properties

        private bool _isDataChanged = false;
        public bool IsDataChanged => _isDataChanged;

        #endregion

        #region Section Methods/Event Handlers, UI Build Methods

        public DataGridEditControl(ControlConfig controlConfig)
        {
            InitializeComponent();


            if (!CreateDataGridColumns(grdItems, controlConfig))
                throw new Exception("DataGridEditControl EXCEPTION: An error occurred while creating the data grid columns.");

            this.Tag = controlConfig;
            btnNew.Click += btnNew_Click;
            btnDelete.Click += btnDelete_Click;

            _dataTracker = new LCI.Windows.MultiDataTracker<ExpandoObject>();
            _dataTracker.DataChanged += DataTracker_DataChanged;
        }

        public bool SetData(List<ExpandoObject> itemsSectionData)
        {
            try
            {              
                _dataTracker.Recs = itemsSectionData.ToObservableCollection();
                grdItems.ItemsSource = _dataTracker.Recs;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DataGridEditControl - Set Data");
            }
            return false;
        }

        public List<ExpandoObject> GetData()
        {
            try
            {
                return _dataTracker.Recs.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DataGridEditControl - Get Data");
            }
            return null;
        }

        #endregion

        #region Column Generation Methods

        private bool CreateDataGridColumns(DataGrid dg, ControlConfig controlConfig)
        {
            try
            {
                CreateDataGridColumnResult result;
                foreach (var columnConfig in controlConfig.ControlType.DetailGridConfig.Columns)
                {
                    switch (columnConfig.ControlType.DataType)
                    {
                        case ControlTypes.CheckBox:
                            result = CreateDataGridCheckBoxColumn(columnConfig);
                            break;

                        case ControlTypes.ComboBox:
                            result = CreateDataGridComboBoxColumn(columnConfig);
                            break;

                        default:
                            result = CreateDataGridTemplateColumn(columnConfig);
                            break;
                    }
                    if (result.ColumnCreateState != CreateDataGridColumnResult.ResultStates.ColumnCreated)
                        return false;

                    dg.Columns.Add(result.Column);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create DataGrid Columns");
            }
            return false;
        }

        private CreateDataGridColumnResult CreateDataGridCheckBoxColumn(ControlConfig columnConfig)
        {
            var result = new CreateDataGridColumnResult()
            {
                ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnNotCreated
            };
            try
            {
                var isEnabled = !columnConfig.IsReadOnly;

                var headerStyle = (Style)Application.Current.Resources["DataGridColumnHeaderCenterAligned"];
                var cellStyle = (Style)Application.Current.Resources["DataGridCellCenterAligned"];
                var checkBoxCellStyle = (Style)Application.Current.Resources["CheckBoxDataGridCellCenterAlignedStyle"];

                var editingElementStyle = new Style(typeof(CheckBox), checkBoxCellStyle);
                editingElementStyle.Setters.Add(new Setter(IsEnabledProperty, isEnabled));

                result.Column = new DataGridCheckBoxColumn()
                {
                    Header = columnConfig.Label,
                    HeaderStyle = headerStyle,
                    CellStyle = cellStyle,
                    EditingElementStyle = editingElementStyle,

                    IsThreeState = false,

                    SortMemberPath = columnConfig.Name,
                    Binding = new Binding(columnConfig.Name)
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    }
                };
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnCreated;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create CheckBox Column");
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.Error;
            }
            return result;
        }

        private CreateDataGridColumnResult CreateDataGridComboBoxColumn(ControlConfig columnConfig)
        {
            var result = new CreateDataGridColumnResult()
            {
                ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnNotCreated
            };
            try
            {
                var selectionItems = columnConfig.ControlType.ComboBoxConfig.GetSelectionItems();

                var isEnabled = !columnConfig.IsReadOnly;
                var editingElementStyle = new Style(typeof(ComboBox));
                editingElementStyle.Setters.Add(new Setter(IsEnabledProperty, isEnabled));

                result.Column = new DataGridComboBoxColumn()
                {
                    Header = columnConfig.Label,

                    ItemsSource = selectionItems,
                    SelectedValueBinding = new Binding(columnConfig.Name)
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    },

                    EditingElementStyle = editingElementStyle
                };
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnCreated;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create ComboBox Column");
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.Error;
            }
            return result;
        }

        private CreateDataGridColumnResult CreateDataGridTemplateColumn(ControlConfig columnConfig)
        {
            var result = new CreateDataGridColumnResult()
            {
                ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnNotCreated
            };
            try
            {
                result.Column = new DataGridTemplateColumn
                {
                    Header = columnConfig.Label,
                    CellTemplate = GetDataGridColumnCellTemplate(columnConfig),
                    CellEditingTemplate = GetDataGridColumnCellEditingTemplate(columnConfig)
                };
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.ColumnCreated;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create Template Column");
                result.ColumnCreateState = CreateDataGridColumnResult.ResultStates.Error;
            }
            return result;
        }

        private DataTemplate GetDataGridColumnCellTemplate(ControlConfig columnConfig)
        {
            var resourceXaml = GetDataGridColumnCellTemplateResourcesXaml(columnConfig);
            var childXaml = GetDataGridColumnCellTemplateChildXaml(columnConfig);

            Debug.WriteLine(childXaml);

            var xaml = "<DataTemplate\r\n" +
                        "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n" +
                        "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'\r\n" +
                        "xmlns:Converters='clr-namespace:DynamicForms.Converters;assembly=DynamicForms'>\r\n" +
                        resourceXaml +
                        childXaml +
                        "</DataTemplate>";

            return (DataTemplate)XamlReader.Parse(xaml);
        }

        private string GetDataGridColumnCellTemplateResourcesXaml(ControlConfig columnConfig)
        {
            switch (columnConfig.ControlType.DataType)
            {
                case ControlTypes.Date:
                    return "<DataTemplate.Resources>\r\n" +
                           "<Converters:DateTimeFormatConverter x:Key='DateTimeFormatConverter'/>\r\n" +
                           "</DataTemplate.Resources>\r\n";

                default:
                    return "";
            }
        }

        private string GetDataGridColumnCellTemplateChildXaml(ControlConfig columnConfig)
        {
            var bindingPropertyName = columnConfig.Name;


            switch (columnConfig.ControlType.DataType)
            {
                case ControlTypes.Date:
                    var dateOnly = columnConfig.ControlType.DateControlConfig.DateOnly;

                    return $"<TextBlock Text='{{Binding {bindingPropertyName}, " +
                           $"Converter={{StaticResource DateTimeFormatConverter}}, ConverterParameter='{dateOnly}'}}'\r\n" +
                            "Style='{StaticResource CheckBoxDataGridCellCenterAlignedStyle}'/>\r\n";

                case ControlTypes.Numeric:
                    return $"<TextBlock Text='{{Binding {bindingPropertyName}}}'\r\n" +
                            "Style='{StaticResource TextBlockDataGridCellRightAlignedStyle}'/>\r\n";

                default:
                    return $"<TextBlock Text='{{Binding {bindingPropertyName}}}'\r\n" +
                            "Style='{StaticResource TextBlockDataGridCellLeftAlignedStyle}'/>\r\n";
            }
        }

        private DataTemplate GetDataGridColumnCellEditingTemplate(ControlConfig columnConfig)
        {
            var resourceXaml = GetDataGridColumnCellEditingTemplateResourcesXaml(columnConfig);
            var childXaml = GetDataGridColumnCellEditingTemplateXaml(columnConfig);
            Debug.WriteLine(childXaml);

            var xaml = "<DataTemplate\r\n" +
                        "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'\r\n" +
                        "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'\r\n" +
                        "xmlns:LciInput='clr-namespace:LCI.Controls;assembly=LCI.Controls.Data.Input'\r\n" +
                        "xmlns:Converters='clr-namespace:DynamicForms.Converters;assembly=DynamicForms'>\r\n" +
                        resourceXaml +
                        childXaml +
                        "</DataTemplate>";

            return (DataTemplate)XamlReader.Parse(xaml);
        }

        private string GetDataGridColumnCellEditingTemplateResourcesXaml(ControlConfig columnConfig)
        {
            switch (columnConfig.ControlType.DataType)
            {
                case ControlTypes.Numeric:
                    return "<DataTemplate.Resources>\r\n" +
                           "<Converters:DoubleToDecimalConverter x:Key='DoubleToDecimalConverter'/>\r\n" +
                           "</DataTemplate.Resources>\r\n";

                default:
                    return "";
            }
        }

        private string GetDataGridColumnCellEditingTemplateXaml(ControlConfig columnConfig)
        {
            var bindingPropertyName = columnConfig.Name;
            var isReadOnly = columnConfig.IsReadOnly;

            switch (columnConfig.ControlType.DataType)
            {
                case ControlTypes.CheckBox:
                    return $"<CheckBox IsChecked='{{Binding {bindingPropertyName}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}\r\n" +
                            "HorizontalAlignment='Center'\r\n" +
                            "VerticalAlignment='Center'\r\n" +
                            $"IsEnabled='{!isReadOnly}'/>\r\n";

                case ControlTypes.Date:
                    var dateControlConfig = columnConfig.ControlType.DateControlConfig;

                    var dateOnly = dateControlConfig.DateOnly;
                    var mask = $"99/99/99{(dateOnly ? "" : " 99:99")}";
                    var dateFormat = $"MM/dd/yy{(dateOnly ? "" : "HH:mm")}";

                    var buttonType = Utilities.GetDateTimeButtonType(dateControlConfig.GetButtonType());
                    var dropDownMode = Utilities.GetDateTimeDropDownMode(dateControlConfig.GetDropDownType());

                    return $"<LciInput:DateTimeTextBox DateValue={{Binding {bindingPropertyName}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}\r\n" +
                           $"Mask='{mask}'\r\n" +
                           $"DateFormat='{dateFormat}'\r\n" +
                           $"DateTimeButtonType='{buttonType}'\r\n" +
                           $"DropDownMode='{dropDownMode}'\r\n" +
                           $"IsReadOnly='{isReadOnly}'/>\r\n";

                case ControlTypes.Numeric:
                    var numericControlConfig = columnConfig.ControlType.NumericControlConfig;

                    var maxIntegerDigits = numericControlConfig.MaxIntegerDigits;
                    var maxIntegerDigitsXaml = maxIntegerDigits.HasValue ? $"MaxIntegerDigits='{maxIntegerDigits}'\r\n" : "";

                    var maxDecimalDigitsXaml = $"MaxDeimalDigits='{numericControlConfig.MaxDecimalDigits}'\r\n";

                    var allowNegative = numericControlConfig.AllowNegative;

                    return $"<LciInput:NumericTextBox Value='{{Binding {bindingPropertyName}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged, Converter={{StaticResource DoubleToDecimalConverter}}}}'\r\n" +
                            maxIntegerDigitsXaml +
                            maxDecimalDigitsXaml +
                            $"AllowNegative='{allowNegative}'\r\n" +
                            $"IsReadOnly='{isReadOnly}'/>\r\n";

                default:
                    var textControlConfig = columnConfig.ControlType.TextControlConfig;
                    var maxLength = textControlConfig.MaxLength;

                    return $"<TextBox Text='{{Binding {bindingPropertyName}, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}}'\r\n" +
                           $"MaxLength='{maxLength}'/>\r\n";
            };
        }

        #endregion

        #region Button-click Event Handlers

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: populate with defaults??
                dynamic newItem = new ExpandoObject();
               _dataTracker.Recs.Add(newItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DataGridEditControl - New");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Delete the selected item?", "Confirm Delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) 
                    return;

                var selectedItem = (ExpandoObject)grdItems.SelectedItem;
                _dataTracker.Recs.Remove(selectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DataGridEditControl - Delete");
            }
        }

        #endregion

        #region Methods/Event Handlers
        
        private void DataTracker_DataChanged(object sender, EventArgs e)
        {
            _isDataChanged = true;
            DataChanged(this, new EventArgs());
        }

        #endregion

        private void grdItems_GotFocus(object sender, RoutedEventArgs e)
        {
            Utilities.DataGridSingleClickEdit(sender, e);
        }
    }
}
