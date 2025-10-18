
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Services;
using DotNetWbapi.Dtos;
using QRCoder;

namespace DotNetWbapi.Controllers
{
    // declare the API controller with route prefix "api/gatepass"
    [ApiController]
    [Route("api/gatepass")]
    public class GatepassController : ControllerBase
    {
        private readonly GatepassService _gatepassService;
        private readonly ILogger<GatepassController> _logger;

        public GatepassController(GatepassService gatepassService, ILogger<GatepassController> logger)
        {
            _gatepassService = gatepassService;
            _logger = logger;
        }


        // GET: api/gatepass
        // Fetches all gatepasses
        [HttpGet]
        public async Task<IActionResult> GetGatepasses()
        {
            _logger.LogInformation("Fetching all gatepasses");
            var gatepasses = await _gatepassService.GetAllGatepassesAsync();
            return Ok(gatepasses);
        }


        // GET: api/gatepass/{id}
        // Fetches a specific gatepass
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGatepass(Guid id)
        {
            _logger.LogInformation("Fetching gatepass with Id: {Id}", id);
            var gatepass = await _gatepassService.GetGatepassByIdAsync(id);
            Console.WriteLine("Gatepass: " + gatepass);
            if (gatepass == null)
            {
                return NotFound();
            }

            return Ok(gatepass);
        }


        // POST: api/gatepass
        // Creates a new gatepass
        [HttpPost]
        public async Task<IActionResult> CreateGatepass([FromBody] GatepassHeaderDto dto)
        {
            try
            {
                _logger.LogInformation("POST request to create gatepass with GatePassNo: {GatePassNo}", dto.GatePassNo);
                var newGatepass = await _gatepassService.CreateGatepassAsync(dto);
                _logger.LogInformation("Created gatepass with Id: {Id}", newGatepass.Id);
                return CreatedAtAction(nameof(GetGatepass), new { id = newGatepass.Id }, newGatepass);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create gatepass: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating gatepass");
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/gatepass/{id}
        // Updates an existing gatepass
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGatepass(Guid id, [FromBody] GatepassHeaderDto dto)
        {
            try
            {
                _logger.LogInformation("PUT request to update gatepass with Id: {Id}", id);
                dto.Id = id;
                var updatedGatepass = await _gatepassService.CreateGatepassAsync(dto);
                _logger.LogInformation("Updated gatepass with Id: {Id}", updatedGatepass.Id);
                return Ok(updatedGatepass);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to update gatepass: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating gatepass");
                return StatusCode(500, "Internal server error");
            }
        }


        // DELETE: api/gatepass/{id}
        // Deletes a gatepass
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGatepass(Guid id)
        {
            try
            {
                _logger.LogInformation("DELETE request for gatepass with Id: {Id}", id);
                var result = await _gatepassService.DeleteGatepassAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                _logger.LogInformation("Deleted gatepass with Id: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting gatepass");
                return StatusCode(500, "Internal server error");
            }
        }


        //     [HttpGet("{id}/qr")]
        //     public IActionResult GetGatepassQr(Guid id)
        //     {
        //         _logger.LogInformation("Generating QR code for gatepass with Id: {Id}", id);
        //         // URL to the gatepass details page
        //         var url = $"http://localhost:5140/GetpassQR/getpassQRinfo.html?id={id}";

        //         var qrGenerator = new QRCodeGenerator();
        //         var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        //         // Generate QR code as PNG
        //         var qrCode = new PngByteQRCode(qrCodeData);
        //         var qrCodeBytes = qrCode.GetGraphic(20);

        //         return File(qrCodeBytes, "image/png");
        //     }
        // }


        // GET: api/gatepass/{id}/qr
        // Generates a QR code for a specific gatepass
        [HttpGet("{id}/qr")]
        public IActionResult GetGatepassQr(Guid id)
        {
            _logger.LogInformation("Generating QR code for gatepass with Id: {Id}", id);

            // Dynamically get base URL (scheme + host)
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Generate full URL dynamically
            var url = $"{baseUrl}/GetpassQR/getpassQRinfo.html?id={id}";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            return File(qrCodeBytes, "image/png");
        }



        // POST: api/gatepass/qr/response
        // Handles the response from the QR code scanner to update outdate and outtime
        [HttpPost("qr/response")]  
        public async Task<IActionResult> SyncGatepassQr([FromBody] List<Guid> ids)
        {
            _logger.LogInformation("Syncing QR for {Count} gatepasses", ids.Count);

            bool success = await _gatepassService.SyncGatepassAsync(ids);

            if (success)
            {
                return Ok(new { message = "OutDate and OutTime updated successfully." });
            }
            else
            {
                return NotFound(new { message = "No gatepasses found or update failed." });
            }
        }
    }
}