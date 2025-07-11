using alunos.Model.Answer;
using alunos.Model.Teacher;
using alunos.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public TeacherController(ApplicationDBContext classDBContext)
        {
            _applicationDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(CreateTeacherModel createTeacherModel)
        {
            try
            {
                if (string.IsNullOrEmpty(createTeacherModel.Name) || string.IsNullOrEmpty(createTeacherModel.Password))
                {
                    return Ok(new Answer("Os campos de usuário e senha são obrigatórios.", 204));
                }

                var hasWithSameName = await _applicationDBContext.Teachers.AnyAsync(teacher => teacher.Name == createTeacherModel.Name);
                if (hasWithSameName)
                {
                    return Ok(new Answer("Já possui um professor com esse usuário.", 204));
                }

                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(createTeacherModel.Password, salt);

                var teacher = new Teacher(createTeacherModel.Name, passwordHash);
                await _applicationDBContext.AddAsync(teacher);
                await _applicationDBContext.SaveChangesAsync();

                var teacherDTO = new TeacherDTO(teacher.Name, teacher.CreatedAt);
                var answer = new Answer<TeacherDTO>("Professor criado com sucesso.", 201, teacherDTO);

                return CreatedAtAction(
                    nameof(GetTeacher),
                    new { teacherId = teacher.Id },
                    answer
                    );
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("{teacherId}")]
        public async Task<ActionResult<Teacher>> GetTeacher(Guid teacherId)
        {
            try
            {
                if (!_applicationDBContext.Teachers.Any())
                {
                    return Ok(new Answer("Não existe nenhum professor cadastrado.", 204));
                }

                var teacher = await _applicationDBContext.Teachers.FindAsync(teacherId);
                if (teacher == null)
                {
                    return Ok(new Answer("Nenhum professor foi encontrado com esse id.", 204));
                }

                var teacherDTO = new TeacherDTO(teacher.Name, teacher.CreatedAt);
                return Ok(new Answer<TeacherDTO>("Requisição feita com sucesso", 200, teacherDTO));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpDelete("{teacherId}")]
        public async Task<ActionResult<Teacher>> DeleteTeacher(Guid teacherId)
        {
            try
            {
                if (!_applicationDBContext.Teachers.Any())
                {
                    return Ok(new Answer("Não existe nenhum professor cadastrado.", 204));
                }

                var teacher = await _applicationDBContext.Teachers.FindAsync(teacherId);
                if (teacher == null)
                {
                    return Ok(new Answer("Nenhum professor foi encontrado com esse id.", 204));
                }

                _applicationDBContext.Teachers.Remove(teacher);
                await _applicationDBContext.SaveChangesAsync();

                return Ok(new Answer("Professor deletado com sucesso.", 200));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }
    }
}
