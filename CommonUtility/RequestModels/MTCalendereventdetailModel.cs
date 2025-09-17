using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class MTCalendereventdetail
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public int? Standardsectionmappingid { get; set; }
        public int? Schoolid { get; set; }
        public string? Attachment { get; set; }
        public DateTime? Createddate { get; set; }
        public DateTime? Modifieddate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

    public class TCalendereventdetailUpdateModel : MTCalendereventdetail
    {
        public int Id { get; set; }
    }

    public class ReadCalendarEvents
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string Venue { get; set; }
        public string Attachment { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }


    public class CalendereventdetailModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string Venue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Attachment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        public int? Statusid { get; set; }
    }

}
