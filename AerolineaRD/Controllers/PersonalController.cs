using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PersonalController : ControllerBase
    {
   private readonly IEquipoService _equipoService;

  public PersonalController(IEquipoService equipoService)
   {
       _equipoService = equipoService;
        }

        /// <summary>
        /// Obtener todo el personal
 /// </summary>
        [HttpGet]
     public async Task<IActionResult> ObtenerTodo()
        {
       try
       {
          var personal = await _equipoService.ObtenerTodoPersonalAsync();
     return Ok(new { success = true, data = personal });
    }
       catch (Exception ex)
   {
      return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener personal por rol
        /// </summary>
  [HttpGet("rol/{rol}")]
  public async Task<IActionResult> ObtenerPorRol(string rol)
  {
            try
            {
      var personal = await _equipoService.ObtenerPersonalPorRolAsync(rol);
   return Ok(new { success = true, data = personal });
            }
   catch (Exception ex)
            {
     return StatusCode(500, new { success = false, message = ex.Message });
          }
 }

        /// <summary>
        /// Obtener personal disponible
        /// </summary>
 [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
    try
            {
   var personal = await _equipoService.ObtenerPersonalDisponibleAsync();
             return Ok(new { success = true, data = personal });
}
            catch (Exception ex)
            {
     return StatusCode(500, new { success = false, message = ex.Message });
   }
        }

        /// <summary>
        /// Obtener personal por ID
      /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
    try
            {
     var personal = await _equipoService.ObtenerPersonalPorIdAsync(id);
  if (personal == null)
               return NotFound(new { success = false, message = "Personal no encontrado" });

          return Ok(new { success = true, data = personal });
     }
    catch (Exception ex)
  {
  return StatusCode(500, new { success = false, message = ex.Message });
  }
        }

     /// <summary>
      /// Crear nuevo personal
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPersonalDto dto)
        {
       try
            {
       var resultado = await _equipoService.CrearPersonalAsync(dto);

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
        /// Actualizar personal
  /// </summary>
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarPersonalDto dto)
        {
        try
            {
                var resultado = await _equipoService.ActualizarPersonalAsync(dto);

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
        /// Eliminar personal (soft delete)
      /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
     try
     {
    var resultado = await _equipoService.EliminarPersonalAsync(id);
   if (!resultado)
       return NotFound(new { success = false, message = "Personal no encontrado" });

return Ok(new { success = true, message = "Personal eliminado exitosamente" });
            }
            catch (Exception ex)
            {
  return StatusCode(500, new { success = false, message = ex.Message });
   }
        }
    }
}
