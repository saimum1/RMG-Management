using DotNetWbapi.Dtos;
using DotNetWbapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotNetWbapi.Controllers
{
    [Route("api/truck")]
    [ApiController]
    public class TruckController : ControllerBase
    {
        private readonly TruckService _truckService;
        private readonly ILogger<TruckController> _logger;

        public TruckController(TruckService truckService, ILogger<TruckController> logger)
        {
            _truckService = truckService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrucks()
        {
            _logger.LogInformation("Fetching all trucks");
            var trucks = await _truckService.GetTrucksAsync();
            return Ok(trucks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTruck(Guid id)
        {
            _logger.LogInformation("Fetching truck with Id: {Id}", id);
            var truck = await _truckService.GetTruckAsync(id);
            if (truck == null)
            {
                _logger.LogWarning("Truck with Id {Id} not found", id);
                return NotFound("Truck not found");
            }
            return Ok(truck);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTruck([FromBody] TruckDto dto)
        {
            _logger.LogInformation("Received DTO: {@Dto}", dto);
            try
            {
                var newTruck = await _truckService.CreateTruckAsync(dto);
                return CreatedAtAction(nameof(GetTruck), new { id = newTruck.Id }, newTruck);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create truck: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> CreateTrucks([FromBody] TruckListDto dto)
        {
            _logger.LogInformation("Received list of {Count} trucks", dto.Trucks.Count);
            try
            {
                var newTrucks = await _truckService.CreateTrucksAsync(dto);
                return CreatedAtAction(nameof(GetTrucks), newTrucks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trucks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTruck(Guid id, [FromBody] TruckDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID mismatch in update request for truck");
                return BadRequest("ID mismatch");
            }

            try
            {
                _logger.LogInformation("PUT request to update truck with Id: {Id}", id);
                var result = await _truckService.UpdateTruckAsync(id, dto);
                if (result)
                {
                    return Ok("Truck updated successfully");
                }
                return NotFound("Truck not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to update truck: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruck(Guid id)
        {
            _logger.LogInformation("DELETE request for truck with Id: {Id}", id);
            bool result = await _truckService.DeleteTruckAsync(id);
            if (result)
            {
                return Ok("Truck was deleted");
            }
            _logger.LogWarning("Truck with Id {Id} not found", id);
            return NotFound("Truck not found");
        }
    }
}