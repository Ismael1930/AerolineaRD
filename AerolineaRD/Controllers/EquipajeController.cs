using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipajeController : ControllerBase
    {
        private readonly IEquipajeService _equipajeService;

        public EquipajeController(IEquipajeService equipajeService)
        {
            _equipajeService = equipajeService;
        }

        /// <summary>
        /// Registrar equipaje para un pasajero
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegistrarEquipaje([FromBody] CrearEquipajeDto dto)
        {
            try
            {
                var equipaje = await _equipajeService.RegistrarEquipajeAsync(dto);
                return Ok(new { success = true, data = equipaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener equipajes de un pasajero
        /// </summary>
        [HttpGet("pasajero/{idPasajero}")]
        public async Task<IActionResult> ObtenerEquipajesPorPasajero(int idPasajero)
        {
            try
            {
                var equipajes = await _equipajeService.ObtenerEquipajesPorPasajeroAsync(idPasajero);
                return Ok(new { success = true, data = equipajes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}