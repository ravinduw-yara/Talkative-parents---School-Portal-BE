using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MStatustypeModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }

    }

    public class MStatustypeUpdateModel : MStatustypeModel
    {
        public int Id { get; set; }
    }
}
