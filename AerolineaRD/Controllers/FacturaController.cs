using AerolineaRD.Data.DTOs;
using AerolineaRD.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AerolineaRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        /// <summary>
        /// Obtener factura por código
        /// </summary>
        [HttpGet("{codigo}")]
        [Authorize]
        public async Task<IActionResult> ObtenerFactura(string codigo)
        {
            try
            {
                var factura = await _facturaService.ObtenerPorCodigoAsync(codigo);
                if (factura == null)
                    return NotFound(new { success = false, message = "Factura no encontrada" });

                return Ok(new { success = true, data = factura });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Pagar una factura
        /// </summary>
        [HttpPost("pagar")]
        [Authorize]
        public async Task<IActionResult> PagarFactura([FromBody] PagarFacturaDto dto)
        {
            try
            {
                var resultado = await _facturaService.PagarFacturaAsync(dto);
                if (!resultado)
                    return NotFound(new { success = false, message = "Factura no encontrada" });

                return Ok(new { success = true, message = "Factura pagada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}