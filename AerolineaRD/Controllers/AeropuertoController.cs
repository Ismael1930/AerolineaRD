using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}