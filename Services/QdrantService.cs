// Services/QdrantService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class QdrantService
{
    private readonly HttpClient _httpClient;
    private readonly string _qdrantEndpoint = "https://b040cbe1-943a-4985-bcb9-71ff9796f967.us-east4-0.gcp.cloud.qdrant.io"; // Change if needed
    private readonly string _qdrantApiKey= "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3MiOiJtIn0.GrRfU4qqCDL8lXOX7Sy2zvy3skXdKwshoWYPBGVai_g"; // Add this

    public QdrantService(string qdrantEndpoint, string qdrantApiKey)
    {
        _qdrantEndpoint = qdrantEndpoint;
        _qdrantApiKey = qdrantApiKey;
        _httpClient = new HttpClient();

        // Add the API key to default headers if provided
        if (!string.IsNullOrWhiteSpace(_qdrantApiKey))
            _httpClient.DefaultRequestHeaders.Add("api-key", _qdrantApiKey);

    }
   
    public async Task AddChunkToQdrantAsync(string id, string chunkContent, float[] embedding, string qdrantEndpoint)
    {
        var guid = Guid.NewGuid();
        var payload = new
        {
            points = new[]
            {
            new
            {
                id = guid,
                vector = embedding,
                payload = new { custom_id = id, content = chunkContent }
            }
        }
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("api-key", _qdrantApiKey); // <-- Add the api-key header

        var response = await client.PutAsJsonAsync($"{qdrantEndpoint}/collections/syllabus/points?wait=true", payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Qdrant error: {response.StatusCode} {error}");
        }
    }
    // You will need an embedding model, e.g., OpenAI
    public async Task<float[]> GetEmbeddingAsync(string text, string openAiApiKey)
    {
        var openAiEndpoint = "https://yaragpt4es2.openai.azure.com/openai/deployments/yara-text-embedding-ada-002/embeddings?api-version=2023-05-15";
        var payload = new
        {
            input = new[] { text },
            model = "text-embedding-ada-002"
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAiApiKey}");

        var response = await client.PostAsJsonAsync(openAiEndpoint, payload);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"OpenAI returned status {response.StatusCode}: {json}");

        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        if (obj.data == null || obj.data.Count == 0)
        {
            throw new Exception("No embedding data returned from OpenAI.");
        }

        return obj.data[0].embedding.ToObject<float[]>();
    }

    public async Task AddDocumentToQdrantAsync(string id, string content, float[] vector)
    {
        var payload = new
        {
            vectors = new[] { vector },
            points = new[] { new { id = id, payload = new { content } } }
        };
        var response = await _httpClient.PutAsJsonAsync($"{_qdrantEndpoint}/collections/syllabus/points", payload);
    }

    public async Task<string> SearchQdrantAsync(string query, float[] vector)
    {
        var payload = new
        {
            vector = vector,
            top = 3 // get top 3 most similar
        };
        var response = await _httpClient.PostAsJsonAsync($"{_qdrantEndpoint}/collections/syllabus/points/search", payload);
        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}
