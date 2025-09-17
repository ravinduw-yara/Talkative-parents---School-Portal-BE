using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Services
{
    public interface IFirebaseService
    {
        string SB_Url(int schoolid, int messageId);
        string Forum_Comments_Url(int schoolid, int messageid);
        string Forum_Likes_Url(int schoolid, int messageid);
        string UserStdMapping(int schoolid, int userid);
        string SchoolUsers(string schoolid, string userid);
        string CalendarEvent(PostCalendarEvents calendarEvents, string eventid);
        byte[] ReadFully(Stream input);
        string PostForumCommentsMethod(ForumComments comments);
        dynamic GetForumCommentsMethod(int schoolid, int messageid);

    }


    public class FirebaseService : IFirebaseService
    {
        public string SB_Url(int schoolid, int messageId)
        {
            return @"https://project-3333275275859062414.firebaseio.com/sb-3/private/" + schoolid + "/comments/" + messageId + ".json";
        }

        public string Forum_Comments_Url(int schoolid, int messageid)
        {
            return @"https://project-3333275275859062414.firebaseio.com/" + schoolid + "/forum-2/comments/" + messageid + ".json";
        }

        public string Forum_Likes_Url(int schoolid, int messageid)
        {
            return @"https://project-3333275275859062414.firebaseio.com/" + schoolid + "/forum-2/Likes/" + messageid + ".json";
        }

        public string UserStdMapping(int schoolid, int userid)
        {
            return @"https://project-3333275275859062414.firebaseio.com/" + schoolid + "/schooluser/" + userid + ".json";
        }

        public string SchoolUsers(string schoolid, string userid)
        {
            //return @"https://project-3333275275859062414.firebaseio.com/7331B796-C835-4A89-BBFD-F412F4819CB4/schooluser/F71C631C-5D0B-4698-B3D6-EB952DA7275D/selectedClass.json";
            return @"https://project-3333275275859062414.firebaseio.com/" + schoolid + "/schooluser/" + userid + "/selectedClass" + ".json";
            //return @"https://project-3333275275859062414.firebaseio.com/" + schoolid + "/schooluser/" + userid + "/selectedClass/" + ".json";
        }

        public string CalendarEvent(PostCalendarEvents calendarEvents, string eventid)
        {
            var school = calendarEvents.SchoolId.ToString().ToUpper();
            var events = eventid.ToString().ToUpper();
            //var client = new Firebase.Database.FirebaseObject(
            //var url = @"https://project-3333275275859062414.firebaseio.com/" + school + "/Calendar-V2/" + events + ".json";
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new PostCalendarEvents
            {
                //Id = new Guid(events.ToString().ToUpper()),
                EventTitle = calendarEvents.EventTitle,
                EventDescription = calendarEvents.EventDescription,
                StandardId = calendarEvents.StandardId,
                SectionId = calendarEvents.SectionId,
                SchoolId = calendarEvents.SchoolId,
                Venue = calendarEvents.Venue,
                Attachment = calendarEvents.Attachment,
                StartDate = calendarEvents.StartDate,
                EndDate = calendarEvents.EndDate,
                CreatedDate = calendarEvents.CreatedDate,
                ModifiedDate = calendarEvents.ModifiedDate

            });
            //string Json = "\""+events+"\""
            var request = WebRequest.CreateHttp("https://project-3333275275859062414.firebaseio.com/" + school + "/Calendar-V2/" + ".json");
            request.Method = "POST";
            request.ContentType = "application/json";
            var buffer = Encoding.UTF8.GetBytes(json);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = request.GetResponse();
            json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
            return response.ToString();
            //return @"https://project-3333275275859062414.firebaseio.com/" + school + "/Calendar-V2/" + events + "/"+calendarEvents + ".json";
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public string PostForumCommentsMethod (ForumComments comments)
        {
            var json = JsonConvert.SerializeObject(new
            {
                commentedById = comments.CommentedById,
                commentedByName = comments.CommentedByName,
                message = comments.message,
                attachment = comments.Attachment,
                numberUploaded = comments.numberUploaded,
                isActive = comments.isActive,
                commentedOn = DateTime.UtcNow,
                updateddate = DateTime.UtcNow
            });
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Forum_Comments_Url(comments.Schoolid, comments.messageid));
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = @"application/json; charset=utf-8";
            var buffer = Encoding.UTF8.GetBytes(json);
            httpWebRequest.ContentLength = buffer.Length;
            httpWebRequest.GetRequestStream().Write(buffer, 0, buffer.Length);
            var response = httpWebRequest.GetResponse();
            return (new StreamReader(response.GetResponseStream())).ReadToEnd();
        }

        public dynamic GetForumCommentsMethod (int schoolid, int messageid)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Forum_Comments_Url(schoolid, messageid));
            //httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = @"application/json; charset=utf-8";
            byte[] myData;
            using (var webResponse = httpWebRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                myData = ReadFully(responseStream);    // done with the stream now, dispose of it
            }
            string responseString = Encoding.ASCII.GetString(myData);
            dynamic jsonvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
            return jsonvalues;
        }


    }


    //Temp Model, remove in the final code or update models
    public class PostCalendarEvents
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string StandardId { get; set; }
        public string SectionId { get; set; }
        public int? SchoolId { get; set; }
        public string Venue { get; set; }
        public string Attachment { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    #region temp models, delete later or update in models
    //temp models, delete later

    public class ForumComments
    {
        public int messageid { get; set; }
        public int CommentedById { get; set; }
        public string CommentedByName { get; set; }
        public string message { get; set; }
        public bool isActive { get; set; }
        public string Attachment { get; set; }
        public int numberUploaded { get; set; }
        public int Schoolid { get; set; }
        public DateTime CommentedOn { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class GetForumComments
    {
        public string commentedById { get; set; }
        public string commentedByName { get; set; }
        public string commentedOn { get; set; }
        public string forumid { get; set; }
        public bool isActive { get; set; }
        public string message { get; set; }
        public int numberUploaded { get; set; }
        public string updateddate { get; set; }
        public string attachment { get; set; }
    }


    public class GetForumCommentsList
    {
        public int Commentcount { get; set; }
        public List<GetForumComments> Getforumcomments { get; set; }
    }
    #endregion

}
