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
   /// Obtener reporte de capacidad de un aeropuerto específico
  /// </summary>
 /// <param name="codigo">Código del aeropuerto (ej: SDQ, JFK)</param>
        /// <param name="fecha">Fecha para consultar (formato: YYYY-MM-DD). Si no se especifica, usa hoy</param>
        [HttpGet("{codigo}/capacidad")]
        [Authorize(Roles = "Admin")]
 public async Task<IActionResult> ObtenerCapacidadAeropuerto(
      string codigo, 
     [FromQuery] DateTime? fecha = null)
        {
   try
 {
     var fechaConsulta = fecha ?? DateTime.Today;
    var capacidad = await _aeropuertoService.ObtenerCapacidadAeropuertoAsync(codigo, fechaConsulta);
      
    return Ok(new 
       {
 success = true,
          data = capacidad,
  message = $"Reporte de capacidad del aeropuerto {capacidad.Nombre} para {fechaConsulta:dd/MM/yyyy}"
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
  /// Obtener reporte de capacidad de todos los aeropuertos
        /// </summary>
  /// <param name="fecha">Fecha para consultar (formato: YYYY-MM-DD). Si no se especifica, usa hoy</param>
  [HttpGet("capacidad/reporte")]
 [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ObtenerReporteCapacidadTodos([FromQuery] DateTime? fecha = null)
 {
      try
 {
  var fechaConsulta = fecha ?? DateTime.Today;
   var reporte = await _aeropuertoService.ObtenerReporteCapacidadTodosAsync(fechaConsulta);
         
    return Ok(new 
         {
      success = true,
     data = reporte,
 message = $"Reporte de capacidad de {reporte.TotalAeropuertos} aeropuertos para {fechaConsulta:dd/MM/yyyy}"
   });
}
        catch (Exception ex)
{
   return StatusCode(500, new { success = false, message = ex.Message });
   }
 }
    }
}