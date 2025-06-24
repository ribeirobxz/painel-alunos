using alunos.Model.Student;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.context;

namespace alunos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {

        private readonly ClassDBContext _classDBContext;

        public StudentController(ClassDBContext classDBContext)
        {
            _classDBContext = classDBContext;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> createStudent(CreateStudentDTO createStudentDTO)
        {
            var hasWithSameName = await _classDBContext.Students.AnyAsync(student => student.name == createStudentDTO.name);
            if (hasWithSameName)
            {
                return BadRequest(new { error = "Já existe um estudante registrado com esse nome" });
            }

            var student = new Student(createStudentDTO.name, createStudentDTO.dayOfWeek);
            await _classDBContext.Students.AddAsync(student);
            await _classDBContext.SaveChangesAsync();

            return Ok(createStudentDTO);
        }

        [HttpGet]
        public async Task<ActionResult<Student>> GetAllStudents()
        {

            var students = await _classDBContext.Students.ToListAsync();
            return Ok(students);
        }
    }
}
