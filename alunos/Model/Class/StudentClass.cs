using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace alunos.Model.Class
{
    public class StudentClass
    {

        [Key]
        public string studentName {  get; set; }
        public long startTime { get; set; }

        public StudentClass(string studentName)
        {
            this.studentName = studentName;

            startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public bool hasEnded()
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return currentTime - startTime >= 10800000;
        }

        public string GetRemainingTimeFormatted()
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var timeLeft = startTime + 10800000 - currentTime;

            if (timeLeft <= 0)
                return "Tempo encerrado";

            var remaining = TimeSpan.FromMilliseconds(timeLeft);

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                remaining.Hours,
                remaining.Minutes,
                remaining.Seconds);
        }

    }
}
