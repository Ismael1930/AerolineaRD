using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        /// <summary>
        /// Obtener notificaciones de un cliente
        /// </summary>
        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> ObtenerNotificacionesPorCliente(int idCliente)
        {
            try
            {
                var notificaciones = await _notificacionService.ObtenerNotificacionesPorClienteAsync(idCliente);
                return Ok(new { success = true, data = notificaciones });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Marcar notificación como leída
        /// </summary>
        [HttpPut("marcar-leida")]
        public async Task<IActionResult> MarcarComoLeida([FromBody] MarcarNotificacionLeidaDto dto)
        {
            try
            {
                var resultado = await _notificacionService.MarcarComoLeidaAsync(dto.IdNotificacion);
                if (!resultado)
                    return NotFound(new { success = false, message = "Notificación no encontrada" });

                return Ok(new { success = true, message = "Notificación marcada como leída" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}