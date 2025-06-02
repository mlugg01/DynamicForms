using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class FormConfig
    {
        public int ConfigId { get; set; }
        public int CustId { get; set; }
        public int FormType { get; set; }
        public string Title { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int PublishedByUserId { get; set; }
        public List<FormSectionConfig> Sections { get; set; } = new List<FormSectionConfig>();
    }
}
