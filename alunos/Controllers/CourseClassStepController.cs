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
    public class CourseClassStepController : ControllerBase
    {

        private readonly ApplicationDBContext _applicationDBContext;

        public CourseClassStepController(ApplicationDBContext classDBContext)
        {
            this._applicationDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<CourseClassStep>> CreateCourseClassStep([FromBody] CreateCourseClassStepModel createCourseClassStepModel)
        {

            var courseClass = await _applicationDBContext.CourseClasses.FindAsync(createCourseClassStepModel.CourseClassId);
            if (courseClass == null)
            {
                return NotFound(new { message = "Nenhuma aula de curso encontrada com esse ID." });
            }

            var courseClassStep = new CourseClassStep
            {
                CourseClassId = createCourseClassStepModel.CourseClassId,
                Name = createCourseClassStepModel.Name
            };
            await _applicationDBContext.CourseClassSteps.AddAsync(courseClassStep);
            await _applicationDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseClassStep), new { stepId = courseClassStep.Id }, courseClassStep);
        }

        [HttpPatch("{stepId}")]
        public async Task<IActionResult> UpdateCourse(int stepId, [FromBody] JsonPatchDocument<CourseClassStep> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { message = "Documento de patch inválido." });
            }

            var stepToUpdate = await _applicationDBContext.CourseClassSteps.FindAsync(stepId);
            if (stepToUpdate == null)
            {
                return NotFound(new { message = "Curso não encontrado com esse Id." });
            }

            patchDoc.ApplyTo(stepToUpdate, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = $"As informações do passo a passo {stepId} foram atualizadas" });
        }

        [HttpGet("{stepId}")]
        public async Task<ActionResult<CourseClassStep>> GetCourseClassStep(Guid stepId)
        {
            var courseClassStep = await _applicationDBContext.CourseClassSteps.FindAsync(stepId);
            if (courseClassStep == null)
            {
                return NotFound(new { message = "Nenhuma etapa de aula de curso encontrada com esse ID." });
            }

            return Ok(courseClassStep);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseClassStep>>> GetAllCourseClassSteps()
        {
            var courseClassSteps = await _applicationDBContext.CourseClassSteps.ToListAsync();
            if (courseClassSteps.IsNullOrEmpty() || !courseClassSteps.Any())
            {
                return NotFound(new { message = "Nenhuma etapa de aula de curso encontrada." });
            }

            return Ok(courseClassSteps);
        }

    }
}
