using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RutaController : ControllerBase
    {
        private readonly IRutaService _rutaService;

        public RutaController(IRutaService rutaService)
        {
            _rutaService = rutaService;
        }

        /// <summary>
        /// Obtiene la duración estimada, hora de llegada y precio sugerido para una ruta
        /// Este endpoint es usado por el frontend para autocompletar campos al crear/editar vuelos
        /// </summary>
        /// <param name="origen">Código del aeropuerto de origen (ej: SDQ)</param>
        /// <param name="destino">Código del aeropuerto de destino (ej: JFK)</param>
        /// <param name="horaSalida">Hora de salida en formato HH:mm (ej: 10:30) - opcional</param>
        /// <example>
        /// GET /api/Ruta/duracion?origen=SDQ&amp;destino=JFK
        /// GET /api/Ruta/duracion?origen=SDQ&amp;destino=JFK&amp;horaSalida=10:30
        /// </example>
        [HttpGet("duracion")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDuracion(
            [FromQuery] string origen, 
            [FromQuery] string destino,
            [FromQuery] string? horaSalida = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Debe especificar origen y destino" 
                    });
                }

                // Parsear hora de salida si se proporcionó
                TimeSpan? horaSalidaParsed = null;
                if (!string.IsNullOrWhiteSpace(horaSalida))
                {
                    if (TimeSpan.TryParse(horaSalida, out var parsed))
                    {
                        horaSalidaParsed = parsed;
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Formato de hora inválido. Use HH:mm (ej: 10:30)"
                        });
                    }
                }

                var resultado = await _rutaService.ObtenerInfoRutaCompletaAsync(
                    origen.ToUpper(), 
                    destino.ToUpper(), 
                    horaSalidaParsed);

                return Ok(new
                {
                    success = true,
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las rutas activas
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var rutas = await _rutaService.ObtenerTodasLasRutasAsync();
                return Ok(new { success = true, data = rutas, total = rutas.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las rutas disponibles desde un aeropuerto de origen
        /// </summary>
        [HttpGet("desde/{origenCodigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerDesdeOrigen(string origenCodigo)
        {
            try
            {
                var rutas = await _rutaService.ObtenerRutasDesdeOrigenAsync(origenCodigo.ToUpper());
                return Ok(new { success = true, data = rutas, total = rutas.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las rutas disponibles hacia un aeropuerto de destino
        /// </summary>
        [HttpGet("hacia/{destinoCodigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerHaciaDestino(string destinoCodigo)
        {
            try
            {
                var rutas = await _rutaService.ObtenerRutasHaciaDestinoAsync(destinoCodigo.ToUpper());
                return Ok(new { success = true, data = rutas, total = rutas.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva ruta (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Crear([FromBody] CrearRutaDto dto)
        {
            try
            {
                var ruta = await _rutaService.CrearRutaAsync(dto);
                return Ok(new { 
                    success = true, 
                    data = ruta, 
                    message = $"Ruta {dto.OrigenCodigo} ? {dto.DestinoCodigo} creada exitosamente" 
                });
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
        /// Actualiza una ruta existente (Admin)
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarRutaDto dto)
        {
            try
            {
                var ruta = await _rutaService.ActualizarRutaAsync(dto);
                if (ruta == null)
                    return NotFound(new { success = false, message = "Ruta no encontrada" });

                return Ok(new { 
                    success = true, 
                    data = ruta, 
                    message = "Ruta actualizada exitosamente" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina (desactiva) una ruta (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _rutaService.EliminarRutaAsync(id);
                if (!resultado)
                    return NotFound(new { success = false, message = "Ruta no encontrada" });

                return Ok(new { success = true, message = "Ruta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
