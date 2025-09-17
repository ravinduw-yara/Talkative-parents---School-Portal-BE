using CommonUtility;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository;
using Repository.DBContext;
using Services.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

//Confirm
namespace Services
{
    //public interface ISMSService : ICommonService
    //{
    //    Task<int> AddEntity(MFeature entity);
    //    Task<MFeature> GetEntityIDForUpdate(int entityID);
    //    Task<int> UpdateEntity(MFeature entity);

    //}
    public class MSMSService //: ISMSService
    {
        private static string _accessToken;
        private static readonly string _userName;
        private static readonly string _password;
        private static readonly int numberCountLimit = 250; //number of SMSs to be sent in one API call.
        private static readonly int channelOnelimit = 160; //character limit for channel 1.
        private static readonly int channelNinelimit = 320; //character limit for channel 9.
        //private static readonly TPDBEntities db = new TPDBEntities();

        static MSMSService()
        {
            //_userName = "Yara_api"; //OLD ONE
            //_password = "yara@123";//OLDONE

            //_userName = "Yara"; //latest login pwd
            //_password = "Yara@789";

            _userName = "Yara_api"; //After Ada Migration
            _password = "Yaratech@123";

            GetAuthorizationCode();
        }

        public static bool SendSMS(string[] numbers, string message, string mask)
        {
            //GetAuthorizationCode();

            int phaseCount = 0;
            bool status = true;

            numbers = SanitizeNumbers(numbers);

            string channel = GetChannel(message);

            int numCount = numbers.Length;

            if (numCount % numberCountLimit == 0)
            {
                phaseCount = numCount / numberCountLimit;
            }
            else
            {
                phaseCount = (numCount / numberCountLimit) + 1;
            }

            string[][] AllNumber = new string[phaseCount][];
            SMSResponse[] responsefromSMS = new SMSResponse[phaseCount];

            for (int i = 0; i < phaseCount; i++)
            {
                int count = numberCountLimit;
                if ((i + 1) == phaseCount)
                {
                    count = (numCount % numberCountLimit);
                }

                string[] currentPhaseNumbers = new string[count];
                AllNumber[i] = currentPhaseNumbers;

                Array.Copy(numbers, i * numberCountLimit, currentPhaseNumbers, 0, count);

                // Send SMS
                GetAuthorizationCode();
                responsefromSMS[i] = ConnectDialogToSendSMS(currentPhaseNumbers, mask, channel, message);

                // if failed regenerate Authorization code.
                if (responsefromSMS[i].error_code != "0")
                {
                    // SMS from Dialogue received a error. 
                    if (responsefromSMS[i].error_code == "104" || responsefromSMS[i].error_code == "105")
                    {
                        GetAuthorizationCode();
                        responsefromSMS[i] = ConnectDialogToSendSMS(currentPhaseNumbers, mask, channel, message);
                    }
                }

                if (status && responsefromSMS[i].error_code != "0")
                    status = false;
            }


            // Log the event to SQL DB. Make it in Async.
            //WriteLog_SMSCounterAsync(AllNumber, mask, channel, message, responsefromSMS);

            return status;

        }

        //private static bool WriteLog_SMSCounterAsync(string[][] currentPhaseNumbers, string mask, string channel, string message, SMSResponse[] response)
        //{
        //    string currentPhaseNumberStr = "";
        //    var schoolId = GetSchoolID(mask);
        //    int phasecount = currentPhaseNumbers.Length;
        //    for (int i = 0; i < phasecount; i++)
        //    {
        //        currentPhaseNumberStr = String.Join(",", currentPhaseNumbers[i]).Trim();
        //        string[] NumberList = currentPhaseNumberStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //        currentPhaseNumberStr = String.Join(",", NumberList).Trim();
        //        try
        //        {
        //            db.SMSCounters.Add(
        //                new SMSCounter()
        //                {
        //                    SchoolId = new Guid(schoolId),
        //                    Message = message.Trim(),
        //                    Id = Guid.NewGuid(),
        //                    Number = currentPhaseNumberStr,
        //                    DateTimeSent = DateTime.Now.AddMinutes(20),
        //                    AuthCode = _accessToken.Trim(),
        //                    Channel = channel.Trim(),
        //                    ref_id = response[i].ref_id.Trim(),
        //                    error = response[i].error_code.Trim()
        //                });

