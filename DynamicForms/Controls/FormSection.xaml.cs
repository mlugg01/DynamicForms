using DynamicForms.Config;
using System;
using System.Collections.Generic;
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


namespace DynamicForms.Controls
{
    /// <summary>
    /// Interaction logic for FormSection.xaml
    /// </summary>
    public partial class FormSection : UserControl
    {
        private LCI.Windows.SingleDataTracker<ExpandoObject> _dataTracker;

        private double _inputControlFixedWidth = 120;        
        private List<Control> _inputControls;
        private string _sectionName;
        private bool _isDataChanged = false;
        private ExpandoObject _sectionData;
        private List<ExpandoObject> _itemsSectionData = null;        
        private DataGridEditControl _dataGridControl;
        private string _mathematicalFormula;
        
        #region Events

        public event EventHandler DataChanged = new EventHandler((e, a) => { });

        #endregion

        #region Properties

        public bool IsDataChanged => _isDataChanged;
        public ExpandoObject Data => _dataTracker?.Rec;
        public List<ExpandoObject> ItemsData => _dataGridControl?.GetData();
        public string SectionName => _sectionName;

        #endregion

        #region Section Methods/Event Handlers, UI Build Methods

        public FormSection(FormSectionConfig sectionConfig, ExpandoObject sectionData = null, List<ExpandoObject> itemsSectionData = null)
        {
            InitializeComponent();

            _sectionName = sectionConfig.SectionName;
            
            if (sectionData != null)
                _sectionData = sectionData;

            if (itemsSectionData != null)
                _itemsSectionData = itemsSectionData;

            if (!string.IsNullOrWhiteSpace(sectionConfig.MathematicalFormula))
                _mathematicalFormula = sectionConfig.MathematicalFormula;

            _inputControls = new List<Control>();
            
            lblSectionHeader.Text = sectionConfig.SectionHeader;
            SectionNameBorder.Visibility = sectionConfig.DisplaySectionHeader ? Visibility.Visible : Visibility.Collapsed;

            Debug.WriteLine($"Section: {sectionConfig.SectionName}\n - Rows: {sectionConfig.RowCount}\n - Cols: {sectionConfig.ColumnCount}");

            if (!SetupLayoutGrid(sectionConfig))
                return;

            if (!CreateAndInitializeInputControls(sectionConfig))
                return;

            if (!SetSectionData(sectionConfig)) 
                return;

            Debug.WriteLine("**********************");
        }

        private bool SetupLayoutGrid(FormSectionConfig sectionConfig)
        {
            try
            {
                for (int i = 0; i < sectionConfig.RowCount; i++)
                    ControlGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                for (int i = 0; i < sectionConfig.ColumnCount; i++)
                {
                    // Add label column definition.
                    ControlGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = GridLength.Auto,
                        MinWidth = 40
                    });

                    // Add control column definition.
                    ControlGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });

