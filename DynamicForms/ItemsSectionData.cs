using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DynamicForms
{
    public class ItemsSectionData //: SectionData
    {
        public int RecId { get; set; }
        public dynamic Data { get; set; }

        public ItemsSectionData()
        {
            Data = new ExpandoObject();
        }
    }
}
