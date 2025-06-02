using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicForms
{
    public class SectionData
    {
        [JsonProperty]
        protected Dictionary<string, object> _sectionData;

        public SectionData()
        {
            _sectionData = new Dictionary<string, object>();    
        }

        public void DeleteValue(string propertyName)
        {
            _sectionData.Remove(propertyName);
        }

        public bool GetBooleanValue(string propertyName)
        {
            if (_sectionData.TryGetValue(propertyName, out object o) && (o != null))
                return bool.Parse(o.ToString());
            return false;
        }
        
        public void SaveBooleanValue(string propertyName, bool value)
        {
            if (value)
            {
                Debug.WriteLine("boolean saved");

                _sectionData[propertyName] = value;
            }
            else
            {
                Debug.WriteLine("boolean cleared");

                DeleteValue(propertyName);
            }
        }

        public DateTime? GetDateTimeValue(string propertyName)
        {
            if (_sectionData.TryGetValue(propertyName, out object o) && (o != null))
                return DateTime.Parse(o.ToString());
            return null;
        }

        public bool SaveDateTimeValue(string propertyName, DateTime? value)
        {
            var savedDate = GetDateTimeValue(propertyName) ?? DateTime.MinValue;
            var currentDate = value ?? DateTime.MinValue;
            if (currentDate != DateTime.MinValue && currentDate != savedDate)
            {
                Debug.WriteLine("date saved");

                _sectionData[propertyName] = value.Value.ToString("yyyy-MM-dd HH:mm", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                return true;
            }
            else if (savedDate != DateTime.MinValue)
            {
                Debug.WriteLine("date cleared");

                DeleteValue(propertyName);
                return true;
            }
            return false;
        }

        public decimal? GetNumericValue(string propertyName)
        {
            if (_sectionData.TryGetValue(propertyName, out object o) && (o != null))
                return decimal.Parse(o.ToString());
            return null;
        }

        public bool SaveNumericValue(string propertyName, decimal? value)
        {
            var savedValue = GetNumericValue(propertyName) ?? decimal.MinValue;
            var currentValue = value ?? decimal.MinValue;
            if (currentValue != decimal.MinValue && currentValue != savedValue)
            {
                Debug.WriteLine("numeric value saved");

                _sectionData[propertyName] = currentValue;
                return true;
            }
            else if (savedValue != decimal.MinValue)
            {
                Debug.WriteLine("numeric value cleared");

                DeleteValue(propertyName);
                return true;
            }
            return false;
        }

        public string GetTextValue(string propertyName)
        {
            if (_sectionData.TryGetValue(propertyName, out object o) && (o != null))
                return o.ToString();
            return string.Empty;
        }

        public bool SaveTextValue(string propertyName, string value)
        {
            var savedValue = GetTextValue(propertyName);
            var currentValue = value ?? string.Empty;
            if (currentValue != string.Empty && savedValue != currentValue)
            {
                Debug.WriteLine("text value saved");

                _sectionData[propertyName] = currentValue;
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(savedValue))
            {
                Debug.WriteLine("text value cleared");

                DeleteValue(propertyName);
                return true;
            }
            return false;
        }
    }
}
