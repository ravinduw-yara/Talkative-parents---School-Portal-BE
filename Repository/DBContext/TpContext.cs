using System;
using CommonUtility.RequestModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
//using Repository.DBContext.Repository.DBContext;

#nullable disable

namespace Repository.DBContext
{
    public partial class TpContext : DbContext
    {
        public TpContext()
        {
        }

        public TpContext(DbContextOptions<TpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appuserdevice> Appuserdevices { get; set; }
        public virtual DbSet<MAcademicyeardetail> MAcademicyeardetails { get; set; }
        public virtual DbSet<MAdmininfo> MAdmininfos { get; set; }
        public virtual DbSet<MAppuserinfo> MAppuserinfos { get; set; }
        public virtual DbSet<MBranch> MBranches { get; set; }
        public virtual DbSet<MBusinessunittype> MBusinessunittypes { get; set; }
        public virtual DbSet<MCategory> MCategories { get; set; }
        public virtual DbSet<MChildinfo> MChildinfos { get; set; }
        public virtual DbSet<MChildschoolmapping> MChildschoolmappings { get; set; }
        public virtual DbSet<MChildtestmapping> MChildtestmappings { get; set; }
        public virtual DbSet<MFeature> MFeatures { get; set; }
        public virtual DbSet<MGender> MGenders { get; set; }
        public virtual DbSet<MGroup> MGroups { get; set; }
        public virtual DbSet<MLocation> MLocations { get; set; }
        public virtual DbSet<MLocationtype> MLocationtypes { get; set; }
        public virtual DbSet<MModule> MModules { get; set; }
        public virtual DbSet<MOverallchildtest> MOverallchildtests { get; set; }
        public virtual DbSet<MParentchildmapping> MParentchildmappings { get; set; }
        public virtual DbSet<MRelationtype> MRelationtypes { get; set; }
        public virtual DbSet<MRole> MRoles { get; set; }
        public virtual DbSet<MSchool> MSchools { get; set; }
        public virtual DbSet<MSchoolDamapping> MSchoolDamappings { get; set; }
        public virtual DbSet<MSchooluserinfo> MSchooluserinfos { get; set; }
        public virtual DbSet<MSchooluserrole> MSchooluserroles { get; set; }
        public virtual DbSet<MSemestertestsmapping> MSemestertestsmappings { get; set; }
        public virtual DbSet<MSemesteryearmapping> MSemesteryearmappings { get; set; }
        public virtual DbSet<MStandardgroupmapping> MStandardgroupmappings { get; set; }
        public virtual DbSet<MStandardsectionmapping> MStandardsectionmappings { get; set; }
        public virtual DbSet<MStatus> MStatuses { get; set; }
        public virtual DbSet<MStatustype> MStatustypes { get; set; }
        public virtual DbSet<MSubject> MSubjects { get; set; }
        public virtual DbSet<MSubjectsectionmapping> MSubjectsectionmappings { get; set; }
        public virtual DbSet<MSubjectsemesterpercentage> MSubjectsemesterpercentages { get; set; }
        public virtual DbSet<MSubjecttestmapping> MSubjecttestmappings { get; set; }
        public virtual DbSet<MTeachersubjectmapping> MTeachersubjectmappings { get; set; }
        public virtual DbSet<MTestsectionmapping> MTestsectionmappings { get; set; }
        public virtual DbSet<MUsermodulemapping> MUsermodulemappings { get; set; }
        public virtual DbSet<TCalendereventdetail> TCalendereventdetails { get; set; }
        public virtual DbSet<TEmaillog> TEmaillogs { get; set; }
        public virtual DbSet<TGclteacherclass> TGclteacherclasses { get; set; }
        public virtual DbSet<TGclvedioclass> TGclvedioclasses { get; set; }
        public virtual DbSet<TGoogleclass> TGoogleclasses { get; set; }
        public virtual DbSet<TNoticeboardmapping> TNoticeboardmappings { get; set; }
        public virtual DbSet<TNoticeboardmessage> TNoticeboardmessages { get; set; }
        public virtual DbSet<TSoundingboardmessage> TSoundingboardmessages { get; set; }
        public virtual DbSet<TToken> TTokens { get; set; }
        public virtual DbSet<VClassdetail> VClassdetails { get; set; }
        public virtual DbSet<VDashboardcount> VDashboardcounts { get; set; }
        public virtual DbSet<MLevel> MLevels { get; set; }
        public virtual DbSet<MSubSubject> MSubSubjects { get; set; }
        public virtual DbSet<MSubsubjectmarks> MSubsubjectmarkss { get; set; }
        //--03/30/2023--Jaliya
        public virtual DbSet<MHiEduBatch> MHiEdubatchs { get; set; }
        public virtual DbSet<MHiEduCourses> MHiEduCourses { get; set; }
        public virtual DbSet<MHiEduCourseSemesterExam> MHiEduCourseSemesterExams { get; set; }
        //--04/25/2023--Sanduni
        public virtual DbSet<MHiEdu_CourseExamMarks> MHiEdu_CourseExamMarkss { get; set; }
        public virtual DbSet<HiEdu_SemesterCourseMapping> HiEdu_SemesterCourseMappings { get; set; }

        public virtual DbSet<MHiEduSubSubject> MHiEduSubSubjects { get; set; }
        public virtual DbSet<HiEdu_SubSubjectMarks> HiEdu_SubSubjectMarkss { get; set; }
        public virtual DbSet<HiEdu_ChildBatchCourseMapping> HiEdu_ChildBatchCourseMappings { get; set; }
        public virtual DbSet<HiEdu_BatchCourseMapping>HiEdu_BatchCourseMappings { get; set; }
        public virtual DbSet<HiEdu_OverallChildTests> HiEdu_OverallChildTestss { get; set; }

        //public virtual DbSet<MHiEduSemesterCourseMapping> MHieduSemesterCourseMappings { get; set; }
        //
        public virtual DbSet<HiEdu_SubjectExamMapping> MHieduSubjectExamMappings { get; set; }
        public virtual DbSet<MHiEduSemester> MHiEduSemesters { get; set; }
        public virtual DbSet<MQuetionPaper> MQuetionPapers { get; set; }
        public virtual DbSet<TClassCalenderevents> TClassCalendereventss { get; set; }
        public virtual DbSet<MSyllabus> MSyllabuss { get; set; }
        public virtual DbSet<MStandardyearmapping> MStandardyearmappings { get; set; }
        public virtual DbSet<MSchoolprompt> MSchoolprompts { get; set; }

        public virtual DbSet<MPrompttype> MPrompttypes { get; set; }
        public virtual DbSet<MQuestionpapertype> MQuestionpapertypes { get; set; }
        public virtual DbSet<MSchoolUserDevice> MSchoolUserDevices { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Data Source=tcp:talkativeparentapidbserver.database.windows.net,1433;Initial Catalog=TalkativeParentAPI_db_Copy;User Id=tpsrilankaadmin@talkativeparentapidbserver;Password=talkativesri123!@#");
              optionsBuilder.UseSqlServer("Data Source=talkativeparentapidbserver.database.windows.net;Initial Catalog=TalkativeParentHistoricalAPI_db;User ID=tpsrilankaadmin;Password=NjA)5A3&/9A£V89");
             // optionsBuilder.UseSqlServer("Data Source=talkativeparentapidbserver.database.windows.net;Initial Catalog=TalkativeParentAPI_db_backup;User ID=tpsrilankaadmin;Password=talkativesri123!@#");
            }
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Appuserdevice>(entity =>
            {
                entity.ToTable("appuserdevice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Appuserid).HasColumnName("appuserid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Deviceid).HasColumnName("deviceid");

                entity.Property(e => e.Devicetype).HasColumnName("devicetype");

                entity.Property(e => e.Groupid).HasColumnName("groupid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Version)
                    .HasMaxLength(256)
                    .HasColumnName("version");

                entity.HasOne(d => d.Appuser)
                    .WithMany(p => p.AppuserdeviceAppusers)
                    .HasForeignKey(d => d.Appuserid)
                    .HasConstraintName("FK__appuserde__appus__7B5B524B");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.AppuserdeviceCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__appuserde__creat__7C4F7684");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.AppuserdeviceModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__appuserde__modif__7D439ABD");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Appuserdevices)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__appuserde__statu__7E37BEF6");
            });

