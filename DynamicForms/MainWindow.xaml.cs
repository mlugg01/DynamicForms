using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Windows;
using DynamicForms.Config;



namespace DynamicForms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void btnGenerateConfig_Click(object sender, RoutedEventArgs e)
        {
            var formConfig = GenerateFormConfig();
            if (formConfig is null)
                return;

            string formConfigJson = CreateFormConfigJson(formConfig);
            if (string.IsNullOrWhiteSpace(formConfigJson))
                return;

            if (!SaveFormConfigJson(formConfigJson))
                return;

            Debug.WriteLine(formConfigJson);
        }
        
        private void btnLaunchForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wnd = new DynamicWnd();
                wnd.ShowDialog();                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(btnLaunchForm_Click));
            }
        }

        private FormConfig GenerateFormConfig()
        {
            try
            {
                // all control columns should start at 0 and work up to the max allowed, to be consistent across platforms.  
                // The specific placement of the control will be handled appropriately by the client technology's layout.
               
                var formConfig = new FormConfig();
                formConfig.ConfigId = 1;
                formConfig.CustId = 17;
                formConfig.FormType = 1;
                formConfig.PublishedByUserId = 23;
                formConfig.PublishedDate = DateTime.Now;
                formConfig.Title = "Transportation Quote";

                #region Header Section

                var section = new FormSectionConfig
                {
                    SectionName = "Header",
                    SectionHeader = "",
                    DisplayOrder = 0,
                    ColumnCount = 3, // In the WPF window, there will be 3 columns per column set (Label column, control column, post label column).  So, when creating the layout, multiply by 3 to get the final column count.
                    RowCount = 3,
                    DisplaySectionHeader = false
                };
                formConfig.Sections.Add(section);

                var controlConfig = new ControlConfig
                {
                    Name = "Customer",
                    Label = "Customer",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 100,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 0,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Center
                    },
                    IsRequired = true,
                    IsReadOnly = false
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PriorityShipping",
                    Label = "Priority Shipping",
                    ControlType= new ControlType
                    {
                        DataType = ControlTypes.CheckBox
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 1,
                        Row = 1,
                        HorizontalAlignment = HorizontalAlignments.Left,
                        VerticalAlignment= VerticalAlignments.Center
                    },
                    IsRequired = false,
                    IsReadOnly = false
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "ScheduledDate",
                    Label = "Scheduled",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Date,
                        DateControlConfig = new DateControlConfig
                        {
                            DateOnly = false,
                            HasButton = true,
                            HasDropDown = true
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 1,
                        Row = 0,
                        HorizontalAlignment = HorizontalAlignments.Left,
                        VerticalAlignment = VerticalAlignments.Center
                    },
                    IsRequired = true,
                    IsReadOnly = false
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "ShippingQuote",
                    Label = "Quote",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Numeric,
                        NumericControlConfig = new NumericControlConfig
                        {
                            AllowNegative = false,
                            MaxDecimalDigits = 2
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 1,
                        Row = 1,
                        HorizontalAlignment = HorizontalAlignments.Left,
                        VerticalAlignment = VerticalAlignments.Center
                    },
                    IsRequired = true,
                    IsReadOnly = false
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "ShippingDescription",
                    Label = "Description",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 2000,
                            IsMultiLine = true
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 3,
                        Row = 2,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false
                };
                section.Controls.Add(controlConfig);

                #endregion

                #region Shipping Info Section

                section = new FormSectionConfig
                {
                    SectionName = "ShippingInfo",
                    SectionHeader = "Shipping Info",
                    DisplayOrder = 10,
                    ColumnCount = 4,
                    RowCount = 7,
                    DisplaySectionHeader = true
                };
                formConfig.Sections.Add(section);

                controlConfig = new ControlConfig
                {
                    Name = "PickupAddressLine1",
                    Label = "Pickup Address",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 0,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 0
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupAddressLine2",
                    Label = "",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 1,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = false,
                    IsReadOnly = false,
                    TabIndex = 1
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupCity",
                    Label = "City",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 2,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 2
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupStateProvince",
                    Label = "State/Province",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 1,
                        Row = 3,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 3
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupPostalCode",
                    Label = "Postal",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 10,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 1,
                        Row = 4,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 4
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupContact",
                    Label = "Contact",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 5,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 5
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "PickupPhone",
                    Label = "Phone",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 20,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 2,
                        Row = 6,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 6
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryAddressLine1",
                    Label = "Delivery Address",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 2,
                        Row = 0,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 7
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryAddressLine2",
                    Label = "",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 2,
                        Row = 1,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = false,
                    IsReadOnly = false,
                    TabIndex = 8
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryCity",
                    Label = "City",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 2,
                        Row = 2,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 9
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryStateProvince",
                    Label = "State/Province",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 1,
                        Row = 3,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 10
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryPostalCode",
                    Label = "Postal",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 10,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 1,
                        Row = 4,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 11
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryContact",
                    Label = "Contact",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 50,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 2,
                        Row = 5,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 12
                };
                section.Controls.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "DeliveryPhone",
                    Label = "Phone",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig
                        {
                            MaxLength = 20,
                            IsMultiLine = false
                        }
                    },
                    Position = new ControlPosition
                    {
                        Column = 2,
                        ColumnSpan = 2,
                        Row = 12,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 13                
                };
                section.Controls.Add(controlConfig);

                #endregion

                #region Shipping Item Details

                section = new FormSectionConfig
                {
                    SectionName = "ShippingItemDetails",
                    SectionHeader = "Shipping Item Details",
                    IsItemsSection = true,
                    ColumnCount = 1,
                    RowCount = 1,
                    DisplayOrder = 20,
                    DisplaySectionHeader = true,
                    MathematicalFormula = "VAR:Weight * VAR:UnitPrice"
                };
                formConfig.Sections.Add(section);

                var detailGridConfig = new ControlConfig
                {
                    Name = "grdItems",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.DetailsGrid,
                        DetailGridConfig = new DetailGridConfig()
                    },
                    Position = new ControlPosition
                    {
                        Column = 0,
                        ColumnSpan = 1,
                        Row = 0,
                        HorizontalAlignment = HorizontalAlignments.Stretch,
                        VerticalAlignment = VerticalAlignments.Stretch
                    },
                    IsRequired = true
                };
                section.Controls.Add(detailGridConfig);

                var columns = new List<ControlConfig>();
                controlConfig = new ControlConfig
                {
                    Name = "ItemId",
                    Label = "Item ID",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig()
                        {
                            MaxLength = 10
                        }               
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 0
                };
                columns.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "Description",
                    Label = "Description",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Text,
                        TextControlConfig = new TextControlConfig()
                        {
                            MaxLength = 100
                        }                        
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 1
                };
                columns.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "Weight",
                    Label = "Weight (kg)",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Numeric,
                        NumericControlConfig = new NumericControlConfig()
                        {
                            MaxDecimalDigits = 3
                        }
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 2
                };
                columns.Add(controlConfig);

                controlConfig = new ControlConfig
                {
                    Name = "UnitPrice",
                    Label = "Unit Price ($/kg)",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Numeric,
                        NumericControlConfig = new NumericControlConfig()
                        {
                            MaxDecimalDigits = 3
                        }
                    },
                    IsRequired = true,
                    IsReadOnly = false,
                    TabIndex = 3
                };
                columns.Add(controlConfig);

                // TODO: add calculation logic.
                controlConfig = new ControlConfig
                {
                    Name = "Price",
                    Label = "Price ($)",
                    ControlType = new ControlType
                    {
                        DataType = ControlTypes.Numeric,
                        NumericControlConfig = new NumericControlConfig()
                        {
                            MaxDecimalDigits = 2
                        }
                    },
                    IsRequired = true,
                    IsReadOnly = true,
                    TabIndex = 4
                };
                columns.Add(controlConfig);
                detailGridConfig.ControlType.DetailGridConfig.Columns = columns;

                #endregion

                return formConfig;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(GenerateFormConfig));
            }
            return null;
        }

        private string CreateFormConfigJson(FormConfig formConfig)
        {
            try
            {
                return Serialization<FormConfig>.SerializeToJson(formConfig);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(CreateFormConfigJson));
            }
            return null;
        }

        private bool SaveFormConfigJson(string formConfigJson)
        {
            try
            {
                PropertyManager.FormConfigJson = formConfigJson;
                PropertyManager.Save();
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(SaveFormConfigJson));
            }
            return false;
        }

        //private dynamic moData;
        //private LCI.Windows.SingleDataTracker<ExpandoObject> moDataTracker;

        //private dynamic mcolItems;
        //private LCI.Windows.MultiDataTracker<ExpandoObject> moItemsDataTracker;

        //public MainWindow()
        //{
        //    InitializeComponent();

        //    string lcJson = Properties.Settings.Default.DataJson;
        //    Debug.WriteLine("Loaded Data: " + lcJson);
        //    moData = JsonConvert.DeserializeObject<ExpandoObject>(lcJson);

        //    ((INotifyPropertyChanged)moData).PropertyChanged += loData_PropertyChanged;


        //    moDataTracker = new LCI.Windows.SingleDataTracker<ExpandoObject>();
        //    moDataTracker.DataChanged += moDataTracker_DataChanged;
        //    moDataTracker.Rec = moData;
        //    this.DataContext = moDataTracker.Rec;

        //    lcJson = Properties.Settings.Default.ItemsDataJson;
        //    Debug.WriteLine("Loaded Items: " + lcJson);
        //    mcolItems = JsonConvert.DeserializeObject<ObservableCollection<ExpandoObject>>(lcJson);
        //    foreach (var loItem in mcolItems)
        //        ((INotifyPropertyChanged)loItem).PropertyChanged += loData_PropertyChanged;

        //    moItemsDataTracker = new LCI.Windows.MultiDataTracker<ExpandoObject>();
        //    moItemsDataTracker.DataChanged += moDataTracker_DataChanged;
        //    moItemsDataTracker.Recs = mcolItems;
        //    grdItems.ItemsSource = moItemsDataTracker.Recs;


        //}

        //private void moDataTracker_DataChanged(object sender, System.EventArgs e)
        //{
        //    Debug.WriteLine("data changed");
        //}

        //private void loData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    Debug.WriteLine(e.PropertyName);
        //}

        //private void btnSaveData_Click(object sender, RoutedEventArgs e)
        //{
        //    string lcJson = JsonConvert.SerializeObject(moDataTracker.Rec);
        //    Properties.Settings.Default.DataJson = lcJson;
        //    Debug.WriteLine("Saved Data: " + lcJson);

        //    lcJson = JsonConvert.SerializeObject(moItemsDataTracker.Recs);
        //    Properties.Settings.Default.ItemsDataJson = lcJson;
        //    Debug.WriteLine("Saved Items: " + lcJson);

        //    Properties.Settings.Default.Save();
        //}

        //private void btnNew_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        int liNewId = 0;
        //        if (moItemsDataTracker.Recs.Count > 0)
        //        {
        //            int liMinId = moItemsDataTracker.Recs.Min(i => int.Parse(((IDictionary<string, object>)i)["Id"].ToString()));
        //            if (liMinId > 0)
        //                liNewId = 0;
        //            else
        //                liNewId = liMinId - 1;
        //        }


        //        dynamic loNewItem = new ExpandoObject();
        //        loNewItem.Id = liNewId;
        //        MessageBox.Show(loNewItem.Id.GetType().ToString());
        //        loNewItem.Desc = "";
        //        loNewItem.Comment = "";
        //        moItemsDataTracker.Recs.Add(loNewItem); 
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "New");
        //    }
        //}

        //private void btnDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (grdItems.SelectedIndex == -1)
        //            return;
        //        moItemsDataTracker.Recs.RemoveAt(grdItems.SelectedIndex);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Delete");
        //    }
        //}
    }
}
