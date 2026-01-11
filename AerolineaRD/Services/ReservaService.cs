using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore; // ? AGREGAR

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
            // VALIDACIÓN: Verificar que el vuelo tiene asientos disponibles
            int asientosDisponibles = await _vueloRepository.ObtenerAsientosDisponiblesAsync(dto.IdVuelo);

            if (asientosDisponibles <= 0)
            {
                throw new InvalidOperationException(
                    $"El vuelo está lleno. No hay asientos disponibles.");
            }

            // VALIDACIÓN: Verificar que el asiento específico no esté reservado
            var reservaExistente = await _reservaRepository.ObtenerPorVueloYAsientoAsync(dto.IdVuelo, dto.NumAsiento);
            if (reservaExistente != null && reservaExistente.Estado == "Confirmada")
            {
                throw new InvalidOperationException(
                    $"El asiento {dto.NumAsiento} ya está reservado en este vuelo.");
            }

            // Crear reserva
            var reserva = _mapper.Map<Reserva>(dto);
            reserva.Codigo = GenerarCodigoReserva();
            reserva.FechaReserva = DateTime.Now;
            reserva.Estado = "Confirmada";

            await _reservaRepository.AddAsync(reserva);
            await _reservaRepository.SaveAsync();

            // ? NUEVO: Crear factura automáticamente
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(dto.IdVuelo);
            if (vuelo != null)
            {
                // ? Calcular precio total según clase solicitada
                decimal montoTotal;
               
                  if (dto.PrecioTotal.HasValue)
                {
                  // Si el cliente proporcionó el precio, usarlo
                     montoTotal = dto.PrecioTotal.Value;
             }
                else
          {
                 // ? Calcular según la clase seleccionada
               string claseReserva = dto.Clase ?? "Economica";
               montoTotal = vuelo.CalcularPrecioTotal(vuelo.PrecioBase, claseReserva);
                   }

                        // ? Actualizar el precio total en la reserva
              reserva.PrecioTotal = montoTotal;
             _reservaRepository.Update(reserva);

             var factura = new Factura
              {
           Codigo = GenerarCodigoFactura(),
       CodReserva = reserva.Codigo,
          Monto = montoTotal,
           MetodoPago = dto.MetodoPago ?? "Pendiente",
          FechaEmision = DateTime.Now,
                 EstadoPago = string.IsNullOrEmpty(dto.MetodoPago) ? "Pendiente" : "Pagado"
                  };

             await _facturaRepository.AddAsync(factura);
              await _facturaRepository.SaveAsync();

         // Enviar notificación de confirmación
           await _notificacionService.EnviarNotificacionAsync(
        reserva.IdCliente,
            "Confirmacion",
  $"Su reserva {reserva.Codigo} ha sido confirmada. Factura: {factura.Codigo} por ${montoTotal:F2}"
          );
 }

            var reservaCreada = await _reservaRepository.ObtenerReservaConDetallesAsync(reserva.Codigo);
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
            // VALIDACIÓN: Verificar si se puede cancelar según el tiempo restante
            bool puedeCancelar = await PuedeCancelarReservaAsync(codigo, 24);

            if (!puedeCancelar)
            {
                throw new InvalidOperationException(
                    "No se puede cancelar la reserva. Debe hacerlo con al menos 24 horas de anticipación al vuelo.");
            }

            var reserva = await _reservaRepository.ObtenerReservaConDetallesAsync(codigo);
            if (reserva == null)
                return false;

            // ? Cambiar estado de reserva a Cancelada
            reserva.Estado = "Cancelada";
            _reservaRepository.Update(reserva);

            // ? NUEVO: Cancelar factura asociada automáticamente
            var factura = await _facturaRepository.Context.Facturas
                .FirstOrDefaultAsync(f => f.CodReserva == codigo);

            if (factura != null)
            {
                // Cambiar estado de factura a Cancelado
                factura.EstadoPago = "Cancelado";
                _facturaRepository.Update(factura);

                // Determinar mensaje de reembolso
                string mensajeReembolso = factura.EstadoPago == "Pagado"
                    ? $"Se procesará un reembolso de ${factura.Monto:F2} en 5-7 días hábiles."
                    : "No se realizó ningún cargo.";

                // Enviar notificación de cancelación
                await _notificacionService.EnviarNotificacionAsync(
                    reserva.IdCliente,
                    "Cancelacion",
                    $"Su reserva {reserva.Codigo} ha sido cancelada. {mensajeReembolso}"
                );
            }

            await _reservaRepository.SaveAsync();

            return true;
        }

        public async Task<bool> PuedeCancelarReservaAsync(string codigo, int horasMinimas = 24)
        {
            var reserva = await _reservaRepository.ObtenerReservaConDetallesAsync(codigo);

            if (reserva == null || reserva.Vuelo == null)
                return false;

            // Calcular la fecha y hora del vuelo
            var fechaHoraVuelo = reserva.Vuelo.Fecha.Date.Add(reserva.Vuelo.HoraSalida);

            // Calcular el tiempo restante
            var tiempoRestante = fechaHoraVuelo - DateTime.Now;

            // Verificar si hay suficiente tiempo
            return tiempoRestante.TotalHours >= horasMinimas;
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