using ClassLibrary1Core;
using Core.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Dto;


namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UniversityController : ControllerBase
{
        private readonly UniversityContext _context;
        
        public UniversityController(UniversityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<University>>> GetUniversities(int pageNumber = 1, int pageSize = 3)
        {
            var universities = _context.Universities.AsQueryable();
        
            var totalCount = await universities.CountAsync();
            var items = await universities.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
            var result = new PagedResult<University>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber
            };
        
            return result;
        }
        
        
        // GET: api/University/ByCountry?country=USA
        // Get universities by country
        [HttpGet("ByCountry")]
        public async Task<ActionResult<IEnumerable<object>>> GetUniversitiesByCountry(string country)
        {
            var universities = _context.Universities
                .Where(u => u.Country.CountryName == country);

            var universityRankingYears = _context.UniversityRankingYears;

            var rankingCriteria = _context.RankingCriteria;

            var result = await universities.Join(
                universityRankingYears,
                u => u.Id,
                ury => ury.UniversityId,
                (u, ury) => new { u, ury }
            ).Join(
                rankingCriteria,
                uAndUry => uAndUry.ury.RankingCriteriaId,
                rc => rc.Id,
                (uAndUry, rc) => new
                {
                    universityId = uAndUry.u.Id,
                    universityName = uAndUry.u.UniversityName,
                    scores = new
                    {
                        year = uAndUry.ury.Year,
                        score = uAndUry.ury.Score,
                        criteriaName = rc.CriteriaName
                    }
                }
            ).ToListAsync();

            return result;
        }
        
        
        
        //zad1
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUniversity(int id, [FromQuery] int year)
        {
            var university = await _context.Universities
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    universityId = u.Id,
                    universityName = u.UniversityName
                })
                .FirstOrDefaultAsync();

            if (university == null)
            {
                return NotFound();
            }

            var universityYear = await _context.UniversityYears
                .Where(u => u.UniversityId == id && u.Year == year)
                .Select(u => new
                {
                    numberOfStudents = u.NumStudents,
                    staffRatio = u.StudentStaffRatio,
                    numberOfInternationalStudents = u.PctInternationalStudents,
                    numberOfFemaleStudents = u.PctFemaleStudents
                })
                .FirstOrDefaultAsync();

            if (universityYear == null)
            {
                return NotFound();
            }

            var result = new
            {
                universityId = university.universityId,
                universityName = university.universityName,
                numberOfStudents = universityYear.numberOfStudents,
                staffRatio = universityYear.staffRatio,
                numberOfInternationalStudents = universityYear.numberOfInternationalStudents,
                numberOfFemaleStudents = universityYear.numberOfFemaleStudents
            };

            return result;
        }
        
        
        //zad2
        [HttpGet("GetUniversityRankingYear/{id}")]
        public async Task<ActionResult<UniversityRankingYear>> GetUniversityRankingYear(int id)
        {
            var universityRankingYear = await _context.UniversityRankingYears.FindAsync(id);

            if (universityRankingYear == null)
            {
                return NotFound();
            }

            return universityRankingYear;
        }        
        
        [HttpPost("{id}/scores")]
        public async Task<IActionResult> AddScore(int id, [FromBody] ScoreInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            var university = await _context.Universities.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }
        
            var existingScore = await _context.UniversityRankingYears
                .FirstOrDefaultAsync(s => s.UniversityId == id && s.Year == input.Year && s.RankingCriteriaId == input.RankingCriteriaId);
            if (existingScore != null)
            {
                return Conflict("Wynik dla tego uniwersytetu, roku i kryterium już istnieje");
            }
        
            var newScore = new UniversityRankingYear
            {
                UniversityId = id,
                Year = input.Year,
                RankingCriteriaId = input.RankingCriteriaId,
                Score = input.Score
            };
            _context.UniversityRankingYears.Add(newScore);
            await _context.SaveChangesAsync();
        
            return Ok(newScore);
        }

        //
        //
        //
        
        
}