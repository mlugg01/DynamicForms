using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicForms.Config
{
    public class ControlConfig
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public ControlType ControlType { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
        public ControlPosition Position { get; set; }
        public int? TabIndex { get; set; }
    }
}
