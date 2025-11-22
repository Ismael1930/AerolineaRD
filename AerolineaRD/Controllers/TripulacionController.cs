using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
                return Ok(new { success = true, data = tripulacion, message = "Tripulación creada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener toda la tripulación
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var tripulacion = await _tripulacionService.ObtenerTodasAsync();
                return Ok(new { success = true, data = tripulacion });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener tripulación por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var tripulacion = await _tripulacionService.ObtenerPorIdAsync(id);
                if (tripulacion == null)
                    return NotFound(new { success = false, message = "Tripulación no encontrada" });

                return Ok(new { success = true, data = tripulacion });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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

        /// <summary>
        /// Actualizar un miembro de tripulación
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ActualizarTripulacion([FromBody] TripulacionDto dto)
        {
            try
            {
                var tripulacion = await _tripulacionService.ActualizarTripulacionAsync(dto);
                return Ok(new { success = true, data = tripulacion, message = "Tripulación actualizada exitosamente" });
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

        /// <summary>
        /// Eliminar un miembro de tripulación
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTripulacion(int id)
        {
            try
            {
                var resultado = await _tripulacionService.EliminarTripulacionAsync(id);
                if (!resultado)
                    return NotFound(new { success = false, message = "Tripulación no encontrada" });

                return Ok(new { success = true, message = "Tripulación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}