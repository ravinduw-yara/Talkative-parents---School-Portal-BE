using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTSoundingboardmessageModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool? Isparentreplied { get; set; }
        public bool? Isstaffreplied { get; set; }
        public string Attachments { get; set; }
        public int? Commentscount { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
        public int? Appuserinfoid { get; set; }
        public int? Childinfoid { get; set; }
        public int? Categoryid { get; set; }
        public int? Groupid { get; set; }
        public bool? Didread { get; set; }
    }

    public class MTSoundingboardmessageUpdteModel : MTSoundingboardmessageModel
    {
        public int Id { get; set; }
    }

    public class SoundingboardmessagePublicAndPrivate
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public int? CategoryKey { get; set; }
        public string CategoryName { get; set; }
        public string ParentName { get; set; }
        public string ParentMiddleName { get; set; }
        public string ParentLastName { get; set; }
        public string ChildName { get; set; }
        public string ChildLastName { get; set; }
        public string ChildMiddleName { get; set; }
        public string Relation { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public string StandardName { get; set; }
        public string SectionName { get; set; }
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
        public string Type { get; set; }
        public int? SBcount { get; set; }
        public int? schooluserid { get; set; }


        // public string CreateDatelong { get; set; }
        // public string UpdateDatelong { get; set; }
        //public Guid CreatedById { get; set; }
        //public string CreatedByName { get; set; }

        public class Model
        {
            public bool DidRead { get; set; }
            public bool IsStaffReplied { get; set; }
            public bool IsParentReplied { get; set; }
        }

        public class SBMessagePushNotiModel
        {
            public int? Id { get; set; }
            public int? Schoolid { get; set; }
            public int? ParentId { get; set; }
            public int? ChildId { get; set; }
            public int? StandardId { get; set; }
            public int? SectionId { get; set; }
            public string Subject { get; set; }
        }
        public class SendSBMessageNotificationModel
        {
            public int? MsgId { get; set; }
            public int? Schoolid { get; set; }
            public int? ParentId { get; set; }
            public int? ChildId { get; set; }
            public string ChildName { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string Description { get; set; }
            public int CommentsCount { get; set; }
            public string CategoryName { get; set; }
            public string Attachment { get; set; }
            //public bool IsStaffReplied { get; set; }
            public bool IsParentReplied { get; set; }
            public bool DidRead { get; set; }
        }

        public class SBMessageStatus
        {
            public int Status { get; set; }
            public int SbCount { get; set; }
        }


        //app model
        public class NotificationDetailModel
        {
            public int Id { get; set; }
            public DateTime DateTime { get; set; }
            public string Message { get; set; }
            public string Subject { get; set; }
            public string Attachments { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
        }

        public class Device
        {
            public string Deviceid { get; set; }
            public int DeviceType { get; set; }
        }

        public class ParentChannel
        {
            public string channel { get; set; }
            public Common.ChannelType channelType { get; set; }
        }

        public class PostCommentModel
        {
            public int id { get; set; }
            public string Sucject { get; set; }
            public string Description { get; set; }
            public bool IsParentReplied { get; set; }
            public bool DidRead { get; set; }
        }


    }

}
