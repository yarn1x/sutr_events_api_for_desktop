using college_events_admin_API.Models;

namespace college_events_admin_API.Services
{
    public class OrganizerService(SutrEventsDbContext db, ILogger<EventsService> logger)
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly ILogger<EventsService> _logger = logger;

        public int UpdateOrganizerList()
        {
            List<int> organizerIds = [];
            HashSet<int> existingOrganizerIds = [];
            List<UserUsertype> usersToAdd = [];
            try
            {
                //получаем уникальные ID организаторов одним запросом
                organizerIds = _db.Events
                    .Where(evnt => evnt.StatusId != 1)
                    .Select(e => e.OrganizerId)
                    .Distinct()
                    .ToList();

                //получаем всех пользователей, которые уже имеют роль организатора
                existingOrganizerIds = _db.UserUsertypes
                    .Where(ut => ut.UserTypeId == 3 && organizerIds.Contains(ut.AuthorizedUserId))
                    .Select(ut => ut.AuthorizedUserId)
                    .ToHashSet();

                //находим только тех, кому нужно добавить роль
                usersToAdd = organizerIds
                    .Where(id => !existingOrganizerIds.Contains(id))
                    .Select(id => new UserUsertype
                    {
                        UserTypeId = 3,
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
