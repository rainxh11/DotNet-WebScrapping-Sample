using Microsoft.AspNetCore.Mvc;
using ProgresMESRS.Middleware.API.Models;
using ProgresMESRS.WebApi.Service;

namespace ProgresMESRS.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private IStudentService _studentService;

        public StudentController(ILogger<StudentController> logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }

        [HttpGet]
        [Route("byMatricule/{matricule}")]
        public async Task<IActionResult> GetStudentByMatricule(string matricule)
        {
            try
            {
                var student = await _studentService.ExtractStudent(matricule, "", false);
                if (student == null)
                    return NotFound();
                return Ok(student);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}