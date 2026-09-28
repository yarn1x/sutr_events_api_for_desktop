using college_events_admin_API.Models;
using college_events_admin_API.Models.Data_transfer_objects;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace college_events_admin_API.Controllers
{
	//[Authorize]
	[ApiController]
	[Route("/college/admin/events")]
	public class ControllerEvents(SutrEventsDbContext db) : Controller
    {
		private readonly SutrEventsDbContext _db = db;


        [HttpGet]
		public ActionResult GETEventList()
		{
			var arr = _db.Events
				.Select(e => new
				{
					e.EventId,

					e.Title,
					e.StartDatetime,
					e.Duration,
					e.EndDatetime,
					e.FullDescription,
                    e.ShortDescription,

                    e.CategoryId,
					CategoryName = e.Category.Name,
					Locations = e.EventLocations.Select(eventLocations => new
					{
						eventLocations.Location.LocationId,
						eventLocations.Location.Place,
						eventLocations.Location.InCollege
					}).ToList(),
					e.StatusId,
                    StatusName = e.Status.Name,

                    //НЕ МЕНЯТЬ ИМЕНА ПОЛЕЙ. ЧЕРЕВАТО НЕИЗВЕСТНЫМИ ОШИБКАМИ
                    //Во всей системе, на стороне настольного приложения
                    //firstname - имя
                    //surname - фамилия
                    //lastname - отчество
                    OrganizerFirstName = e.Organizer.User.FirstName,
					OrganizerSurname = e.Organizer.User.LastName,
					OrganizerLastname = e.Organizer.User.MiddleName,
					e.OrganizerOrganization,
					e.OrganizerPosition,

					e.AdditionalInfo,
					e.MaxListenersCount,
					e.MaxParticipantsCount
				})
				.OrderBy(e => e.StatusId).ThenBy(e => e.StartDatetime)
				.ToList();

			
			return Ok(arr);
		}





		[HttpGet("categories")]
		public ActionResult GETCategoryList()
		{
			var arr = _db.Categories
				.Select(c => new
				{
					c.CategoryId,
					c.Name
				})
				.ToList();
			return Ok(arr);
		}





		[HttpGet("{EventId}/groups")]
		public ActionResult GETEventGroups(int EventId)
		{
			var arr = _db.EventGroups.Select(e => new
			{
				e.EventGroupId,
				e.EventId,
				e.GroupId,
				e.Group.Name,
				SupervisorName = e.Group.AuthorizedUser!.User.FirstName,
				SupervisorSurname = e.Group.AuthorizedUser!.User.LastName,
				SupervisorLastname = e.Group.AuthorizedUser!.User.MiddleName,
				e.ExpectedListenersCount,
				e.ExpectedParticipantsCount,
				e.ExpectedSuperParticipantsCount,
			})
			.Where(e => e.EventId == EventId)
			.ToList();

			return Ok(arr);
		}





        [HttpPut("update/{EventId}/groups")]
		public ActionResult<bool> PUTEventGroups(int EventId, [FromBody] List<EventGroupPost> groups)
		{
			using var transaction = _db.Database.BeginTransaction();

			try
			{
				// выборки данных
				// получаем существующие группы для этого события
				var existingGroups = _db.EventGroups
					.Where(g => g.EventId == EventId)
					.ToList();
				
				// удалить (есть в БД, нет в запросе)
				var groupsToDelete = existingGroups
					.Where(e => !groups.Any(g => g.EventGroupId == e.EventGroupId))
					.ToList();

				// добавить (есть в запросе, нет в БД)
				var groupsToAdd = groups
					.Where(g => !existingGroups.Any(e => e.EventGroupId == g.EventGroupId))
					.ToList();

				// обновить (есть и там, и там)
				var groupsToUpdate = groups
					.Where(g => existingGroups.Any(e => e.EventGroupId == g.EventGroupId))
					.ToList();

				// далее идут сами методы внесения изменений
				if (groupsToDelete.Any())
				{
					_db.EventGroups.RemoveRange(groupsToDelete);
				}

				if (groupsToAdd.Any())
				{
					var newGroups = groupsToAdd.Select(dto => new EventGroup
					{
						EventId = EventId,
						GroupId = dto.GroupId,
						ExpectedListenersCount = dto.ExpectedListenersCount,
						ExpectedParticipantsCount = dto.ExpectedParticipantsCount,
						ExpectedSuperParticipantsCount = dto.ExpectedSuperParticipantsCount,
					});
					_db.EventGroups.AddRange(newGroups);
				}

                var existingGroupsDict = existingGroups.ToDictionary(e => e.EventGroupId);

                foreach (var updateGroup in groupsToUpdate)
                {
                    if (existingGroupsDict.TryGetValue(updateGroup.EventGroupId, out var existing))
                    {
                        existing.GroupId = updateGroup.GroupId;
                        existing.ExpectedListenersCount = updateGroup.ExpectedListenersCount;
                        existing.ExpectedParticipantsCount = updateGroup.ExpectedParticipantsCount;
                        existing.ExpectedSuperParticipantsCount = updateGroup.ExpectedSuperParticipantsCount;
                    }
                }

                _db.SaveChanges();
				transaction.Commit();

				return Ok();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				return Problem(ex.ToString(), null, 400, "Error updating groups");
			}
		}




        [HttpPut("update/{EventId}")]
        public ActionResult<bool> PUTUpdateFullEvent(int EventId, [FromBody] EventUpdateDto dto)
        {
            using var transaction = _db.Database.BeginTransaction();

            try
            {
                //ОБНОВЛЯЕМ ОСНОВНУЮ ИНФОРМАЦИЮ
                var eventToUpdate = _db.Events.FirstOrDefault(e => e.EventId == EventId);
                if (eventToUpdate == null)
                {
                    return NotFound($"Мероприятие ID={EventId} не найдено");
                }

                //обновляем поля мероприятия
                eventToUpdate.Title = dto.Event.Title;
                eventToUpdate.CategoryId = dto.Event.CategoryId;
                eventToUpdate.OrganizerId = dto.Event.OrganizerId;
                eventToUpdate.MaxListenersCount = dto.Event.MaxListenersCount;
                eventToUpdate.MaxParticipantsCount = dto.Event.MaxParticipantsCount;
                eventToUpdate.OrganizerOrganization = dto.Event.OrganizerOrganization;
                eventToUpdate.OrganizerPosition = dto.Event.OrganizerPosition;
                eventToUpdate.ShortDescription = dto.Event.ShortDescription;
                eventToUpdate.FullDescription = dto.Event.FullDescription;
                eventToUpdate.AdditionalInfo = dto.Event.AdditionalInfo;
                eventToUpdate.StartDatetime = dto.Event.StartDateTime;
                eventToUpdate.EndDatetime = dto.Event.EndDateTime;


                
                var existingLocationIds = _db.EventLocations
					.Where(el => el.EventId == EventId)
					.Select(el => el.LocationId)
					.ToList();	

                //подготавливаем список новых ID от пользователя (защита от null)
                var incomingLocationIds = dto.Event.EventLocationsIds ?? new List<int>();

                //находим, какие локации нужно УДАЛИТЬ (они есть в БД, но их нет в новом списке)
                var idsToRemove = existingLocationIds.Except(incomingLocationIds).ToList();
                if (idsToRemove.Any())
                {
                    var locationsToRemove = _db.EventLocations
                        .Where(el => el.EventId == EventId && idsToRemove.Contains(el.LocationId));
                    _db.EventLocations.RemoveRange(locationsToRemove);
                }

                //находим, какие локации нужно ДОБАВИТЬ (они есть в новом списке, но их нет в БД)
                var idsToAdd = incomingLocationIds.Except(existingLocationIds).ToList();
                if (idsToAdd.Any())
                {
                    var newLocations = idsToAdd.Select(locId => new EventLocation
                    {
                        EventId = EventId,
                        LocationId = locId
                    });
                    _db.EventLocations.AddRange(newLocations);
                }

                //ОБНОВЛЯЕМ ГРУППЫ (логика выборочного изменения)
                var existingGroups = _db.EventGroups
                    .Where(g => g.EventId == EventId)
                    .ToList();

                var groups = dto.Groups;

                var groupsToDelete = existingGroups
                    .Where(e => !groups.Any(g => g.EventGroupId == e.EventGroupId))
                    .ToList();

                var groupsToAdd = groups
                    .Where(g => !existingGroups.Any(e => e.EventGroupId == g.EventGroupId))
                    .ToList();

                var groupsToUpdate = groups
                    .Where(g => existingGroups.Any(e => e.EventGroupId == g.EventGroupId))
                    .ToList();

                if (groupsToDelete.Any())
                    _db.EventGroups.RemoveRange(groupsToDelete);

                if (groupsToAdd.Any())
                {
                    var newGroups = groupsToAdd.Select(g => new EventGroup
                    {
                        EventId = EventId,
                        GroupId = g.GroupId,
                        ExpectedListenersCount = g.ExpectedListenersCount,
                        ExpectedParticipantsCount = g.ExpectedParticipantsCount,
                        ExpectedSuperParticipantsCount = g.ExpectedSuperParticipantsCount,
                    });
                    _db.EventGroups.AddRange(newGroups);
                }

                var existingGroupsDict = existingGroups.ToDictionary(e => e.EventGroupId);

                foreach (var updateGroup in groupsToUpdate)
                {
                    if (existingGroupsDict.TryGetValue(updateGroup.EventGroupId, out var existing))
                    {
                        existing.GroupId = updateGroup.GroupId;
                        existing.ExpectedListenersCount = updateGroup.ExpectedListenersCount;
                        existing.ExpectedParticipantsCount = updateGroup.ExpectedParticipantsCount;
                        existing.ExpectedSuperParticipantsCount = updateGroup.ExpectedSuperParticipantsCount;
                    }
                }

                //СОХРАНЯЕМ ВСЁ В ОДНОЙ ТРАНЗАКЦИИ
                _db.SaveChanges();
                transaction.Commit();

                return Ok(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Problem(ex.ToString(), null, 400, "Error updating event");
            }
        }






        [HttpPut("{EventId}/status/{StatusId}")]
		public async Task<ActionResult> PUTEventStatus(int EventId, int StatusId)
		{
            if (!await _db.Statuses.AnyAsync(s => s.StatusId == StatusId)) return BadRequest("Некорректный id статуса");

			try
			{
				var ev = await _db.Events.FindAsync(EventId);

				if (ev == null)
				{
					return NotFound($"Мероприятие с id={EventId} не найдено");
				}
				ev.StatusId = StatusId;
				_db.SaveChanges();

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}




		[HttpGet("{EventId}/statistics")]
		public async Task<ActionResult> GETEventGroupsStatistics(int EventId)
		{
			var a = await _db.ActualAttendances
				.Where(a => a.EventGroup.EventId == EventId)
				.Select(a => new
				{
					a.ActualAttendanceId,
					a.EventGroupId,
					a.ActualListenersCount,
					a.ActualParticipantsCount,
					a.ActualSuperParticipantsCount,
					a.TotalScore,

					a.EventGroup.Group.GroupId,
					groupName = a.EventGroup.Group.Name,

                    supervisorName = a.EventGroup.Group.AuthorizedUser!.User.FirstName,
					supervisorSurname = a.EventGroup.Group.AuthorizedUser!.User.LastName,
                    supervisorLastname = a.EventGroup.Group.AuthorizedUser!.User.MiddleName,

                })
				.ToListAsync();

			return Ok(a);
		}




		[HttpPut("{EventId}/statistics")]
		public async Task<ActionResult> PUTUpdateEventGroupsStatistics(int EventId, [FromBody] List<ActualAttendanceUpdateDto> body)
		{
            using IDbContextTransaction? transaction = _db.Database.BeginTransaction();

            try
            {
                // получаем существующие группы для этого события
                List<ActualAttendance>? existingGroups = await _db.ActualAttendances
                    .Where(g => g.EventGroup.EventId == EventId)
                    .ToListAsync();


				List<ActualAttendance>? groupsToDelete = existingGroups
					.Where(e => !body.Any(g => g.ActualAttendanceId == e.ActualAttendanceId))
					.ToList();
				if (groupsToDelete.Any())
				{
					_db.ActualAttendances.RemoveRange(groupsToDelete);
				}


                List<ActualAttendanceUpdateDto>? groupsToCreate = body
					.Where(e => !existingGroups.Any(g => g.ActualAttendanceId == e.ActualAttendanceId))
					.ToList();
				if (groupsToCreate.Any())
				{
					IEnumerable<ActualAttendance>? newGroups = groupsToCreate.Select(dto => new ActualAttendance()
					{
						EventGroupId = dto.EventGroupId,
						ActualListenersCount = dto.ActualListenersCount,
						ActualParticipantsCount = dto.ActualParticipantsCount,
						ActualSuperParticipantsCount = dto.ActualSuperParticipantsCount,
						TotalScore = dto.TotalScore,
					});
					_db.ActualAttendances.AddRange(newGroups);
				}


                List<ActualAttendanceUpdateDto>? groupsToUpdate = body
					.Where(e => existingGroups.Any(g => g.ActualAttendanceId == e.ActualAttendanceId))
					.ToList();

                var existingGroupsDict = existingGroups.ToDictionary(e => e.ActualAttendanceId);
                foreach (var updateGroup in groupsToUpdate)
                {
                    if (existingGroupsDict.TryGetValue(updateGroup.ActualAttendanceId, out var existing))
                    {
                        existing.EventGroupId = updateGroup.EventGroupId;
                        existing.ActualListenersCount = updateGroup.ActualListenersCount;
                        existing.ActualParticipantsCount = updateGroup.ActualParticipantsCount;
                        existing.ActualSuperParticipantsCount = updateGroup.ActualSuperParticipantsCount;
						existing.TotalScore = updateGroup.TotalScore;
                    }
                }


                _db.SaveChanges();
				transaction.Commit();

				return NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
				return Problem(ex.ToString(), null, 400, "Error updating groups");
            }
        }
	}
}
