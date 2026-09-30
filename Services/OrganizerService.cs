using college_events_admin_API.Models;

namespace college_events_admin_API.Services
{
    public class OrganizerService(SutrEventsDbContext db, ILogger<OrganizerService> logger)
    {
        private readonly SutrEventsDbContext _db = db;
        private readonly ILogger<OrganizerService> _logger = logger;

        public int UpdateOrganizerList()
        {
            List<int> organizerIds = [];
            HashSet<int> existingOrganizerIds = [];
            List<UserUsertype> usersToAdd = [];
            try
            {
                //получаем уникальные ID пользователей, которых обозначили как организатора мероприятия
                organizerIds = _db.Events
                    .Where(evnt => 
                        evnt.StatusId == EventConstant.status_applied 
                        || evnt.StatusId == EventConstant.status_done_report_needed
                        || evnt.StatusId == EventConstant.status_done
                        || evnt.StatusId == EventConstant.status_rescheduled
                    )
                    .Select(e => e.OrganizerId)
                    .Distinct()
                    .ToList();

                //получаем всех пользователей, которые уже имеют роль организатора
                existingOrganizerIds = _db.UserUsertypes
                    .Where(ut => ut.UserTypeId == UserConstant.organizerTypeId && organizerIds.Contains(ut.AuthorizedUserId))
                    .Select(ut => ut.AuthorizedUserId)
                    .ToHashSet();

                //находим только тех, кому нужно добавить роль
                usersToAdd = organizerIds
                    .Where(id => !existingOrganizerIds.Contains(id))
                    .Select(id => new UserUsertype
                    {
                        UserTypeId = UserConstant.organizerTypeId,
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
