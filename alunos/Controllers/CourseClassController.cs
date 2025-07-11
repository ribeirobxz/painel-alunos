using alunos.Model.Answer;
using alunos.Model.Course;
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
    [Authorize(Roles = "teacher,student")]
    public class CourseClassController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public CourseClassController(ApplicationDBContext classDBContext)
        {
            _applicationDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<CourseClass>> CreateCourseClass([FromBody] CreateCourseClassModel createCourseClassModel)
        {
            try
            {
                var course = await _applicationDBContext.Courses.FindAsync(createCourseClassModel.CourseId);
                if (course == null)
                {
                    return Ok(new Answer("Nenhum curso foi encontrado com esse Id.", 204));
                }

                var courseClass = new CourseClass
                {
                    CourseId = createCourseClassModel.CourseId
                };
                await _applicationDBContext.CourseClasses.AddAsync(courseClass);
                await _applicationDBContext.SaveChangesAsync();

                var answer = new Answer<CourseClass>("Requisição feita com sucesso.", 201, courseClass);
                return CreatedAtAction(nameof(GetCourseClass), new { classId = courseClass.Id }, answer);
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpPatch("{classId}")]
        public async Task<IActionResult> UpdateCourseClass(Guid classId, [FromBody] JsonPatchDocument<CourseClass> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return Ok(new Answer("Documento de patch inválido.", 204));
                }

                var courseClassToUpdate = await _applicationDBContext.CourseClasses.FindAsync(classId);
                if (courseClassToUpdate == null)
                {
                    return Ok(new Answer("Nenhuma aula foi encontrada com esse id.", 204));
                }

                patchDoc.ApplyTo(courseClassToUpdate, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _applicationDBContext.SaveChangesAsync();

                return Ok(new Answer($"As informações da aula {classId} foram atualizadas", 200));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("{classId}")]
        public async Task<ActionResult<CourseClass>> GetCourseClass(int classId)
        {
            if (!_applicationDBContext.CourseClasses.Any())
            {
                return Ok(new Answer("Não existe nenhuma aula de curso cadastrada.", 204));
            }

            var courseClass = await _applicationDBContext.CourseClasses.FindAsync(classId);
            if (courseClass == null)
            {
                return NotFound(new { message = "Nenhuma aula de curso encontrada com esse ID." });
            }

            return Ok(courseClass);
        }

        [HttpGet("{classId}/steps")]
        public async Task<ActionResult<IEnumerable<CourseClassStepController>>> GetCourseClassSteps(Guid classId)
        {
            var courseClassExists = await _applicationDBContext.CourseClasses.AnyAsync(c => c.Id == classId);
            if (!courseClassExists)
            {
                return NotFound($"A aula com o ID {classId} não foi encontrada.");
            }

            var steps = await _applicationDBContext.CourseClassSteps
                                      .Where(step => step.CourseClassId == classId)
                                      .ToListAsync();

            return Ok(steps);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseClass>>> GetAllCourseClass()
        {
            var courseClasses = await _applicationDBContext.CourseClasses.ToListAsync();
            if (courseClasses.IsNullOrEmpty() || !courseClasses.Any())
            {
                return NotFound(new { message = "Nenhuma aula de curso encontrada." });
            }

            return Ok(courseClasses);
        }

        [HttpDelete("{classId}")]
        public async Task<ActionResult<CourseClass>> DeleteCourseClass(int classId)
        {
            var courseClass = await _applicationDBContext.CourseClasses.FindAsync(classId);
            if (courseClass == null)
            {
                return NotFound(new { message = "Nenhuma aula de curso foi encontrada com esse ID." });
            }

            _applicationDBContext.CourseClasses.Remove(courseClass);
            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = $"Aula {classId} do curso {courseClass.CourseId} foi deletada com sucesso." });
        }
    }
}
