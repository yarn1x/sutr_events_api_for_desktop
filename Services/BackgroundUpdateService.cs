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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private void DoWork(object? state)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation($"~~~ Проверка актуальности мероприятий\t~~~\tВремя: {DateTime.Now} ~~~");
                    var eventService = scope.ServiceProvider.GetRequiredService<EventsService>();
                    int savedCount = eventService.UpdateEventsStatuses();

                    if (savedCount > 0)
                    {
                        _logger.LogInformation($"~~~ Обновлено мероприятий: {savedCount} ~~~");
                    }
                    else
                    {
                        _logger.LogInformation($"~~~ Нет мероприятий для обновления ~~~");
                    }


                    _logger?.LogInformation($"~~~ Назначение новых организаторов\t~~~\tВремя: {DateTime.Now} ~~~");
                    var organizerService = scope.ServiceProvider.GetRequiredService<OrganizerService>();
                    savedCount = organizerService.UpdateOrganizerList();

                    if (savedCount > 0)
                    {
                        _logger?.LogInformation($"~~~ Назначено новых организаторов: {savedCount} ~~~");
                    }
                    else
                    {
                        _logger?.LogInformation($"~~~ Нет новых организаторов ~~~");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"### Ошибка ошибка фоновой службы! {ex} ###");
            }
        }
    }
}
