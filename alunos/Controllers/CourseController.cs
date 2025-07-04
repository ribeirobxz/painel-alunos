﻿using alunos.Model.Course;
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
            if (createCourseModel.Name.IsNullOrEmpty())
            {
                return BadRequest(new { message = "O nome do curso não pode ser vazio." });
            }

            if(createCourseModel.Description.IsNullOrEmpty())
            {
                return BadRequest(new { message = "A descrição do curso não pode ser vazia." });
            }

            var course = new Course
            {
                Name = createCourseModel.Name,
                Description = createCourseModel.Description,
                IconUrl = createCourseModel.IconUrl
            };
            await _applicationDBContext.Courses.AddAsync(course);
            await _applicationDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { courseId = course.Id }, course);
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
        public async Task<ActionResult<Course>> GetCourse(int courseId)
        {
            var course = await _applicationDBContext.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound(new { message = "Curso não encontrado com esse Id." });
            }

            return Ok(course);
        }

        [HttpGet("{courseId}/classes")]
        public async Task<ActionResult<IEnumerable<CourseClass>>> GetCourseClasses(Guid courseId)
        {
            var courseClasses = await _applicationDBContext.CourseClasses
                .Where(course => course.CourseId == courseId)
                .ToListAsync();
            if (courseClasses == null || !courseClasses.Any())
            {
                return NotFound(new { message = "Nenhuma aula encontrada para este curso." });
            }

            return Ok(courseClasses);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await _applicationDBContext.Courses.ToListAsync();
            if (courses == null || !courses.Any())
            {
                return NotFound(new { message = "Nenhum curso encontrado." });
            }

            return Ok(courses);
        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult<Course>> DeleteCourse(int couseId)
        {
            var course = await _applicationDBContext.Courses.FindAsync(couseId);
            if (course == null)
            {
                return NotFound(new { message = "Nenhum curso foi encontrado com esse Id." });
            }

            _applicationDBContext.Courses.Remove(course);
            await _applicationDBContext.SaveChangesAsync();

            return Ok(new { message = "Curso deletado com sucesso." });
        }
    }
}
