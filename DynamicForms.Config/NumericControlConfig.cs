using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class NumericControlConfig
    {
        public int? MaxIntegerDigits { get; set; }
        public int MaxDecimalDigits { get; set; } = 0;
        public bool AllowNegative { get; set; } = false;
    }
}
