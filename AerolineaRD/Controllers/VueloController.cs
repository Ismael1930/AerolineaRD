using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VueloController : ControllerBase
    {
        private readonly IVueloService _vueloService;

        public VueloController(IVueloService vueloService)
        {
            _vueloService = vueloService;
        }

        /// <summary>
        /// Buscar vuelos con filtros opcionales.
        /// Devuelve cada vuelo una sola vez con todas las clases disponibles.
        /// La selección de clase se realiza posteriormente en el formulario de reserva.
        /// </summary>
        [HttpPost("buscar")]
        public async Task<IActionResult> BuscarVuelos([FromBody] BuscarVueloDto filtros)
        {
            try
            {
                // BuscarVuelosAsync devuelve vuelos únicos con todas las clases disponibles
                var vuelos = await _vueloService.BuscarVuelosAsync(filtros);

                return Ok(new
                {
                    success = true,
                    data = vuelos,
                    count = vuelos.Count,
                    message = vuelos.Count == 0
                    ? "No se encontraron vuelos con los criterios especificados"
                    : $"Se encontraron {vuelos.Count} vuelo(s)"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener detalles completos de un vuelo específico
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerVuelo(int id)
        {
            try
            {
                var vuelo = await _vueloService.ObtenerVueloPorIdAsync(id);
                if (vuelo == null)
                    return NotFound(new { success = false, message = "Vuelo no encontrado" });

                return Ok(new { success = true, data = vuelo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener asientos disponibles de un vuelo para una clase específica
        /// </summary>
        [HttpGet("{idVuelo}/asientos/{clase}")]
        public async Task<IActionResult> ObtenerAsientosDisponibles(int idVuelo, string clase)
        {
            try
            {
                var asientos = await _vueloService.ObtenerAsientosDisponiblesAsync(idVuelo, clase);

                return Ok(new
                {
                    success = true,
                    data = asientos,
                    total = asientos.Count,
                    disponibles = asientos.Count(a => a.Disponible),
                    ocupados = asientos.Count(a => !a.Disponible)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}