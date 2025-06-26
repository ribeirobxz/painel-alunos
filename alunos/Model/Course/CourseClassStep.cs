using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Course
{
    public class CourseClassStep
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CourseClassId { get; set; }
         
        [ForeignKey("CourseClassId")]
        public virtual CourseClass CourseClass { get; set; }
    }
}
