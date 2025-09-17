using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Models
{
    public class AppSettings
    {
        public string JwtKey { get; set; }
        public string TokenLifetime { get; set; }
        public string SmtpHost { get; set; }
        public string NetworkCredentialMail { get; set; }
        public string NetworkCredentialmailPassword { get; set; }
        public string port { get; set; }
    }
}