        //        }
        //        catch (Exception e)
        //        {
        //            // Need to log the event
        //            //throw;
        //            return false;
        //        }
        //    }
        //    db.SaveChangesAsync();
        //    return true;
        //}

        private static string[] SanitizeNumbers(string[] numbers)
        {
            string[] sanitizedNumbers = numbers;
            for (int i = 0; i < numbers.Length; i++)
            {
                sanitizedNumbers[i] = sanitizedNumbers[i].Replace("+", "");
            }
            return sanitizedNumbers;
        }

        private static void GetAuthorizationCode()
        {
            var dataObj = JsonConvert.SerializeObject(new { u_name = _userName, passwd = _password });
            var httpClient = new HttpClient();

            var content = new StringContent(dataObj, Encoding.UTF8, "application/json");
            //var result = httpClient.PostAsync("https://digitalreachapi.dialog.lk/refresh_token.php", content).Result; //old one
           var result = httpClient.PostAsync("https://api.adareach.lk/api/v1/login/api-based", content).Result; //new one
            var response = JsonConvert.DeserializeAnonymousType(result.Content.ReadAsStringAsync().Result, new { access_token = string.Empty });
            _accessToken = response.access_token;
        }

        private static string GetChannel(string message)
        {
            string channel = "";

            if (message.Length < channelNinelimit)
            {
                if (message.Length > 0 && message.Length <= channelOnelimit)
                    channel = "1";
                else if (message.Length > channelOnelimit && message.Length < channelNinelimit)
                    channel = "9";
            }
            return channel;
        }

        public struct SMSResponse
        {
            public string ref_id;
            public string error_code;
        };


        public static async Task<bool> SendSingleSMS(string number, string message, string mask)
        {
            GetAuthorizationCode();
            string channel = "";
            bool status = false;
            var SanitizeNumber = number.Replace("+", "");

            if (message.Length < channelNinelimit)
            {
                if (message.Length > 0 && message.Length <= channelOnelimit)
                    channel = "1";
                else if (message.Length > channelOnelimit && message.Length < channelNinelimit)
                    channel = "9";
            }

            var dataObj = JsonConvert.SerializeObject(new
            {
                msisdn = SanitizeNumber,
                channel,
                mt_port = "GETTALKTIVE",
                // s_time = DateTime.Now.AddHours(5).AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss"),
                // e_time = DateTime.Now.AddHours(6).AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss"), sanduni 30/5/2024
                s_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                e_time = DateTime.Now.AddHours(8).AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss"),
                msg = message,
                callback_url = ""
            });
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", _accessToken);

            var content = new StringContent(dataObj, Encoding.UTF8, "application/json");
        
            var result = httpClient.PostAsync("https://api.adareach.lk/api/v1/sms-campaign/send-sms", content).Result;
             //var result = httpClient.PostAsync("https://digitalreachapi.dialog.lk/camp_req.php", content).Result;
            var response = JsonConvert.DeserializeAnonymousType(result.Content.ReadAsStringAsync().Result, new { error = string.Empty, camp_id = string.Empty, ref_id = string.Empty });

            if (response.error != "0")
            {
                if (response.error == "104" || response.error == "105")
                {
                    GetAuthorizationCode();
                    status = await SendSingleSMS(number, message, mask);
                }

            }
            return status;

        }