                    // Add spacer column definition.
                    ControlGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = GridLength.Auto,
                        MinWidth = 40
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Setup Layout Grid");
            }
            return false;
        }

        private bool CreateAndInitializeInputControls(FormSectionConfig sectionConfig)
        {
            try
            {
                CheckBox chk;
                ComboBox cmb;
                TextBlock lbl;
                TextBox txt;
                LCI.Controls.NumericTextBox numericTxt;
                LCI.Controls.DateTimeTextBox dateTimeTxt;
                int controlRow;
                int controlColumn;
                int controlColumnSpan;
                ControlType controlType;
                string controlName;
                foreach (var controlConfig in SortedControlConfigs(sectionConfig.Controls))
                {
                    controlType = controlConfig.ControlType;

                    if (controlType.DataType != ControlTypes.DetailsGrid)
                    {
                        controlRow = controlConfig.Position.Row;
                        controlColumn = GetColumn(controlConfig.Position.Column);
                        controlColumnSpan = GetColumnSpan(controlConfig.Position.ColumnSpan);
                    }
                    else
                    {
                        controlRow = 0;
                        controlColumn = 1;
                        controlColumnSpan = 1;
                    }
                    controlName = controlConfig.Name;

                    Debug.WriteLine($"Name: {controlName}\n - Row: {controlRow}\n - Col: {controlColumn}\n - ColSpan: {controlColumnSpan}");

                    if (controlType.DataType != ControlTypes.CheckBox && 
                        controlType.DataType != ControlTypes.DetailsGrid && 
                        !string.IsNullOrWhiteSpace(controlConfig.Label))
                    {
                        // The CheckBox's Content will be the Label text.
                        lbl = CreateLabel(controlConfig.Label);
                        Grid.SetRow(lbl, controlRow);
                        Grid.SetColumn(lbl, controlColumn - 1);
                        ControlGrid.Children.Add(lbl);
                    }

                    switch (controlType.DataType)
                    {
                        case ControlTypes.CheckBox:
                            chk = CreateCheckBoxControl(controlConfig);
                            chk.Checked += CheckBox_Checked;
                            chk.Unchecked += CheckBox_Unchecked;
                            Grid.SetRow(chk, controlRow);
                            Grid.SetColumn(chk, controlColumn);
                            ControlGrid.Children.Add(chk);
                            _inputControls.Add(chk);
                            break;

                        case ControlTypes.ComboBox:
                            cmb = CreateComboBoxControl(controlConfig);
                            cmb.SelectionChanged += ComboBox_SelectionChanged;
                            Grid.SetRow(cmb, controlRow);
                            Grid.SetColumn(cmb, controlColumn);
                            Grid.SetColumnSpan(cmb, controlColumnSpan);
                            ControlGrid.Children.Add(cmb);
                            _inputControls.Add(cmb);
                            break;

                        case ControlTypes.Date:
                            dateTimeTxt = CreateDateTimeTextBoxControl(controlConfig);
                            dateTimeTxt.TextChanged += DateTimeTextBox_TextChanged;
                            Grid.SetRow(dateTimeTxt, controlRow);
                            Grid.SetColumn(dateTimeTxt, controlColumn);
                            ControlGrid.Children.Add(dateTimeTxt);
                            _inputControls.Add(dateTimeTxt);
                            break;

                        case ControlTypes.Numeric:
                            numericTxt = CreateNumericTextBoxControl(controlConfig);
                            numericTxt.ValueChanged += NumericTextBox_ValueChanged;
                            Grid.SetRow(numericTxt, controlRow);
                            Grid.SetColumn(numericTxt, controlColumn);
                            ControlGrid.Children.Add(numericTxt);
                            _inputControls.Add(numericTxt);
                            break;

                        case ControlTypes.Text:
                            txt = CreateTextBoxControl(controlConfig);
                            txt.TextChanged += TextBox_TextChanged;
                            Grid.SetRow(txt, controlRow);
                            Grid.SetColumn(txt, controlColumn);
                            Grid.SetColumnSpan(txt, controlColumnSpan);
                            ControlGrid.Children.Add(txt);
                            _inputControls.Add(txt);
                            break;

                        case ControlTypes.DetailsGrid:
                            _dataGridControl = CreateDataGridControl(controlConfig);
                            if (_dataGridControl is null)
                                return false;

                            _dataGridControl.DataChanged += DataGrid_DataChanged; 
                            Grid.SetRow(_dataGridControl, controlRow);
                            Grid.SetColumn(_dataGridControl, controlColumn);
                            Grid.SetColumnSpan(_dataGridControl, controlColumnSpan);
                            ControlGrid.Children.Add(_dataGridControl);
                            _inputControls.Add(_dataGridControl);
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create/Initialize Input Controls");
            }
            return false;
        }

        private bool SetSectionData(FormSectionConfig sectionConfig)
        {
            try
            {
                if (sectionConfig.IsItemsSection)
                {
                    _dataGridControl.SetData(_itemsSectionData);
                }
                else
                {
                    _dataTracker = new LCI.Windows.SingleDataTracker<ExpandoObject>();
                    _dataTracker.Rec = _sectionData;
                    _dataTracker.DataChanged += DataTracker_DataChanged;
                    this.DataContext = _dataTracker.Rec;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Initialize Section Data");
            }
            return false;
        }

        private List<ControlConfig> SortedControlConfigs(List<ControlConfig> controlConfigs)
        {
            return controlConfigs.OrderBy(c => c.Position.Row).ThenBy(c => c.Position.Column).ToList(); 
        }

        private int GetColumnSpan(int configColumnSpan)
        {
            /*
             *  There are 3 layout columns per config column. 
             *  This is to accommodate a label and post-control label.
             *  When a control's config column span is > 1, the control's layout column span is: configColumnSpan * 3 - 2 , since it
             *  will span 2 label columns for every column span > 1. 
             * 
             *  For example: 
             *      when configColumnSpan = 1, the layout column span is 1, since 0 label columns are spanned.
             *      when configColumnSpan = 2, the layout column span is 4, since 2 label columns are spanned between the 2 control columns.
             *      when configColumnSpan = 3, the layout column span is 7, since 4 label columns are spanned between the 3 control columns.
             */            
            return configColumnSpan <= 1 ? 1 : configColumnSpan * 3 - 2;
        }

        private int GetColumn(int configColumn)
        {
            /*
             *  There are 3 sets of columns per config column. 
             *  This is to accommodate a label and post-control label.
             *  The config columns will be from 0 to sectionConfig.ColumnCount - 1.
             *  Since there are 3 layout columns per config column, the layout column will be configColumn * 3 + 1.
             * 
             *  For example: 
             *      when configColumn = 0, the layout column is 1.
             *      when configColumn = 1, the layout column is 4.
             *      when configColumn = 2, the layout column is 7.
             */
            return configColumn * 3 + 1;
        }

        private HorizontalAlignment GetHorizontalAlignment(string configHorizontalAlignment)
        {
            switch (configHorizontalAlignment ?? HorizontalAlignments.Left)
            {
                case HorizontalAlignments.Right:
                    return HorizontalAlignment.Right;

                case HorizontalAlignments.Stretch:
                    return HorizontalAlignment.Stretch;

                case HorizontalAlignments.Center:
                    return HorizontalAlignment.Center;

                default:
                    return HorizontalAlignment.Left;
            }
        }

        private VerticalAlignment GetVerticalAlignment(string configVerticalAlignment)
        {
            switch (configVerticalAlignment ?? VerticalAlignments.Center)
            {
                case VerticalAlignments.Top:
                    return VerticalAlignment.Top;

                case VerticalAlignments.Bottom:
                    return VerticalAlignment.Bottom;

                case VerticalAlignments.Stretch:
                    return VerticalAlignment.Stretch;

                default:
                    return VerticalAlignment.Center;
            }
        }

        private int GetTabIndex(int? configTabIndex)
        {
            // TabIndex's default value is int.MaxValue.
            return configTabIndex ?? int.MaxValue;
        }

        #endregion

        #region Methods/Event Handlers

        private void DataTracker_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        private void OnDataChanged()
        {
            _isDataChanged = true;
            DataChanged(this, new EventArgs()); 
        }

        #endregion

        #region Label Methods

        private TextBlock CreateLabel(string labelText)
        {
            return new TextBlock()
            {
                Text = labelText + ":",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(2),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
        }

        #endregion

        #region CheckBox Methods/Event Handlers

        private CheckBox CreateCheckBoxControl(ControlConfig controlConfig)
        {
            var chk = new CheckBox()
            {
                Content = controlConfig.Label,                
                Margin = new Thickness(3),
                VerticalAlignment = GetVerticalAlignment(controlConfig.Position.VerticalAlignment),
                HorizontalAlignment = GetHorizontalAlignment(controlConfig.Position.HorizontalAlignment),
                IsEnabled = !controlConfig.IsReadOnly,
                TabIndex = GetTabIndex(controlConfig.TabIndex),
                Tag = controlConfig
            };

            chk.SetBinding(
                CheckBox.IsCheckedProperty, 
                new Binding(controlConfig.Name)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            );

            return chk;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCheckBoxCheckedChanged(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CheckBox Checked");
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                OnCheckBoxCheckedChanged(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CheckBox Unchecked");
            }
        }

        private void OnCheckBoxCheckedChanged(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            var controlConfig = (ControlConfig)chk.Tag;
            var propertyName = controlConfig.Name;

            //_sectionData.SaveBooleanValue(propertyName, (bool)chk.IsChecked);
            //OnDataChanged();
        }

        #endregion

        #region ComboBox Methods/Event Handlers

        private ComboBox CreateComboBoxControl(ControlConfig controlConfig)
        {
            var cmb = new ComboBox()
            {
                Margin = new Thickness(3),
                VerticalAlignment = GetVerticalAlignment(controlConfig.Position.VerticalAlignment),
                HorizontalAlignment = GetHorizontalAlignment(controlConfig.Position.HorizontalAlignment),
                IsReadOnly = controlConfig.IsReadOnly,
                TabIndex = GetTabIndex(controlConfig.TabIndex),
                Tag = controlConfig,
                ItemsSource = GetComboBoxSelectionItems(
                    controlConfig.ControlType.ComboBoxConfig.SelectionItems,
                    controlConfig.IsRequired
                )
            };

            cmb.SetBinding(
                ComboBox.SelectedValueProperty,
                new Binding(controlConfig.Name)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            );

            return cmb;
        }

        private List<string> GetComboBoxSelectionItems(string selectionItems, bool isRequired)
        {
            var items = selectionItems.
                            Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).
                            ToList();

            if (!isRequired)
                items.Insert(0, "");

            return items;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // TODO: ComboBox_SelectionChanged
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ComboBox Selection Changed");
            }
        }

        #endregion

        #region DateTimeTextBox Methods/Event Handlers

        private LCI.Controls.DateTimeTextBox CreateDateTimeTextBoxControl(ControlConfig controlConfig)
        {
            var dateControlConfig = controlConfig.ControlType.DateControlConfig;
            var txt = new LCI.Controls.DateTimeTextBox()
            {
                Margin = new Thickness(3),
                VerticalAlignment = GetVerticalAlignment(controlConfig.Position.VerticalAlignment),
                HorizontalAlignment = GetHorizontalAlignment(controlConfig.Position.HorizontalAlignment),
                Width = _inputControlFixedWidth,
                IsReadOnly = controlConfig.IsReadOnly,
                TabIndex = GetTabIndex(controlConfig.TabIndex),
                Tag = controlConfig,
                DateFormat = dateControlConfig.GetDateFormat(),
                Mask = dateControlConfig.GetMask(),
                DateTimeButtonType = Utilities.GetDateTimeButtonType(dateControlConfig.GetButtonType()),
                DropDownMode = Utilities.GetDateTimeDropDownMode(dateControlConfig.GetDropDownType())
            };

            txt.SetBinding(
                LCI.Controls.DateTimeTextBox.DateValueProperty,
                new Binding(controlConfig.Name)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            );

            return txt;
        }

        private void DateTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var txt = (LCI.Controls.DateTimeTextBox)sender;
                var controlConfig = (ControlConfig)txt.Tag;
                var propertyName = controlConfig.Name;

                //if (_sectionData.SaveDateTimeValue(propertyName, txt.DateValue))
                //OnDataChanged();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Date/Time Changed");
            }
        }

        #endregion

        #region NumericTextBox Methods/Event Handlers

        private LCI.Controls.NumericTextBox CreateNumericTextBoxControl(ControlConfig controlConfig)
        {
            var txt = new LCI.Controls.NumericTextBox()
            {
                Margin = new Thickness(3),
                VerticalAlignment = GetVerticalAlignment(controlConfig.Position.VerticalAlignment),
                HorizontalAlignment = GetHorizontalAlignment(controlConfig.Position.HorizontalAlignment),
                Width = _inputControlFixedWidth,
                IsReadOnly = controlConfig.IsReadOnly,
                TabIndex = GetTabIndex(controlConfig.TabIndex),
                Tag = controlConfig
            };

            var numericControlConfig = controlConfig.ControlType.NumericControlConfig;
            if ((numericControlConfig.MaxIntegerDigits ?? 0) > 0)
                txt.MaxIntegerDigits = numericControlConfig.MaxIntegerDigits.Value;

            if (numericControlConfig.MaxDecimalDigits > 0)
                txt.MaxDeimalDigits = numericControlConfig.MaxDecimalDigits;

            txt.AllowNegative = numericControlConfig.AllowNegative;


            var converter = (Converters.DoubleToDecimalConverter)Application.Current.Resources["DoubleToDecimalConverter"];

            txt.SetBinding(
                LCI.Controls.NumericTextBox.ValueProperty,
                new Binding(controlConfig.Name)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    TargetNullValue = string.Empty,
                    Converter = converter
                }
            );

            return txt;
        }

        private void NumericTextBox_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var txt = (LCI.Controls.NumericTextBox)sender;
                var controlConfig = (ControlConfig)txt.Tag;
                var propertyName = controlConfig.Name;

                //if (_sectionData.SaveNumericValue(propertyName, txt.Value))
                //    OnDataChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Numeric Value Changed");
            }
        }

        #endregion

        #region Textbox Methods/Event Handlers

        private TextBox CreateTextBoxControl(ControlConfig controlConfig)
        {
            var txt = new TextBox()
            {
                Margin = new Thickness(3),
                VerticalAlignment = GetVerticalAlignment(controlConfig.Position.VerticalAlignment),
                HorizontalAlignment = GetHorizontalAlignment(controlConfig.Position.HorizontalAlignment),
                IsReadOnly = controlConfig.IsReadOnly,
                TabIndex = GetTabIndex(controlConfig.TabIndex),
                MaxLength = controlConfig.ControlType.TextControlConfig.MaxLength,
                Tag = controlConfig
            };
            if (controlConfig.ControlType.TextControlConfig.IsMultiLine)
            {
                txt.AcceptsReturn = true;
                txt.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                txt.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                txt.MinHeight = 24;
                txt.Height = double.NaN;
            }
            else
            {
                txt.AcceptsReturn = false;
                txt.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                txt.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                txt.Height = 24;
            }

            txt.SetBinding(
                TextBox.TextProperty,
                new Binding(controlConfig.Name)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                }
            );

            return txt;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var txt = (TextBox)sender;
                var controlConfig = (ControlConfig)txt.Tag;
                var propertyName = controlConfig.Name;

                //if (_sectionData.SaveTextValue(propertyName, txt.Text))
                //    OnDataChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TextBox Text Changed");
            }
        }

        #endregion

        #region DataGrid Methods/Event Handlers

        private DataGridEditControl CreateDataGridControl(ControlConfig controlConfig)
        {
            try
            {
                return new DataGridEditControl(controlConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create Data Grid Control");
            }
            return null;
        }

        private void DataGrid_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        #endregion

        #region Validate Methods/Event Handlers

        public bool ValidRecord()
        {
            try
            {
                ControlConfig controlConfig;

                foreach (var inputControl in _inputControls)
                {
                    if (inputControl.Tag is null)
                        continue;

                    controlConfig = (ControlConfig)inputControl.Tag;
                    if (!RequiredFieldPopulated(inputControl, controlConfig))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _sectionName + " - Valid Record");
            }
            return false;
        }

        private bool RequiredFieldPopulated(Control inputControl, ControlConfig controlConfig)
        {
            try
            {
                if (!controlConfig.IsRequired)
                    return true;

                var controlName = controlConfig.Label;
                var messageFormat = controlName + " must be {0}.";
                var messageTitle = _sectionName + " - Required Entry";

                switch (inputControl)
                {
                    case ComboBox cmb:
                        if (cmb.SelectedIndex == -1)
                        {
                            MessageBox.Show(string.Format(messageFormat, "selected"), messageTitle);
                            return false;
                        }
                        break;

                    case LCI.Controls.DateTimeTextBox dateTimeTxt:
                        if (dateTimeTxt.DateValue is null)
                        {
                            MessageBox.Show(string.Format(messageFormat, "entered"), messageTitle);
                            return false;
                        }
                        break;

                    case LCI.Controls.NumericTextBox numericTxt:
                        if (numericTxt.Value is null)
                        {
                            MessageBox.Show(string.Format(messageFormat, "entered"), messageTitle);
                            return false;
                        }
                        break;
                        
                    case LCI.Controls.TextBox txt:
                        if (string.IsNullOrWhiteSpace(txt.Text))
                        {
                            MessageBox.Show(string.Format(messageFormat, "entered"), messageTitle);
                            return false;
                        }
                        break;
                }               

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, _sectionName + " - Required Field Populated");
            }
            return false;
        }

        #endregion

        #region Helper Classes/Enums

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

        #endregion
    }
}
