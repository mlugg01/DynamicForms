using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicForms.Config
{
    public class DetailGridConfig
    {
        public List<ControlConfig> Columns { get; set; } = new List<ControlConfig>();
        //TODO: DetailItemSortOptions
    }
}
