using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MLocationModel
    {
        public string Name { get; set; }
        public int? Parentid { get; set; }
        public int? Locationtypeid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }
    public class MLocationUpdateModel : MLocationModel
    {
        public int Id { get; set; }
    }
}
