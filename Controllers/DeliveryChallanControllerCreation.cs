using DotNetWbapi.Dtos;
using DotNetWbapi.Services;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Threading.Tasks;

namespace DotNetWbapi.Controllers
{

    // declare the API controller with route prefix "api/deliverychallan"
    [Route("api/deliverychallan")]
    [ApiController]
    public class DeliveryChallanControllerCreation : ControllerBase
    {
        private readonly DeliveryChallanServiceCreation _deliveryChallanService;
        private readonly ILogger<DeliveryChallanControllerCreation> _logger;

        public DeliveryChallanControllerCreation(DeliveryChallanServiceCreation deliveryChallanService, ILogger<DeliveryChallanControllerCreation> logger)
        {
            _deliveryChallanService = deliveryChallanService;
            _logger = logger;
        }


        // GET: api/deliverychallan
        // Fetches all delivery challans
        [HttpGet]
        public async Task<IActionResult> GetDeliveryChallans()
        {
            _logger.LogInformation("Fetching all delivery challans");
            var challans = await _deliveryChallanService.GetDeliveryChallansAsync();
            return Ok(challans);
        }


        // POST: api/deliverychallan
        // Creates a new delivery challan
        [HttpPost]
        public async Task<IActionResult> CreateDeliveryChallan([FromBody] DeliveryChallanHeaderCreationDto dto)
        {
            _logger.LogInformation("Received DTO: {@Dto}", dto);
            var newChallan = await _deliveryChallanService.CreateDeliveryChallanAsync(dto);
            return CreatedAtAction(nameof(GetDeliveryChallan), new { id = newChallan.Id }, newChallan);
        }


        // PUT: api/deliverychallan/{id}
        // Updates an existing delivery challan
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryChallan(Guid id, [FromBody] DeliveryChallanHeaderCreationDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID mismatch in update request for delivery challan");
                return BadRequest("ID mismatch");
            }

            try
            {
                _logger.LogInformation("PUT request to update delivery challan with Id: {Id}", id);
                var updatedChallan = await _deliveryChallanService.CreateDeliveryChallanAsync(dto); // Reuse Create method for update
                return Ok(updatedChallan);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to update delivery challan: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery challan");
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/deliverychallan/{id}
        // Fetches a specific delivery challan
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryChallan(Guid id)
        {
            _logger.LogInformation("Fetching delivery challan with Id: {Id}", id);
            var challan = await _deliveryChallanService.GetDeliveryChallanAsync(id);
            if (challan == null)
            {
                _logger.LogWarning("Delivery challan with Id {Id} not found", id);
                return NotFound("Delivery challan not found");
            }
            return Ok(challan);
        }


        // DELETE: api/deliverychallan/{id}
        // Deletes a delivery challan
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryChallan(Guid id)
        {
            _logger.LogInformation("DELETE request for delivery challan with Id: {Id}", id);
            bool result = await _deliveryChallanService.DeleteDeliveryChallanAsync(id);
            if (result)
            {
                return Ok("Delivery challan was deleted");
            }
            _logger.LogWarning("Delivery challan with Id {Id} not found", id);
            return NotFound("Delivery challan not found");
        }


        // GET: api/deliverychallan/{id}/qr
        // Generates a QR code for a specific delivery challan
        [HttpGet("{id}/qr")]
        public IActionResult GetDeliveryChallanQr(Guid id)
        {
            _logger.LogInformation("Generating QR code for delivery challan with Id: {Id}", id);
            var url = $"http://localhost:5500/delivery-challan-creation.html?id={id}"; // Link to this page

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

            // Use PngByteQRCode for cross-platform compatibility
            var base64Qr = new PngByteQRCode(qrCodeData).GetGraphic(20);

            return File(base64Qr, "image/png");
        }
    }
}