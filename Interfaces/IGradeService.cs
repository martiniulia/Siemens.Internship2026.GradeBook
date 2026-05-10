using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<Grade>> GetAllGradesAsync();
    Task<Grade?> GetGradeByIdAsync(int id);
    Task<GradeStatistics> GetStatisticsAsync();
    Task<IEnumerable<Grade>> GetTopPassingGradesAsync(int count);
}
