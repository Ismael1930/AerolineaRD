using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class VueloAdminController : ControllerBase
    {
        private readonly IVueloAdminService _vueloAdminService;

        public VueloAdminController(IVueloAdminService vueloAdminService)
        {
            _vueloAdminService = vueloAdminService;
        }

        /// <summary>
        /// Crear un nuevo vuelo (Admin)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CrearVuelo([FromBody] CrearVueloDto dto)
        {
            try
            {
                var vuelo = await _vueloAdminService.CrearVueloAsync(dto);
                return Ok(new { success = true, data = vuelo, message = "Vuelo creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los vuelos (Admin)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var vuelos = await _vueloAdminService.ObtenerTodosLosVuelosAsync();
                return Ok(new { success = true, data = vuelos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener detalles completos de un vuelo
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerVueloDetalle(int id)
        {
            try
            {
                var vuelo = await _vueloAdminService.ObtenerVueloDetalleAsync(id);
                if (vuelo == null)
                    return NotFound(new { success = false, message = "Vuelo no encontrado" });

                return Ok(new { success = true, data = vuelo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un vuelo
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ActualizarVuelo([FromBody] ActualizarVueloDto dto)
        {
            try
            {
                var vuelo = await _vueloAdminService.ActualizarVueloAsync(dto);
                return Ok(new { success = true, data = vuelo, message = "Vuelo actualizado exitosamente" });
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
        /// Eliminar un vuelo
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVuelo(int id)
        {
            try
            {
                var resultado = await _vueloAdminService.EliminarVueloAsync(id);
                if (!resultado)
                    return NotFound(new { success = false, message = "Vuelo no encontrado" });

                return Ok(new { success = true, message = "Vuelo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}