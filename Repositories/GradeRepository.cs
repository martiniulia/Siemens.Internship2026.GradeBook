using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly HttpClient _httpClient;
    private const string Url = "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/145b121103dd1cee3737a681c487f7295ac82e6b/gistfile1.txt";

    public GradeRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        var grades = await GetAllAsync();
        return grades.FirstOrDefault(g => g.Id == id);
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<GistResponse>(Url);
        if (response?.Items == null)
        {
            return Enumerable.Empty<Grade>();
        }

        return response.Items.Select(i => new Grade
        {
            Id = i.Id,
            Value = i.Value,
            IsActive = i.IsActive
        });
    }

    public Task AddAsync(Grade grade) => throw new NotSupportedException("External repository is read-only.");
    public Task UpdateAsync(Grade grade) => throw new NotSupportedException("External repository is read-only.");
    public Task DeleteAsync(int id) => throw new NotSupportedException("External repository is read-only.");

    private class GistResponse
    {
        [JsonPropertyName("items")]
        public List<GistItem> Items { get; set; } = new();
    }

    private class GistItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
