using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class ComboBoxControlConfig
    {
        public string SelectionItems { get; set; }

        public List<string> GetSelectionItems()
        {
            var selectionItems = new List<string>();

            if (!string.IsNullOrWhiteSpace(this.SelectionItems))
            {
                foreach (var item in this.SelectionItems.Split(','))
                    selectionItems.Add(item.Trim());
            }
                
            return selectionItems;
        }
    }
}
