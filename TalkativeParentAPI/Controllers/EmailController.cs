using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using SendGrid;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Net.Mail;

namespace TalkativeParentAPI.Controllers
{

    //Controller only used for testing

    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        #region Temp data. Actual data has to be pulled from the DB

        private static string username = "azure_ec5cdea122590d2196cd01694b3e0746@azure.com";
        private static string password = "g6tQM3Tz8thkmOr";
        private static string schoolName = "KVM";
        private static string to = "";
        private static string subject = "Demo Mail Mark II";
        private static string Message = "Howdy my fellow person. How are you doing? All is well im hoping.Regards,Also Person.";
        private static string firstname = "Ranjan";
        private static string lastname = "Nadig";
        private static string Attachments = null;
        private static string template = "818d817a-69d9-46a2-a4a2-e379a0b228c1";
        private static string template2 = "59f8219a-4c48-4ec9-9f00-5439742d082d";
        private static string template3 = "c669728e-8164-4ffc-99c3-08bf6e34d6e7";
        static int SentEmailCounter = 0;

        //apikey = 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;
        public class AppUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        private readonly List<AppUser> users = new List<AppUser>()
        {
            new AppUser()
            {
                FirstName = "",
                LastName = "",
                Email = ""
            },
        };
        #endregion


        private readonly IEmailService emailService;
        public EmailController(IEmailService _emailService)
        {
            emailService = _emailService;
        }


        [Route("SendGridEmail")]
        [HttpPost]
        public void SendGridEmail(string[] args)
        {
            try
            {
                foreach (AppUser p in users)
                {
                    emailService.SendMailWithTemplateFromTalkative(p.FirstName, p.LastName, p.Email, subject, template, Message);
                }

                //emailService.SendMailFromTalkative(firstname, lastname, to, subject, template, Message);

                //emailService.SendgridExecute().Wait();

                //emailService.SendMailWithTemplateFromTalkative(firstname, lastname, to, subject, template3, Message);

                //emailService.SendrackspaceEmail(subject, Message, to, "filename", "QnJvaGl0");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [Route("bulkchecksendgrid")]
        [HttpPost]

        public string bulkchecksendgrid()
        {
            var mailingList = new List<string>()
            {
                "nad@gmail.com",
                "nadgmail.com",
            };

            int chunkSize = 80;

            var mailingChunks = SplitMany(mailingList, chunkSize);
            foreach (var item in mailingChunks)
            {

                IEnumerable<Task> sendMailingChunks = mailingChunks.Select(mailingChunk => Task.Run(() => emailService.SendMailSendgrid("Brohit", "Sharma", item, "sub mail", template, "yolo")));


                // Create a Task that will complete when emails were sent to all the "mailingList".
                Task sendBuilkEmails = Task.WhenAll(sendMailingChunks);

                // Displaying the progress of bulk email sending.
                if (!sendBuilkEmails.IsCompleted)
                {
                    Console.WriteLine($"{SentEmailCounter,5} emails have been sent!");
                    Task.Delay(1000).Wait();
                }
                else {
                    return ("Failed");
                }
            }
            return ("Done");
        }



        static List<List<string>> SplitMany(List<string> source, int size)
        {
            var sourceChunks = new List<List<string>>();

            for (int i = 0; i < source.Count; i += size)
                sourceChunks.Add(source.GetRange(i, Math.Min(size, source.Count - i)));

            return sourceChunks;
        }

        [HttpPost]
        [Route("Sendgridsmtptest")]
        public IActionResult Sendgridsmtptest(Sendgridtest sg)
        {
            try
            {
                var sgu = emailService.SendgridEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64, sg.mask);

                if (sgu)
                {
                    return Ok("Sent");
                }
                return BadRequest("Failed");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [Route("RackspaceEmail")]
        [HttpPost]
        public IActionResult RackspaceEmail(Sendgridtest sg)
        {
            try
            {
                var sgu = emailService.SendrackspaceEmail(sg.Subject, sg.Message, sg.To, sg.Filename, sg.Base64);

                if (sgu)
                {
                    return Ok("Sent");
                }
                return BadRequest("Failed");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        public class Sendgridtest
        {
            public string Subject { get; set; }
            public string Message { get; set; }
            public string To { get; set; }
            public string Filename { get; set; }
            public string Base64 { get; set; }
            public string mask { get; set; }

        }

    }
}