        private static SMSResponse ConnectDialogToSendSMS(string[] currentPhaseNumbers, string mask, string channel, string message)
        {
            if (Common.SendSMS == false)
            {
                SMSResponse ret = new SMSResponse();
                ret.error_code = "500";
                return ret;
            }
            //currentPhaseNumbers = DivideNumbers(numbers, numberCountLimit, phaseCount);

            var countNumList = currentPhaseNumbers.Length;
            List<string> numberList = new List<string>();
            for (int i = 0; i < countNumList; i++)
            {
                string currentPhaseNumberStr = String.Join(",", currentPhaseNumbers[i]).Trim();
                string[] NumberList = currentPhaseNumberStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                numberList.AddRange(NumberList);
            }

            var dataObj = JsonConvert.SerializeObject(new
            {
                msisdn = numberList.ToArray(),
                mt_port = mask,
                channel,
                e_time = DateTime.Now.AddHours(5).AddMinutes(50).ToString("yyyy-MM-dd HH:mm:ss"),
                //e_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                msg = message
            });

            SMSResponse resp = new SMSResponse();
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", _accessToken);

            var content = new StringContent(dataObj, Encoding.UTF8, "application/json");
            //var result = httpClient.PostAsync("https://bulk.digitalreach.dialog.lk/bulk_camp_req.php", content).Result;
           // var result = httpClient.PostAsync("https://bulkdigitalreach.dialog.lk/bulk_camp_req.php", content).Result;//Correct old one
            var result = httpClient.PostAsync("https://api.adareach.lk/api/v1/sms-campaign/send-bulk-sms", content).Result; //new one
            var response = JsonConvert.DeserializeAnonymousType(result.Content.ReadAsStringAsync().Result, new { error = string.Empty, ref_id = string.Empty });

            resp.error_code = response.error;
            resp.ref_id = response.ref_id;
            return resp;
        }
        //private readonly IRepository<MFeature> repository;
        //private DbSet<MFeature> localDBSet;

        //public MSMSService(IRepository<MFeature> repository)
        //{
        //    this.repository = repository;
        //}

        //public async Task<int> AddEntity(MFeature entity)
        //{
        //    var temp = await this.repository.Insert(entity);
        //    if (temp)
        //    {
        //        return entity.Id;
        //    }
        //    return 0;
        //}

        //private async Task AllEntityValue() => localDBSet = (DbSet<MFeature>)await this.repository.GetAll();

        //private static Object Mapper(MFeature x) => new
        //{
        //    x.Id,
        //    x.Schoolid,
        //    x.Maxmsgcount,
        //    School = new
        //    {
        //        School = x.School != null ? x.School.Name : string.Empty,
        //        x.Schoolid
        //    },
        //    Status = new
        //    {
        //        Status = x.Status != null ? x.Status.Name : string.Empty,
        //        x.Statusid
        //    }
        //};

        //private async Task<IQueryable<MFeature>> GetAllEntitiesPvt()
        //{
        //    await AllEntityValue();
        //    return this.localDBSet
        //    .Include(x => x.CreatedbyNavigation)
        //    .Include(x => x.ModifiedbyNavigation)
        //    .Include(x => x.Status);
        //}

        //public async Task<IQueryable<object>> GetAllEntities() => (await this.GetAllEntitiesPvt()).Select(x => Mapper(x));

        //public async Task<MFeature> GetEntityIDForUpdate(int entityID) => await Task.Run(() => this.repository.GetAll().Result.SingleOrDefault(x => x.Id.Equals(entityID)));

        //public async Task<object> GetEntityByID(int entityID) => (await this.GetAllEntitiesPvt()).Where(x => x.Id.Equals(entityID)).Select(x => Mapper(x)).SingleOrDefault();

        ////Confirm - Is it required?? No, as Name field is not available
        //public async Task<IQueryable<object>> GetEntityByName(string EntityName) => (await this.GetAllEntitiesPvt()).Where(x => x.Schoolid.Equals(EntityName.Trim())).Select(x => Mapper(x));

        //public async Task<int> UpdateEntity(MFeature entity)
        //{
        //    var temp = await this.repository.Update(entity);
        //    if (temp)
        //    {
        //        return entity.Id;
        //    }
        //    return 0;
        //}


    }
}
