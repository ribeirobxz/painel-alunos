using System.Security.Claims;
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

        private readonly ClassDBContext _classDbContext;
        private readonly TokenService _tokenService;

        public AuthController(ClassDBContext classDbContext, TokenService tokenService)
        {
            _classDbContext = classDbContext;
            _tokenService = tokenService;
        }

        [HttpGet("login")]
        public async Task<ActionResult<ResponseLoginDTO>> RequestLogin(RequestLoginDTO requestLoginDTO)
        {

            var student = await _classDbContext.Students.FirstOrDefaultAsync(s => s.Name == requestLoginDTO.Name);

            if (student != null)
            {
                if (BCrypt.Net.BCrypt.Verify(requestLoginDTO.Password, student.PasswordHash))
                {
                    var token = _tokenService.GenerateToken(student.Id, "student");
                    return Ok(new ResponseLoginDTO { Token = token });
                }
            }

            var teacher = await _classDbContext.Teachers.FirstOrDefaultAsync(t => t.Name == requestLoginDTO.Name);
            if (teacher != null)
            {
                if (BCrypt.Net.BCrypt.Verify(requestLoginDTO.Password, teacher.PasswordHash))
                {
                    var token = _tokenService.GenerateToken(student.Id, "teacher");
                    return Ok(new ResponseLoginDTO { Token = token });
                }
            }

            return Unauthorized("Credenciais inválidas");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<Student>> GetYourself()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idString.IsNullOrEmpty())
            {
                return Unauthorized("O ID do usuário não foi encontrado no token.");
            }

            if (!Guid.TryParse(idString, out var userId))
            {
                return BadRequest("ID do usuário no formato inválido.");
            }

            var student = await _classDbContext.Students.FindAsync(userId);
            if(student != null)
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
                return Ok(studentDTO);
            }

            var teacher = await _classDbContext.Teachers.FindAsync(userId);
            if (teacher != null)
            {
                var teacherDTO = new TeacherDTO(
                    teacher.Name,
                    teacher.CreatedAt
                    );
                return Ok(teacherDTO);
            }

            return BadRequest("Você não está logado.");
        }
    }
}
