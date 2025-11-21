using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasajeroController : ControllerBase
    {
        private readonly IPasajeroService _pasajeroService;

        public PasajeroController(IPasajeroService pasajeroService)
        {
            _pasajeroService = pasajeroService;
        }

        /// <summary>
        /// Crear un nuevo pasajero
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearPasajero([FromBody] CrearPasajeroDto dto)
        {
            try
            {
                var pasajero = await _pasajeroService.CrearPasajeroAsync(dto);
                return Ok(new { success = true, data = pasajero });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener pasajero por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPasajero(int id)
        {
            try
            {
                var pasajero = await _pasajeroService.ObtenerPorIdAsync(id);
                if (pasajero == null)
                    return NotFound(new { success = false, message = "Pasajero no encontrado" });

                return Ok(new { success = true, data = pasajero });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los pasajeros
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var pasajeros = await _pasajeroService.ObtenerTodosAsync();
                return Ok(new { success = true, data = pasajeros });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}