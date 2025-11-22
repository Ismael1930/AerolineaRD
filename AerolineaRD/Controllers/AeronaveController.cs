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
        /// Obtener aeronaves disponibles
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
    }
}