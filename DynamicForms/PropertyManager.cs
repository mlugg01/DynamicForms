using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicForms
{
    internal class PropertyManager
    {
        public static void Save() => Properties.Settings.Default.Save();

        public static string FormConfigJson
        {
            get => Properties.Settings.Default.FormConfigJson;
            set => Properties.Settings.Default.FormConfigJson = value;
        }

        public static string FormDataJson
        {
            get => Properties.Settings.Default.FormDataJson;
            set => Properties.Settings.Default.FormDataJson = value;
        }

        public static int LastFormRecId
        {
            get => Properties.Settings.Default.LastFormRecId;
            set => Properties.Settings.Default.LastFormRecId = value;
        }
    }
}
