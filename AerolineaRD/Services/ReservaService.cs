using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IAsientoRepository _asientoRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly INotificacionService _notificacionService;
        private readonly IMapper _mapper;

        public ReservaService(
            IReservaRepository reservaRepository,
            IAsientoRepository asientoRepository,
            IFacturaRepository facturaRepository,
            INotificacionService notificacionService,
            IMapper mapper)
        {
            _reservaRepository = reservaRepository;
            _asientoRepository = asientoRepository;
            _facturaRepository = facturaRepository;
            _notificacionService = notificacionService;
            _mapper = mapper;
        }

        public async Task<ReservaResponseDto> CrearReservaAsync(CrearReservaDto dto)
        {
            // Validar que el asiento esté disponible
            if (!string.IsNullOrEmpty(dto.NumAsiento))
            {
                var existe = await _reservaRepository.ExisteReservaActivaAsync(dto.IdVuelo, dto.NumAsiento);
                if (existe)
                    throw new InvalidOperationException("El asiento seleccionado ya está reservado.");

                // Marcar asiento como ocupado
                var asiento = await _asientoRepository.ObtenerAsientoPorNumeroAsync(dto.NumAsiento);
                if (asiento != null)
                {
                    asiento.Disponibilidad = "Ocupado";
                    _asientoRepository.Update(asiento);
                }
            }

            // Crear reserva
            var reserva = new Reserva
            {
                Codigo = GenerarCodigoReserva(),
                IdPasajero = dto.IdPasajero,
                IdVuelo = dto.IdVuelo,
                IdCliente = dto.IdCliente,
                NumAsiento = dto.NumAsiento,
                FechaReserva = DateTime.Now,
                Estado = "Confirmada",
                PrecioTotal = 150.00m // Calcular según lógica de negocio
            };

            await _reservaRepository.AddAsync(reserva);
            await _reservaRepository.SaveAsync();

            // Crear factura automáticamente
            var factura = new Factura
            {
                Codigo = GenerarCodigoFactura(),
                CodReserva = reserva.Codigo,
                Monto = reserva.PrecioTotal,
                MetodoPago = dto.MetodoPago,
                FechaEmision = DateTime.Now,
                EstadoPago = "Pendiente"
            };

            await _facturaRepository.AddAsync(factura);
            await _facturaRepository.SaveAsync();

            // Enviar notificación
            await _notificacionService.EnviarNotificacionAsync(
                dto.IdCliente,
                "Confirmacion",
                $"Su reserva {reserva.Codigo} ha sido confirmada exitosamente."
            );

            // Obtener reserva con detalles completos
            var reservaCreada = await _reservaRepository.ObtenerReservaPorCodigoAsync(reserva.Codigo);
            return _mapper.Map<ReservaResponseDto>(reservaCreada);
        }

        public async Task<ReservaResponseDto?> ObtenerReservaPorCodigoAsync(string codigo)
        {
            var reserva = await _reservaRepository.ObtenerReservaPorCodigoAsync(codigo);
            return reserva != null ? _mapper.Map<ReservaResponseDto>(reserva) : null;
        }

        public async Task<List<ReservaResponseDto>> ObtenerReservasPorClienteAsync(int idCliente)
        {
            var reservas = await _reservaRepository.ObtenerReservasPorClienteAsync(idCliente);
            return _mapper.Map<List<ReservaResponseDto>>(reservas);
        }

        public async Task<ReservaResponseDto> ModificarReservaAsync(ModificarReservaDto dto)
        {
            var reserva = await _reservaRepository.ObtenerReservaPorCodigoAsync(dto.CodigoReserva);
            if (reserva == null)
                throw new KeyNotFoundException("Reserva no encontrada.");

            // Liberar asiento anterior
            if (!string.IsNullOrEmpty(reserva.NumAsiento))
            {
                var asientoAnterior = await _asientoRepository.ObtenerAsientoPorNumeroAsync(reserva.NumAsiento);
                if (asientoAnterior != null)
                {
                    asientoAnterior.Disponibilidad = "Disponible";
                    _asientoRepository.Update(asientoAnterior);
                }
            }

            // Actualizar datos
            if (dto.NuevoIdVuelo.HasValue)
                reserva.IdVuelo = dto.NuevoIdVuelo.Value;

            if (!string.IsNullOrEmpty(dto.NuevoNumAsiento))
            {
                var existe = await _reservaRepository.ExisteReservaActivaAsync(reserva.IdVuelo, dto.NuevoNumAsiento);
                if (existe)
                    throw new InvalidOperationException("El nuevo asiento ya está ocupado.");

                reserva.NumAsiento = dto.NuevoNumAsiento;

                var nuevoAsiento = await _asientoRepository.ObtenerAsientoPorNumeroAsync(dto.NuevoNumAsiento);
                if (nuevoAsiento != null)
                {
                    nuevoAsiento.Disponibilidad = "Ocupado";
                    _asientoRepository.Update(nuevoAsiento);
                }
            }

            reserva.Estado = "Modificada";
            _reservaRepository.Update(reserva);
            await _reservaRepository.SaveAsync();

            // Notificar cambio
            await _notificacionService.EnviarNotificacionAsync(
                reserva.IdCliente,
                "Cambio",
                $"Su reserva {reserva.Codigo} ha sido modificada."
            );

            var reservaActualizada = await _reservaRepository.ObtenerReservaPorCodigoAsync(dto.CodigoReserva);
            return _mapper.Map<ReservaResponseDto>(reservaActualizada);
        }

        public async Task<bool> CancelarReservaAsync(string codigo)
        {
            var reserva = await _reservaRepository.ObtenerReservaPorCodigoAsync(codigo);
            if (reserva == null)
                return false;

            // Liberar asiento
            if (!string.IsNullOrEmpty(reserva.NumAsiento))
            {
                var asiento = await _asientoRepository.ObtenerAsientoPorNumeroAsync(reserva.NumAsiento);
                if (asiento != null)
                {
                    asiento.Disponibilidad = "Disponible";
                    _asientoRepository.Update(asiento);
                }
            }

            reserva.Estado = "Cancelada";
            _reservaRepository.Update(reserva);

            // Actualizar factura
            var factura = await _facturaRepository.ObtenerPorReservaAsync(reserva.Codigo);
            if (factura != null)
            {
                factura.EstadoPago = "Reembolsado";
                _facturaRepository.Update(factura);
            }

            await _reservaRepository.SaveAsync();

            // Notificar cancelación
            await _notificacionService.EnviarNotificacionAsync(
                reserva.IdCliente,
                "Cancelacion",
                $"Su reserva {reserva.Codigo} ha sido cancelada."
            );

            return true;
        }

        private string GenerarCodigoReserva()
        {
            return "RES" + DateTime.Now.Ticks.ToString().Substring(7);
        }

        private string GenerarCodigoFactura()
        {
            return "FAC" + DateTime.Now.Ticks.ToString().Substring(7);
        }
    }
}