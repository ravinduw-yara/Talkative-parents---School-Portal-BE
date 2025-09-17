using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? SBAccessRankId { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    //public class MCategoryUpdateModel : MCategoryModel
    //{
    //    public int Id { get; set; }
    //}

    public class GetCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SbAccessRankId { get; set; }
        public string RankName { get; set; }
        public string Remarks { get; set; }
        public int SelectionType { get; set; }
    }

    public class GetCategory_APP
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
