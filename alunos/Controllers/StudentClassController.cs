
using alunos.Model;
using alunos.Model.Answer;
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
            try
            {
                if (!_applicationDBContext.classes.Any())
                {
                    return Ok(new Answer("Não existe nenhuma aula em andamento.", 204));
                }

                var classes = await _applicationDBContext.classes
                    .Select(c => new ViewStudentClassDTO
                    {
                        Id = c.Id,
                        StudentName = c.Student.Name,
                        DayOfWeek = c.DayOfWeek,
                        RemainingTimeInSeconds = (c.StartTime + 7200000 - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000
                    })
                    .ToListAsync();

                return Ok(new Answer<List<ViewStudentClassDTO>>("Requisição feita com sucesso", 200, classes));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500);
            }
        }

        [HttpGet("{studentName}")]
        public async Task<ActionResult<StudentClass>> GetClass(string studentName) {

            try
            {
                if (!_applicationDBContext.classes.Any())
                {
                    return Ok(new Answer("Não existe nenhuma aula em andamento.", 204));
                }

                var studentClass = await _applicationDBContext.classes.FindAsync(studentName);
                if (studentClass == null)
                {
                    return Ok(new Answer("Não foi possível encontrar nenhuma aula desse aluno.", 204));

                }

                return Ok(new Answer<StudentClass>("Requisição feita com sucesso", 200, studentClass));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpPost]
        public async Task<ActionResult<StudentClass>> CreateClass(CreateStudentClassDTO createStudentClassDTO)
        {
            try
            {
                if (!_applicationDBContext.Students.Any())
                {
                    return Ok(new Answer("Não existe nenhum estudante cadastrado.", 204));
                }

                var student = await _applicationDBContext.Students.FindAsync(createStudentClassDTO.studentId);
                if (student == null)
                {
                    return Ok(new Answer("Não foi possivel encontrar nenhum aluno com esse id.", 204));
                }

                var studentClass = new StudentClass(student.Id, createStudentClassDTO.dayOfWeek);
                var alreadyHasClass = _applicationDBContext.classes.Any(c => c.StudentId == createStudentClassDTO.studentId);
                if (alreadyHasClass)
                {
                    return Ok(new Answer("Já existe uma aula em andamento para esse aluno.", 204));
                }

                await _applicationDBContext.classes.AddAsync(studentClass);
                await _applicationDBContext.SaveChangesAsync();

                var answer = new Answer<StudentClass>("Requisição feita com sucesso", 200, studentClass);
                return CreatedAtAction(nameof(GetClass), new { studentName = studentClass.Student.Name }, answer);
            } catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }
    }
}
