using System.Linq.Expressions;
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
    public class CourseController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public CourseController(ApplicationDBContext classDBContext)
        {
            _applicationDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourse([FromBody] CreateCourseModel createCourseModel)
        {
            try
            {
                if (string.IsNullOrEmpty(createCourseModel.Name) || string.IsNullOrEmpty(createCourseModel.Description)
                    || string.IsNullOrEmpty(createCourseModel.IconUrl))
                {
                    return Ok(new Answer("Os campos do curso não podem estar em branco.", 204));
                }

                var course = new Course
                {
                    Name = createCourseModel.Name,
                    Description = createCourseModel.Description,
                    IconUrl = createCourseModel.IconUrl
                };
                await _applicationDBContext.Courses.AddAsync(course);
                await _applicationDBContext.SaveChangesAsync();

                var answer = new Answer<Course>("Curso criado com sucesso", 201, course);
                return CreatedAtAction(nameof(GetCourse), new { courseId = course.Id }, answer);
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpPatch("{courseId}")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] JsonPatchDocument<Course> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "Documento de patch inválido." });
            }

            var courseToUpdate = await _applicationDBContext.Courses.FindAsync(courseId);
            if (courseToUpdate == null)
            {
                return NotFound(new { message = "Curso não encontrado com esse Id." });
            }

            patchDoc.ApplyTo(courseToUpdate, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = $"As informações do curso {courseId} foram atualizadas"});
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<Course>> GetCourse(Guid courseId)
        {
            try
            {
                var course = await _applicationDBContext.Courses.FindAsync(courseId);
                if (course == null)
                {
                    return Ok(new Answer("Nenhum curso foi encontrado com esse id.", 204));
                }

                return Ok(new Answer<Course>("Requisição feita com sucesso.", 200, course));
            }catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("{courseId}/classes")]
        public async Task<ActionResult<IEnumerable<CourseClass>>> GetCourseClasses(Guid courseId)
        {
            try
            {
                var courseClasses = await _applicationDBContext.CourseClasses
                    .Where(course => course.CourseId == courseId)
                    .ToListAsync();
                if (courseClasses == null || !courseClasses.Any())
                {
                    return Ok(new Answer("Nenhuma aula encontrada para este curso.", 204));
                }

                return Ok(new Answer<List<CourseClass>>("Requisição feita com sucesso.", 200, courseClasses));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            try
            {
                var courses = await _applicationDBContext.Courses.ToListAsync();
                if (courses == null || !courses.Any())
                {
                    return Ok(new Answer("Nenhum curso foi encontrado.", 204));
                }

                return Ok(new Answer<List<Course>>("Requisição feita com suceso", 200, courses));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult<Course>> DeleteCourse(int couseId)
        {
            try
            {
                var course = await _applicationDBContext.Courses.FindAsync(couseId);
                if (course == null)
                {
                    return Ok(new Answer("Nenhum curso foi encontrado com esse Id.", 204));
                }

                _applicationDBContext.Courses.Remove(course);
                await _applicationDBContext.SaveChangesAsync();

                return Ok(new Answer("Curso deletado com sucesso.", 200));
            } catch(Exception e) {
                return BadRequest(new Answer(e.Message, 500));
            }
        }
    }
}
