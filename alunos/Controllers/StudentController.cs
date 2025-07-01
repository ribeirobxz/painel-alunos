using System.Security.Claims;
using alunos.Model.Class;
using alunos.Model.Login;
using alunos.Model.Students;
using alunos.Model.Teacher;
using alunos.Service;
using Microsoft.AspNetCore.Authorization;
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

        private readonly ClassDBContext _classDBContext;
        private readonly TokenService _tokenService;

        public StudentController(ClassDBContext classDBContext, TokenService tokenService)
        {
            _classDBContext = classDBContext;
            _tokenService = tokenService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<Student>> GetYourself()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(idString.IsNullOrEmpty())
            {
                return Unauthorized("ID do estudante não foi encontrado no token.");
            }

            if (!Guid.TryParse(idString, out var studentId))
            {
                return BadRequest("ID do usuário no formato inválido.");
            }

            var student = await _classDBContext.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Estudante não encontrado.");
            }

            var studentDTO = new StudentDTO(
                studentId,
                student.Name,
                student.DaysOfWeek,
                student.Courses,
                student.CoursesClass,
                student.CoursesClassStep,
                student.RegisteredAt
                );
            return Ok(studentDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent([FromBody] CreateStudentDTO createStudentDTO)
        {
            var hasWithSameName = await _classDBContext.Students.AnyAsync(student => student.Name == createStudentDTO.Name);
            if (hasWithSameName)
            {
                return BadRequest(new { error = "Já existe um estudante registrado com esse nome" });
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createStudentDTO.Password, salt);

            var student = new Student(createStudentDTO.Name, passwordHash, createStudentDTO.DaysOfWeek);
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
