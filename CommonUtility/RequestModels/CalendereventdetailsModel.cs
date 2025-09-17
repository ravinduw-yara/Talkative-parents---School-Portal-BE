using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility.RequestModels
{
    public class CalendereventdetailsModel
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string Venue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public int? Standardsectionmappingid { get; set; }
        public int? StandardId { get; set; }
        public int? SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Attachment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Createdby { get; set; }
        public int? Modifiedby { get; set; }
        //public int? Statusid { get; set; }
        public List<CalenderSectionIdList> sectionidlist { get; set; }
        public CalendereventdetailsModel()
        {
            sectionidlist = new List<CalenderSectionIdList>();
        }
    }




    public class Devices
    {
        public string Deviceid { get; set; }
        public int DeviceType { get; set; }
    }

    public class PostCalendarEvents
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public int StandardId { get; set; }
        public int SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Venue { get; set; }
        public string Attachment { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        // public string StartDatelong { get; set; }
        // public string EndDatelong { get; set; }
        //  public Guid EventId { get; set; }
        // public string EventType { get; set; }
        //  public string StandardName { get; set; }
        //  public string SectionName { get; set; }
        // public DateTime StartDate { get; set; }
        // public DateTime EndDate { get; set; }
        // public string StartTime { get; set; }
        // public string EndTime { get; set; }
    }

    public class CalendarStatus
    {
        public int Status { get; set; }
    }

    public class GetCalendarEvents
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public int StandardId { get; set; }
        public int SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Venue { get; set; }
        public string Attachment { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<CalenderSectionIdList> sectionidlist { get; set; }
        public GetCalendarEvents()
        {
            sectionidlist = new List<CalenderSectionIdList>();
        }
        // public string StartDatelong { get; set; }
        //public string EndDatelong { get; set; }
        //public string EventType { get; set; }
        // public string StandardName { get; set; }
        // public string SectionName { get; set; }
        //public DateTime? StartDate { get; set; }
        // public DateTime ? EndDate { get; set; }
        // public Nullable<System.TimeSpan> StartTime { get; set; }
        // public Nullable<System.TimeSpan> EndTime { get; set; }
    }
    public class CalenderSectionIdList
    {
        public int? sectionid { get; set; }
    }


}
