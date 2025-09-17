using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UglyToad.PdfPig;
namespace TalkativeParentAPI.Controllers
{
    // Controllers/SyllabusController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class SyllabusController : ControllerBase
    {
        private readonly QdrantService _qdrantService;
        private readonly OpenAIService _openAIService;

        private readonly string _openAiApiKey = "e281b9e4dcaf4fac99db2548e00fa77c";
        private readonly string _qdrantEndpoint = "https://b040cbe1-943a-4985-bcb9-71ff9796f967.us-east4-0.gcp.cloud.qdrant.io"; // Change if needed
        private readonly string _qdrantApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3MiOiJtIn0.GrRfU4qqCDL8lXOX7Sy2zvy3skXdKwshoWYPBGVai_g"; // Add this

        public SyllabusController()
        {
            _qdrantService = new QdrantService(_qdrantEndpoint, _qdrantApiKey);
            _openAIService = new OpenAIService(_openAiApiKey);
        }



        [HttpPost("UploadSyllabus")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSyllabus(IFormFile file, [FromForm] string id, [FromForm] string title)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Step 1: Extract text (PDF parsing, using UglyToad.PdfPig)
            string content = "";
            using (var stream = file.OpenReadStream())
            using (var pdf = UglyToad.PdfPig.PdfDocument.Open(stream))
            {
                foreach (var page in pdf.GetPages())
                {
                    content += page.Text;
                }
            }

            // Step 2: Chunk the text
            var chunks = ChunkText(content, 2000); // ~2000 words per chunk

            // Step 3: Embed and send to Qdrant
            string openAiApiKey = _openAiApiKey;
            string qdrantEndpoint = _qdrantEndpoint; // or your cloud endpoint

            int chunkNum = 0;
            foreach (var chunk in chunks)
            {
                // Get embedding for this chunk
                var embedding = await _qdrantService.GetEmbeddingAsync(chunk, openAiApiKey);

                // Unique ID for this chunk (syllabus id + chunk number)
                var chunkId = $"{id}_chunk_{chunkNum}";
                await _qdrantService.AddChunkToQdrantAsync(chunkId, chunk, embedding, qdrantEndpoint);

                chunkNum++;
            }

            return Ok(new { message = $"Syllabus '{title}' uploaded and chunked into {chunkNum} pieces!" });
        }
        public static IEnumerable<string> ChunkText(string text, int maxWords = 2000)
        {
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i += maxWords)
            {
                yield return string.Join(" ", words.Skip(i).Take(maxWords));
            }
        }
        [HttpPost("SearchSyllabus")]
        public async Task<IActionResult> SearchSyllabus([FromBody] string query)
        {
            var embedding = await _qdrantService.GetEmbeddingAsync(query, _openAiApiKey);
            var searchResults = await _qdrantService.SearchQdrantAsync(query, embedding);

            return Ok(searchResults);
        }

        [HttpPost("GenerateQuestions")]
        public async Task<IActionResult> GenerateQuestions([FromBody] QuestionRequest request)
        {
            // Retrieve document content by ID (from MongoDB or elsewhere)
            string content = "The full syllabus text for this ID"; // Get from DB

            var questions = await _openAIService.GenerateQuestionsAsync(content, request.NumberOfQuestions);
            return Ok(questions);
        }
    }
    // Models/QuestionRequest.cs
    public class QuestionRequest
    {
        public string SyllabusId { get; set; }
        public int NumberOfQuestions { get; set; }
    }


}
