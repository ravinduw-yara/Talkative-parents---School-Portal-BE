using CommonUtility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace TalkativeParentAPI.Installers
{

    public static class InstallerExtentions
    {
        public static void InstallServicesAssembly(this IServiceCollection services, IConfiguration configuraiton)
        {

            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
                             typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                             .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.InstallServices(configuraiton, services));
        }
    }

    public interface IInstaller
    {
        void InstallServices(IConfiguration configuration, IServiceCollection services);
    }

    public class DependencyRegistration : IInstaller
    {

        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<TpContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("TpConnectionString"));
            });

            #region Dependency Registrations
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<TpContext>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IMBranchService, MBranchService>();

            services.AddScoped<IMStandardsectionmappingService, MStandardsectionmappingService>();

            services.AddScoped<IMBusinessUnitTypeService, MBusinessUnitTypeService>();

            services.AddScoped<IMChildinfoService, MChildinfoService>();

            services.AddScoped<IMGenderService, MGenderService>();

            services.AddScoped<IMLocationService, MLocationService>();

            services.AddScoped<IMLocationtypeService, MLocationtypeService>();

            services.AddScoped<IMModuleService, MModuleService>();

            services.AddScoped<IMRelationtypeService, MRelationtypeService>();

            services.AddScoped<IMSchoolService, MSchoolService>();

            services.AddScoped<IMStatusService, MStatusService>();

            services.AddScoped<IMStatustypeService, MStatustypeService>();

            services.AddScoped<IUtilityService, UtilityService>();

            services.AddScoped<IMChildschoolmappingService, MChildschoolmappingService>();

            services.AddScoped<IMParentchildmappingService, MParentchildmappingService>();

            services.AddScoped<IMUsermodulemappingService, MUsermodulemappingService>();

            services.AddScoped<IAppuserdeviceService, AppuserdeviceService>();

            services.AddScoped<IMFeatureService, MFeatureService>();

            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<ITSoundingboardmessageService, TSoundingboardmessageService>();

            services.AddScoped<ITNoticeboardmessageService, TNoticeboardmessageService>();

            services.AddScoped<ITNoticeboardmappingService, TNoticeboardmappingService>();

            services.AddScoped<IDataConnector, DataConnector>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ITEmaillogService, TEmaillogService>();

            services.AddScoped<IFirebaseService, FirebaseService>();

            services.AddScoped<ICloudBlobManager, CloudBlobManager>();

            services.AddScoped<ITCalendereventdetailService, TCalendereventdetailService>();

            services.AddScoped<IMCategoryService, MCategoryService>();

            services.AddScoped<IMGroupService, MGroupService>();

            services.AddScoped<IMStandardgroupmappingService, MStandardgroupmappingService>();

            services.AddScoped<IMSchooluserinfoService, MSchooluserinfoService>();

            services.AddScoped<IMSchooluserroleService, MSchooluserroleService>();

            services.AddScoped<ITTokenService, TTokenService>();

            services.AddScoped<ITGoogleclassService, TGoogleclassService>();

            services.AddScoped<ITGclvedioclassService, TGclvedioclassService>();

            services.AddScoped<IMAppuserinfoService, MAppuserinfoService>();

            services.AddScoped<IDataAnalysisServices, DataAnalysisServices>();

            services.AddScoped<IMSubjectsService, MSubjectService>();

            services.AddScoped<IMTeacherSubjectMappingService, MTeacherSubjectMappingService>();

            services.AddScoped<IMSemesterTestsMappingService, MSemesterTestsMappingService>();

            services.AddScoped<IMSubjectTestMappingService, MSubjectTestMappingService>();

            services.AddScoped<IMSubjectSectionMappingService, MSubjectSectionMappingService>();

            services.AddScoped<IMSemesterYearMappingService, MSemesterYearMappingService>();

            services.AddScoped<IMSubSubjectService, MSubSubjectService>();

            services.AddScoped<IMLevelService, MLevelService>();

            services.AddScoped<IMHiEduCourseService, MHiEduCourseService>();

            services.AddScoped<IMHiEduSubjectExamMappingService, MHiEduSubjectExamMappingService>();

            services.AddScoped<IMHiEduBatchService, MHiEduBatchService>();

            services.AddScoped<IMHiEduSubSubjectService, MHiEduSubSubjectService>();
            services.AddScoped<IMHiEduChildInfoStudentService, MHiEduChildInfoStudentService>();
            services.AddScoped<IMHiEduCoursesemestersExamsService, MHiEduCoursesemestersExamsService>(); 
            services.AddScoped<IMHiEduSemesterService, MHiEduSemesterService>();
            services.AddScoped<IMAcadamicyearService, MAcadamicyearService>();
            services.AddScoped<IMQuetionPaperService, MQuetionPaperService>();
            services.AddScoped<ITClassCalendereventsService, TClassCalendereventsService>();
            services.AddScoped<IMSyllabusService, MSyllabusService>();


            #endregion
        }
    }
}
