using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AeronaveController : ControllerBase
    {
        private readonly IAeronaveService _aeronaveService;

        public AeronaveController(IAeronaveService aeronaveService)
        {
            _aeronaveService = aeronaveService;
        }

        /// <summary>
        /// Crear una nueva aeronave
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearAeronave([FromBody] CrearAeronaveDto dto)
        {
            try
            {
                var aeronave = await _aeronaveService.CrearAeronaveAsync(dto);
                return Ok(new { success = true, data = aeronave, message = "Aeronave creada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todas las aeronaves
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var aeronaves = await _aeronaveService.ObtenerTodasAsync();
                return Ok(new { success = true, data = aeronaves });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener aeronave por matrícula
        /// </summary>
        [HttpGet("{matricula}")]
        public async Task<IActionResult> ObtenerPorMatricula(string matricula)
        {
            try
            {
                var aeronave = await _aeronaveService.ObtenerPorMatriculaAsync(matricula);
                if (aeronave == null)
                    return NotFound(new { success = false, message = "Aeronave no encontrada" });

                return Ok(new { success = true, data = aeronave });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener aeronaves disponibles (operativas)
        /// </summary>
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var aeronaves = await _aeronaveService.ObtenerAeronavesDisponiblesAsync();
                return Ok(new { success = true, data = aeronaves });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener aeronaves disponibles para un horario específico
        /// Filtra por: operativas, con equipo asignado, y sin conflictos de horario
        /// </summary>
        /// <param name="fecha">Fecha del vuelo (formato: yyyy-MM-dd)</param>
        /// <param name="horaSalida">Hora de salida (formato: HH:mm)</param>
        /// <param name="horaLlegada">Hora de llegada (formato: HH:mm)</param>
        /// <param name="vueloId">ID del vuelo a excluir (para edición, opcional)</param>
        /// <example>
        /// GET /api/Aeronave/disponibles-horario?fecha=2025-01-20&amp;horaSalida=10:30&amp;horaLlegada=14:45
        /// GET /api/Aeronave/disponibles-horario?fecha=2025-01-20&amp;horaSalida=22:15&amp;horaLlegada=00:45 (vuelo nocturno)
        /// GET /api/Aeronave/disponibles-horario?fecha=2025-01-20&amp;horaSalida=10:30&amp;horaLlegada=14:45&amp;vueloId=5
        /// </example>
        [HttpGet("disponibles-horario")]
        public async Task<IActionResult> ObtenerDisponiblesParaHorario(
            [FromQuery] DateTime fecha,
            [FromQuery] string horaSalida,
            [FromQuery] string horaLlegada,
            [FromQuery] int? vueloId = null)
        {
            try
            {
                // Validar parámetros
                if (string.IsNullOrWhiteSpace(horaSalida) || string.IsNullOrWhiteSpace(horaLlegada))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Debe especificar horaSalida y horaLlegada en formato HH:mm"
                    });
                }

                // Parsear horas
                if (!TimeSpan.TryParse(horaSalida, out var horaSalidaParsed))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Formato de horaSalida inválido. Use HH:mm (ej: 10:30)"
                    });
                }

                if (!TimeSpan.TryParse(horaLlegada, out var horaLlegadaParsed))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Formato de horaLlegada inválido. Use HH:mm (ej: 14:45)"
                    });
                }

                // Nota: NO validamos que horaLlegada > horaSalida porque los vuelos pueden
                // cruzar la medianoche (ej: salida 22:15, llegada 00:45 del día siguiente)

                var resultado = await _aeronaveService.ObtenerAeronavesDisponiblesParaHorarioAsync(
                    fecha,
                    horaSalidaParsed,
                    horaLlegadaParsed,
                    vueloId);

                return Ok(new
                {
                    success = true,
                    data = resultado,
                    message = resultado.Disponibles.Count > 0
                        ? $"Se encontraron {resultado.Disponibles.Count} aeronave(s) disponible(s)"
                        : "No hay aeronaves disponibles para el horario solicitado"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar una aeronave
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ActualizarAeronave([FromBody] ActualizarAeronaveDto dto)
        {
            try
            {
                var aeronave = await _aeronaveService.ActualizarAeronaveAsync(dto);
                return Ok(new { success = true, data = aeronave, message = "Aeronave actualizada exitosamente" });
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
        /// Eliminar una aeronave
        /// </summary>
        [HttpDelete("{matricula}")]
        public async Task<IActionResult> EliminarAeronave(string matricula)
        {
            try
            {
                var resultado = await _aeronaveService.EliminarAeronaveAsync(matricula);
                if (!resultado)
                    return NotFound(new { success = false, message = "Aeronave no encontrada" });

                return Ok(new { success = true, message = "Aeronave eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todas las aeronaves con información de disponibilidad de asientos
        /// Incluye cálculo de asientos reservados y disponibles por clase
        /// </summary>
        [HttpGet("disponibilidad")]
        public async Task<IActionResult> ObtenerTodasConDisponibilidad()
        {
            try
            {
                var aeronaves = await _aeronaveService.ObtenerTodasConDisponibilidadAsync();
                return Ok(new
                {
                    success = true,
                    data = aeronaves,
                    message = $"Se encontraron {aeronaves.Count} aeronave(s) con información de disponibilidad"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener una aeronave específica con información de disponibilidad de asientos
        /// </summary>
        [HttpGet("{matricula}/disponibilidad")]
        public async Task<IActionResult> ObtenerConDisponibilidad(string matricula)
        {
            try
            {
                var aeronave = await _aeronaveService.ObtenerConDisponibilidadAsync(matricula);
                if (aeronave == null)
                    return NotFound(new { success = false, message = "Aeronave no encontrada" });

                return Ok(new { success = true, data = aeronave });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}