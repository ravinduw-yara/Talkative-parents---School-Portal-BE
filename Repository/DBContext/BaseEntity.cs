using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.DBContext
{
    public class BaseEntity
    {
    }

    #region Master Tables
    public partial class MBranch: BaseEntity { }
    public partial class MStandardsectionmapping : BaseEntity { }
    public partial class MBusinessunittype : BaseEntity { }
    public partial class MChildinfo : BaseEntity { }
    public partial class MGender : BaseEntity { }
    public partial class MLocation : BaseEntity { }
    public partial class MLocationtype : BaseEntity { }
    public partial class MModule : BaseEntity { }
    public partial class MRelationtype : BaseEntity { }
    public partial class MRole : BaseEntity { }
    public partial class MSchool : BaseEntity { }
    public partial class MStatus : BaseEntity { }
    public partial class MStatustype : BaseEntity { }
    public partial class MUserinfo : BaseEntity { }
    public partial class MUserrole : BaseEntity { }    
    public partial class MSchooluserinfo : BaseEntity { }    
    public partial class MSchooluserrole : BaseEntity { }    
    public partial class MChildschoolmapping : BaseEntity { }    
    public partial class MParentchildmapping : BaseEntity { }    
    public partial class MUsermodulemapping : BaseEntity { }    
    public partial class Appuserdevice : BaseEntity { }    
    public partial class MFeature : BaseEntity { }
    public partial class MCategory : BaseEntity { }
    public partial class MGroup : BaseEntity { }
    public partial class MStandardgroupmapping : BaseEntity { }
    public partial class MAppuserinfo : BaseEntity { }
    public partial class MSubject : BaseEntity { }
    public partial class MTeachersubjectmapping : BaseEntity { }
    public partial class MSemestertestsmapping : BaseEntity { }
    public partial class MSubjecttestmapping : BaseEntity { }
    public partial class MSubjectsectionmapping : BaseEntity { }
    public partial class MLevel : BaseEntity { }
    public partial class MSubSubject : BaseEntity { }
    public partial class MSubsubjectmarks : BaseEntity { }
    //HiEdu Tables
    public partial class MHiEduBatch : BaseEntity { }
    public partial class MHiEduCourses : BaseEntity { }
    public partial class MHiEduCourseSemesterExam : BaseEntity { }
    public partial class MHiEduSubSubject : BaseEntity { }
    public partial class HiEdu_SubjectExamMapping : BaseEntity { }
    public partial class MHiEduSemester : BaseEntity { }
    public partial class MQuetionPaper : BaseEntity { }
    #endregion


    #region Transaction Tables
    public partial class TCalendereventdetail : BaseEntity { }
    public partial class TSoundingboardmessage : BaseEntity { }
    public partial class TNoticeboardmessage : BaseEntity { }
    public partial class TNoticeboardmapping : BaseEntity { }
    public partial class TEmaillog : BaseEntity { }
    public partial class NoticeboardmessageModel : BaseEntity { }
    public partial class MAppuserinfo : BaseEntity { }
    public partial class TCalendereventdetail : BaseEntity { }
    public partial class TGoogleclass : BaseEntity { }
    public partial class TGclvedioclass : BaseEntity { }
    public partial class TToken : BaseEntity { }
    public partial class MSemesteryearmapping : BaseEntity { }

    public partial class MAcademicyeardetail : BaseEntity { }
    public partial class TClassCalenderevents : BaseEntity { }
    public partial class MSyllabus : BaseEntity { }
    #endregion
}