            modelBuilder.Entity<MSchoolUserDevice>(entity =>
            {
                entity.ToTable("m_schooluserdevice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SchoolUserid).HasColumnName("schooluserid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Deviceid).HasColumnName("deviceid");

                entity.Property(e => e.Devicetype).HasColumnName("devicetype");

                entity.Property(e => e.Groupid).HasColumnName("groupid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Version)
                    .HasMaxLength(256)
                    .HasColumnName("version");

                
            });

            modelBuilder.Entity<MQuestionpapertype>(entity =>
            {
                entity.ToTable("m_questionpapertype");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Questionpapertype)
                  .HasMaxLength(128)
                  .HasColumnName("questionpapertype");




            });

            //jaliya----03/30/2023-- 22/4/2023 Hiedu tables

            modelBuilder.Entity<MHiEduBatch>(entity =>
            {
                entity.ToTable("m_HiEdu_Batch");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Batch)
                    .HasMaxLength(128)
                    .HasColumnName("Batch");

                entity.Property(e => e.CourseId)
                    .HasColumnName("CourseId");

                entity.Property(e => e.Created_Year)
                    .HasColumnName("Created_Year");

                entity.Property(e => e.Created_Month)
                    .HasColumnName("Created_Month");

                entity.Property(e => e.Created_Date)
                    .HasColumnName("Created_Date");

                entity.Property(e => e.Batch_Created_Date)
                    .HasColumnType("datetime")
                    .HasColumnName("Batch_Created_Date");
            });
            modelBuilder.Entity<MHiEduCourses>(entity =>
            {
                entity.ToTable("m_HiEdu_Courses");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Course)
                .HasMaxLength(128)
                .HasColumnName("Course");

                entity.Property(e => e.SchoolId)
                    .HasColumnName("SchoolId");

                entity.Property(e => e.DepartmentId)
                   .HasColumnName("DepartmentId");

                entity.Property(e => e.Duration).HasColumnName("Duration");

                //entity.HasOne(d => d.Gender)
                //    .WithMany(p => p.MChildinfos)
                //    .HasForeignKey(d => d.Genderid)
                //    .HasConstraintName("FK__m_childin__gende__0F624AF8");

            });
         
            //End Hiedu section Jaliya

            //report card - sanduni
            modelBuilder.Entity<HiEdu_OverallChildTests>(entity =>
            {
                entity.ToTable("m_HiEdu_OverallChildTests");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.ChildId)
                    .HasColumnName("ChildId");

                entity.Property(e => e.BatchCourseMappingId)
                   .HasColumnName("BatchCourseMappingId");
                entity.Property(e => e.OverallBatchPostition)
                   .HasColumnName("OverallBatchPostition");
                entity.Property(e => e.OverallSemesterExamPrecentage)
                   .HasColumnName("OverallSemesterExamPrecentage");
                entity.Property(e => e.OverallComment)
                    .HasMaxLength(128)
                   .HasColumnName("OverallComment");
            });
            modelBuilder.Entity<HiEdu_ChildBatchCourseMapping>(entity =>
            {
                entity.ToTable("m_HiEdu_ChildBatchCourseMapping");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.BatchCourseMappingId)
                    .HasColumnName("BatchCourseMappingId");

                entity.Property(e => e.ChildId)
                   .HasColumnName("ChildId");
            });
            //modelBuilder.Entity<HiEdu_BatchCourseMapping>(entity =>
            //{
            //    entity.ToTable("m_HiEdu_BatchCourseMapping");

            //    entity.Property(e => e.Id).HasColumnName("Id");

            //    entity.Property(e => e.BatchId)
            //        .HasColumnName("BatchId");

            //    entity.Property(e => e.CourseId)
            //       .HasColumnName("CourseId");
            //});
            modelBuilder.Entity<MHiEdu_CourseExamMarks>(entity =>
            {
                entity.ToTable("m_HiEdu_CourseExamMarks");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Marks)
                    .HasColumnName("Marks");

                entity.Property(e => e.ChildId)
                   .HasColumnName("ChildId");

                entity.Property(e => e.SubjectExamMappingId)
                    .HasColumnName("SubjectExamMappingId");
            });
            modelBuilder.Entity<HiEdu_SemesterCourseMapping>(entity =>
            {
                entity.ToTable("m_HiEdu_SemesterCourseMapping");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.SemesterName)
                    .HasColumnName("SemesterName");

                entity.Property(e => e.CourseId)
                   .HasColumnName("CourseId");

            });

            modelBuilder.Entity<HiEdu_SubjectExamMapping>(entity =>
            {
                entity.ToTable("m_HiEdu_SubjectExamMapping");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Subject)
                    .HasMaxLength(128)
                    .HasColumnName("Subject");

                entity.Property(e => e.CourseSemesterExamId)
                   .HasColumnName("CourseSemesterExamId");

            });
            modelBuilder.Entity<MHiEduSubSubject>(entity =>
            {
                entity.ToTable("m_HiEdu_SubSubject");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.SubSubjectName)
                    .HasMaxLength(128)
                    .HasColumnName("SubSubjectName");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectId");

                entity.Property(e => e.Weight)
                    .HasColumnName("Weight");

                entity.Property(e => e.Maxmarks)
                   .HasColumnName("Maxmarks");
            });
            modelBuilder.Entity<HiEdu_SubSubjectMarks>(entity =>
            {
                entity.ToTable("m_HiEdu_SubSubjectMarks");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.CourseExamMarksId)
                    .HasColumnName("CourseExamMarksId");

                entity.Property(e => e.SubSubjectId)
                    .HasColumnName("SubSubjectId");

                entity.Property(e => e.SubSubjectMarks)
                    .HasColumnName("SubSubjectMarks");

                entity.Property(e => e.Comment).HasMaxLength(128).HasColumnName("Comment");

                entity.Property(e => e.IsAbsent).HasColumnName("IsAbsent");

            });
            modelBuilder.Entity<MHiEduCourseSemesterExam>(entity =>
            {
                entity.ToTable("m_HiEdu_CourseSemesterExam");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Exam)
                    .HasMaxLength(128)
                    .HasColumnName("Exam");

                entity.Property(e => e.SemesterCourseMappingId)
                    .HasMaxLength(128)
                    .HasColumnName("SemesterCourseMappingId");



            });






            //modelBuilder.Entity<MHieduChildBatchCourseMapping>(entity =>
            //{
            //    entity.ToTable("HiEdu_ChildBatchCourseMapping");

            //    entity.Property(e => e.Id).HasColumnName("id");



            //    entity.Property(e => e.BatchCourseMappingId)
            //      .HasColumnName("BatchCourseMappingId");

            //    entity.Property(e => e.ChildId)
            //     .HasColumnName("ChildId");
            //    //entity.HasOne(d => d.Course)
            //    //    .WithMany(p => p.MHieduSemesterCourseMappings)
            //    //    .HasForeignKey(d => d.CourseId)
            //    //    .HasConstraintName("FK__HiEdu_Sem__Cours__51BA1E3A");


            //});

            modelBuilder.Entity<Mlevelsectionmapping>(entity =>
            {
                entity.ToTable("m_levelsectionmapping");
                entity.Property(e => e.levelId).HasColumnName("levelId");
                entity.Property(e => e.standardsectionmappingId).HasColumnName("standardsectionmappingId");
                
            });
            modelBuilder.Entity<MSubsubjectmarks>(entity =>
            {
                entity.ToTable("m_subsubjectmarks");

                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.SubSubjectId).HasColumnName("SubSubjectId");
                entity.Property(e => e.Marks).HasColumnName("Marks");
                entity.Property(e => e.ChildTestMappingId).HasColumnName("ChildTestMappingId");
                entity.Property(e => e.SubPrecentage).HasColumnName("SubPrecentage");
                entity.Property(e => e.Comment).HasColumnName("Comment"); 
                entity.Property(e => e.Absent).HasColumnName("Absent");

            });

            modelBuilder.Entity<MSubSubject>(entity =>
            {
                entity.ToTable("m_subsubject");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.SubSubject).HasColumnName("SubSubject");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.Precentage).HasColumnName("Precentage");
                entity.Property(e => e.SubMaxMarks).HasColumnName("SubMaxMarks");
                entity.Property(e => e.ExcelSheetOrder).HasColumnName("ExcelSheetOrder");


            });

            modelBuilder.Entity<MStandardyearmapping>(entity =>
            {
                entity.ToTable("m_standardyearmapping");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.AcademicYearId)
                    .HasColumnName("AcademicYearId");

                entity.Property(e => e.GradeId)
                    .HasColumnName("GradeId");

                entity.Property(e => e.FreezeEnable)
                    .HasColumnName("FreezeEnable");

                entity.Property(e => e.LevelId)
                   .HasColumnName("LevelId");


                entity.Property(e => e.SemesterId)
                   .HasColumnName("SemesterId");

                entity.Property(e => e.ExamId)
                   .HasColumnName("ExamId");

            });
            modelBuilder.Entity<MAcademicyeardetail>(entity =>
            {
                entity.ToTable("m_academicyeardetails");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.YearName).HasMaxLength(150);

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MAcademicyeardetailCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_academi__Creat__39237A9A");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MAcademicyeardetailModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_academi__Modif__3A179ED3");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MAcademicyeardetails)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_academi__Schoo__373B3228");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MAcademicyeardetails)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_academi__Statu__382F5661");

                entity.Property(e => e.Currentyear).HasColumnName("Currentyear");
            });
            //sanduni DRC

            modelBuilder.Entity<MLevel>(entity =>
            {
                entity.ToTable("m_level");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.levels)
                 .HasMaxLength(100)
                 .HasColumnName("levels");

                entity.Property(e => e.schoolid)
                    .HasColumnName("schoolid");

            });
            modelBuilder.Entity<MAdmininfo>(entity =>
            {
                entity.ToTable("m_admininfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Emailid)
                    .HasMaxLength(128)
                    .HasColumnName("emailid");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Password)
                    .HasMaxLength(256)
                    .HasColumnName("password");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(32)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Username)
                    .HasMaxLength(256)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<MAppuserinfo>(entity =>
            {
                entity.ToTable("m_appuserinfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("dob");

                entity.Property(e => e.Emailid)
                    .HasMaxLength(128)
                    .HasColumnName("emailid");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(128)
                    .HasColumnName("firstname");

                entity.Property(e => e.Genderid).HasColumnName("genderid");

                entity.Property(e => e.Isofferoptedin).HasColumnName("isofferoptedin");

                entity.Property(e => e.Issmsuser).HasColumnName("issmsuser");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(128)
                    .HasColumnName("lastname");

                entity.Property(e => e.Middlename)
                    .HasMaxLength(128)
                    .HasColumnName("middlename");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Phonenumber)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MAppuserinfoCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_appuser__creat__7F2BE32F");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.MAppuserinfos)
                    .HasForeignKey(d => d.Genderid)
                    .HasConstraintName("FK__m_appuser__gende__00200768");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MAppuserinfoModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_appuser__modif__01142BA1");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MAppuserinfos)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_appuser__statu__02084FDA");
            });

            modelBuilder.Entity<MBranch>(entity =>
            {
                entity.ToTable("m_branch");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(256)
                    .HasColumnName("address");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(256)
                    .HasColumnName("description");

                entity.Property(e => e.Locaionid).HasColumnName("locaionid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Pincode).HasColumnName("pincode");

                entity.Property(e => e.Principalname)
                    .HasMaxLength(128)
                    .HasColumnName("principalname");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MBranchCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_branch__create__02FC7413");

                entity.HasOne(d => d.Locaion)
                    .WithMany(p => p.MBranches)
                    .HasForeignKey(d => d.Locaionid)
                    .HasConstraintName("FK__m_branch__locaio__03F0984C");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MBranchModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_branch__modifi__04E4BC85");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MBranches)
                    .HasForeignKey(d => d.Schoolid)
                    .HasConstraintName("FK__m_branch__school__05D8E0BE");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MBranches)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_branch__status__06CD04F7");
            });

            modelBuilder.Entity<MBusinessunittype>(entity =>
            {
                entity.ToTable("m_businessunittype");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Type)
                    .HasMaxLength(32)
                    .HasColumnName("type");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MBusinessunittypeCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_busines__creat__07C12930");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MBusinessunittypeModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_busines__modif__08B54D69");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MBusinessunittypes)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_busines__statu__09A971A2");
            });

            modelBuilder.Entity<MCategory>(entity =>
            {
                entity.ToTable("m_category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(256)
                    .HasColumnName("description");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Roleid).HasColumnName("roleid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MCategoryCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_categor__creat__0A9D95DB");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MCategoryModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_categor__modif__0B91BA14");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.MCategories)
                    .HasForeignKey(d => d.Roleid)
                    .HasConstraintName("FK__m_categor__rolei__0C85DE4D");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MCategories)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_categor__statu__0D7A0286");
            });

            modelBuilder.Entity<MChildinfo>(entity =>
            {
                entity.ToTable("m_childinfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("dob");

                entity.Property(e => e.Email)
                    .HasMaxLength(128)
                    .HasColumnName("email");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(128)
                    .HasColumnName("firstname");

                entity.Property(e => e.Genderid).HasColumnName("genderid");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(128)
                    .HasColumnName("lastname");

                entity.Property(e => e.Middlename)
                    .HasMaxLength(128)
                    .HasColumnName("middlename");

                entity.Property(e => e.StudentImageLink)
                    .HasMaxLength(2048)
                    .HasColumnName("StudentImageLink");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(32)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MChildinfoCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_childin__creat__0E6E26BF");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.MChildinfos)
                    .HasForeignKey(d => d.Genderid)
                    .HasConstraintName("FK__m_childin__gende__0F624AF8");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MChildinfoModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_childin__modif__10566F31");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MChildinfos)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_childin__statu__114A936A");
            });

            modelBuilder.Entity<MChildschoolmapping>(entity =>
            {
                entity.ToTable("m_childschoolmapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Childid).HasColumnName("childid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Registerationnumber)
                    .HasMaxLength(50)
                    .HasColumnName("registerationnumber");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.DRCEnable1).HasColumnName("drcenable1");
                entity.Property(e => e.DRCEnable2).HasColumnName("drcenable2");
                entity.Property(e => e.DRCEnable3).HasColumnName("drcenable3");
                entity.Property(e => e.Promoted).HasColumnName("promoted");

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.MChildschoolmappings)
                    .HasForeignKey(d => d.AcademicYearId)
                    .HasConstraintName("FK__m_childsc__Acade__41B8C09B");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.MChildschoolmappings)
                    .HasForeignKey(d => d.Childid)
                    .HasConstraintName("FK__m_childsc__child__123EB7A3");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MChildschoolmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_childsc__creat__1332DBDC");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MChildschoolmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_childsc__modif__14270015");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.MChildschoolmappings)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__m_childsc__stand__151B244E");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MChildschoolmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_childsc__statu__160F4887");
            });

            modelBuilder.Entity<MChildtestmapping>(entity =>
            {
                entity.ToTable("m_childtestmapping");

                entity.Property(e => e.Comments).HasMaxLength(500);

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.MChildtestmappings)
                    .HasForeignKey(d => d.ChildId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_childte__Child__17036CC0");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MChildtestmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_childte__Creat__17F790F9");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MChildtestmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_childte__Modif__18EBB532");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MChildtestmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_childte__Statu__19DFD96B");

                entity.HasOne(d => d.SubjectTestMapping)
                    .WithMany(p => p.MChildtestmappings)
                    .HasForeignKey(d => d.SubjectTestMappingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_childte__Subje__1AD3FDA4");
            });

            modelBuilder.Entity<MFeature>(entity =>
            {
                entity.ToTable("m_features");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Isgcl).HasColumnName("isgcl");

                entity.Property(e => e.Mask)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mask");

                entity.Property(e => e.Maxmsgcount).HasColumnName("maxmsgcount");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MFeatureCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_feature__creat__1BC821DD");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MFeatureModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_feature__modif__1CBC4616");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MFeatures)
                    .HasForeignKey(d => d.Schoolid)
                    .HasConstraintName("FK__m_feature__schoo__1DB06A4F");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MFeatures)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_feature__statu__1EA48E88");
            });

            modelBuilder.Entity<MGender>(entity =>
            {
                entity.ToTable("m_gender");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Icon)
                    .HasMaxLength(128)
                    .HasColumnName("icon");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Type)
                    .HasMaxLength(128)
                    .HasColumnName("type");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MGenderCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_gender__create__1F98B2C1");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MGenderModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_gender__modifi__208CD6FA");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MGenders)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_gender__status__2180FB33");
            });

            modelBuilder.Entity<MGroup>(entity =>
            {
                entity.ToTable("m_group");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MGroupCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_group__created__22751F6C");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MGroupModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_group__modifie__236943A5");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MGroups)
                    .HasForeignKey(d => d.Schoolid)
                    .HasConstraintName("FK__m_group__schooli__245D67DE");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MGroups)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_group__statusi__25518C17");
            });

            modelBuilder.Entity<MLocation>(entity =>
            {
                entity.ToTable("m_location");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Locationtypeid).HasColumnName("locationtypeid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Parentid).HasColumnName("parentid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MLocationCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_locatio__creat__2645B050");

                entity.HasOne(d => d.Locationtype)
                    .WithMany(p => p.MLocations)
                    .HasForeignKey(d => d.Locationtypeid)
                    .HasConstraintName("FK__m_locatio__locat__2739D489");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MLocationModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_locatio__modif__282DF8C2");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MLocations)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_locatio__statu__29221CFB");
            });

            modelBuilder.Entity<MLocationtype>(entity =>
            {
                entity.ToTable("m_locationtype");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Type)
                    .HasMaxLength(32)
                    .HasColumnName("type");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MLocationtypeCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_locatio__creat__2A164134");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MLocationtypeModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_locatio__modif__2B0A656D");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MLocationtypes)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_locatio__statu__2BFE89A6");
            });

            modelBuilder.Entity<MModule>(entity =>
            {
                entity.ToTable("m_module");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Icon)
                    .HasMaxLength(265)
                    .HasColumnName("icon");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Parentid).HasColumnName("parentid");

                entity.Property(e => e.Selected).HasColumnName("selected");

                entity.Property(e => e.State)
                    .HasMaxLength(32)
                    .HasColumnName("state");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MModuleCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_module__create__2CF2ADDF");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MModuleModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_module__modifi__2DE6D218");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MModules)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_module__status__2EDAF651");
            });

            modelBuilder.Entity<MOverallchildtest>(entity =>
            {
                entity.ToTable("m_overallchildtests");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.OverallComments).HasMaxLength(500);

                entity.Property(e => e.OverallCommentstwo).HasMaxLength(500);

                entity.Property(e => e.OverallCommenthree).HasMaxLength(500);

                entity.Property(e => e.OverallPosition).HasMaxLength(50);

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.MOverallchildtests)
                    .HasForeignKey(d => d.AcademicYearId)
                    .HasConstraintName("FK__m_overall__Acade__4589517F");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.MOverallchildtests)
                    .HasForeignKey(d => d.ChildId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_overall__Child__2FCF1A8A");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MOverallchildtestCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_overall__Creat__30C33EC3");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MOverallchildtestModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_overall__Modif__31B762FC");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.MOverallchildtests)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_overall__Secti__32AB8735");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MOverallchildtests)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_overall__Statu__339FAB6E");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.MOverallchildtests)
                    .HasForeignKey(d => d.TestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_overall__TestI__3493CFA7");
            });
            modelBuilder.Entity<MParentchildmapping>(entity =>
            {
                entity.ToTable("m_parentchildmapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Appuserid).HasColumnName("appuserid");

                entity.Property(e => e.Childid).HasColumnName("childid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Relationtypeid).HasColumnName("relationtypeid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.Appuser)
                    .WithMany(p => p.MParentchildmappings)
                    .HasForeignKey(d => d.Appuserid)
                    .HasConstraintName("FK__m_parentc__appus__3587F3E0");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.MParentchildmappings)
                    .HasForeignKey(d => d.Childid)
                    .HasConstraintName("FK__m_parentc__child__367C1819");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MParentchildmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_parentc__creat__37703C52");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MParentchildmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_parentc__modif__3864608B");

                entity.HasOne(d => d.Relationtype)
                    .WithMany(p => p.MParentchildmappings)
                    .HasForeignKey(d => d.Relationtypeid)
                    .HasConstraintName("FK__m_parentc__relat__395884C4");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MParentchildmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_parentc__statu__3A4CA8FD");
            });

            modelBuilder.Entity<MRelationtype>(entity =>
            {
                entity.ToTable("m_relationtype");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Icon)
                    .HasMaxLength(128)
                    .HasColumnName("icon");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Type)
                    .HasMaxLength(128)
                    .HasColumnName("type");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MRelationtypeCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_relatio__creat__3B40CD36");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MRelationtypeModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_relatio__modif__3C34F16F");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MRelationtypes)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_relatio__statu__3D2915A8");
            });

            modelBuilder.Entity<MRole>(entity =>
            {
                entity.ToTable("m_roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(256)
                    .HasColumnName("description");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Rank).HasColumnName("rank");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MRoleCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_roles__created__3E1D39E1");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MRoleModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_roles__modifie__3F115E1A");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MRoles)
                    .HasForeignKey(d => d.Schoolid)
                    .HasConstraintName("FK__m_roles__schooli__40058253");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MRoles)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_roles__statusi__40F9A68C");
            });

            modelBuilder.Entity<MSchool>(entity =>
            {
                entity.ToTable("m_school");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Allowcategory).HasColumnName("allowcategory");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(265)
                    .HasColumnName("description");

                entity.Property(e => e.Emailid)
                    .HasMaxLength(128)
                    .HasColumnName("emailid");

                entity.Property(e => e.Ipgurl)
                   .HasMaxLength(128)
                   .HasColumnName("Ipgurl");

                entity.Property(e => e.Issbsms).HasColumnName("issbsms");

                entity.Property(e => e.Logo)
                    .HasMaxLength(128)
                    .HasColumnName("logo");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Primaryphonenumber)
                    .HasMaxLength(256)
                    .HasColumnName("primaryphonenumber");

                entity.Property(e => e.Secondaryphonenumber)
                    .HasMaxLength(256)
                    .HasColumnName("secondaryphonenumber");

                entity.Property(e => e.Staffcount).HasColumnName("staffcount");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Websitelink)
                    .HasMaxLength(256)
                    .HasColumnName("websitelink");

                entity.Property(e => e.pdfmodel).HasColumnName("pdfmodel");

                entity.Property(e => e.drcmodel).HasColumnName("drcmodel");

                entity.Property(e => e.eduloanstatus).HasColumnName("eduloanstatus"); 

              //  entity.Property(e => e.ismigrated).HasColumnName("ismigrated");




                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSchoolCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_school__create__41EDCAC5");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSchoolModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_school__modifi__42E1EEFE");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSchools)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_school__status__43D61337");
            });

            modelBuilder.Entity<MSchoolDamapping>(entity =>
            {
                entity.ToTable("m_schoolDAmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Daflag).HasColumnName("DAFlag");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSchoolDamappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_schoolD__Creat__44CA3770");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSchoolDamappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_schoolD__Modif__45BE5BA9");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.MSchoolDamappings)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_schoolD__Schoo__46B27FE2");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSchoolDamappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_schoolD__Statu__47A6A41B");
            });

            modelBuilder.Entity<MSchooluserinfo>(entity =>
            {
                entity.ToTable("m_schooluserinfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Branchid).HasColumnName("branchid");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Emailid)
                    .HasMaxLength(128)
                    .HasColumnName("emailid");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(128)
                    .HasColumnName("firstname");

                entity.Property(e => e.Genderid).HasColumnName("genderid");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(128)
                    .HasColumnName("lastname");

                entity.Property(e => e.Middlename)
                    .HasMaxLength(128)
                    .HasColumnName("middlename");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(32)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Profilephoto)
                    .HasMaxLength(128)
                    .HasColumnName("profilephoto");

                entity.Property(e => e.Salutation)
                    .HasMaxLength(10)
                    .HasColumnName("salutation");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Username)
                    .HasMaxLength(256)
                    .HasColumnName("username");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.MSchooluserinfos)
                    .HasForeignKey(d => d.Branchid)
                    .HasConstraintName("FK__m_schoolu__branc__489AC854");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.MSchooluserinfos)
                    .HasForeignKey(d => d.Genderid)
                    .HasConstraintName("FK__m_schoolu__gende__498EEC8D");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSchooluserinfos)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_schoolu__statu__4A8310C6");

                entity.Property(e => e.Subjectteacher).HasColumnName("subjectteacher");

                entity.Property(e => e.Isquantaenabled).HasColumnName("isquantaenabled");
            });

            modelBuilder.Entity<MSchooluserrole>(entity =>
            {
                entity.ToTable("m_schooluserroles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Categoryid).HasColumnName("categoryid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Groupid).HasColumnName("groupid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Schooluserid).HasColumnName("schooluserid");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MSchooluserroles)
                    .HasForeignKey(d => d.Categoryid)
                    .HasConstraintName("FK__m_schoolu__categ__4B7734FF");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSchooluserroleCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_schoolu__creat__4C6B5938");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MSchooluserroles)
                    .HasForeignKey(d => d.Groupid)
                    .HasConstraintName("FK__m_schoolu__group__4D5F7D71");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSchooluserroleModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_schoolu__modif__4E53A1AA");

                entity.HasOne(d => d.Schooluser)
                    .WithMany(p => p.MSchooluserroleSchoolusers)
                    .HasForeignKey(d => d.Schooluserid)
                    .HasConstraintName("FK__m_schoolu__schoo__4F47C5E3");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.MSchooluserroles)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__m_schoolu__stand__503BEA1C");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSchooluserroles)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_schoolu__statu__51300E55");
            });

            modelBuilder.Entity<MSemestertestsmapping>(entity =>
            {
                entity.ToTable("m_semestertestsmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.MSemestertestsmappings)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_semeste__Branc__5224328E");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSemestertestsmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_semeste__Creat__531856C7");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSemestertestsmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_semeste__Modif__540C7B00");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSemestertestsmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_semeste__Statu__55009F39");
            });

            modelBuilder.Entity<MSemesteryearmapping>(entity =>
            {
                entity.ToTable("m_semesteryearmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.MSemesteryearmappings)
                    .HasForeignKey(d => d.AcademicYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_semeste__Acade__3DE82FB7");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSemesteryearmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_semeste__Creat__3FD07829");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSemesteryearmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_semeste__Modif__40C49C62");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.MSemesteryearmappings)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_semeste__Semes__3CF40B7E");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSemesteryearmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_semeste__Statu__3EDC53F0");
            });

            modelBuilder.Entity<MStandardgroupmapping>(entity =>
            {
                entity.ToTable("m_standardgroupmapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Groupid).HasColumnName("groupid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MStandardgroupmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_standar__creat__55F4C372");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.MStandardgroupmappings)
                    .HasForeignKey(d => d.Groupid)
                    .HasConstraintName("FK__m_standar__group__56E8E7AB");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MStandardgroupmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_standar__modif__57DD0BE4");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.MStandardgroupmappings)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__m_standar__stand__58D1301D");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MStandardgroupmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_standar__statu__59C55456");
            });

            modelBuilder.Entity<MStandardsectionmapping>(entity =>
            {
                entity.ToTable("m_standardsectionmapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Branchid).HasColumnName("branchid");

                entity.Property(e => e.Businessunittypeid).HasColumnName("businessunittypeid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.LevelID).HasColumnName("levelid");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(265)
                    .HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Parentid).HasColumnName("parentid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.MStandardsectionmappings)
                    .HasForeignKey(d => d.Branchid)
                    .HasConstraintName("FK__m_standar__branc__5AB9788F");

                entity.HasOne(d => d.Businessunittype)
                    .WithMany(p => p.MStandardsectionmappings)
                    .HasForeignKey(d => d.Businessunittypeid)
                    .HasConstraintName("FK__m_standar__busin__5BAD9CC8");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MStandardsectionmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_standar__creat__5CA1C101");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MStandardsectionmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_standar__modif__5D95E53A");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MStandardsectionmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_standar__statu__5E8A0973");
            });

            modelBuilder.Entity<MStatus>(entity =>
            {
                entity.ToTable("m_status");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(265)
                    .HasColumnName("description");

                entity.Property(e => e.Isactive).HasColumnName("isactive");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Statustypeid).HasColumnName("statustypeid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MStatusCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_status__create__5F7E2DAC");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MStatusModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_status__modifi__607251E5");

                entity.HasOne(d => d.Statustype)
                    .WithMany(p => p.MStatuses)
                    .HasForeignKey(d => d.Statustypeid)
                    .HasConstraintName("FK__m_status__status__6166761E");
            });

            modelBuilder.Entity<MStatustype>(entity =>
            {
                entity.ToTable("m_statustype");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description)
                    .HasMaxLength(265)
                    .HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MStatustypeCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_statust__creat__625A9A57");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MStatustypeModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_statust__modif__634EBE90");
            });

            modelBuilder.Entity<MSubject>(entity =>
            {
                entity.ToTable("m_subjects");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.MSubjects)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Branc__6442E2C9");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSubjectCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_subject__Creat__65370702");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSubjectModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_subject__Modif__662B2B3B");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSubjects)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_subject__Statu__671F4F74");

                entity.Property(e => e.DRCSubjectOrder).HasColumnName("DRCSubjectOrder");
            });
            modelBuilder.Entity<MQuetionPaper>(entity =>
            {
                entity.ToTable("m_quetionpapers");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.PaperName)
                    .HasMaxLength(128)
                    .HasColumnName("PaperName");

                entity.Property(e => e.AcademicYearId)
                    .HasColumnName("AcademicYearId");

                entity.Property(e => e.GradeId)
                    .HasColumnName("GradeId");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectId");

                entity.Property(e => e.SemesterId)
                    .HasColumnName("SemesterId");

                entity.Property(e => e.ExamId)
                   .HasColumnName("ExamId");

                entity.Property(e => e.QuestionPaperTypeId)
                  .HasColumnName("QuestionPaperTypeId");

                entity.Property(e => e.UploadedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UploadedDate");

                entity.Property(e => e.UploadedBy)
                   .HasMaxLength(128)
                   .HasColumnName("UploadedBy");

                entity.Property(e => e.Content)
                   .HasMaxLength(128)
                   .HasColumnName("Content");

                entity.Property(e => e.PDFContent)
                 .HasMaxLength(128)
                 .HasColumnName("PDFContent");
            });

            modelBuilder.Entity<MSyllabus>(entity =>
            {
                entity.ToTable("m_syllabus");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.PaperName)
                    .HasMaxLength(128)
                    .HasColumnName("PaperName");

                entity.Property(e => e.AcademicYearId)
                    .HasColumnName("AcademicYearId");

                entity.Property(e => e.GradeId)
                    .HasColumnName("GradeId");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectId");

                entity.Property(e => e.SemesterId)
                    .HasColumnName("SemesterId");

                entity.Property(e => e.ExamId)
                   .HasColumnName("ExamId");


                entity.Property(e => e.UploadedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UploadedDate");

                entity.Property(e => e.UploadedBy)
                   .HasMaxLength(128)
                   .HasColumnName("UploadedBy");

                entity.Property(e => e.Content)
                   .HasMaxLength(128)
                   .HasColumnName("Content");

                entity.Property(e => e.PDFContent)
                  .HasMaxLength(128)
                  .HasColumnName("PDFContent");


            });
            //QI Jaliya 30/8/2024
            modelBuilder.Entity<MSchoolprompt>(entity =>
            {
                entity.ToTable("m_schoolprompt");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.SchoolId)
                    .HasColumnName("SchoolId");

                entity.Property(e => e.Prompttype1)
                    .HasColumnName("Prompttype1");

                entity.Property(e => e.Prompttype2)
                    .HasColumnName("Prompttype2");

                entity.Property(e => e.Prompttype3)
                    .HasColumnName("Prompttype3");

                entity.Property(e => e.Prompttype4)
                    .HasColumnName("Prompttype4");

                entity.Property(e => e.Prompttype5)
                    .HasColumnName("Prompttype5");


            });

            modelBuilder.Entity<MPrompttype>(entity =>
            {
                entity.ToTable("m_prompttype");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Promptypename)
                  .HasMaxLength(128)
                  .HasColumnName("Promptypename");

                entity.Property(e => e.Promtypefromat)
                  .HasMaxLength(128)
                  .HasColumnName("Promtypefromat");


            });

            modelBuilder.Entity<MGenerateQuetionPaper>(entity =>
            {
                entity.ToTable("m_generatequetionpapers");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.PaperName)
                    .HasMaxLength(128)
                    .HasColumnName("PaperName");

                entity.Property(e => e.AcademicYearFromId)
                    .HasColumnName("AcademicYearFromId");

                entity.Property(e => e.AcademicYearToId)
                   .HasColumnName("AcademicYearToId");

                entity.Property(e => e.GradeId)
                    .HasColumnName("GradeId");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("SubjectId");

                entity.Property(e => e.SemesterId)
                    .HasColumnName("SemesterId");

                entity.Property(e => e.ExamId)
                   .HasColumnName("ExamId");


                entity.Property(e => e.UploadedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UploadedDate");

                entity.Property(e => e.UploadedBy)
                   .HasMaxLength(128)
                   .HasColumnName("UploadedBy");

                entity.Property(e => e.Content)
                   .HasMaxLength(128)
                   .HasColumnName("Content");

                entity.Property(e => e.PDFContent)
                 .HasMaxLength(128)
                 .HasColumnName("PDFContent");
            });
            modelBuilder.Entity<MSubjectsectionmapping>(entity =>
            {
                entity.ToTable("m_subjectsectionmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSubjectsectionmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_subject__Creat__681373AD");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSubjectsectionmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_subject__Modif__690797E6");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.MSubjectsectionmappings)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Secti__69FBBC1F");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSubjectsectionmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_subject__Statu__6AEFE058");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.MSubjectsectionmappings)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Subje__6BE40491");

               
            });

            modelBuilder.Entity<MSubjectsemesterpercentage>(entity =>
            {
                entity.ToTable("m_subjectsemesterpercentage");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.OverallSemesterPosition).HasMaxLength(50);

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.MSubjectsemesterpercentages)
                    .HasForeignKey(d => d.ChildId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Child__6CD828CA");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MSubjectsemesterpercentageCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_subject__Creat__6DCC4D03");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MSubjectsemesterpercentageModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_subject__Modif__6EC0713C");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.MSubjectsemesterpercentages)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Semes__6FB49575");

                entity.HasOne(d => d.Standard)
                    .WithMany(p => p.MSubjectsemesterpercentages)
                    .HasForeignKey(d => d.StandardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Stand__70A8B9AE");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MSubjectsemesterpercentages)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_subject__Statu__719CDDE7");

                entity.HasOne(d => d.SubjectSectionMapping)
                    .WithMany(p => p.MSubjectsemesterpercentages)
                    .HasForeignKey(d => d.SubjectSectionMappingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Subje__72910220");
            });

            modelBuilder.Entity<MSubjecttestmapping>(entity =>
            {
                entity.ToTable("m_subjecttestmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.SemesterYearMapping)
                    .WithMany(p => p.MSubjecttestmappings)
                    .HasForeignKey(d => d.SemesterYearMappingId)
                    .HasConstraintName("FK__m_subject__Semes__43A1090D");

                entity.HasOne(d => d.SubjectSectionMapping)
                    .WithMany(p => p.MSubjecttestmappings)
                    .HasForeignKey(d => d.SubjectSectionMappingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__Subje__73852659");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.MSubjecttestmappings)
                    .HasForeignKey(d => d.TestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_subject__TestI__74794A92");
            });
            modelBuilder.Entity<MTeachersubjectmapping>(entity =>
            {
                entity.ToTable("m_teachersubjectmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.Property(e => e.AcademicYearID).HasColumnName("AcademicYearID");
                entity.Property(e => e.TeacherId).HasColumnName("TeacherId");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MTeachersubjectmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_teacher__Creat__756D6ECB");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MTeachersubjectmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_teacher__Modif__76619304");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MTeachersubjectmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_teacher__Statu__7755B73D");

                entity.HasOne(d => d.SubjectSection)
                    .WithMany(p => p.MTeachersubjectmappings)
                    .HasForeignKey(d => d.SubjectSectionId)
                    .HasConstraintName("FK__m_teacher__Subje__7849DB76");

            });

            modelBuilder.Entity<MTestsectionmapping>(entity =>
            {
                entity.ToTable("m_testsectionmapping");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.Modifieddate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MTestsectionmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_testsec__Creat__7A3223E8");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MTestsectionmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_testsec__Modif__7B264821");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.MTestsectionmappings)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_testsec__Secti__7C1A6C5A");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MTestsectionmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_testsec__Statu__7D0E9093");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.MTestsectionmappings)
                    .HasForeignKey(d => d.TestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__m_testsec__TestI__7E02B4CC");
            });

            modelBuilder.Entity<MUsermodulemapping>(entity =>
            {
                entity.ToTable("m_usermodulemapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Moduleid).HasColumnName("moduleid");

                entity.Property(e => e.Schooluserid).HasColumnName("schooluserid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.MUsermodulemappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__m_usermod__creat__7EF6D905");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.MUsermodulemappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__m_usermod__modif__7FEAFD3E");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.MUsermodulemappings)
                    .HasForeignKey(d => d.Moduleid)
                    .HasConstraintName("FK__m_usermod__modul__00DF2177");

                entity.HasOne(d => d.Schooluser)
                    .WithMany(p => p.MUsermodulemappingSchoolusers)
                    .HasForeignKey(d => d.Schooluserid)
                    .HasConstraintName("FK__m_usermod__schoo__01D345B0");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MUsermodulemappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__m_usermod__statu__02C769E9");
            });

            modelBuilder.Entity<TCalendereventdetail>(entity =>
            {
                entity.ToTable("t_calendereventdetails");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment).HasColumnName("attachment");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Venue)
                    .HasMaxLength(256)
                    .HasColumnName("venue");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TCalendereventdetailCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_calende__creat__03BB8E22");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TCalendereventdetailModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_calende__modif__04AFB25B");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.TCalendereventdetails)
                    .HasForeignKey(d => d.Schoolid)
                    .HasConstraintName("FK__t_calende__schoo__05A3D694");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TCalendereventdetails)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_calende__stand__0697FACD");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TCalendereventdetails)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_calende__statu__078C1F06");
            });

            modelBuilder.Entity<TClassCalenderevents>(entity =>
            {
                entity.ToTable("t_classcalenderevents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.sectionId).HasColumnName("sectionId");

                entity.Property(e => e.schoolid).HasColumnName("schoolid");

                entity.Property(e => e.calendereventid).HasColumnName("calendereventid");
            });

            modelBuilder.Entity<TEmaillog>(entity =>
            {
                entity.ToTable("t_emaillog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Emailcount).HasColumnName("emailcount");

                entity.Property(e => e.Fromemailid)
                    .HasMaxLength(128)
                    .HasColumnName("fromemailid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Noticeboardmsgid).HasColumnName("noticeboardmsgid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Toemailid)
                    .HasMaxLength(128)
                    .HasColumnName("toemailid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TEmaillogCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_emaillo__creat__0880433F");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TEmaillogModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_emaillo__modif__09746778");

                entity.HasOne(d => d.Noticeboardmsg)
                    .WithMany(p => p.TEmaillogs)
                    .HasForeignKey(d => d.Noticeboardmsgid)
                    .HasConstraintName("FK__t_emaillo__notic__0A688BB1");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TEmaillogs)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_emaillo__statu__0B5CAFEA");
            });

            modelBuilder.Entity<TGclteacherclass>(entity =>
            {
                entity.ToTable("t_gclteacherclass");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Branchid).HasColumnName("branchid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.IsApproved).HasColumnName("isApproved");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.TeacherId).HasColumnName("teacherId");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TGclteacherclasses)
                    .HasForeignKey(d => d.Branchid)
                    .HasConstraintName("FK__t_gclteac__branc__0C50D423");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TGclteacherclassCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_gclteac__creat__0D44F85C");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TGclteacherclassModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_gclteac__modif__0E391C95");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TGclteacherclasses)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_gclteac__stand__0F2D40CE");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TGclteacherclasses)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_gclteac__statu__10216507");
            });

            modelBuilder.Entity<TGclvedioclass>(entity =>
            {
                entity.ToTable("t_gclvedioclass");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Enddate)
                    .HasColumnType("datetime")
                    .HasColumnName("enddate");

                entity.Property(e => e.Meetinglink)
                    .HasMaxLength(256)
                    .HasColumnName("meetinglink");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Startdate)
                    .HasColumnType("datetime")
                    .HasColumnName("startdate");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TGclvedioclassCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_gclvedi__creat__11158940");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TGclvedioclassModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_gclvedi__modif__1209AD79");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TGclvedioclasses)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_gclvedi__stand__12FDD1B2");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TGclvedioclasses)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_gclvedi__statu__13F1F5EB");
            });

            modelBuilder.Entity<TGoogleclass>(entity =>
            {
                entity.ToTable("t_googleclass");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Gcourseid)
                    .HasMaxLength(128)
                    .HasColumnName("gcourseid");

                entity.Property(e => e.Gownerid)
                    .HasMaxLength(128)
                    .HasColumnName("gownerid");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Teacherfolderid)
                    .HasMaxLength(128)
                    .HasColumnName("teacherfolderid");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TGoogleclassCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_googlec__creat__14E61A24");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TGoogleclassModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_googlec__modif__15DA3E5D");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TGoogleclasses)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_googlec__stand__16CE6296");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TGoogleclasses)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_googlec__statu__17C286CF");
            });

            modelBuilder.Entity<TNoticeboardmapping>(entity =>
            {
                entity.ToTable("t_noticeboardmapping");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Appuserid).HasColumnName("appuserid");

                entity.Property(e => e.Childid).HasColumnName("childid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Noticeboardmsgid).HasColumnName("noticeboardmsgid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.HasOne(d => d.Appuser)
                    .WithMany(p => p.TNoticeboardmappings)
                    .HasForeignKey(d => d.Appuserid)
                    .HasConstraintName("FK__t_noticeb__appus__18B6AB08");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.TNoticeboardmappings)
                    .HasForeignKey(d => d.Childid)
                    .HasConstraintName("FK__t_noticeb__child__19AACF41");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TNoticeboardmappingCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_noticeb__creat__1A9EF37A");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TNoticeboardmappingModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_noticeb__modif__1B9317B3");

                entity.HasOne(d => d.Noticeboardmsg)
                    .WithMany(p => p.TNoticeboardmappings)
                    .HasForeignKey(d => d.Noticeboardmsgid)
                    .HasConstraintName("FK__t_noticeb__notic__1C873BEC");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TNoticeboardmappings)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_noticeb__statu__1D7B6025");
            });

            modelBuilder.Entity<TNoticeboardmessage>(entity =>
            {
                entity.ToTable("t_noticeboardmessage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachments).HasColumnName("attachments");

                entity.Property(e => e.Branchid).HasColumnName("branchid");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Isemail).HasColumnName("isemail");

                entity.Property(e => e.Ispriority).HasColumnName("ispriority");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Schooluserid).HasColumnName("schooluserid");

                entity.Property(e => e.Sms).HasColumnName("sms");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Subject).HasColumnName("subject");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TNoticeboardmessages)
                    .HasForeignKey(d => d.Branchid)
                    .HasConstraintName("FK__t_noticeb__branc__1E6F845E");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TNoticeboardmessageCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_noticeb__creat__1F63A897");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TNoticeboardmessageModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_noticeb__modif__2057CCD0");

                entity.HasOne(d => d.Schooluser)
                    .WithMany(p => p.TNoticeboardmessageSchoolusers)
                    .HasForeignKey(d => d.Schooluserid)
                    .HasConstraintName("FK__t_noticeb__schoo__214BF109");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TNoticeboardmessages)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_noticeb__stand__22401542");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TNoticeboardmessages)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_noticeb__statu__2334397B");
            });

            modelBuilder.Entity<TSoundingboardmessage>(entity =>
            {
                entity.ToTable("t_soundingboardmessage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Appuserinfoid).HasColumnName("appuserinfoid");

                entity.Property(e => e.Attachments).HasColumnName("attachments");

                entity.Property(e => e.Categoryid).HasColumnName("categoryid");

                entity.Property(e => e.Childinfoid).HasColumnName("childinfoid");

                entity.Property(e => e.Commentscount).HasColumnName("commentscount");

                entity.Property(e => e.Createdby).HasColumnName("createdby");

                entity.Property(e => e.Createddate)
                    .HasColumnType("datetime")
                    .HasColumnName("createddate");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Didread).HasColumnName("didread");

                entity.Property(e => e.Groupid).HasColumnName("groupid");

                entity.Property(e => e.Isparentreplied).HasColumnName("isparentreplied");

                entity.Property(e => e.Isstaffreplied).HasColumnName("isstaffreplied");

                entity.Property(e => e.Modifiedby).HasColumnName("modifiedby");

                entity.Property(e => e.Modifieddate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifieddate");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Subject).HasColumnName("subject");

                entity.HasOne(d => d.Appuserinfo)
                    .WithMany(p => p.TSoundingboardmessageAppuserinfos)
                    .HasForeignKey(d => d.Appuserinfoid)
                    .HasConstraintName("FK__t_soundin__appus__24285DB4");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.TSoundingboardmessages)
                    .HasForeignKey(d => d.Categoryid)
                    .HasConstraintName("FK__t_soundin__categ__251C81ED");

                entity.HasOne(d => d.Childinfo)
                    .WithMany(p => p.TSoundingboardmessages)
                    .HasForeignKey(d => d.Childinfoid)
                    .HasConstraintName("FK__t_soundin__child__2610A626");

                entity.HasOne(d => d.CreatedbyNavigation)
                    .WithMany(p => p.TSoundingboardmessageCreatedbyNavigations)
                    .HasForeignKey(d => d.Createdby)
                    .HasConstraintName("FK__t_soundin__creat__2704CA5F");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.TSoundingboardmessages)
                    .HasForeignKey(d => d.Groupid)
                    .HasConstraintName("FK__t_soundin__group__27F8EE98");

                entity.HasOne(d => d.ModifiedbyNavigation)
                    .WithMany(p => p.TSoundingboardmessageModifiedbyNavigations)
                    .HasForeignKey(d => d.Modifiedby)
                    .HasConstraintName("FK__t_soundin__modif__28ED12D1");

                entity.HasOne(d => d.Standardsectionmapping)
                    .WithMany(p => p.TSoundingboardmessages)
                    .HasForeignKey(d => d.Standardsectionmappingid)
                    .HasConstraintName("FK__t_soundin__stand__29E1370A");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TSoundingboardmessages)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_soundin__statu__2AD55B43");
            });

            modelBuilder.Entity<TToken>(entity =>
            {
                entity.ToTable("t_token");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("ipaddress");

                entity.Property(e => e.Referenceid).HasColumnName("referenceid");

                entity.Property(e => e.Statusid).HasColumnName("statusid");

                entity.Property(e => e.Ttl)
                    .HasColumnType("datetime")
                    .HasColumnName("ttl");

                entity.Property(e => e.Usertype)
                    .HasMaxLength(32)
                    .HasColumnName("usertype");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TTokens)
                    .HasForeignKey(d => d.Statusid)
                    .HasConstraintName("FK__t_token__statusi__2BC97F7C");
            });

            modelBuilder.Entity<VClassdetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_classdetails");

                entity.Property(e => e.Businessunittypeid).HasColumnName("businessunittypeid");

                entity.Property(e => e.Parentid).HasColumnName("parentid");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");

                entity.Property(e => e.Standardsectionmappingid).HasColumnName("standardsectionmappingid");

                entity.Property(e => e.Userid).HasColumnName("userid");
            });

            modelBuilder.Entity<VDashboardcount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_dashboardcount");

                entity.Property(e => e.Childid).HasColumnName("childid");

                entity.Property(e => e.Parentid).HasColumnName("parentid");

                entity.Property(e => e.Schoolid).HasColumnName("schoolid");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
