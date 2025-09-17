using CommonUtility.RequestModels;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
//using Nancy.Json;
using System.Text.Json;
//using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Repository.DBContext;
using System.Net;
using Aspose.Words;
using Microsoft.AspNetCore.Http;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Body = DocumentFormat.OpenXml.Wordprocessing.Body;

namespace TalkativeParentAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class QIGeneratorController : Controller
    {
        private readonly IDataAnalysisServices dataAnalysisServices;
        private readonly IMQuetionPaperService _quetionPaperService;
        private readonly IMStandardsectionmappingService mStandardsectionmapping;
        public QIGeneratorController(
          IDataAnalysisServices _dataAnalysisServices,
           IMQuetionPaperService quetionPaperService,
            IMStandardsectionmappingService _mStandardsectionmapping
            )
        {
            dataAnalysisServices = _dataAnalysisServices;
            _quetionPaperService = quetionPaperService;
            mStandardsectionmapping = _mStandardsectionmapping;
        }


        private const string API_KEY = "e281b9e4dcaf4fac99db2548e00fa77c"; // Replace with your actual API key
        private const string ENDPOINT = "https://yaragpt4es2.openai.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-15-preview"; // Update endpoint if needed
        public class EditImageBody
        {
            public string image { get; set; }
            public string editType { get; set; }
            public string lang { get; set; }
        }
        [HttpPost]
        [Route("EditImage")]
        public async Task<IActionResult> EditImage([FromBody] EditImageBody body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.image) || string.IsNullOrWhiteSpace(body.editType) || string.IsNullOrWhiteSpace(body.lang))
                return BadRequest("Missing image or parameters");
            try
            {
                var systemMessage = $"You are an image editor. Perform '{body.editType}' on the given SVG image and respond in '{body.lang}' language.";
                var payload = new
                {
                    messages = new[]
                    {
        new { role = "system", content = systemMessage },
        new { role = "user", content = body.image }
    },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 8000,
                    stream = false
                };
                string responseImage = "";
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);
                    var res = await httpClient.PostAsync(
                        ENDPOINT,
                        new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                    );
                    if (!res.IsSuccessStatusCode)
                        return StatusCode((int)res.StatusCode, await res.Content.ReadAsStringAsync());
                    var content = await res.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<JsonElement>(content);
                    responseImage = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
                }
                return Ok(new
                {
                    editedImage = responseImage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("ModifyGeneratedQuetions")]
        public async Task<IActionResult> ModifyGeneratedQuetions(IFormFile file, [FromQuery] string editType, [FromQuery] string lang)
        {
            if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(editType) || string.IsNullOrWhiteSpace(lang))
                return BadRequest("Missing file or parameters");
            try
            {
                byte[] docxBytes;
                if (file.FileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
                {
                    using var docStream = file.OpenReadStream();
                    var doc = new Aspose.Words.Document(docStream);
                    using var outStream = new MemoryStream();
                    doc.Save(outStream, Aspose.Words.SaveFormat.Docx);
                    docxBytes = outStream.ToArray();
                }
                else if (file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    docxBytes = ms.ToArray();
                }
                else
                {
                    return BadRequest("Only .doc or .docx files are supported");
                }
                string inputText;
                using var memStream = new MemoryStream(docxBytes);
                using (var wordDoc = WordprocessingDocument.Open(memStream, false))
                {
                    inputText = wordDoc.MainDocumentPart?.Document?.Body?.InnerText ?? "";
                }
                var systemMessage = $"You are a question editor. Perform '{editType}' to the given document content and respond in '{lang}' language.";
                var payload = new
                {
                    messages = new[]
                    {
                new { role = "system", content = systemMessage },
                new { role = "user", content = inputText }
            },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 8000,
                    stream = false
                };
                string responseText = "";
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);
                    var res = await httpClient.PostAsync(
                        ENDPOINT,
                        new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                    );
                    if (!res.IsSuccessStatusCode)
                        return StatusCode((int)res.StatusCode, await res.Content.ReadAsStringAsync());
                    var content = await res.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<JsonElement>(content);
                    responseText = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
                }
                using var outputStream = new MemoryStream();
                using (var resultDoc = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document, true))
                {
                    var mainPart = resultDoc.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body(new Paragraph(new Run(new Text(responseText)))));
                }
                outputStream.Position = 0;
                var fileBytes = outputStream.ToArray();
                var fileBase64 = Convert.ToBase64String(fileBytes);
                return Ok(new
                {
                    editedText = responseText,
                    fileName = "edited_output.docx",
                    fileBase64 = fileBase64
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Exception: {ex.Message}");
            }
        }
        [HttpGet]
        //sanduni LevelStandardSectionMapping
        [Route("GetSubjectTeacherLevelStandardSectionMapping")]
        public async Task<IActionResult> GetSubjectTeacherLevelStandardSectionMapping(int SchoolId, int SubjectTeacherId)
        {
            try
            {
                var temp = await this.mStandardsectionmapping.GetSubjectTeacherEntityByLevel(SchoolId, SubjectTeacherId);
                if (temp == null)
                {
                    return NotFound(new
                    {
                        Data = "MStandardsectionmapping doesn't exists with this SchoolID.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(temp);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    Data = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                });
            }

        }

        [HttpPost]
        [Route("GenerateQuestions")]
        public async Task<IActionResult> GenerateQuestions(List<Prompt> prompt, int schoolid, int academicYearId, int gradeId , int subjectId, int quetioncount,int Lang)
        {
            var results = new List<string>();

            // A list to store any errors encountered
            var errors = new List<string>();
            try
            {
                List<Prompt> promptsresultlist = new List<Prompt>();
                promptsresultlist = (List<Prompt>)await dataAnalysisServices.GetQIGeneratedQuestions(prompt, schoolid, Lang, quetioncount, subjectId);
                string questionPapers = await _quetionPaperService.QIGetQuestionPapers(academicYearId, gradeId, subjectId);

                // A list to store the results of all processed prompts

                // Example question

                foreach (var item in promptsresultlist)
                {
                    string QUESTION = item.PromptContent;
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            // Add API key to headers
                            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);

                            // Construct payload
                            var payload = new
                            {
                                messages = new[]
                                {
                        new { role = "system", content = QUESTION },
                        new { role = "user", content = questionPapers }
                    },
                                temperature = 0.7,
                                top_p = 0.95,
                                max_tokens = 8000,
                                stream = false
                            };

                            //// Send POST request
                            //var response = await httpClient.PostAsync(
                            //    ENDPOINT,
                            //    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                            //);

                            // Send POST request
                            var response = await httpClient.PostAsync(
                                ENDPOINT,
                                new StringContent(new Nancy.Json.JavaScriptSerializer().Serialize(payload), Encoding.UTF8, "application/json")
                            );

                            // Handle response
                            if (response.IsSuccessStatusCode)
                            {
                                // Read and parse the response
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var jsonResponse = new Nancy.Json.JavaScriptSerializer().Deserialize<dynamic>(responseContent);
                                // var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                var content = jsonResponse?.choices?[0]?.message?.content?.ToString();

                                // Add the content to the results list
                                results.Add(content ?? "No content found.");
                            }
                            else
                            {
                                // Log the error details for the current prompt
                                var errorDetails = await response.Content.ReadAsStringAsync();
                                errors.Add($"Error for QUESTION: {QUESTION}. Status Code: {response.StatusCode}, Details: {errorDetails}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Catch any unexpected exceptions for the current iteration
                            errors.Add($"Exception for QUESTION: {QUESTION}. Details: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions outside the loop
                errors.Add($"General Exception: {ex.Message}");
            }

            // Combine results and errors into a response object
            var responsePayload = new
            {
                Results = results, // Successful results
                Errors = errors    // Errors encountered
            };

            // Return the combined response
            return Ok(responsePayload);
        }


        [HttpPost]
        [Route("GenerateSyllabus")]
        public async Task<IActionResult> GenerateSyllabus(string prompt, int schoolid, int academicYearId, int gradeId, int subjectId, int Lang)
        {
            var results = new List<string>();

            // A list to store any errors encountered
            var errors = new List<string>();
            try
            {
                List<Prompt> promptsresultlist = new List<Prompt>();
                promptsresultlist = (List<Prompt>)await dataAnalysisServices.GetQIGeneratedSyllabusPrompt(prompt, schoolid, academicYearId, gradeId, subjectId, Lang);
                string questionPapers = await _quetionPaperService.QIGetSyllabusContent(academicYearId, gradeId, subjectId);

                // A list to store the results of all processed prompts

                // Example question

                foreach (var item in promptsresultlist)
                {
                    string QUESTION = item.PromptContent;
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            // Add API key to headers
                            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);

                            // Construct payload
                            var payload = new
                            {
                                messages = new[]
                                {
                        new { role = "system", content = QUESTION },
                        new { role = "user", content = questionPapers }
                    },
                                temperature = 0.7,
                                top_p = 0.95,
                                max_tokens = 8000,
                                stream = false
                            };

                            //// Send POST request
                            //var response = await httpClient.PostAsync(
                            //    ENDPOINT,
                            //    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                            //);

                            // Send POST request
                            var response = await httpClient.PostAsync(
                                ENDPOINT,
                                new StringContent(new Nancy.Json.JavaScriptSerializer().Serialize(payload), Encoding.UTF8, "application/json")
                            );

                            // Handle response
                            if (response.IsSuccessStatusCode)
                            {
                                // Read and parse the response
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var jsonResponse = new Nancy.Json.JavaScriptSerializer().Deserialize<dynamic>(responseContent);
                                // var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                var content = jsonResponse?.choices?[0]?.message?.content?.ToString();

                                // Add the content to the results list
                                results.Add(content ?? "No content found.");
                            }
                            else
                            {
                                // Log the error details for the current prompt
                                var errorDetails = await response.Content.ReadAsStringAsync();
                                errors.Add($"Error for QUESTION: {QUESTION}. Status Code: {response.StatusCode}, Details: {errorDetails}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Catch any unexpected exceptions for the current iteration
                            errors.Add($"Exception for QUESTION: {QUESTION}. Details: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions outside the loop
                errors.Add($"General Exception: {ex.Message}");
            }

            // Combine results and errors into a response object
            var responsePayload = new
            {
                Results = results, // Successful results
                Errors = errors    // Errors encountered
            };

            // Return the combined response
            return Ok(responsePayload);
        }


        [HttpPost]
        [Route("EditDiagram")]
        public async Task<IActionResult> EditDiagram(string special_query, string avgimage, string quetioncontent, int schoolid, int quetioncount, int subjectId, int Lang, List<QueList> quelist)
        {
            var results = new List<string>();
            // A list to store any errors encountered
            var errors = new List<string>();
            try
            {
                List<Prompt> promptsresultlist = new List<Prompt>();
                List<Prompt> prompt = null;
                foreach (var prompts in prompt)
                {
                    prompts.PromptTypeName = "ImageEdit";

                }
                string combinedContent = $"{avgimage} {quetioncontent}"; // Adjust the format as needed

                promptsresultlist = (List<Prompt>)await dataAnalysisServices.QIEditDiagram(special_query, avgimage, quetioncontent, subjectId, Lang, quelist);
                //  promptsresultlist = (List<Prompt>)await dataAnalysisServices.QIEditDiagram(special_query, avgimage, quetioncontent, subjectId, Lang, quelist);
                // string questionPapers = await _quetionPaperService.QIGetQuestionPapers(academicYearId, gradeId, subjectId);

                // A list to store the results of all processed prompts

                // Example question

                foreach (var item in promptsresultlist)
                {
                    string QUESTION = item.PromptContent;
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            // Add API key to headers
                            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);

                            // Construct payload
                            var payload = new
                            {
                                messages = new[]
                                {
                        new { role = "system", content = QUESTION },
                        new { role = "user", content = combinedContent }
                    },
                                temperature = 0.7,
                                top_p = 0.95,
                                max_tokens = 8000,
                                stream = false
                            };

                            //// Send POST request
                            //var response = await httpClient.PostAsync(
                            //    ENDPOINT,
                            //    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                            //);

                            // Send POST request
                            var response = await httpClient.PostAsync(
                                ENDPOINT,
                                new StringContent(new Nancy.Json.JavaScriptSerializer().Serialize(payload), Encoding.UTF8, "application/json")
                            );

                            // Handle response
                            if (response.IsSuccessStatusCode)
                            {
                                // Read and parse the response
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var jsonResponse = new Nancy.Json.JavaScriptSerializer().Deserialize<dynamic>(responseContent);
                                // var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                var content = jsonResponse?.choices?[0]?.message?.content?.ToString();

                                // Add the content to the results list
                                results.Add(content ?? "No content found.");
                            }
                            else
                            {
                                // Log the error details for the current prompt
                                var errorDetails = await response.Content.ReadAsStringAsync();
                                errors.Add($"Error for QUESTION: {QUESTION}. Status Code: {response.StatusCode}, Details: {errorDetails}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Catch any unexpected exceptions for the current iteration
                            errors.Add($"Exception for QUESTION: {QUESTION}. Details: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions outside the loop
                errors.Add($"General Exception: {ex.Message}");
            }

            // Combine results and errors into a response object
            var responsePayload = new
            {
                Results = results, // Successful results
                Errors = errors    // Errors encountered
            };

            // Return the combined response
            return Ok(responsePayload);
        }
        public class ConvertQuesRequest
        {
            public string QuetionContent { get; set; }
            public int SchoolId { get; set; }
            public int SubjectId { get; set; }
            public int Lang { get; set; }
        }
        [HttpPost]
        [Route("ConvertQues")]
        public async Task<IActionResult> ConvertQues([FromBody] ConvertQuesRequest request)
        {
            string quetioncontent = request.QuetionContent;
            int schoolid = request.SchoolId;
            int subjectId = request.SubjectId;
            int Lang = request.Lang;

            var results = new List<string>();
            // A list to store any errors encountered
            var errors = new List<string>();
            try
            {
                List<Prompt> promptsresultlist = new List<Prompt>();

                //string combinedContent = $"{avgimage} {quetioncontent}"; // Adjust the format as needed
                promptsresultlist = (List<Prompt>)await dataAnalysisServices.CovertTotLang(quetioncontent, schoolid, subjectId, Lang);
                //promptsresultlist = (List<Prompt>)await dataAnalysisServices.QIEditDiagram(special_query, avgimage, quetioncontent, subjectId, Lang, quelist);
                //  promptsresultlist = (List<Prompt>)await dataAnalysisServices.QIEditDiagram(special_query, avgimage, quetioncontent, subjectId, Lang, quelist);
                // string questionPapers = await _quetionPaperService.QIGetQuestionPapers(academicYearId, gradeId, subjectId);

                // A list to store the results of all processed prompts

                // Example question

                foreach (var item in promptsresultlist)
                {
                    string QUESTION = item.PromptContent;
                    using (var httpClient = new HttpClient())
                    {
                        try
                        {
                            // Add API key to headers
                            httpClient.DefaultRequestHeaders.Add("api-key", API_KEY);

                            // Construct payload
                            var payload = new
                            {
                                messages = new[]
                                {
                        new { role = "system", content = QUESTION },
                        new { role = "user", content = quetioncontent }
                    },
                                temperature = 0.7,
                                top_p = 0.95,
                                max_tokens = 8000,
                                stream = false
                            };

                            //// Send POST request
                            //var response = await httpClient.PostAsync(
                            //    ENDPOINT,
                            //    new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
                            //);

                            // Send POST request
                            var response = await httpClient.PostAsync(
                                ENDPOINT,
                                new StringContent(new Nancy.Json.JavaScriptSerializer().Serialize(payload), Encoding.UTF8, "application/json")
                            );

                            // Handle response
                            if (response.IsSuccessStatusCode)
                            {
                                // Read and parse the response
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var jsonResponse = new Nancy.Json.JavaScriptSerializer().Deserialize<dynamic>(responseContent);
                                // var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                var content = jsonResponse?.choices?[0]?.message?.content?.ToString();

                                // Add the content to the results list
                                results.Add(content ?? "No content found.");
                            }
                            else
                            {
                                // Log the error details for the current prompt
                                var errorDetails = await response.Content.ReadAsStringAsync();
                                errors.Add($"Error for QUESTION: {QUESTION}. Status Code: {response.StatusCode}, Details: {errorDetails}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Catch any unexpected exceptions for the current iteration
                            errors.Add($"Exception for QUESTION: {QUESTION}. Details: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected exceptions outside the loop
                errors.Add($"General Exception: {ex.Message}");
            }

            // Combine results and errors into a response object
            var responsePayload = new
            {
                Results = results, // Successful results
                Errors = errors    // Errors encountered
            };

            // Return the combined response
            return Ok(responsePayload);
        }

    }
}
