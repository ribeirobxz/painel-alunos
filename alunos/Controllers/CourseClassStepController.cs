using alunos.Model.Course;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseClassStepController : ControllerBase
    {

        private readonly ClassDBContext classDBContext;

        public CourseClassStepController(ClassDBContext classDBContext)
        {
            this.classDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<CourseClassStep>> CreateCourseClassStep([FromBody] CreateCourseClassStepModel createCourseClassStepModel)
        {
            if (createCourseClassStepModel.CourseClassId <= 0)
            {
                return BadRequest(new { message = "O ID da aula do curso não pode ser menor ou igual a zero." });
            }

            var courseClass = await classDBContext.CourseClasses.FindAsync(createCourseClassStepModel.CourseClassId);
            if (courseClass == null)
            {
                return NotFound(new { message = "Nenhuma aula de curso encontrada com esse ID." });
            }

            var courseClassStep = new CourseClassStep
            {
                CourseClassId = createCourseClassStepModel.CourseClassId
            };
            await classDBContext.CourseClassSteps.AddAsync(courseClassStep);
            await classDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseClassStep), new { stepId = courseClassStep.Id }, courseClassStep);
        }

        [HttpGet("{stepId}")]
        public async Task<ActionResult<CourseClassStep>> GetCourseClassStep(int stepId)
        {
            var courseClassStep = await classDBContext.CourseClassSteps.FindAsync(stepId);
            if (courseClassStep == null)
            {
                return NotFound(new { message = "Nenhuma etapa de aula de curso encontrada com esse ID." });
            }

            return Ok(courseClassStep);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseClassStep>>> GetAllCourseClassSteps()
        {
            var courseClassSteps = await classDBContext.CourseClassSteps.ToListAsync();
            if (courseClassSteps.IsNullOrEmpty() || !courseClassSteps.Any())
            {
                return NotFound(new { message = "Nenhuma etapa de aula de curso encontrada." });
            }

            return Ok(courseClassSteps);
        }

    }
}
