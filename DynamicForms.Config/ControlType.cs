using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class ControlType
    {
        public string DataType { get; set; }
        public ComboBoxControlConfig ComboBoxConfig { get; set; }
        public DateControlConfig DateControlConfig { get; set; }
        public NumericControlConfig NumericControlConfig { get; set; }
        public TextControlConfig TextControlConfig { get; set; }
        public DetailGridConfig DetailGridConfig { get; set; }
    }
}
