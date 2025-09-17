using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class PushCalNotiModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        //public int? StandardId { get; set; }
        //public int? SectionId { get; set; }
        public int[] StandardId { get; set; }
        public int[] SectionId { get; set; }
        public int? Schoolid { get; set; }
        public string Venue { get; set; }
        public string Attachment { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
    }
    public class PostCalendereventdetailsModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string Venue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public int? Standardsectionmappingid { get; set; }
        public int[] StandardId { get; set; }
        public int[] SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Attachment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        //public int? Statusid { get; set; }
    }
}
