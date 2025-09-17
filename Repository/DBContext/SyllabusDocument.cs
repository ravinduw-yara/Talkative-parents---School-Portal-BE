using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DBContext
{
    public class SyllabusDocument
    {
        public string Id { get; set; }          // Unique identifier (e.g., "math101")
        public string Title { get; set; }       // Title of the syllabus (e.g., "Basic Mathematics")
        public string Content { get; set; }     // Full syllabus content (the actual text)
    }
  
}
