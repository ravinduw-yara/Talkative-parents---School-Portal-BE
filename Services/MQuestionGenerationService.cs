//using System;
//using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Microsoft.EntityFrameworkCore;
//using System.Net.Http;
//using Google.Apis.Http;

//namespace Services
//{
//    public interface IQuestionGenerationService
//    {
//        Task<string> GenerateQuestionsFromPastPapers(
//            bool mcq,
//            bool context,
//            string subject,
//            string aggQuestionList,
//            string userSpecialQuery,
//            int noQuestions,
//            int gradeId,
//            int subjectId,
//            int academicYearFromId,
//            int academicYearToId,
//            bool imageGeneration);
//    }

//    public class QuestionGenerationService : IQuestionGenerationService
//    {
//        private readonly HttpClient httpClient;
//        private readonly string azureOpenAIKey;
//        private readonly string azureOpenAIEndpoint;
//        private readonly string deploymentName;
//        private readonly IConfiguration configuration;

//        public QuestionGenerationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
//        {
//            this.configuration = configuration;
//            azureOpenAIKey = configuration["AzureOpenAI:Key"];
//            azureOpenAIEndpoint = configuration["AzureOpenAI:Endpoint"];
//            deploymentName = configuration["AzureOpenAI:DeploymentName"];

//            httpClient = httpClientFactory.CreateClient("AzureOpenAI");
//            httpClient.BaseAddress = new Uri(azureOpenAIEndpoint);
//            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {azureOpenAIKey}");
//        }

//        public async Task<string> GenerateQuestionsFromPastPapers(
//            bool mcq,
//            bool context,
//            string subject,
//            string aggQuestionList,
//            string userSpecialQuery,
//            int noQuestions,
//            int gradeId,
//            int subjectId,
//            int academicYearFromId,
//            int academicYearToId,
//            bool imageGeneration)
//        {
//            string pastQuestions = ProcessQuestionList(aggQuestionList);

//            // Initialize empty outputs
//            List<object> mcqOutput = new();
//            List<object> contextQuesOutput = new();

//            // Fetch the prompt formats
//            var promptFormats = await GetPrompts(academicYearFromId, academicYearToId, gradeId, subjectId);
//            string mcqPrompt = promptFormats.mcq;
//            string contextPrompt = promptFormats.context;

//            // Generate MCQ questions if needed
//            if (mcq)
//            {
//                string mcqQues = string.Format(mcqPrompt, pastQuestions, userSpecialQuery, noQuestions, subject);
//                mcqOutput = await GenerateGPTOutput(mcqQues);
//            }

//            // Generate context questions if needed
//            if (context)
//            {
//                string contextQues = string.Format(contextPrompt, pastQuestions, userSpecialQuery, noQuestions, subject);
//                contextQuesOutput = await GenerateGPTOutput(contextQues);
//            }

//            var outputJson = JsonSerializer.Serialize(new
//            {
//                context_ques = contextQuesOutput,
//                mcq_ques = mcqOutput,
//                image = imageGeneration
//            });

//            return outputJson;
//        }

//        private async Task<(string mcq, string context)> GetPrompts(int academicYearFromId, int academicYearToId, int gradeId, int subjectId)
//        {
//            string url = configuration["PromptsApi:Url"];

//            var response = await httpClient.GetAsync(url + "?" + new FormUrlEncodedContent(new Dictionary<string, string>
//            {
//                { "academicYearFromId", academicYearFromId.ToString() },
//                { "academicYearToId", academicYearToId.ToString() },
//                { "gradeId", gradeId.ToString() },
//                { "subjectId", subjectId.ToString() }
//            }));

//            if (response.IsSuccessStatusCode)
//            {
//                string responseData = await response.Content.ReadAsStringAsync();
//                var jsonData = JsonSerializer.Deserialize<PromptResponse>(responseData);

//                return (
//                    jsonData.Values[0].Prompttype1Format,
//                    jsonData.Values[0].Prompttype2Format
//                );
//            }
//            else
//            {
//                throw new Exception($"Error calling prompts API: {response.StatusCode} {response.ReasonPhrase}");
//            }
//        }

//        private string ProcessQuestionList(string aggQuestionList)
//        {
//            var formattedOutput = new StringBuilder();
//            var questionData = JsonSerializer.Deserialize<List<QuestionData>>(aggQuestionList);

//            foreach (var item in questionData)
//            {
//                formattedOutput.AppendLine($"Past Paper: {item.SubjectType} - {item.Grade}:\n----------");
//                foreach (var question in item.QuestionList)
//                {
//                    formattedOutput.AppendLine(question.Content);
//                }
//                formattedOutput.AppendLine("---------\n");
//            }
//            return formattedOutput.ToString();
//        }

//        private async Task<List<object>> GenerateGPTOutput(string prompt)
//        {
//            var requestBody = new
//            {
//                prompt,
//                max_tokens = 1000,
//                temperature = 0.7,
//                top_p = 0.95,
//                n = 1
//            };

//            var response = await httpClient.PostAsync(
//                $"openai/deployments/{deploymentName}/completions?api-version=2024-05-01-preview",
//                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
//            );

//            if (response.IsSuccessStatusCode)
//            {
//                string responseData = await response.Content.ReadAsStringAsync();
//                var gptResponse = JsonSerializer.Deserialize<GPTResponse>(responseData);
//                return gptResponse.Choices;
//            }
//            else
//            {
//                throw new Exception($"Error generating GPT output: {response.StatusCode} {response.ReasonPhrase}");
//            }
//        }
//    }

//    // Supporting Classes
//    public class PromptResponse
//    {
//        public List<PromptValue> Values { get; set; }
//    }

//    public class PromptValue
//    {
//        public string Prompttype1Format { get; set; }
//        public string Prompttype2Format { get; set; }
//    }

//    public class QuestionData
//    {
//        public string SubjectType { get; set; }
//        public string Grade { get; set; }
//        public List<Question> QuestionList { get; set; }
//    }

//    public class Question
//    {
//        public string Content { get; set; }
//    }

//    public class GPTResponse
//    {
//        public List<object> Choices { get; set; }
//    }
//}
