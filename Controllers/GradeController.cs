using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly ILogger<GradeController> _logger;

    public GradeController(IGradeService gradeService, ILogger<GradeController> logger)
    {
        _gradeService = gradeService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET api/grade called");

        var grades = await _gradeService.GetAllGradesAsync();
        var stats = await _gradeService.GetStatisticsAsync();

        return Ok(new
        {
            items = grades,
            Statistics = stats
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET api/grade/{Id} called", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid id: {Id}", id);
            return BadRequest("Id must be a positive integer.");
        }

        var grade = await _gradeService.GetGradeByIdAsync(id);
        if (grade == null)
        {
            _logger.LogWarning("Grade {Id} not found", id);
            return NotFound($"Grade with Id {id} was not found.");
        }

        return Ok(grade);
    }

    [HttpGet("passing/{n}")]
    public async Task<IActionResult> GetPassing(int n)
    {
        _logger.LogInformation("GET api/grade/passing/{N} called", n);

        if (n < 0)
        {
            return BadRequest("N must be a non-negative integer.");
        }

        var grades = await _gradeService.GetTopPassingGradesAsync(n);
        return Ok(grades);
    }
}
