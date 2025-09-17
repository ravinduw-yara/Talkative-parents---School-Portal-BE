using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtility
{
    public static class ApplicationConstants
    {
        //Temporary values, needs to be discussed
        public enum ApplicationStatus
        {
            Active = 1,
            Inactive = 2
        }

        public enum UserStatus
        {
            Active = 13,
            Inactive = 14,
            Block = 15
        }

        public const string TPConnectionString = "TPConnectionString";

        public const string GetAllSchool = "GetAllSchool";

        public const string GetAllSchoolByCode = "GetAllSchoolByCode";

        public const string CountDashboardData = "sp_countdashboarddata";

        public const string GetAccessPermissionBySchoolUserId = "sp_GetAccessPermissionBySchoolUserId";

        public const string GetParentsListSP = "GetParentsList"; 

        public const string GetParentsStudentView = "sp_GetParentsStudentView";

        public const string GetParentsNewListSP = "GetParentsList"; 

        public const string GetStudentDetailsByChildIdListSP = "sp_GetStudentDetailsByChildId";

        public const string GetsblingsListAPIListSP = "sp_GetsblingsList"; 

        public const string GetStudentDetailsByParentIdAPIListSP = "sp_GetStudentDetailsByParentId";

        public const string GetStudentDetailsParentByChildIdAPIListSP = "sp_GetStudentDetailsParentByChildId";

        public const string UpdateStudentInfoAPI = "sp_UpdateStudentDetailsByChildId";

        public const string UpdateOTPAppuser = "sp_UpdateOTPAppuser"; 

        public const string UpdateEmailToken = "sp_UpdateEmailToken";

        public const string UpdateEmailPassword = "sp_SetParentEmailPassword";

        public const string GetParentSP = "sp_GetParents";

        public const string getTeachClasSP = "sp_getTeachClass";

        public const string UpdateReportCard = "sp_UpdateSubjectSemesterPercentage";

        public const string PostTestSectionMapping = "sp_PostTestSectionMapping";

        public const string GetSemesterTestMapping = "sp_GetSemesterTestMapping"; 

        public const string AddGradeLevelSp = "sp_AddGradeLevelSp"; 

        public const string GetSubSubjectMarksByChildVSClassSp = "sp_GetSubSubjectMarksByChildVSClass"; 

        public const string GetSubjectMarksByChildVSClass_1Sp = "sp_GetSubjectMarksByChildVSClass_1"; 

        public const string UpdateSubSubjectMarksSp = "sp_UpdateSubSubjectMarks";

        //Dijnidu Modify by 2023/03/16 subject weight and sub subject max marks  SP

        public const string updateSubSubjecweight = "sp_UpdateSubSubjectweight";

        public const string updateSubSubjecSubMaxMarks = "sp_UpdateSubSubjectsubmaxMarks"; 

        public const string GetsubjectGradeprecentage = "sp_GetsubjectGradeprecentage";

        public const string GetPastYearsGradeSubjectMarks = "sp_GetPastYearsGradeSubjectMarks";

        public const string AddHiEduStudentDetails = "sp_HiEduAddStudentDetails"; 

        public const string GetChildInfoBySchoolCourseBatch = "sp_HiEduGetChildInfoBySchoolCourseBatch";

        public const string HiEduGetSemesterModule = "sp_HiEduGetSemesterModule"; 

        public const string UpdateHiEduSubSubjectMarksSp = "sp_UpdateHiEduSubSubjectMarksSp";
        public const string HiEduGetSubSubject = "sp_HiEduGetSubSubject";
        public const string GetHieduStudentDetailsParentByChildIdAPIListSP = "sp_GetHiEduNewStudentDetailsByChildId";
        public const string UpdateHieduStudentInfoAPI = "sp_UpdateHiEduStudentDetailsByChildId";
        public const string GetHiEduBatch = "sp_HiEduGetBatch"; 
        public const string DeleteHiEduStudent = "sp_DeleteHiEduStudent"; 
        public const string AddHiEduCourse = "sp_AddHiEduCourse";
        public const string HiEduGetCourseSemesterexam = "sp_HiEduGetCourseSemesterexam"; 
        public const string UpdateHiEduSubjectMarksSp = "sp_UpdateHiEduSubjectMarksSp";
        public const string UpdateYear = "sp_UpdateAcademicYear"; 
        public const string UpdateLevels = "sp_UpdateLevels"; 
        public const string UpdateSemester = "sp_UpdateSemester"; 
        public const string UpdateGrade = "sp_UpdateStandard"; 
        public const string UpdateSubSubject = "sp_UpdateSubSubject";
        public const string PromoteStudentSp = "sp_UpdateStudentGradeSection"; 
        public const string GetPercentageScoreBySubjectChart = "sp_GetPercentageScoreBySubjectChart"; 

        public const string GetNumberofstudentforsubjectChart = "sp_GetNumberofstudentforsubjectChart"; 

        public const string AddSchoolAddStudentDetails = "sp_SchoolAddStudentDetails"; 

        public const string AddSchoolTeacherDetails = "sp_SchoolAddTeacher";
        public const string GetTeacherViewDetails = "sp_getTeacherDetails";
        public const string GetTeacharsSubjectPercentageScoreByYearChart = "sp_GetTeacharsSubjectPercentageScoreByYearChart";
        public const string GetDropdownTeacherViewDetails = "sp_GetDropdownTeacherDetails";
        public const string GetSingleTeacherViewDetails = "sp_GetSingleTeacherDetails";
        public const string PostSubjecttestmapping = "sp_PostSubjecttestmapping";
        public const string GetStandardSectionMappingByBranchId = "sp_GetStandardSectionMappingByBranchId";
        public const string AddQuetionPaper = "sp_AddQuetionPaper";
        public const string GetQuetionPaper = "sp_GetQuetionPaper"; 
        public const string UpdateDrcEnableForChildren = "sp_PostDrcenable";
        public const string GetGeneratedQuestions = "sp_GetGenerateQuetions";
        public const string DeleteSchoolStudentDetails = "sp_DeleteStudentDetails"; 
        public const string GetOneQuetionPaper = "sp_GetOneQuetionPaper"; 
        public const string AddGenerateQuetionPaper = "sp_AddgenerateQuetionPaper";
        public const string GetGenerateQuetionPaper = "sp_GetGenerateQuetionsPaper"; 
        public const string UpdateCurrentYear = "sp_UpdateCurrentYear"; 
        public const string GetFreezeEnable = "sp_GetFreezeEnable";
        public const string AddFreezeEnable = "sp_PostFreezeEnable";

        public const string UpdateStudentInactiveStatusForChildren = "sp_UpdateBulkStudentInactive";
        public const string AddSyllabus = "sp_AddSyllabus";
        public const string GetSyllabus = "sp_GetSyllabus";
        public const string GetOneSyllabus = "sp_GetOneSyllabus";
        public const string GetGeneratedSyllabus = "sp_GetGenerateSyllabus";
        public const string DeleteQuestionPaper = "sp_DeleteQuestionPaper";
        public const string GetNoticeBoardMsgBySchoolId = "sp_GetNoticeBoardMsgBySchoolId";
        public const string GetNoticeBoardMsgBySchoolId2 = "sp_GetNoticeBoardMsgBySchoolId2";
        public const string GetNoticeBoardMsgBySchoolIdandSchooluser = "sp_GetNoticeBoardMsgBySchoolIdandSchooluser";
        public const string AddParentForExistingChild = "sp_AddParentForExistingChild";
        public const string DeleteGenerateQuestionPaper = "sp_DeleteGenerateQuetionPapers";
        public const string DeleteSyllabus = "sp_Deletesyllabus";
        public const string GetAveragePerformanceForStudent = "sp_GetAveragePerformanceForStudent"; 
        public const string GetSchoolUserDetails = "sp_GetSchoolUserDetails";
        public const string GetPromptsBySchoolId = "sp_GetPromptsBySchoolId"; 
        public const string AddPromptToSchool = "sp_GetPromptsBySchoolId";
        public const string UpdatePromptsForSchool = "sp_UpdatePromptForSchool"; 
        public const string GetPromptByPromptId = "sp_GetPromptByPromptId"; 
        public const string DeleteSchoolUser = "sp_DeleteSchoolUser"; 
        public const string AddSchool = "sp_AddSchool"; 
        public const string QIGetPromptsBySchool = "sp_QIGetPromptsBySchool"; 
        public const string QIGetQuetionPaper = "sp_QIGetQuetionPaper"; 
        public const string QIGetPromptsBySchoolImageEdit = "sp_QIGetPromptsBySchoolImageEdit";
        public const string QIGetLangConvertPromptsBySchool = "sp_QIGetLangConvertPromptsBySchool"; 
        public const string QIGetSyllabusContent = "sp_QIGetSyllabusContent"; 
        public const string QIGetSyllabusPromptsBySchool = "sp_QIGetSyllabusPromptsBySchool";
        public const string GetSubjectTeacherStandardSectionMappingBySectionSubject = "sp_GetSubjectTeacherStandardSectionMappingBySectionSubject";
        public const string Parentdelete = "sp_Deleteparent"; 
        public const string GetChildDRCByChildIdYearAPIListSP = "sp_GetChildDRCByChildIdYearAPIListSP";
        public const string GetTeacherDetailsSubjects = "sp_GetTeacherDetailsSubjects"; 
        public const string GetClassMarksSummary = "sp_GetClassMarksSummary"; 
        public const string GetClassListSummary = "sp_GetClassListSummary"; 
        public const string AddGradeAndSectionSp = "sp_AddGradeAndSection";
        public const string DeleteBulkNBMessagesSP = "sp_DeleteBulkNBMessages"; 
        public const string GetBulkNBMessagesSP = "sp_GetBulkNBMessages"; 
        public const string UpdateGradeAndSectionName = "sp_UpdateGradeAndSectionName";




    }
}
