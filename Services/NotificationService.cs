using CommonUtility;
using Microsoft.EntityFrameworkCore;

//using Newtonsoft.Json;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
//using Newtonsoft.Json;
using System.Text.Json;
namespace Services
{
    public interface INotificationService //: ICommonService
    {
        string PushNotification(string message, string messageType, string apikey, string authorization, string Players, int? Devicetype, int? ChildId, int SchoolId, int SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask);
        string PushNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, string mask);
        //string PushNoti1(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, Guid? ChildId, Guid? SchoolId, Guid? SectionId, Guid? StandardId, int? Gender, string Subject, object _MessageId, string mask);

        public string PushNoti1(string message, string messageType, string apikey, string authorization, string Players, int? Devicetype, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask);


        string PushCalNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask);
        //string PushSBNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, Guid? ParentId, Guid? ChildId, Guid? SchoolId, string SectionId, string StandardId, string Subject, Guid? _MessageId, string mask);
        string PushSBNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ParentId, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, string Subject, int? _MessageId, string mask);
        string PushTeacherAppSBNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ParentId, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, string Subject, int? _MessageId, string mask);

    }
    public class NotificationService : INotificationService
    {
        //SchoolMessageController
        //messageType is hardcoded as '2'

        public string PushNotification(string message, string messageType, string apikey, string authorization, string Players, int? Devicetype, int? ChildId, int SchoolId, int SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask)
        {
            if (CommonUtility.Common.SendNotification == false)
            {
                return "Blocked Notification";
            }
            if (_MessageId == null)
            {
                return "Blocked Notification";
            }

                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            
            //if (Devicetype == 1)
           // {
                //apikey = "8c09536b-71d0-4ee5-b085-dfe25387cdb9";
                 //authorization = "MjIxZmM5MDEtYjY4NS00ZmJkLTlkOWEtMTZmNWI0YmU5OWMz";
               // authorization = "YWVmYmY5NjYtZTdkNS00NjBmLWE3ZTItNzMxMDg2ZjJkZDc3"; //24/8/2024
               // authorization = "NjljZDQwNGEtZDJkNi00MjRiLWFiOGYtMjBjZTFlYjFiZmU1";
                //
               // }
            request.Headers.Add("authorization", "Basic " + authorization);
            var noticeBoard = "{\"id\":\"" + _MessageId + "\""
                               + ",\"subject\":\"" + Subject + "\""
                               + " }";

            var feature = "NB";

            var childObject = "{\"childId\":\"" + ChildId + "\""
                               + ",\"schoolId\":\"" + SchoolId + "\""
                               + ",\"sectionId\":\"" + SectionId + "\""
                               + ",\"standardId\":\"" + StandardId + "\""
                               + ",\"Gender\":\"" + Gender + "\""
                               + " }";

            var payload = "{"
               + "\"app_id\": \"" + apikey + "\","
               + "\"include_player_ids\": [\"" + Players + "\"],"
               + "\"ios_badgeType\": \"Increase\","
               + "\"ios_badgeCount\": 1,"
               + "\"priority\": 10,"

                + "\"contents\": { \"en\": \"" + Subject + "\"},"
               + "\"data\": { \"feature\":\"" + feature + "\""
                                     + ",\"child\":" + childObject

                                     + ",\"NoticeBoard\":" + noticeBoard
                                     + " },"


               + "\"headings\": { \"en\": \"" + mask + " " + "sent you a notification\"}}";


            byte[] byteArray = Encoding.UTF8.GetBytes(payload);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }

                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return "NULL";
            }

        }
        public string PushNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, string mask)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("authorization", "Basic " + authorization);

            var payload = "{"
                 + "\"app_id\": \"" + apikey + "\","
                 + "\"include_player_ids\": [\"" + Players + "\"],"
                 + "\"ios_badgeType\": \"Increase\","
                 + "\"ios_badgeCount\": 1,"
                 + "\"priority\": 10,"
                 + "\"data\": { \"MessageType\":" + messageType + " },"
                 + "\"contents\": { \"en\": \"" + message + "\"},"
                 + "\"headings\": { \"en\": \"" + mask + " " + "sent you a notification\"}}";

            byte[] byteArray = Encoding.UTF8.GetBytes(payload);
            string responseContent = null;
            Console.WriteLine(payload);
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0; 
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }
                            count = Convert.ToInt32( item["Recipients"]); // Added Casting for the Error given as Cannot implicityly convert type long to int
                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return "NULL";
            }
        }

        //SchoolMessageController


        public string PushNoti1(string message, string messageType, string apikey, string authorization, string Players, int? Devicetype, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask)
        {
            if (Common.SendNotification == false)
            {
                return "Blocked Notification";
            }

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + authorization);



            var noticeBoard = "{\"id\":\"" + _MessageId + "\""
                               + ",\"subject\":\"" + Subject + "\""
                               + " }";

            var feature = "NB";

            var childObject = "{\"childId\":\"" + ChildId + "\""
                               + ",\"schoolId\":\"" + SchoolId + "\""
                               + ",\"sectionId\":\"" + SectionId + "\""
                               + ",\"standardId\":\"" + StandardId + "\""
                               + ",\"Gender\":\"" + Gender + "\""
                               + " }";

            var payload = "{"
               + "\"app_id\": \"" + apikey + "\","
               + "\"include_player_ids\": [\"" + Players + "\"],"
               + "\"ios_badgeType\": \"Increase\","
               + "\"ios_badgeCount\": 1,"
               + "\"priority\": 10,"

                + "\"contents\": { \"en\": \"" + message + "\"},"
               + "\"data\": { \"feature\":\"" + feature + "\""
                                     + ",\"child\":" + childObject

                                     + ",\"NoticeBoard\":" + noticeBoard
                                     + " },"


               + "\"headings\": { \"en\": \"" + mask + " " + "sent you a notification\"}}";


            byte[] byteArray = Encoding.UTF8.GetBytes(payload);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }
                            count = Convert.ToInt32(item["Recipients"]); // Added Casting for the Error given as Cannot implicityly convert type long to int
                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return "NULL";
            }
        }

        //SchoolCalendarEventsController
        public string PushCalNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, int? Gender, string Subject, object _MessageId, string mask)
        {

            if (Common.SendNotification == false)
            {
                return "Blocked Notification";
            }
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + authorization);



            var Calendar = "{\"id\":\"" + _MessageId + "\""

                               + ",\"subject\":\"" + Subject + "\""
                               + " }";

            var feature = "CAL";

            var childObject = "{\"childId\":\"" + ChildId + "\""
                               + ",\"schoolId\":\"" + SchoolId + "\""
                               + ",\"sectionId\":\"" + SectionId + "\""
                               + ",\"standardId\":\"" + StandardId + "\""
                               + ",\"Gender\":\"" + Gender + "\""
                               + " }";

            var payload = "{"
               + "\"app_id\": \"" + apikey + "\","
               + "\"include_player_ids\": [\"" + Players + "\"],"
               + "\"ios_badgeType\": \"Increase\","
               + "\"ios_badgeCount\": 1,"
               + "\"priority\": 10,"

            + "\"contents\": { \"en\": \"" + message + "\"},"
               + "\"data\": { \"feature\":\"" + feature + "\""
                                     + ",\"child\":" + childObject

                                     + ",\"Calendar\":" + Calendar
                                     + " },"


               + "\"headings\": { \"en\": \"" + mask + " " + "sent you a calendar update\"}}";


            byte[] byteArray = Encoding.UTF8.GetBytes(payload);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }
                            //  count = Convert.ToInt32(item["Recipients"]);// Added Casting for the Error given as Cannot implicityly convert type long to int
                            //count = Convert.ToInt32(item["Id"]);
                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return "NULL";
            }


        }

        //SoundingBoardController and StorageController
        public string PushSBNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ParentId, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, string Subject, int? _MessageId, string mask)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + authorization);

            var soundingBoard = "{\"id\":\"" + _MessageId + "\""
                               + ",\"subject\":\"" + Subject + "\""
                               + " }";



            var childObject = "{\"childId\":\"" + ChildId + "\""
                               + ",\"schoolId\":\"" + SchoolId + "\""
                               + ",\"sectionId\":\"" + SectionId + "\""
                               + ",\"standardId\":\"" + StandardId + "\""
                               + ",\"ParentId\":\"" + ParentId + "\""
                               + " }";

            var feature = "SB";

            var payload = "{"
                 + "\"app_id\": \"" + apikey + "\","
               + "\"include_player_ids\": [\"" + Players + "\"],"
               + "\"ios_badgeType\": \"Increase\","
               + "\"ios_badgeCount\": 1,"
               + "\"priority\": 10,"
                + "\"contents\": { \"en\": \"" + Subject + "\"},"
               + "\"data\": { \"feature\":\"" + feature + "\""
                                     + ",\"child\":" + childObject
                                     + ",\"SoundingBoard\":" + soundingBoard
                                     + " },"
                                     + "\"headings\": { \"en\": \"" + mask + " " + "has replied to your message\"}}";

            byte[] byteArray = Encoding.UTF8.GetBytes(payload);
            string responseContent = null;
            Console.WriteLine(payload);
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }
                            //count = Convert.ToInt32(item["Id"]);// Added Casting for the Error given as Cannot implicityly convert type long to int
                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return null;
            }
        }

        public string PushTeacherAppSBNoti(string message, string messageType, string apikey, string authorization, string Players, int Devicetype, int? ParentId, int? ChildId, int? SchoolId, int? SectionId, int? StandardId, string Subject, int? _MessageId, string mask)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + authorization);

            var soundingBoard = "{\"id\":\"" + _MessageId + "\""
                               + ",\"subject\":\"" + Subject + "\""
                               + " }";



            var childObject = "{\"childId\":\"" + ChildId + "\""
                               + ",\"schoolId\":\"" + SchoolId + "\""
                               + ",\"sectionId\":\"" + SectionId + "\""
                               + ",\"standardId\":\"" + StandardId + "\""
                               + ",\"ParentId\":\"" + ParentId + "\""
                               + " }";

            var feature = "SB";

            var payload = "{"
                 + "\"app_id\": \"" + apikey + "\","
               + "\"include_player_ids\": [\"" + Players + "\"],"
               + "\"ios_badgeType\": \"Increase\","
               + "\"ios_badgeCount\": 1,"
               + "\"priority\": 10,"
                + "\"contents\": { \"en\": \"" + Subject + "\"},"
               + "\"data\": { \"feature\":\"" + feature + "\""
                                     + ",\"child\":" + childObject
                                     + ",\"SoundingBoard\":" + soundingBoard
                                     + " },"
                                     + "\"headings\": { \"en\": \"" + mask + " " + "has replied to your message\"}}";

            byte[] byteArray = Encoding.UTF8.GetBytes(payload);
            string responseContent = null;
            Console.WriteLine(payload);
            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }
                var id = "";
                int count = 0;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                        Nancy.Json.JavaScriptSerializer serializer = new Nancy.Json.JavaScriptSerializer();
                        dynamic item = serializer.Deserialize<object>(responseContent.ToString());
                        if (item["Id"] != "")
                        {
                            if (Devicetype == 1)
                            {
                                id = "A_" + item["Id"];
                            }
                            if (Devicetype == 2)
                            {
                                id = "I_" + item["Id"];
                            }
                            //count = Convert.ToInt32(item["Id"]);// Added Casting for the Error given as Cannot implicityly convert type long to int
                            return id;
                        }
                        else
                        {
                            return "NULL";
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return null;
            }
        }

        //Not Implemented
        //public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    return true;
        //}

        //Not Implemented
        //public static string FCMNotification()
        //{
        //    Notification notification = new Notification();
        //    string serverKey = "AIz..........Fep0";
        //    string senderId = "30............8";
        //    WebRequest tRequest = WebRequest.Create("https://project-3333275275859062414.firebaseio.com/fcm/send");
        //    tRequest.Method = "post";
        //    tRequest.ContentType = "application/json";
        //    var objNotification = new
        //    {
        //        to = notification.DeviceToken,
        //        data = new
        //        {
        //            title = notification.NotificationTitle,
        //            body = notification.NotificationBody
        //        }
        //    };
        //    string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

        //    Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
        //    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
        //    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
        //    tRequest.ContentLength = byteArray.Length;
        //    tRequest.ContentType = "application/json";
        //    using (Stream dataStream = tRequest.GetRequestStream())
        //    {
        //        dataStream.Write(byteArray, 0, byteArray.Length);

        //        using (WebResponse tResponse = tRequest.GetResponse())
        //        {
        //            using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //            {
        //                using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                {
        //                    String responseFromFirebaseServer = tReader.ReadToEnd();

        //                    FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);
        //                    return response.canonical_ids.ToString();
        //                    //if (response.success == 1)
        //                    //{
        //                    //    new NotificationBLL().InsertNotificationLog(dayNumber, notification, true);
        //                    //}
        //                    //else if (response.failure == 1)
        //                    //{
        //                    //    new NotificationBLL().InsertNotificationLog(dayNumber, notification, false);
        //                    //    sbLogger.AppendLine(string.Format("Error sent from FCM server, after sending request : {0} , for following device info: {1}", responseFromFirebaseServer, jsonNotificationFormat));
        //                    //}

        //                }
        //            }

        //        }
        //    }
        //}



    }
}
