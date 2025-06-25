using alunos.Model;
using alunos.Model.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly ClassDBContext _classDBContext;

        public StudentController(ClassDBContext classDBContext)
        {
            _classDBContext = classDBContext;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Student>> createStudent([FromBody] CreateStudentDTO createStudentDTO)
        {
            var hasWithSameName = await _classDBContext.Students.AnyAsync(student => student.Name == createStudentDTO.name);
            if (hasWithSameName)
            {
                return BadRequest(new { error = "Já existe um estudante registrado com esse nome" });
            }

            var student = new Student(createStudentDTO.name, createStudentDTO.daysOfWeek);
            await _classDBContext.Students.AddAsync(student);
            await _classDBContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStudent),      
                new { studentId = student.Id },
                student                
            );
        }

        [HttpDelete("{studentId}")]
        [Authorize]
        public async Task<ActionResult<Student>> DeleteStudent(int studentId)
        {
            var student = await _classDBContext.Students.FindAsync(studentId);
            if(student == null)
            {
                return NotFound(new { message = "Nenhum estudante encontrado com esseId." });
            }

            _classDBContext.Students.Remove(student);
            await _classDBContext.SaveChangesAsync();

            return Ok(new { message = "Estudante deletado com sucesso" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {

            var students = await _classDBContext.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentClass>> GetStudent(int studentId)
        {
            var student = await _classDBContext.classes.FindAsync(studentId);
            if (student == null)
            {
                return NotFound(new { error = "Não foi encontrado nenhum estudante com esse id." });

            }

            return Ok(student);
        }
    }
}
