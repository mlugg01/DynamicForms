using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class ControlPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan { get; set; }
        public int ColumnSpan { get; set; }
        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }
    }
}
