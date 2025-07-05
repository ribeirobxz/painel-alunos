using System.Security.Claims;
using alunos.Model.Class;
using alunos.Model.Course;
using alunos.Model.Login;
using alunos.Model.Students;
using alunos.Model.Teacher;
using alunos.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public StudentController(ApplicationDBContext classDBContext)
        {
            _applicationDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent([FromBody] CreateStudentDTO createStudentDTO)
        {
            var hasWithSameName = await _applicationDBContext.Students.AnyAsync(student => student.Name == createStudentDTO.Name);
            if (hasWithSameName)
            {
                return BadRequest(new { error = "Já existe um estudante registrado com esse nome" });
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createStudentDTO.Password, salt);

            var student = new Student(createStudentDTO.Name, passwordHash, createStudentDTO.DaysOfWeek);
            await _applicationDBContext.Students.AddAsync(student);
            await _applicationDBContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStudent),      
                new { studentId = student.Id },
                student                
            );
        }

        [HttpPatch("{studentId}")]
        public async Task<IActionResult> UpdateCourseClass(Guid studentId, [FromBody] JsonPatchDocument<Student> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "Documento de patch inválido." });
            }

            var studentToUpdate = await _applicationDBContext.Students.FindAsync(studentId);
            if (studentToUpdate == null)
            {
                return NotFound(new { message = "Estudante não encontrado com esse Id." });
            }

            patchDoc.ApplyTo(studentToUpdate, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = $"As informações do estudante {studentToUpdate.Name} foram atualizadas" });
        }

        [HttpDelete("{studentId}")]
        [Authorize]
        public async Task<ActionResult<Student>> DeleteStudent(int studentId)
        {
            var student = await _applicationDBContext.Students.FindAsync(studentId);
            if(student == null)
            {
                return NotFound(new { message = "Nenhum estudante encontrado com esseId." });
            }

            _applicationDBContext.Students.Remove(student);
            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = "Estudante deletado com sucesso" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {

            var students = await _applicationDBContext.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentClass>> GetStudent(int studentId)
        {
            var student = await _applicationDBContext.classes.FindAsync(studentId);
            if (student == null)
            {
                return NotFound(new { error = "Não foi encontrado nenhum estudante com esse id." });

            }

            return Ok(student);
        }
    }
}
