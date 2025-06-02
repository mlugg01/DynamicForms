using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicForms
{
    internal class FormData : FormHeaderData
    {
        public int FormTypeId { get; set; }
        public int FormTypeVersion { get; set; }

        public FormData()
        {
            _formHeaderData = new FormHeaderData();
            _sectionData = new Dictionary<string, ExpandoObject>();
            _itemsSectionData = new Dictionary<string, List<ExpandoObject>>();
        }


        [JsonProperty]
        private FormHeaderData _formHeaderData;

        public FormHeaderData GetFormHeaderData()
        {
            return _formHeaderData;
        }

        public void SaveFormHeaderData()
        {
            if (_formHeaderData.RecId == 0)
                _formHeaderData.Create();
            else
                _formHeaderData.Update();
        }

        
        [JsonProperty]
        private Dictionary<string, ExpandoObject> _sectionData;

        public void SaveSectionData(string sectionName, ExpandoObject sectionData)
        {
            _sectionData[sectionName] = sectionData;
        }

        public ExpandoObject GetSectionData(string sectionName)
        {
            if (_sectionData.TryGetValue(sectionName, out var sectionData) && (sectionData != null))
                return sectionData;
            
            return new ExpandoObject();
        }


        [JsonProperty]
        private Dictionary<string, List<ExpandoObject>> _itemsSectionData;
        public void SaveItemsSectionData(string sectionName, List<ExpandoObject> itemsSectionData)
        {
            _itemsSectionData[sectionName] = itemsSectionData;
        }

        public List<ExpandoObject> GetItemsSectionData(string sectionName)
        {
            if (_itemsSectionData.TryGetValue(sectionName, out var itemsSectionData) && (itemsSectionData != null))
                return itemsSectionData;

            return new List<ExpandoObject>();
        }
    }
}
