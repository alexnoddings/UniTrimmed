using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.Excel;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolRepository _schoolRepository;
        private readonly IExcelDataService _excelDataService;

        public SchoolsController(ISchoolRepository schoolRepository, IExcelDataService excelDataService)
        {
            _schoolRepository = schoolRepository;
            _excelDataService = excelDataService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<School>> GetSchoolByIdAsync(int id)
        {
            return Ok(await _schoolRepository.GetSchoolAsync(id));
        }

        [HttpGet("radius")]
        public async Task<ActionResult<List<School>>> GetSchoolsInRadiusAsync(double radiusKm, double latitude,
            double longitude)
        {
            if (radiusKm <= 0)
                return BadRequest("Distance value cannot be less than or equal to 0");

            return Ok(await _schoolRepository.GetSchoolsInRadiusAsync(latitude, longitude, radiusKm));
        }

        [HttpPost("excel")]
        public async Task<ActionResult> PostExcelSchoolsFileAsync(IFormFile file)
        {
            if (file == null)
                return BadRequest("No file");

            IEnumerable<School> schools;
            using (Stream s = file.OpenReadStream())
            {
                schools = _excelDataService.GetSchoolsFromWorkbook(s);
            }

            await _schoolRepository.SelectiveUpdateSchoolsAsync(schools);
            return Ok();
        }
    }
}