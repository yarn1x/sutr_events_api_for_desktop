namespace college_events_admin_API.Services
{
    public class BackgroundUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundUpdateService> _logger;
        private Timer? _timer = null;

        public BackgroundUpdateService(IServiceProvider serviceProvider, ILogger<BackgroundUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("~~~ Фоновая служба обновления статусов мероприятия запущена ~~~");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        }

        private void DoWork(object? state)
        {
            _logger.LogInformation($"~~~ Проверка актуальности мероприятий ~~~ Время проверки: {DateTime.Now} ~~~");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<EventsService>();
                    int savedCount = service.UpdateEventsStatuses();

                    if (savedCount > 0)
                    {
                        _logger.LogInformation($"~~~ Обновлено мероприятий: {savedCount} ~~~");
                    }
                    else
                    {
                        _logger.LogInformation($"~~~ Нет мероприятий для обновления ~~~");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"~~~ Ошибка обновления статусов! {ex.Message} !!!");
            }
        }
    }
}
