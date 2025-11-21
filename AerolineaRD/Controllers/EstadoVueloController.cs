using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoVueloController : ControllerBase
    {
        private readonly IEstadoVueloService _estadoVueloService;

        public EstadoVueloController(IEstadoVueloService estadoVueloService)
        {
            _estadoVueloService = estadoVueloService;
        }

        /// <summary>
        /// Obtener estado actual de un vuelo
        /// </summary>
        [HttpGet("vuelo/{idVuelo}")]
        public async Task<IActionResult> ObtenerEstadoPorVuelo(int idVuelo)
        {
            try
            {
                var estado = await _estadoVueloService.ObtenerEstadoPorVueloAsync(idVuelo);
                if (estado == null)
                    return NotFound(new { success = false, message = "Estado de vuelo no encontrado" });

                return Ok(new { success = true, data = estado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar estado de un vuelo (Admin)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ActualizarEstado([FromBody] ActualizarEstadoVueloDto dto)
        {
            try
            {
                var estado = await _estadoVueloService.ActualizarEstadoAsync(dto);
                return Ok(new { success = true, data = estado, message = "Estado actualizado exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}