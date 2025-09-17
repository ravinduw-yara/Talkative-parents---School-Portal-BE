using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MStatusModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Statustypeid { get; set; }
        public bool? Isactive { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
    }

    public class MStatusUpdateModel : MStatusModel
    {
        public int Id { get; set; }
    }
}
