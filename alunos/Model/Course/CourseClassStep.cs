using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Course
{
    public class CourseClassStep
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid CourseClassId { get; set; }
         
        [ForeignKey("CourseClassId")]
        public virtual CourseClass CourseClass { get; set; }

        public string Name { get; set; }
    }
}
