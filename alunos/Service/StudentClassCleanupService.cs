
using WebApplication3.context;

namespace alunos.Service
{
    public class StudentClassCleanupService : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public StudentClassCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var databaseContext = scope.ServiceProvider.GetRequiredService<ClassDBContext>();

                    var finishedStudentClasses = databaseContext.classes
                        .Where(studentClass => (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - studentClass.startTime) >= 10800000)
                        .ToList();

                    if (finishedStudentClasses.Any())
                    {
                        databaseContext.classes.RemoveRange(finishedStudentClasses);
                        await databaseContext.SaveChangesAsync();
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
