using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using SendGridMessage = SendGrid.Helpers.Mail.SendGridMessage;
using System.Net.Mail;
using System.IO;
using System.Threading;

namespace Services
{
    public interface IEmailService
    {
        void SendMailWithTemplateFromTalkative(string firstname, string lastname, string to, string subject, string template, string message);
        void SendMailFromTalkative(string firstname, string lastname, string to, string subject, string template, string message);

        bool SendrackspaceEmail(string subject, string message, string to_address, string filename, string base64);

        Task SendgridExecute();

        bool SendgridEmail(string subject, string message, string to_address, string filename, string base64, string mask);

        public void SendMailSendgrid(string firstname, string lastname, IEnumerable<string> too, string subject, string template, string message); 

        bool FeedbackSendgridEmail(string subject, string message, string to_address);
    }

    public class EmailService : IEmailService
    {
        private static string username = "azure_ec5cdea122590d2196cd01694b3e0746@azure.com";
        private static string password = "g6tQM3Tz8thkmOr";
         private static string fromEmail = "apiaccess@talkativeparents.com"; 
        //private static string toEmail = 'no-reply@talkativeparents.com'
        private static string toEmail = "support@yaratechnologies.com";
        //private static string toEmail = "support@gettalkative.zohosupport.com";
        private static string fromName = "Talkative Parents";
        static int SentEmailCounter = 0;

