using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtility
{
    public static class Common
    {
        public static bool SendSMS = true;
        public static bool SendNotification = true;
        public static bool SaveNotificationToDB = true;
        public static bool SendEMail = true;

        public static void DisableAll()
        {
            SendSMS = false;
            SendNotification = false;
            SaveNotificationToDB = false;
            SendEMail = false;
        }

        public static void EnableAll()
        {
            SendSMS = true;
            SendNotification = true;
            SaveNotificationToDB = true;
            SendEMail = true;
        }

        public enum ChannelType
        {
            AdminNotification, AdminToParent, AdminToSchool, AdminToAllSchools, AdminToCity, AdminToState, AdminToCountry, SchoolToParent, SchoolSoundingBoard, SchoolToAllInSchool, SchoolToStandard, SchoolToSection, ParentToParent, ParentToSection, ParentToSchoolCategory
        }

    }
}
