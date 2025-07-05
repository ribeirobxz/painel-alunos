
using alunos.Model;
using alunos.Model.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.context;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentClassController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public StudentClassController(ApplicationDBContext classDBContext)
        {
            _applicationDBContext = classDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<StudentClass>> GetAllClasses()
        {
            var classes = await _applicationDBContext.classes
                .Select(c => new ViewStudentClassDTO
                {
                    Id = c.Id,
                    StudentName = c.Student.Name,
                    DayOfWeek = c.DayOfWeek,
                    RemainingTimeInSeconds = (c.StartTime + 7200000 - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000
                })
                .ToListAsync();
            if(classes.IsNullOrEmpty() || !classes.Any())
            {
                return NotFound(new { error = "Nenhuma aula em andamento." });
            }

            return Ok(classes);
        }

        [HttpGet("{studentName}")]
        public async Task<ActionResult<StudentClass>> GetClass(string studentName) {
            var studentClass = await _applicationDBContext.classes.FindAsync(studentName);
            if(studentClass == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhuma aula para esse aluno"});

            }

            return Ok(studentClass);
        }

        [HttpPost]
        public async Task<ActionResult<StudentClass>> CreateClass(CreateStudentClassDTO createStudentClassDTO)
        {
            var student = await _applicationDBContext.Students.FindAsync(createStudentClassDTO.studentId);
            if(student == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhum aluno com esse nome" });
            }

            var studentClass = new StudentClass(student.Id, createStudentClassDTO.dayOfWeek);
            var alreadyHasClass = _applicationDBContext.classes.Any(c => c.StudentId == createStudentClassDTO.studentId);
            if(alreadyHasClass)
            {
                return BadRequest(new { error = "Já existe uma aula para esse aluno" });
            }

            await _applicationDBContext.classes.AddAsync(studentClass);
            await _applicationDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { studentName = studentClass.Student.Name }, studentClass);
        }
    }
}
