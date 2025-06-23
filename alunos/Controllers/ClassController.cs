
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication3.context;
using WebApplication3.Model;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassController : ControllerBase
    {

        private readonly ClassDBContext _classDBContext;

        public ClassController(ClassDBContext classDBContext)
        {
            _classDBContext = classDBContext;
        }

        [HttpGet]
        public async Task<ActionResult<StudentClass>> GetAllClasses()
        {
            var classes = await _classDBContext.classes.ToListAsync();
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

        [HttpGet("{studentName}/has-ended")]
        public async Task<ActionResult<StudentClass>> HasEnded(string studentName)
        {
            var studentClass = await _classDBContext.classes.FindAsync(studentName);
            if (studentClass == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhuma aula para esse aluno" });

            }

            string status = studentClass.hasEnded() ? "Finalizada" : "Em andamento";
            return Ok(new { Status = status });
        }

        [HttpGet("{studentName}/remaining-time")]
        public async Task<ActionResult<StudentClass>> RemainingTime(string studentName)
        {
            var studentClass = await _classDBContext.classes.FindAsync(studentName);
            if (studentClass == null)
            {
                return NotFound(new { error = "Não foi possível encontrar nenhuma aula para esse aluno" });

            }

            return Ok(new { Duration = studentClass.GetRemainingTimeFormatted() });
        }

        [HttpPost]
        public async Task<ActionResult<StudentClass>> createClass(CreateStudentClassDTO createStudentClassDTO)
        {
            StudentClass studentClass = new StudentClass(createStudentClassDTO.studentName);
            Boolean hasWithSameName = _classDBContext.classes.Any(otherClass => otherClass.studentName == studentClass.studentName);
            if(hasWithSameName)
            {
                return BadRequest(new { error = "Já existe uma aula para esse aluno" });
            }

            _classDBContext.classes.Add(studentClass);
            await _classDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { studentName = studentClass.studentName }, studentClass);
        }
    }
}
