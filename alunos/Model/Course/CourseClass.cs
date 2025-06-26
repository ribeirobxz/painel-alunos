using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Course
{
    public class CourseClass
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
