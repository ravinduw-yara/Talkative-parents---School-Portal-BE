using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MChildschoolmappingModel
    {
        public int? Childid { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? DRCEnable1 { get; set; }
        public int? DRCEnable2 { get; set; }
        public int? DRCEnable3 { get; set; }
        public int? Promoted { get; set; }
    }
    public class StudentSection
    {
        public int StandardId { get; set; }
        public string StandardName { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; }

    }
    public class MChildschoolmappingUpdateModel : MChildschoolmappingModel
    {
        public int Id { get; set; }
    }
}
