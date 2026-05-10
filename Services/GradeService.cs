using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _repository;

    public GradeService(IGradeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Grade>> GetAllGradesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Grade?> GetGradeByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<GradeStatistics> GetStatisticsAsync()
    {
        var grades = await _repository.GetAllAsync();
        var gradeList = grades.ToList();

        return new GradeStatistics
        {
            TotalCount = gradeList.Count,
            AverageScore = gradeList.Any() ? gradeList.Average(g => g.Value) : 0,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<Grade>> GetTopPassingGradesAsync(int count)
    {
        var grades = await _repository.GetAllAsync();
        return grades
            .Where(g => g.IsActive && g.Value >= 5)
            .Take(count);
    }
}
