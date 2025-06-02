using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DynamicForms
{
    internal class Serialization<T>
    {
        public static string SerializeToJson(T rec)
        {
            return JsonConvert.SerializeObject(
                rec, 
                Formatting.Indented, 
                new JsonSerializerSettings {  NullValueHandling = NullValueHandling.Ignore }
            );
        }

        public static T DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
