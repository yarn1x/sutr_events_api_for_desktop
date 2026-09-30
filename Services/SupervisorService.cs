using college_events_admin_API.Models;

namespace college_events_admin_API.Services
{
    public class SupervisorService(SutrEventsDbContext dbContext, ILogger<SupervisorService> logger)
    {
        private readonly SutrEventsDbContext _db = dbContext;
        private readonly ILogger<SupervisorService> _logger = logger;

        public int UpdateSupervisorList()
        {
            List<int> supervisorsIds = [];
            HashSet<int> existingSupervisorsIds = [];
            List<UserUsertype> usersToAdd = [];
            try
            {
                //получаем уникальные ID пользователей, которых назначили куратором групп
                supervisorsIds = _db.Groups
                    .Select(e => e.AuthorizedUserId)
                    .Distinct()
                    .ToList();

                //получаем всех пользователей, которые уже имеют роль куратора
                existingSupervisorsIds = _db.UserUsertypes
                    .Where(ut => ut.UserTypeId == UserConstant.supervisorTypeId && supervisorsIds.Contains(ut.AuthorizedUserId))
                    .Select(ut => ut.AuthorizedUserId)
                    .ToHashSet();

                //находим только тех, кому нужно добавить роль
                usersToAdd = supervisorsIds
                    .Where(id => !existingSupervisorsIds.Contains(id))
                    .Select(id => new UserUsertype
                    {
                        UserTypeId = UserConstant.supervisorTypeId,
                        AuthorizedUserId = id
                    })
                    .ToList();

                //добавляем всех разом
                _db.UserUsertypes.AddRange(usersToAdd);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger?.LogError($"### Ошибка обновления списка организаторов ###\n### Сообщение ошибки {ex} ###");
            }

            return usersToAdd.Count;
        }

    }
}