        public async Task SendgridExecute()
        {
            //var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient("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");
            var from = new EmailAddress("apiaccess@talkativeparents.com", "TP");
            var subject = "Sending with SendGrid through .net";
            //insert email here
            var to = new EmailAddress("", "");
            var plainTextContent = "This mail is being sent through SendGrid";
            var htmlContent = "<strong>Sent through SendGrid</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        public void SendMailWithTemplateFromTalkative(string firstname, string lastname, string to, string subject, string template, string message)
        {
            var client = new SendGridClient("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");

            SendGridMessage email = new SendGridMessage();
            email.HtmlContent = " ";
            email.PlainTextContent = " ";
            email.AddTo(to);
            email.From = new EmailAddress(fromEmail, fromName); // new System.Net.Mail.MailAddress(fromEmail, fromName);
            email.Subject = subject;
            if (!string.IsNullOrEmpty(template))
            {
                email.SetTemplateId(template);
                email.AddSubstitution("-fname-", firstname);
                email.AddSubstitution("-lname-", lastname);
                email.AddSubstitution("-subject-", subject);
                email.AddSubstitution("-message-", message);
                //email.AddSubstitution("-school-", school);
            }

            client.SendEmailAsync(email).Wait();
            //await HitSend(email);
        }

        public void SendMailSendgrid(string firstname, string lastname, IEnumerable<string> too, string subject, string template, string message)
        {
            var client = new SendGridClient("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");

            SendGridMessage email = new SendGridMessage();
            email.HtmlContent = " ";
            email.PlainTextContent = " ";
            
            email.From = new EmailAddress(fromEmail, fromName); 
            email.Subject = subject;

            foreach (var item in too)
            {
                var to = new EmailAddress(item, firstname);
                var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                var from = new EmailAddress(fromEmail, fromName);

                SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                //var response = await client.SendEmailAsync(msg).Wait();

                client.SendEmailAsync(msg).Wait();
            }
        }


        public void SendMailFromTalkative(string firstname, string lastname, string to, string subject, string template, string message)
        {
            var client = new SendGridClient("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");

            SendGridMessage email = new SendGridMessage();
            email.HtmlContent = " ";
            email.PlainTextContent = " ";
            email.AddTo(to);
            email.From = new EmailAddress(fromEmail, fromName); ;// new System.Net.Mail.MailAddress(fromEmail, fromName);
            email.Subject = subject;
            if (!string.IsNullOrEmpty(template))
            {
                // email.EnableTemplateEngine(template);
                email.AddSubstitution("-fname-", firstname);
                email.AddSubstitution("-lname-", lastname);
                email.AddSubstitution("-subject-", subject);
                email.AddSubstitution("-message-", message);
                //email.AddSubstitution("-school-", school);

                client.SendEmailAsync(email).Wait();
            }
        }

        public static async Task SendMailToTakativeSupportAsync(string from, string subject, string content)
        {
            try
            {
           var apiKey = "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";
                    //Environment.GetEnvironmentVariable("Talkative_Feedback");
            var client = new SendGridClient(apiKey);
            var fromemail = new EmailAddress("no-reply@talkativeparents.com", from);
                var subjectmail = subject;
                var to = new EmailAddress(toEmail, "Talkative Parents");
                var plainTextContent = content;
                var htmlContent = content;
                var msg = MailHelper.CreateSingleEmail(fromemail, to, subjectmail, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
           }
            catch (Exception e)
            {
                throw e;
            }
}
        public static void SendMailToTakativeSupport(string from, string subject, string content)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //var client = new SendGridClient("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");

               // var client = new SendGridClient("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");
                var client = new SendGridClient("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");
               // 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 newly created api id - 24/7/2023

                var fromemail = new EmailAddress(from, from);
                var subjectmail = subject;
                var toEmail = "support@yaratechnologies.com";
                var to = new EmailAddress(toEmail, "Talkative Parents");
                var plainTextContent = content;
                var htmlContent = content;
                var msg = MailHelper.CreateSingleEmail(fromemail, to, subject, plainTextContent, htmlContent);

               // client.SendEmailAsync(msg).Wait();
                var response =  client.SendEmailAsync(msg).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public bool SendrackspaceEmail(string subject, string message, string to_address, string filename, string base64)
        {
            //string username = System.Configuration.ConfigurationManager.AppSettings["RackSpaceLogin"];
            //string password = System.Configuration.ConfigurationManager.AppSettings["RackSpacePwd"];
            string username = "no-reply@talkativeparents.com";
            string password = "7'Y9Dq&F}L";
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(username);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    //string BCCids = to_address;
                    mail.Bcc.Add(to_address);

                    //mail.To.Add(to_address);
                    if (filename != null && base64 != null)
                    {
                        if (filename.Trim().Length > 1 && base64.Trim().Length > 1)
                        {
                            var bytes = Convert.FromBase64String(base64);
                            MemoryStream strm = new MemoryStream(bytes);
                            System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(strm, filename);
                            mail.Attachments.Add(data);
                        }
                    }
                    using (SmtpClient smtp = new SmtpClient("secure.emailsrvr.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(username, password);
                        smtp.Host = "secure.emailsrvr.com";
                        smtp.Port = 587;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }

                }
                return true;

            }
            catch (Exception e) { return false; }
        }


        //New SendGrid Credentials
        public bool SendgridEmail(string subject, string message, string to_address, string filename, string base64, string mask)
        {
            var toEmail = "no-reply@talkativeparents.com";
            string fromEmail = toEmail;
            string username = "apikey"; 
                string password = "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";
            //string password = "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";
            //var client = new SendGridClient("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");
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.Subject = mask + " - " + subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    //string BCCids = to_address;
                    mail.Bcc.Add(to_address);

                    //mail.To.Add(to_address);
                    if (filename != null && base64 != null)
                    {
                        if (filename.Trim().Length > 1 && base64.Trim().Length > 1)
                        {
                            var bytes = Convert.FromBase64String(base64);
                            MemoryStream strm = new MemoryStream(bytes);
                            System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(strm, filename);
                            mail.Attachments.Add(data);
                        }
                    }
                    using (SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(username, password);
                        smtp.Host = "smtp.sendgrid.net";
                        smtp.Port = 587;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }

                }
                return true;

            }
            catch (Exception e) { return false; }
        }

        //24/7/2023 Feedback
        public bool FeedbackSendgridEmail(string subject, string message, string from)
        {
            string fromEmail = from; 
            string username = "apikey";
            string password = "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";
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    string to_address = "support@yaratechnologies.com";
                    mail.Bcc.Add(to_address);

                   
                    using (SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(username, password);
                        smtp.Host = "smtp.sendgrid.net";
                        smtp.Port = 587;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }

                }
                return true;

            }
            catch (Exception e) { return false; }
        }
    }
}
