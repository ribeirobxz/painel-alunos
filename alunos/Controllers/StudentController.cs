using System.Security.Claims;
using alunos.Model.Answer;
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
            try
            {
                if (string.IsNullOrEmpty(createStudentDTO.Name) || string.IsNullOrEmpty(createStudentDTO.Password))
                {
                    return Ok(new Answer("O nome de usuário e a senha são obrigatórios.", 204));
                }

                var hasWithSameName = await _applicationDBContext.Students.AnyAsync(student => student.Name == createStudentDTO.Name);
                if (hasWithSameName)
                {
                    return Ok(new Answer("Já existe um estudante registrado com esse nome", 204));
                }

                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(createStudentDTO.Password, salt);

                var student = new Student(createStudentDTO.Name, passwordHash, createStudentDTO.DaysOfWeek);
                await _applicationDBContext.Students.AddAsync(student);
                await _applicationDBContext.SaveChangesAsync();

                var studentDTO = new StudentDTO(student.Id, student.Name, student.DaysOfWeek, student.Courses, student.CoursesClass, student.CoursesClassStep, student.RegisteredAt);
                var answer = new Answer<StudentDTO>(
                    "Estudante criado com sucesso.",
                    201,
                    studentDTO
                );
                return CreatedAtAction(nameof(GetStudent), new { studentId = student.Id }, answer);
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpPatch("{studentId}")]
        public async Task<IActionResult> UpdateCourseClass(Guid studentId, [FromBody] JsonPatchDocument<Student> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return Ok(new Answer("Documento de patch inválido.", 204));
                }

                var student = await _applicationDBContext.Students.FindAsync(studentId);
                if (student == null)
                {
                    return Ok(new Answer("Nenhum estudante encontrado com esse id.", 204));
                }

                patchDoc.ApplyTo(student, ModelState);
                if (!ModelState.IsValid)
                {
                    return Ok(new Answer("Nenhum estudante encontrado com esse id.", 204));
                }

                await _applicationDBContext.SaveChangesAsync();
                return Ok(new Answer($"As informações do estudante {student.Name} foram atualizadas", 200));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpDelete("{studentId}")]
        [Authorize]
        public async Task<ActionResult<Student>> DeleteStudent(Guid studentId)
        {
            try
            {
                if(!_applicationDBContext.Students.Any())
                {
                    return Ok(new Answer("Não existe nenhum estudante cadastrado.", 204));
                }

                var student = await _applicationDBContext.Students.FindAsync(studentId);
                if (student == null)
                {
                    return Ok(new Answer("Nenhum estudante encontrado com esse id.", 204));
                }

                _applicationDBContext.Students.Remove(student);
                await _applicationDBContext.SaveChangesAsync();

                return Ok(new Answer("Estudante deletado com sucesso.", 200));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            try
            {
                if (!_applicationDBContext.Students.Any())
                {
                    return Ok(new Answer("Não existe nenhum estudante cadastrado.", 204));
                }

                var studentsDTO = await _applicationDBContext.Students
                    .Select(s => new StudentDTO(s.Id, s.Name, s.DaysOfWeek, s.Courses, s.CoursesClass, s.CoursesClassStep, s.RegisteredAt))
                    .ToListAsync();
                if (studentsDTO.IsNullOrEmpty() || !studentsDTO.Any())
                {
                    return Ok(new Answer("Nenhum estudante foi encontrado.", 204));
                }

                return Ok(new Answer<List<StudentDTO>>("Requisição feita com sucesso.", 200, studentsDTO));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentClass>> GetStudent(Guid studentId)
        {
            try
            {
                if (!_applicationDBContext.Students.Any())
                {
                    return Ok(new Answer("Não existe nenhum estudante cadastrado.", 204));
                }

                var student = await _applicationDBContext.Students.FindAsync(studentId);
                if (student == null)
                {
                    return Ok(new Answer("Nenhum estudante encontrado com esse id.", 204));
                }

                var studentDTO = new StudentDTO(student.Id, student.Name, student.DaysOfWeek, student.Courses, student.CoursesClass, student.CoursesClassStep, student.RegisteredAt);
                return Ok(new Answer<StudentDTO>("Requisição feita com sucesso.", 200, studentDTO));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }
    }
}
