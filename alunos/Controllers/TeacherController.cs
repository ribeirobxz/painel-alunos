using alunos.Model;
using alunos.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {

        private readonly ClassDBContext _classDBContext;
        private readonly TokenService _tokenService;

        public TeacherController(ClassDBContext classDBContext, TokenService tokenService)
        {
            _classDBContext = classDBContext;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Teacher>> RequestLogin([FromBody] TeacherRequestLoginDTO teacherRequestLoginDTO)
        {
            var teacher = await _classDBContext.Teachers.FirstAsync(s => s.Name == teacherRequestLoginDTO.name);
            if(teacher == null)
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            if(!BCrypt.Net.BCrypt.Verify(teacherRequestLoginDTO.password, teacher.Password))
            {
                return Unauthorized(new { message = "Credenciais inválidas." });
            }

            var token = _tokenService.GenerateToken(teacher);
            return Ok(new TeacherResponseLoginDTO { Token = token });
        }


        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(CreateTeacherModel createTeacherModel)
        {
            var hasWithSameName = await _classDBContext.Teachers.AnyAsync(teacher => teacher.Name == createTeacherModel.Name);
            if(hasWithSameName)
            {
                return BadRequest(new { message = "Já possui um professor com esse usuário." });
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createTeacherModel.Password, salt);

            var teacher = new Teacher(createTeacherModel.Name, passwordHash);
            await _classDBContext.AddAsync(teacher);
            await _classDBContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTeacher),
                new { teacherId = teacher.Id },
                teacher
                );
        }

        [HttpGet("{teacherId}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int teacherId)
        {
            var teacher = await _classDBContext.Teachers.FindAsync(teacherId);
            if(teacher == null)
            {
                return NotFound(new { message = "Nenhum professor foi encontrado com esse Id." });
            }

            return Ok(teacher);
        }

        [HttpDelete("{teacherId}")]
        public async Task<ActionResult<Teacher>> DeleteTeacher(int teacherId)
        {
            var teacher = await _classDBContext.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                return NotFound(new { message = "Nenhum professor foi encontrado com esse Id." });
            }

            _classDBContext.Teachers.Remove(teacher);
            await _classDBContext.SaveChangesAsync();

            return Ok(new { message = "Professor deletado com sucesso." });
        }
    }
}
