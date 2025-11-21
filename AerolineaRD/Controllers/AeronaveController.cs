using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class AeronaveController : ControllerBase
    {
        private readonly IAeronaveService _aeronaveService;

        public AeronaveController(IAeronaveService aeronaveService)
        {
            _aeronaveService = aeronaveService;
        }

        /// <summary>
        /// Crear una nueva aeronave
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearAeronave([FromBody] CrearAeronaveDto dto)
        {
            try
            {
                var aeronave = await _aeronaveService.CrearAeronaveAsync(dto);
                return Ok(new { success = true, data = aeronave });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener aeronaves disponibles
        /// </summary>
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var aeronaves = await _aeronaveService.ObtenerAeronavesDisponiblesAsync();
                return Ok(new { success = true, data = aeronaves });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}