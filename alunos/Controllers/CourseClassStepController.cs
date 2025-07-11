using alunos.Model.Answer;
using alunos.Model.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            try
            {
                var courseClass = await _applicationDBContext.CourseClasses.FindAsync(createCourseClassStepModel.CourseClassId);
                if (courseClass == null)
                {
                    return Ok(new Answer("Nenhuma aula de curso foi encontrada com esse Id.", 204));
                }

                var courseClassStep = new CourseClassStep
                {
                    CourseClassId = createCourseClassStepModel.CourseClassId,
                    Name = createCourseClassStepModel.Name
                };
                await _applicationDBContext.CourseClassSteps.AddAsync(courseClassStep);
                await _applicationDBContext.SaveChangesAsync();

                var answer = new Answer<CourseClassStep>("Passo a passo da aula criado com sucesso.", 201, courseClassStep);
                return CreatedAtAction(nameof(GetCourseClassStep), new { stepId = courseClassStep.Id }, answer);
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpPatch("{stepId}")]
        public async Task<IActionResult> UpdateCourse(Guid stepId, [FromBody] JsonPatchDocument<CourseClassStep> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    return Ok(new Answer("Documento de patch inválido", 204));
                }

                var stepToUpdate = await _applicationDBContext.CourseClassSteps.FindAsync(stepId);
                if (stepToUpdate == null)
                {
                    return Ok(new Answer("Nenhum curso foi encontrado com esse Id.", 204));
                }

                patchDoc.ApplyTo(stepToUpdate, ModelState);
                if (!ModelState.IsValid)
                {
                    return Ok(new Answer<ModelStateDictionary>("Model state está inválido.", 204, ModelState));
                }

                await _applicationDBContext.SaveChangesAsync();

                return Ok(new Answer($"As informações do passo a passo {stepId} foram atualizadas", 200));
            } catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet("{stepId}")]
        public async Task<ActionResult<CourseClassStep>> GetCourseClassStep(Guid stepId)
        {
            try {
                if (!_applicationDBContext.CourseClassSteps.Any())
                {
                    return Ok(new Answer("Não existe nenhuma etapa de aula cadastrada.", 204));
                }

                var courseClassStep = await _applicationDBContext.CourseClassSteps.FindAsync(stepId);
                if (courseClassStep == null)
                {
                    return Ok(new Answer("Nenhuma etapa de aula de curso encontrada com esse ID.", 204));
                }

                return Ok(new Answer<CourseClassStep>("Requisição feita com sucesso.", 200, courseClassStep));
            } catch(Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseClassStep>>> GetAllCourseClassSteps()
        {
            try
            {
                if (!_applicationDBContext.CourseClassSteps.Any())
                {
                    return Ok(new Answer("Não existe nenhuma etapa de aula cadastrada.", 204));
                }

                var courseClassSteps = await _applicationDBContext.CourseClassSteps.ToListAsync();
                if (courseClassSteps.IsNullOrEmpty() || !courseClassSteps.Any())
                {
                    return Ok(new Answer("Nenhuma etapa de aula de curso encontrada.", 204));
                }

                return Ok(new Answer<List<CourseClassStep>>("Requisição feita com sucesso.", 200, courseClassSteps));
            }
            catch (Exception e)
            {
                return BadRequest(new Answer(e.Message, 500));
            }
        }

    }
}
