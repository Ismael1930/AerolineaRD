using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReservaController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        /// <summary>
        /// Crear una nueva reserva
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearReserva([FromBody] CrearReservaDto dto)
        {
            try
            {
                var reserva = await _reservaService.CrearReservaAsync(dto);
                return Ok(new { success = true, data = reserva, message = "Reserva creada exitosamente" });
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
        /// Obtener reserva por código
        /// </summary>
        [HttpGet("{codigo}")]
        public async Task<IActionResult> ObtenerReserva(string codigo)
        {
            try
            {
                var reserva = await _reservaService.ObtenerReservaPorCodigoAsync(codigo);
                if (reserva == null)
                    return NotFound(new { success = false, message = "Reserva no encontrada" });

                return Ok(new { success = true, data = reserva });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener reservas de un cliente
        /// </summary>
        [HttpGet("cliente/{idCliente}")]
        [Authorize]
        public async Task<IActionResult> ObtenerReservasPorCliente(int idCliente)
        {
            try
            {
                var reservas = await _reservaService.ObtenerReservasPorClienteAsync(idCliente);
                return Ok(new { success = true, data = reservas, count = reservas.Count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Modificar una reserva existente
        /// </summary>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ModificarReserva([FromBody] ModificarReservaDto dto)
        {
            try
            {
                var reserva = await _reservaService.ModificarReservaAsync(dto);
                return Ok(new { success = true, data = reserva, message = "Reserva modificada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
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
        /// Cancelar una reserva
        /// </summary>
        [HttpDelete("{codigo}")]
        [Authorize]
        public async Task<IActionResult> CancelarReserva(string codigo)
        {
            try
            {
                var resultado = await _reservaService.CancelarReservaAsync(codigo);
                if (!resultado)
                    return NotFound(new { success = false, message = "Reserva no encontrada" });

                return Ok(new { success = true, message = "Reserva cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}