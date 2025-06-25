
using alunos.Model;
using alunos.Model.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.context;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentClassController : ControllerBase
    {

        private readonly ClassDBContext _classDBContext;

        public StudentClassController(ClassDBContext classDBContext)
        {
            _classDBContext = classDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<StudentClass>> GetAllClasses()
        {
            var classes = await _classDBContext.classes
                .Select(c => new ViewStudentClassDTO
                {
                    Id = c.Id,
                    StudentName = c.Student.Name,
                    DayOfWeek = c.DayOfWeek,
                    RemainingTimeInSeconds = (c.StartTime + 7200000 - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000
                })
                .ToListAsync();
            return Ok(classes);
        }

        [HttpGet("{studentName}")]
        public async Task<ActionResult<StudentClass>> GetClass(string studentName) {
            var studentClass = await _classDBContext.classes.FindAsync(studentName);
            if(studentClass == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhuma aula para esse aluno"});

            }

            return Ok(studentClass);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StudentClass>> createClass(CreateStudentClassDTO createStudentClassDTO)
        {
            var student = await _classDBContext.Students.FindAsync(createStudentClassDTO.studentId);
            if(student == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhum aluno com esse nome" });
            }

            var studentClass = new StudentClass(student.Id, createStudentClassDTO.dayOfWeek);
            var alreadyHasClass = _classDBContext.classes.Any(c => c.StudentId == createStudentClassDTO.studentId);
            if(alreadyHasClass)
            {
                return BadRequest(new { error = "Já existe uma aula para esse aluno" });
            }

            await _classDBContext.classes.AddAsync(studentClass);
            await _classDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { studentName = studentClass.Student.Name }, studentClass);
        }
    }
}
