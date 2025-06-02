using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicForms.Config
{
    public class DateControlConfig
    {
        public bool DateOnly { get; set; }
        public bool HasButton { get; set; } 
        public bool HasDropDown { get; set; }   
        
        public string GetDateFormat() => "MM/dd/yy" + (this.DateOnly ? "" : " HH:mm");
        public string GetMask() => "99/99/99" + (this.DateOnly ? "" : " 99:99");
        
        public string GetButtonType() => this.HasButton ?
                                            (this.DateOnly ? 
                                                DateControlButtonTypes.Today : 
                                                DateControlButtonTypes.Now) :
                                            DateControlButtonTypes.None;

        public string GetDropDownType() => this.HasDropDown ? 
                                                (this.DateOnly ? 
                                                    DateControlDropDownTypes.DateOnly : 
                                                    DateControlDropDownTypes.DateAndTime) :
                                                DateControlDropDownTypes.None;                          
    }                                           
}
