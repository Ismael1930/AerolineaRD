using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EquipoController : ControllerBase
    {
        private readonly IEquipoService _equipoService;

        public EquipoController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        /// <summary>
        /// Obtener todos los equipos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var equipos = await _equipoService.ObtenerTodosEquiposAsync();
                return Ok(new { success = true, data = equipos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener equipos disponibles
        /// </summary>
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var equipos = await _equipoService.ObtenerEquiposDisponiblesAsync();
                return Ok(new { success = true, data = equipos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener equipo por ID con detalle completo
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var equipo = await _equipoService.ObtenerEquipoPorIdAsync(id);
                if (equipo == null)
                    return NotFound(new { success = false, message = "Equipo no encontrado" });

                return Ok(new { success = true, data = equipo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Crear nuevo equipo
        /// </summary>
        /// <remarks>
        /// El equipo debe tener:
        /// - 1 Piloto
        /// - 1 Copiloto
        /// - 1 Sobrecargo Jefe
        /// - 3 a 6 Sobrecargos
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearEquipoDto dto)
        {
            try
            {
                var resultado = await _equipoService.CrearEquipoAsync(dto);

                if (resultado.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = resultado.Data,
                        message = resultado.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = resultado.Message,
                    errors = resultado.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor: " + ex.Message });
            }
        }

        /// <summary>
        /// Actualizar equipo
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarEquipoDto dto)
        {
            try
            {
                var resultado = await _equipoService.ActualizarEquipoAsync(dto);

                if (resultado.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = resultado.Data,
                        message = resultado.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = resultado.Message,
                    errors = resultado.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor: " + ex.Message });
            }
        }

        /// <summary>
        /// Eliminar equipo (solo si no tiene asignación activa)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _equipoService.EliminarEquipoAsync(id);
                if (!resultado)
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se puede eliminar el equipo. Puede que no exista o tenga una asignación activa"
                    });

                return Ok(new { success = true, message = "Equipo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Validar composición de un equipo
        /// </summary>
        [HttpPost("validar")]
        public async Task<IActionResult> ValidarComposicion([FromBody] List<int> idsPersonal)
        {
            try
            {
                var resultado = await _equipoService.ValidarComposicionEquipoAsync(idsPersonal);
                return Ok(new { success = true, data = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Marcar equipo como "En Servicio"
        /// </summary>
        [HttpPost("{id}/en-servicio")]
        public async Task<IActionResult> MarcarEnServicio(int id)
        {
            try
            {
                var resultado = await _equipoService.MarcarEquipoEnServicioAsync(id);

                if (resultado.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = resultado.Data,
                        message = resultado.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = resultado.Message,
                    errors = resultado.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Marcar equipo en descanso después de un vuelo
        /// </summary>
        [HttpPost("{id}/descanso")]
        public async Task<IActionResult> MarcarEnDescanso(int id, [FromBody] DateTime finVuelo)
        {
            try
            {
                var resultado = await _equipoService.MarcarEquipoEnDescansoAsync(id, finVuelo);

                if (resultado.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        data = resultado.Data,
                        message = resultado.Message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = resultado.Message,
                    errors = resultado.Errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar estados de equipos (pasar de Descanso a Disponible automáticamente)
        /// </summary>
        [HttpPost("actualizar-estados")]
        public async Task<IActionResult> ActualizarEstados()
        {
            try
            {
                await _equipoService.ActualizarEstadosEquiposAsync();
                return Ok(new { success = true, message = "Estados actualizados exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
