﻿using Microsoft.AspNetCore.Mvc;
using ZealandIdScanner.Models;
using Microsoft.EntityFrameworkCore;
using ZealandIdScanner.EBbContext;
using System.Data.Entity;
using Microsoft.Data.SqlClient;

namespace ZealandIdScanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ZealandIdContext _dbContext;

        public SensorsController(ZealandIdContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("resetTable")]
        public IActionResult ResetSensorer()
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE Sensorer");
                _dbContext.SaveChanges();

                //_context.Sensorer.AddRange(new List<Sensor> { new Sensor("ZA1", 1) });
                //_context.SaveChanges();

                return Ok("Sensorer table reset successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error resetting Sensorer table: {ex.Message}");
            }
        }

        // GET: api/Sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sensors>>> GetSensorer()
        {
            if (_dbContext.Sensors == null)
            {
                return NotFound();
            }
            return await _dbContext.Sensors.ToListAsync();
        }

        // GET: api/Sensors/2
        [HttpGet("Id/2")]
        public async Task<ActionResult<Sensors>> GetSensor(int id)
        {
            if (_dbContext == null)
            {
                return NotFound("DbContext can't be null");
            }

            var sensor = await _dbContext.Sensors.FindAsync(id);

            if (sensor == null)
            {
                return NotFound("No Such sensor exists");
            }

            return Ok(sensor);
        }


        // PUT: api/Sensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Sensors>> PutSensor(Sensors sensor)
        {
            if (sensor == null)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                _dbContext.Sensors.Add(sensor);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction("GetSensor", new { id = sensor.SensorId }, sensor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // POST: api/Sensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult PostNewStudent(Sensors sensors)
        {
            if(sensors == null)
            {
                return BadRequest(sensors);
            }
            if(sensors.SensorId == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _dbContext.SaveChangesAsync();
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            // Assuming you have configured DbContextOptions in your application's startup
            var optionsBuilder = new DbContextOptionsBuilder<ZealandIdContext>();
            optionsBuilder.UseSqlServer("Data Source=mssql11.unoeuro.com;Initial Catalog=zealandid_dk_db_test;User ID=zealandid_dk;Password=********;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"); // Replace with your actual connection string

            using (var ctx = new ZealandIdContext(optionsBuilder.Options))
            {
                ctx.Sensors.Add(new Sensors()
                {
                    SensorId = sensors.SensorId,
                    Navn = sensors.Navn,
                });

                ctx.SaveChangesAsync();
            }

            return Ok();
        }




        // DELETE: api/Sensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            if (_dbContext.Sensors == null)
            {
                return NotFound();
            }
            var sensor = await _dbContext.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }

            _dbContext.Sensors.Remove(sensor);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool SensorExists(int id)
        {
            return (_dbContext.Sensors?.Any(e => e.SensorId == id)).GetValueOrDefault();
        }
    }
}
