using System;

namespace DynamicForms
{
    internal class FormHeaderData
    {

        public int RecId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Version { get; set; }

        public void Create()
        {
            this.RecId = PropertyManager.LastFormRecId + 1;
            PropertyManager.LastFormRecId = this.RecId;
            PropertyManager.Save();

            this.CreatedBy = Environment.UserName;
            this.CreatedDate = DateTime.Now;
            this.Version = 1;
        }

        public void Update()
        {
            this.UpdatedBy = Environment.UserName;
            this.UpdatedDate = DateTime.Now;
            this.Version += 1;
        }
    }
}