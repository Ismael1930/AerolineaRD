using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AerolineaRD.Controllers
{
  [Route("api/[controller]")]
    [ApiController]
    public class AeropuertoController : ControllerBase
    {
        private readonly IAeropuertoService _aeropuertoService;

   public AeropuertoController(IAeropuertoService aeropuertoService)
        {
          _aeropuertoService = aeropuertoService;
        }

      /// <summary>
   /// Obtener lista de aeropuertos disponibles
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerAeropuertos()
  {
     try
   {
  var aeropuertos = await _aeropuertoService.ObtenerTodosAsync();
   return Ok(new { success = true, data = aeropuertos });
     }
         catch (Exception ex)
 {
return BadRequest(new { success = false, message = ex.Message });
         }
    }

        /// <summary>
   /// Obtener reporte de capacidad de un aeropuerto específico en un rango de fechas
  /// </summary>
 /// <param name="codigo">Código del aeropuerto (ej: SDQ, JFK, ATL)</param>
        /// <param name="fechaInicio">Fecha de inicio del rango (formato: YYYY-MM-DD). Si no se especifica, usa hoy</param>
        /// <param name="fechaFin">Fecha de fin del rango (formato: YYYY-MM-DD). Si no se especifica, usa 30 días después de fechaInicio</param>
        [HttpGet("{codigo}/capacidad")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerCapacidadAeropuerto(
            string codigo,
     [FromQuery] DateTime? fechaInicio = null,
      [FromQuery] DateTime? fechaFin = null)
  {
     try
    {
     var fechaInicioConsulta = fechaInicio ?? DateTime.Today;
    var fechaFinConsulta = fechaFin ?? fechaInicioConsulta.AddDays(30); // Por defecto, 30 días

        var capacidad = await _aeropuertoService.ObtenerCapacidadAeropuertoAsync(
          codigo, 
        fechaInicioConsulta, 
       fechaFinConsulta);

   return Ok(new
          {
          success = true,
     data = capacidad,
          message = $"Reporte de capacidad del aeropuerto {capacidad.Nombre} desde {fechaInicioConsulta:dd/MM/yyyy} hasta {fechaFinConsulta:dd/MM/yyyy}"
  });
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
  /// Obtener reporte de capacidad de todos los aeropuertos en un rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del rango (formato: YYYY-MM-DD). Si no se especifica, usa hoy</param>
        /// <param name="fechaFin">Fecha de fin del rango (formato: YYYY-MM-DD). Si no se especifica, usa 30 días después de fechaInicio</param>
        [HttpGet("capacidad/reporte")]
  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerReporteCapacidadTodos(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
     try
            {
      var fechaInicioConsulta = fechaInicio ?? DateTime.Today;
     var fechaFinConsulta = fechaFin ?? fechaInicioConsulta.AddDays(30);

    var reporte = await _aeropuertoService.ObtenerReporteCapacidadTodosAsync(
 fechaInicioConsulta, 
               fechaFinConsulta);

        return Ok(new
       {
   success = true,
       data = reporte,
message = $"Reporte de capacidad de {reporte.TotalAeropuertos} aeropuertos desde {fechaInicioConsulta:dd/MM/yyyy} hasta {fechaFinConsulta:dd/MM/yyyy}"
  });
   }
    catch (Exception ex)
        {
return StatusCode(500, new { success = false, message = ex.Message });
   }
        }
    }
}