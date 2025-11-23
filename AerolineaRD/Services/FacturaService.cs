using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IMapper _mapper;

        public FacturaService(IFacturaRepository facturaRepository, IMapper mapper)
        {
            _facturaRepository = facturaRepository;
            _mapper = mapper;
        }

        public async Task<FacturaResponseDto> CrearFacturaAsync(CrearFacturaDto dto)
        {
            var factura = _mapper.Map<Factura>(dto);
            factura.FechaEmision = DateTime.Now;

            await _facturaRepository.AddAsync(factura);
            await _facturaRepository.SaveAsync();

            return _mapper.Map<FacturaResponseDto>(factura);
        }

        public async Task<List<FacturaResponseDto>> ObtenerTodasAsync()
        {
            var facturas = await _facturaRepository.GetAllAsync();
            return _mapper.Map<List<FacturaResponseDto>>(facturas);
        }

        public async Task<FacturaResponseDto?> ObtenerPorCodigoAsync(string codigo)
        {
            var factura = await _facturaRepository.ObtenerPorCodigoAsync(codigo);
            return factura != null ? _mapper.Map<FacturaResponseDto>(factura) : null;
        }

        public async Task<bool> PagarFacturaAsync(PagarFacturaDto dto)
        {
            var factura = await _facturaRepository.ObtenerPorCodigoAsync(dto.CodigoFactura);
            if (factura == null)
                return false;

            factura.MetodoPago = dto.MetodoPago;
            factura.EstadoPago = "Pagado";

            _facturaRepository.Update(factura);
            await _facturaRepository.SaveAsync();

            return true;
        }
    }
}