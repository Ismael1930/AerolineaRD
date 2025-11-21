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
        /// Buscar vuelos con filtros
        /// </summary>
        [HttpPost("buscar")]
        public async Task<IActionResult> BuscarVuelos([FromBody] BuscarVueloDto filtros)
        {
            try
            {
                var vuelos = await _vueloService.BuscarVuelosAsync(filtros);
                return Ok(new { success = true, data = vuelos, count = vuelos.Count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener detalles de un vuelo específico
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
    }
}