using college_events_admin_API.Models.Data_transfer_objects;
using college_events_admin_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace college_events_admin_API.Controllers
{
    [ApiController]
    [Route("/college/admin/places")]
    public class ControllerLocations(SutrEventsDbContext db) : Controller
    {
        private readonly SutrEventsDbContext _db = db;

        [HttpGet]
        public ActionResult<List<Location>> GETPlaceList()
        {
            var arr = _db.Locations.ToList();
            return Ok(arr);
        }


        [HttpPost("place")]
        public ActionResult<Location> POSTPlace([FromBody] LocationPostDto newLocation)
        {
            try
            {
                var location = new Location
                {
                    Place = newLocation.Place,
                    InCollege = newLocation.InCollege,
                };
                _db.Locations.Add(location);
                _db.SaveChanges();
                return Created();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, null, 400, "Error posting new location");
            }
        }

        [HttpDelete("place")]
        public ActionResult<Location> DELETEPlace([Required] int locationId)
        {
            Location? location = _db.Locations.FirstOrDefault(x => x.LocationId == locationId);
            if (location == null)
            {
                return NotFound();
            }

            //удаление существующих записей с использованием локации (удаление внешних ключей)
            List<EventLocation> array = _db.EventLocations.Where(el => el.LocationId == location.LocationId).ToList();
            if (array != null)
            {
                try
                {
                    _db.RemoveRange(array);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            //удаление самой локации
            try
            {
                _db.Locations.Remove(location);
                _db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
