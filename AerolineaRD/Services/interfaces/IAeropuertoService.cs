using AerolineaRD.Data.DTOs;

namespace AerolineaRD.Services.interfaces
{
    public interface IAeropuertoService
    {
        Task<List<AeropuertoDto>> ObtenerTodosAsync();
    }
}