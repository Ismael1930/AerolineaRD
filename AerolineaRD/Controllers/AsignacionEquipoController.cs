using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AsignacionEquipoController : ControllerBase
    {
        private readonly IEquipoService _equipoService;

        public AsignacionEquipoController(IEquipoService equipoService)
      {
            _equipoService = equipoService;
   }

    /// <summary>
 /// Obtener todas las asignaciones de equipos a aeronaves
  /// </summary>
   [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
 {
      try
 {
var asignaciones = await _equipoService.ObtenerTodasAsignacionesAsync();
return Ok(new { success = true, data = asignaciones });
            }
     catch (Exception ex)
      {
          return StatusCode(500, new { success = false, message = ex.Message });
      }
        }

 /// <summary>
        /// Obtener asignación activa de una aeronave
  /// </summary>
        [HttpGet("aeronave/{matricula}")]
        public async Task<IActionResult> ObtenerPorAeronave(string matricula)
  {
            try
   {
  var asignacion = await _equipoService.ObtenerAsignacionActivaPorAeronaveAsync(matricula);
      
     if (asignacion == null)
      return NotFound(new 
      { 
           success = false, 
    message = $"No hay equipo asignado a la aeronave '{matricula}'" 
         });

    return Ok(new { success = true, data = asignacion });
     }
 catch (Exception ex)
            {
   return StatusCode(500, new { success = false, message = ex.Message });
   }
   }

 /// <summary>
        /// Obtener asignación activa de un equipo
        /// </summary>
   [HttpGet("equipo/{idEquipo}")]
        public async Task<IActionResult> ObtenerPorEquipo(int idEquipo)
 {
     try
  {
var asignacion = await _equipoService.ObtenerAsignacionActivaPorEquipoAsync(idEquipo);
      
        if (asignacion == null)
                 return NotFound(new 
     { 
       success = false, 
   message = $"El equipo {idEquipo} no tiene asignación activa" 
  });

     return Ok(new { success = true, data = asignacion });
 }
            catch (Exception ex)
  {
             return StatusCode(500, new { success = false, message = ex.Message });
   }
        }

        /// <summary>
   /// Asignar un equipo a una aeronave
/// </summary>
        /// <remarks>
        /// Validaciones:
/// - El equipo debe estar completo (1 piloto, 1 copiloto, 1 sobrecargo jefe, 3-6 sobrecargos)
        /// - El equipo debe estar disponible
        /// - La aeronave debe estar operativa
 /// - La aeronave no debe tener otro equipo asignado
        /// - El equipo no debe estar asignado a otra aeronave
        /// - El piloto y copiloto deben tener certificación para el modelo de aeronave
        /// </remarks>
     [HttpPost("asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarEquipoAeronaveDto dto)
 {
      try
         {
        var resultado = await _equipoService.AsignarEquipoAeronaveAsync(dto);

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
   return StatusCode(500, new 
{ 
         success = false, 
    message = "Error interno del servidor: " + ex.Message 
       });
     }
        }

    /// <summary>
   /// Desasignar un equipo de una aeronave
        /// </summary>
        [HttpPost("desasignar")]
   public async Task<IActionResult> Desasignar([FromBody] DesasignarEquipoDto dto)
{
   try
     {
         var resultado = await _equipoService.DesasignarEquipoAeronaveAsync(dto);

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
return StatusCode(500, new 
    { 
       success = false, 
    message = "Error interno del servidor: " + ex.Message 
  });
       }
      }

    /// <summary>
        /// Obtener resumen de asignaciones (estadísticas)
        /// </summary>
        [HttpGet("resumen")]
        public async Task<IActionResult> ObtenerResumen()
        {
 try
   {
      var asignaciones = await _equipoService.ObtenerTodasAsignacionesAsync();
    
   var resumen = new
                {
         totalAsignaciones = asignaciones.Count,
    asignacionesActivas = asignaciones.Count(a => a.Activa),
asignacionesInactivas = asignaciones.Count(a => !a.Activa),
       aeronavesConEquipo = asignaciones.Where(a => a.Activa).Select(a => a.Matricula).Distinct().Count(),
       equiposAsignados = asignaciones.Where(a => a.Activa).Select(a => a.IdEquipo).Distinct().Count()
      };

   return Ok(new { success = true, data = resumen });
  }
         catch (Exception ex)
     {
     return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener aeronaves sin equipo asignado
        /// </summary>
 [HttpGet("aeronaves-sin-equipo")]
        public async Task<IActionResult> ObtenerAeronavesSinEquipo()
   {
     try
      {
      // Esta funcionalidad requiere IAeronaveService
       // Por ahora retornamos mensaje informativo
       return Ok(new 
  { 
      success = true, 
 message = "Implementar con servicio de aeronaves",
       data = new List<object>()
});
 }
        catch (Exception ex)
  {
   return StatusCode(500, new { success = false, message = ex.Message });
        }
 }

  /// <summary>
        /// Obtener equipos sin asignación
  /// </summary>
  [HttpGet("equipos-sin-asignacion")]
public async Task<IActionResult> ObtenerEquiposSinAsignacion()
        {
  try
   {
         var equiposDisponibles = await _equipoService.ObtenerEquiposDisponiblesAsync();
          var asignacionesActivas = await _equipoService.ObtenerTodasAsignacionesAsync();
                
     var idsEquiposAsignados = asignacionesActivas
        .Where(a => a.Activa)
           .Select(a => a.IdEquipo)
   .ToHashSet();

       var equiposSinAsignacion = equiposDisponibles
         .Where(e => !idsEquiposAsignados.Contains(e.Id))
     .ToList();

    return Ok(new { success = true, data = equiposSinAsignacion });
            }
catch (Exception ex)
            {
   return StatusCode(500, new { success = false, message = ex.Message });
         }
   }
    }
}
