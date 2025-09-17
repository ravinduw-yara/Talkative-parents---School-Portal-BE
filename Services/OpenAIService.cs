// Services/OpenAIService.cs
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> GenerateQuestionsAsync(string content, int numQuestions)
    {
        var prompt = $"Generate {numQuestions} exam questions from the following syllabus:\n\n{content}";
        var request = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", request);
        var json = await response.Content.ReadAsStringAsync();
        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        return obj.choices[0].message.content;
    }
}
