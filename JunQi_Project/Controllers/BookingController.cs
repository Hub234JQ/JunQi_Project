using JunQi_Project.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JunQi_Project.Models;
using System.Diagnostics.Metrics;

namespace JunQi_Project.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GetAll: All details
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Bookings);
        }

        //GetById: specific detail with the selected ID
        [HttpGet("{id}")]
        public IActionResult GetById(int? id)
        {
            var booking = _context.Bookings.FirstOrDefault(e => e.BookingID == id);
            if (booking == null)
                return Problem(detail: "Booking with ID " + id + " is not found.", statusCode: 404);
            return Ok(booking);
        }

        //GetByStatus: All, pending, approved & rejected
        [HttpGet("{BookingStatus}")]
        public IActionResult GetByStatus(string? BookingStatus = "All")
        {
            switch (BookingStatus.ToLower())
            {
                case "all":
                    return Ok(_context.Bookings);
                case "pending":
                    return Ok(_context.Bookings.Where(m => m.BookingStatus.ToLower() == "pending"));
                case "approved":
                    return Ok(_context.Bookings.Where(m => m.BookingStatus.ToLower() == "approved"));
                case "rejected":
                    return Ok(_context.Bookings.Where(m => m.BookingStatus.ToLower() == "rejected"));
                default:
                    return Problem(detail: "Booking with Status " + BookingStatus + " is not found.", statusCode: 404);
            }
        }

        //Post: Add new details. Remove the BookingId if got. 
        [HttpPost]
        public IActionResult Post(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return CreatedAtAction("GetAll", new { id = booking.BookingID }, booking);
        }

        //Put: Update details. Remove the BookingId if got. 
        [HttpPut]
        public IActionResult Put(int? id, Booking booking)
        {
            var entity = _context.Bookings.FirstOrDefault(m => m.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with id " + id + " is not found.", statusCode: 404);

            entity.FacilityDescription = booking.FacilityDescription;
            entity.BookingDateFrom = booking.BookingDateFrom;
            entity.BookingDateTo = booking.BookingDateTo;
            entity.BookedBy = booking.BookedBy;
            entity.BookingStatus = booking.BookingStatus;

            _context.SaveChanges();

            return Ok(entity);
        }

        //Delete: Delete. Just press button 
        [HttpDelete]
        public IActionResult Delete(int? id, Booking booking)
        {
            var entity = _context.Bookings.FirstOrDefault(m => m.BookingID == id);
            if (entity == null)
                return Problem(detail: "Booking with id " + id + " is not found.", statusCode: 404);

            _context.Bookings.Remove(entity);
            _context.SaveChanges();

            return Ok(entity);
        }

    }
}
