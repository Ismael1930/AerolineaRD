using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Obtener todos los clientes
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Operador")]
        public async Task<ActionResult<List<ClienteDto>>> ObtenerTodos()
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Obtener cliente por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ClienteDto>> ObtenerPorId(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Obtener cliente con sus reservas
        /// </summary>
        [HttpGet("{id}/reservas")]
        [Authorize]
        public async Task<ActionResult<ClienteConReservasDto>> ObtenerConReservas(int id)
        {
            var cliente = await _clienteService.ObtenerClienteConReservasAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Obtener cliente por UserId
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<ClienteDto>> ObtenerPorUserId(string userId)
        {
            var cliente = await _clienteService.ObtenerPorUserIdAsync(userId);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Obtener cliente por email
        /// </summary>
        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<ClienteDto>> ObtenerPorEmail(string email)
        {
            var cliente = await _clienteService.ObtenerPorEmailAsync(email);
            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Crear un nuevo cliente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Crear([FromBody] CrearClienteDto dto)
        {
            try
            {
                var cliente = await _clienteService.CrearAsync(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un cliente existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ClienteDto>> Actualizar(int id, [FromBody] ActualizarClienteDto dto)
        {
            try
            {
                var cliente = await _clienteService.ActualizarAsync(id, dto);
                return Ok(cliente);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar un cliente
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var resultado = await _clienteService.EliminarAsync(id);
            if (!resultado)
                return NotFound(new { message = "Cliente no encontrado" });

            return NoContent();
        }
    }
}