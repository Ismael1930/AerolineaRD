using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketSoporteController : ControllerBase
    {
        private readonly ITicketSoporteService _ticketService;

        public TicketSoporteController(ITicketSoporteService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Crear un ticket de soporte
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearTicket([FromBody] CrearTicketDto dto)
        {
            try
            {
                var ticket = await _ticketService.CrearTicketAsync(dto);
                return Ok(new { success = true, data = ticket, message = "Ticket creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener tickets de un cliente
        /// </summary>
        [HttpGet("cliente/{idCliente}")]
        [Authorize]
        public async Task<IActionResult> ObtenerTicketsPorCliente(int idCliente)
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorClienteAsync(idCliente);
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener tickets abiertos (Admin/Soporte)
        /// </summary>
        [HttpGet("abiertos")]
        [Authorize(Roles = "Administrador,Soporte")]
        public async Task<IActionResult> ObtenerTicketsAbiertos()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsAbiertosAsync();
                return Ok(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar estado de ticket (Admin/Soporte)
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Administrador,Soporte")]
        public async Task<IActionResult> ActualizarEstado([FromBody] ActualizarTicketDto dto)
        {
            try
            {
                var resultado = await _ticketService.ActualizarEstadoTicketAsync(dto);
                if (!resultado)
                    return NotFound(new { success = false, message = "Ticket no encontrado" });

                return Ok(new { success = true, message = "Estado actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}