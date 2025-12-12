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
        private readonly IVueloRepository _vueloRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly INotificacionService _notificacionService;
        private readonly IClienteRepository _clienteRepository;
        private readonly IPasajeroRepository _pasajeroRepository;
        private readonly IMapper _mapper;

        public ReservaService(
            IReservaRepository reservaRepository,
            IVueloRepository vueloRepository,
            IFacturaRepository facturaRepository,
            INotificacionService notificacionService,
            IClienteRepository clienteRepository,
            IPasajeroRepository pasajeroRepository,
            IMapper mapper)
        {
            _reservaRepository = reservaRepository;
            _vueloRepository = vueloRepository;
            _facturaRepository = facturaRepository;
            _notificacionService = notificacionService;
            _clienteRepository = clienteRepository;
            _pasajeroRepository = pasajeroRepository;
            _mapper = mapper;
        }

        public async Task<ReservaResponseDto> CrearReservaAsync(CrearReservaDto dto)
        {
            // Validar existencia de FK: cliente, pasajero y vuelo
            var cliente = await _clienteRepository.GetByIdAsync(dto.IdCliente);
            if (cliente == null)
                throw new InvalidOperationException("Cliente no encontrado.");

            var pasajero = await _pasajeroRepository.GetByIdAsync(dto.IdPasajero);
            if (pasajero == null)
                throw new InvalidOperationException("Pasajero no encontrado.");

            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(dto.IdVuelo);
            if (vuelo == null)
                throw new InvalidOperationException("Vuelo no encontrado.");

            // Validar asiento si fue provisto
            if (!string.IsNullOrEmpty(dto.NumAsiento))
            {
                var existe = await _reservaRepository.ExisteReservaActivaAsync(dto.IdVuelo, dto.NumAsiento);
                if (existe)
                    throw new InvalidOperationException("El asiento seleccionado ya está reservado.");

                if (vuelo.Aeronave?.Asientos == null)
                    throw new InvalidOperationException("No se pudo validar el vuelo o la aeronave.");

                var asientoExiste = vuelo.Aeronave.Asientos.Any(a => a.NumeroAsiento == dto.NumAsiento);
                if (!asientoExiste)
                    throw new InvalidOperationException("El asiento seleccionado no existe en esta aeronave.");
            }

            // Crear reserva
            var reserva = new Reserva
            {
                Codigo = GenerarCodigoReserva(),
                IdPasajero = dto.IdPasajero,
                IdVuelo = dto.IdVuelo,
                IdCliente = dto.IdCliente,
                NumAsiento = dto.NumAsiento,
                Clase = dto.Clase,
                FechaReserva = DateTime.Now,
                Estado = "Confirmada",
                PrecioTotal = dto.PrecioTotal ?? 150.00m
            };

            await _reservaRepository.AddAsync(reserva);
            // Guardar la reserva antes de crear la factura para asegurar FK
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

        public async Task<List<ReservaResponseDto>> ObtenerTodasAsync()
        {
            var reservas = await _reservaRepository.ObtenerTodasConDetallesAsync();
            return _mapper.Map<List<ReservaResponseDto>>(reservas);
        }

        public async Task<ReservaResponseDto> ModificarReservaAsync(ModificarReservaDto dto)
        {
            var reserva = await _reservaRepository.ObtenerReservaPorCodigoAsync(dto.CodigoReserva);
            if (reserva == null)
                throw new KeyNotFoundException("Reserva no encontrada.");

            if (dto.NuevoIdVuelo.HasValue)
                reserva.IdVuelo = dto.NuevoIdVuelo.Value;

            if (!string.IsNullOrEmpty(dto.NuevoNumAsiento))
            {
                var existe = await _reservaRepository.ExisteReservaActivaAsync(reserva.IdVuelo, dto.NuevoNumAsiento);
                if (existe)
                    throw new InvalidOperationException("El nuevo asiento ya está ocupado.");

                var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(reserva.IdVuelo);
                if (vuelo?.Aeronave?.Asientos == null)
                    throw new InvalidOperationException("No se pudo validar el vuelo.");

                var asientoExiste = vuelo.Aeronave.Asientos.Any(a => a.NumeroAsiento == dto.NuevoNumAsiento);
                if (!asientoExiste)
                    throw new InvalidOperationException("El nuevo asiento no existe en esta aeronave.");

                reserva.NumAsiento = dto.NuevoNumAsiento;
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

            reserva.Estado = "Cancelada";
            _reservaRepository.Update(reserva);

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