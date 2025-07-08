using System.Security.Claims;
using alunos.Model.Answer;
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
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDBContext classDbContext, TokenService tokenService)
        {
            _applicationDBContext = classDbContext;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseLoginDTO>> RequestLogin([FromBody] RequestLoginDTO requestLoginDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(requestLoginDTO.Name) || string.IsNullOrEmpty(requestLoginDTO.Password))
                {
                    return Ok(new Answer("O nome de usuário e a senha são obrigatórios.", 204));
                }

                var student = await _applicationDBContext.Students.FirstOrDefaultAsync(s => s.Name == requestLoginDTO.Name);
                if (student != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(requestLoginDTO.Password, student.PasswordHash))
                    {
                        var token = _tokenService.GenerateToken(student.Id, "student");
                        var responseLoginDTO = new ResponseLoginDTO
                        {
                            Token = token
                        };
                        return Ok(new Answer<ResponseLoginDTO>("Logou com sucesso.", 200, responseLoginDTO));
                    }
                }

                var teacher = await _applicationDBContext.Teachers.FirstOrDefaultAsync(t => t.Name == requestLoginDTO.Name);
                if (teacher != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(requestLoginDTO.Password, teacher.PasswordHash))
                    {
                        var token = _tokenService.GenerateToken(teacher.Id, "teacher");
                        var responseLoginDTO = new ResponseLoginDTO
                        {
                            Token = token
                        };
                        return Ok(new Answer<ResponseLoginDTO>("Logou com sucesso.", 200, responseLoginDTO));
                    }
                }

                return Ok(new Answer("Credenciais inválidas", 204));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<Student>> GetYourself()
        {
            try { 
                var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(idString))
                {
                    return Ok(new Answer("O ID do usuário não foi encontrado no token.", 204));
                }

                if (!Guid.TryParse(idString, out var userId))
                {
                    return Ok(new Answer("ID do usuário no formato inválido.", 204));
                }

                var student = await _applicationDBContext.Students.FindAsync(userId);
                if (student != null)
                {
                    var studentDTO = new StudentDTO(
                        userId,
                        student.Name,
                        student.DaysOfWeek,
                        student.Courses,
                        student.CoursesClass,
                        student.CoursesClassStep,
                        student.RegisteredAt
                        );
                    return Ok(new Answer<StudentDTO>("Requisição feita com sucesso.", 200, studentDTO));
                }

                var teacher = await _applicationDBContext.Teachers.FindAsync(userId);
                if (teacher != null)
                {
                    var teacherDTO = new TeacherDTO(
                        teacher.Name,
                        teacher.CreatedAt
                        );
                    return Ok(new Answer<TeacherDTO>("Requisição feita com sucesso.", 200, teacherDTO));
                }

                return Ok(new Answer("Você não está logado", 204));
            } catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }

        }
    }
}
