using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DynamicForms.Config;
using DynamicForms.Controls;

namespace DynamicForms
{
    /// <summary>
    /// Interaction logic for DynamicWnd.xaml
    /// </summary>
    public partial class DynamicWnd : Window
    {
        private FormConfig _formConfig;
        private List<FormSection> _formSections;
        private FormData _formData;

        #region Window Methods, Event Handlers, UI Build Methods

        public DynamicWnd()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _formConfig = GetFormConfig();
            if (_formConfig is null)
                return;

            LoadRecordAndBuildUi();
        }

        private FormConfig GetFormConfig()
        {
            string formConfigJson = PropertyManager.FormConfigJson;
            if (string.IsNullOrWhiteSpace(formConfigJson))
                return null;

            return Serialization<FormConfig>.DeserializeJson(formConfigJson);
        }

        private void LoadRecordAndBuildUi()
        {
            _formData = GetFormData();
            if (_formData is null)
                return;

            if (!SetFormHeader())
                return;

            if (!BuildUi(_formConfig))
                return;

            ConfigureToolBarButtons();
        }

        private FormData GetFormData()
        {
            string formDataJson = PropertyManager.FormDataJson;
            if (string.IsNullOrWhiteSpace(formDataJson))
                return new FormData();

            return Serialization<FormData>.DeserializeJson(formDataJson);
        }

        private bool SetFormHeader()
        {
            try
            {
                var formHeaderData = _formData.GetFormHeaderData();
                this.DataContext = formHeaderData;
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Set Form Header");
            }
            return false;
        }
        
        private bool BuildUi(FormConfig formConfig)
        {
            try
            {
                controlGrid.Children.Clear();
                controlGrid.RowDefinitions.Clear();
                _formSections = new List<FormSection>();

                int gridRow = 0;
                FormSection section;
                foreach (var sectionConfig in formConfig.Sections.OrderBy(s => s.DisplayOrder))
                {
                    controlGrid.RowDefinitions.Add(NewRowDefinition());

                    section = NewFormSection(sectionConfig);
                    if (section is null)
                        return false;

                    section.DataChanged += FormSection_DataChanged;
                    _formSections.Add(section);
                    controlGrid.Children.Add(section);
                    Grid.SetRow(section, gridRow);                 

                    gridRow++;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(BuildUi));   
            }
            return false;
        }

        private RowDefinition NewRowDefinition()
        {
            return new RowDefinition
            {
                Height = GridLength.Auto
            };
        }

        private FormSection NewFormSection(FormSectionConfig sectionConfig)
        {
            try
            {
                ExpandoObject sectionData = null;
                List<ExpandoObject> itemsSectionData = null;
                if (sectionConfig.IsItemsSection)
                {
                    itemsSectionData = _formData.GetItemsSectionData(sectionConfig.SectionName);
                }
                else
                {
                    sectionData = _formData.GetSectionData(sectionConfig.SectionName);
                }

                var formSection = new FormSection(sectionConfig, sectionData, itemsSectionData);
                return formSection;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(NewFormSection) + " - " + sectionConfig.SectionName);
            }
            return null;
        }

        #endregion

        #region Button-click Event Handlers

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!SaveRecord())
                return;

            LoadRecordAndBuildUi();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Discard all changes?", "Confirm Undo", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            LoadRecordAndBuildUi();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods, Event Handlers

        private void FormSection_DataChanged(object sender, EventArgs e)
        {
            ConfigureToolBarButtons();
        }

        private void ConfigureToolBarButtons()
        {
            bool isDataChanged = IsDataChanged();
            btnSave.IsEnabled = isDataChanged;
            btnUndo.IsEnabled = isDataChanged;
        }

        private bool IsDataChanged()
        {
            return _formSections.Any(s => s.IsDataChanged);
        }

        private bool SaveRecord()
        {
            try
            {
                if (!ValidRecord())
                    return false;

                foreach (var section in _formSections)
                {
                    if (section.IsDataChanged)
                    {
                        _formData.SaveSectionData(section.SectionName, section.Data);
                        _formData.SaveItemsSectionData(section.SectionName, section.ItemsData);
                    }
                }

                _formData.SaveFormHeaderData();

                string formDataJson = Serialization<FormData>.SerializeToJson(_formData);
                if (string.IsNullOrWhiteSpace(formDataJson))
                    return false;

                PropertyManager.FormDataJson = formDataJson;
                PropertyManager.Save();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save");
            }
            return false;
        }

        private bool ValidRecord()
        {
            try
            {
                foreach (var section in _formSections)
                {
                    if (!section.ValidRecord())
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Title + " - Valid Record");
            }
            return false;
        }

        #endregion
    }
}
