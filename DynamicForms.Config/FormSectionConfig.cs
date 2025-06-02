using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class FormSectionConfig
    {
        public string SectionName { get; set; }
        public string SectionHeader { get; set; }
        public bool IsItemsSection { get; set; }
        public bool DisplaySectionHeader { get; set; }
        public int DisplayOrder { get; set; }
        public int ColumnCount { get; set; } = 1;
        public int RowCount { get; set; } = 1;
        public List<ControlConfig> Controls { get; set; } = new List<ControlConfig>();
        public string MathematicalFormula { get; set; }
    }
}
