using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IFacturaService
    {
        Task<FacturaResponseDto> CrearFacturaAsync(CrearFacturaDto dto);
        Task<List<FacturaResponseDto>> ObtenerTodasAsync();
        Task<FacturaResponseDto?> ObtenerPorCodigoAsync(string codigo);
        Task<bool> PagarFacturaAsync(PagarFacturaDto dto);
    }
}