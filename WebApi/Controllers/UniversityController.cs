using ClassLibrary1Core;
using Core.Models;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}