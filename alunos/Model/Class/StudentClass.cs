﻿using alunos.Model.Students;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alunos.Model.Class
{
    public class StudentClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public string DayOfWeek { get; set; }

        public long StartTime { get; set; }

        public StudentClass() { } 

        public StudentClass(Guid studentId, string dayOfWeek)
        {
            StudentId = studentId;
            DayOfWeek = dayOfWeek;

            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}