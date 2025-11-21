using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class TripulacionController : ControllerBase
    {
        private readonly ITripulacionService _tripulacionService;

        public TripulacionController(ITripulacionService tripulacionService)
        {
            _tripulacionService = tripulacionService;
        }

        /// <summary>
        /// Crear un miembro de tripulación
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearTripulacion([FromBody] CrearTripulacionDto dto)
        {
            try
            {
                var tripulacion = await _tripulacionService.CrearTripulacionAsync(dto);
                return Ok(new { success = true, data = tripulacion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener tripulación por rol
        /// </summary>
        [HttpGet("rol/{rol}")]
        public async Task<IActionResult> ObtenerPorRol(string rol)
        {
            try
            {
                var tripulacion = await _tripulacionService.ObtenerPorRolAsync(rol);
                return Ok(new { success = true, data = tripulacion });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}