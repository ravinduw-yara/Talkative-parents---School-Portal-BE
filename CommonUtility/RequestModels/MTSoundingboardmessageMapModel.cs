using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTSoundingboardmessageMapModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public int? CategoryKey { get; set; }
        public string CategoryName { get; set; }
        public string ParentName { get; set; }
        public string ChildName { get; set; }
        public string Relation { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int? CommentsCount { get; set; }
        public int? LikesCount { get; set; }
        public string Attachment { get; set; }
        public int? AttachmentCount { get; set; }
        public bool? IsStaffReplied { get; set; }
        public bool? IsParentReplied { get; set; }
        public int? Schoolid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? DidRead { get; set; }
        public int? IsActive { get; set; }
        public int? Type { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

}
