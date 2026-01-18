using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;

namespace AerolineaRD.Services
{
    public class RutaService : IRutaService
    {
        private readonly IRutaRepository _rutaRepository;
        private readonly IAeropuertoRepository _aeropuertoRepository;

        // Constantes para cálculo de precios
        private const decimal PRECIO_BASE_POR_MINUTO = 1.5m;      // $1.50 por minuto de vuelo
        private const decimal PRECIO_MINIMO = 100m;               // Precio mínimo $100
        private const decimal CARGO_INTERNACIONAL = 50m;          // Cargo adicional para vuelos internacionales
        private const decimal CARGO_INTERCONTINENTAL = 150m;      // Cargo adicional para vuelos intercontinentales
        private const decimal INCREMENTO_EJECUTIVA = 100m;        // +$100 para clase ejecutiva
        private const decimal INCREMENTO_PRIMERA = 200m;          // +$200 para primera clase

        public RutaService(IRutaRepository rutaRepository, IAeropuertoRepository aeropuertoRepository)
        {
            _rutaRepository = rutaRepository;
            _aeropuertoRepository = aeropuertoRepository;
        }

        public async Task<RutaDuracionDto> ObtenerDuracionRutaAsync(string origenCodigo, string destinoCodigo)
        {
            return await ObtenerInfoRutaCompletaAsync(origenCodigo, destinoCodigo, null);
        }

        public async Task<RutaDuracionDto> ObtenerInfoRutaCompletaAsync(string origenCodigo, string destinoCodigo, TimeSpan? horaSalida = null)
        {
            // Validar que no sea el mismo aeropuerto
            if (origenCodigo == destinoCodigo)
            {
                return new RutaDuracionDto
                {
                    OrigenCodigo = origenCodigo,
                    DestinoCodigo = destinoCodigo,
                    DuracionMinutos = 0,
                    DuracionFormato = "0m",
                    Duracion = TimeSpan.Zero,
                    RutaEncontrada = false,
                    Mensaje = "El origen y destino no pueden ser el mismo aeropuerto"
                };
            }

            var ruta = await _rutaRepository.ObtenerRutaAsync(origenCodigo, destinoCodigo);

            if (ruta == null)
            {
                return new RutaDuracionDto
                {
                    OrigenCodigo = origenCodigo,
                    DestinoCodigo = destinoCodigo,
                    DuracionMinutos = 0,
                    DuracionFormato = "0m",
                    Duracion = TimeSpan.Zero,
                    RutaEncontrada = false,
                    Mensaje = $"No existe una ruta definida entre {origenCodigo} y {destinoCodigo}. Ingrese la duración manualmente."
                };
            }

            // Determinar tipo de ruta
            var tipoRuta = DeterminarTipoRuta(ruta.Origen, ruta.Destino, ruta.DuracionMinutos);

            // Calcular precio sugerido
            var precioBase = CalcularPrecioSugerido(ruta.DuracionMinutos, tipoRuta);

            // Calcular precios por clase
            var preciosPorClase = new PreciosPorClaseDto
            {
                Economica = precioBase,
                Ejecutiva = precioBase + INCREMENTO_EJECUTIVA,
                Primera = precioBase + INCREMENTO_PRIMERA,
                EconomicaFormato = $"${precioBase:N2}",
                EjecutivaFormato = $"${precioBase + INCREMENTO_EJECUTIVA:N2}",
                PrimeraFormato = $"${precioBase + INCREMENTO_PRIMERA:N2}"
            };

            var resultado = new RutaDuracionDto
            {
                OrigenCodigo = origenCodigo,
                DestinoCodigo = destinoCodigo,
                DuracionMinutos = ruta.DuracionMinutos,
                DuracionFormato = FormatearDuracion(ruta.DuracionMinutos),
                Duracion = TimeSpan.FromMinutes(ruta.DuracionMinutos),
                RutaEncontrada = true,
                Mensaje = $"Duración estimada: {FormatearDuracion(ruta.DuracionMinutos)}",
                PrecioSugerido = precioBase,
                PrecioFormato = $"${precioBase:N2}",
                PreciosPorClase = preciosPorClase,
                DistanciaKm = ruta.DistanciaKm,
                TipoRuta = tipoRuta
            };

            // Si se proporcionó hora de salida, calcular hora de llegada
            if (horaSalida.HasValue)
            {
                var horaLlegada = CalcularHoraLlegada(horaSalida.Value, ruta.DuracionMinutos);
                resultado.HoraLlegadaCalculada = horaLlegada;
                resultado.HoraLlegadaFormato = FormatearHora(horaLlegada);
                resultado.Mensaje = $"Duración: {FormatearDuracion(ruta.DuracionMinutos)} | Llegada: {FormatearHora(horaLlegada)}";
            }

            return resultado;
        }

        public async Task<List<RutaDto>> ObtenerTodasLasRutasAsync()
        {
            var rutas = await _rutaRepository.ObtenerRutasActivasAsync();
            return rutas.Select(MapToDto).ToList();
        }

        public async Task<List<RutaDto>> ObtenerRutasDesdeOrigenAsync(string origenCodigo)
        {
            var rutas = await _rutaRepository.ObtenerRutasDesdeOrigenAsync(origenCodigo);
            return rutas.Select(MapToDto).ToList();
        }

        public async Task<List<RutaDto>> ObtenerRutasHaciaDestinoAsync(string destinoCodigo)
        {
            var rutas = await _rutaRepository.ObtenerRutasHaciaDestinoAsync(destinoCodigo);
            return rutas.Select(MapToDto).ToList();
        }

        public async Task<RutaDto> CrearRutaAsync(CrearRutaDto dto)
        {
            // Validar que los aeropuertos existan
            var origen = await _aeropuertoRepository.GetByIdAsync(dto.OrigenCodigo);
            var destino = await _aeropuertoRepository.GetByIdAsync(dto.DestinoCodigo);

            if (origen == null)
                throw new KeyNotFoundException($"Aeropuerto de origen '{dto.OrigenCodigo}' no encontrado");

            if (destino == null)
                throw new KeyNotFoundException($"Aeropuerto de destino '{dto.DestinoCodigo}' no encontrado");

            // Validar que no exista ya la ruta
            if (await _rutaRepository.ExisteRutaAsync(dto.OrigenCodigo, dto.DestinoCodigo))
                throw new InvalidOperationException($"Ya existe una ruta entre {dto.OrigenCodigo} y {dto.DestinoCodigo}");

            var ruta = new Ruta
            {
                OrigenCodigo = dto.OrigenCodigo,
                DestinoCodigo = dto.DestinoCodigo,
                DuracionMinutos = dto.DuracionMinutos,
                DistanciaKm = dto.DistanciaKm,
                Activa = true
            };

            await _rutaRepository.AddAsync(ruta);
            await _rutaRepository.SaveAsync();

            // Recargar con navegaciones
            var rutaCreada = await _rutaRepository.ObtenerRutaAsync(dto.OrigenCodigo, dto.DestinoCodigo);
            return MapToDto(rutaCreada!);
        }

        public async Task<RutaDto?> ActualizarRutaAsync(ActualizarRutaDto dto)
        {
            var ruta = await _rutaRepository.GetByIdAsync(dto.Id);
            if (ruta == null)
                return null;

            if (dto.DuracionMinutos.HasValue)
                ruta.DuracionMinutos = dto.DuracionMinutos.Value;

            if (dto.DistanciaKm.HasValue)
                ruta.DistanciaKm = dto.DistanciaKm.Value;

            if (dto.Activa.HasValue)
                ruta.Activa = dto.Activa.Value;

            _rutaRepository.Update(ruta);
            await _rutaRepository.SaveAsync();

            var rutaActualizada = await _rutaRepository.ObtenerRutaAsync(ruta.OrigenCodigo, ruta.DestinoCodigo);
            return rutaActualizada != null ? MapToDto(rutaActualizada) : null;
        }

        public async Task<bool> EliminarRutaAsync(int id)
        {
            var ruta = await _rutaRepository.GetByIdAsync(id);
            if (ruta == null)
                return false;

            // Soft delete - solo desactivar
            ruta.Activa = false;
            _rutaRepository.Update(ruta);
            await _rutaRepository.SaveAsync();

            return true;
        }

        #region Helpers privados

        /// <summary>
        /// Calcula la hora de llegada basándose en la hora de salida y duración
        /// </summary>
        private static TimeSpan CalcularHoraLlegada(TimeSpan horaSalida, int duracionMinutos)
        {
            var llegada = horaSalida.Add(TimeSpan.FromMinutes(duracionMinutos));
            
            // Si pasa de las 24 horas, ajustar (el vuelo llega al día siguiente)
            if (llegada.TotalHours >= 24)
            {
                llegada = TimeSpan.FromHours(llegada.TotalHours - 24);
            }
            
            return llegada;
        }

        /// <summary>
        /// Calcula el precio sugerido basado en la duración y tipo de ruta
        /// </summary>
        private decimal CalcularPrecioSugerido(int duracionMinutos, string tipoRuta)
        {
            // Precio base = duración en minutos * tarifa por minuto
            var precio = duracionMinutos * PRECIO_BASE_POR_MINUTO;

            // Agregar cargos según tipo de ruta
            precio += tipoRuta switch
            {
                "Internacional" => CARGO_INTERNACIONAL,
                "Intercontinental" => CARGO_INTERCONTINENTAL,
                _ => 0m
            };

            // Asegurar precio mínimo
            precio = Math.Max(precio, PRECIO_MINIMO);

            // Redondear a múltiplos de 5 para precios más "limpios"
            precio = Math.Ceiling(precio / 5) * 5;

            return precio;
        }

        /// <summary>
        /// Determina el tipo de ruta basado en los países y duración
        /// </summary>
        private static string DeterminarTipoRuta(Aeropuerto? origen, Aeropuerto? destino, int duracionMinutos)
        {
            if (origen == null || destino == null)
                return "Desconocido";

            // Si son del mismo país
            if (origen.Pais == destino.Pais)
                return "Nacional";

            // Definir regiones
            var regionCaribe = new[] { "Republica Dominicana", "Cuba", "Puerto Rico", "Jamaica", "Haiti" };
            var regionNorteamerica = new[] { "Estados Unidos", "Canada", "Mexico" };
            var regionCentroamerica = new[] { "Panama", "Costa Rica", "Guatemala", "Honduras", "El Salvador", "Nicaragua" };
            var regionSudamerica = new[] { "Colombia", "Venezuela", "Ecuador", "Peru", "Brasil", "Argentina", "Chile" };
            var regionEuropa = new[] { "Espana", "Francia", "Alemania", "Italia", "Reino Unido", "Portugal" };

            var origenRegion = ObtenerRegion(origen.Pais, regionCaribe, regionNorteamerica, regionCentroamerica, regionSudamerica, regionEuropa);
            var destinoRegion = ObtenerRegion(destino.Pais, regionCaribe, regionNorteamerica, regionCentroamerica, regionSudamerica, regionEuropa);

            // Si están en la misma región o regiones cercanas
            if (origenRegion == destinoRegion)
                return "Regional";

            // Si cruzan continentes (Caribe/América a Europa)
            if ((origenRegion == "Caribe" || origenRegion == "Norteamerica" || origenRegion == "Sudamerica") && destinoRegion == "Europa")
                return "Intercontinental";

            if ((destinoRegion == "Caribe" || destinoRegion == "Norteamerica" || destinoRegion == "Sudamerica") && origenRegion == "Europa")
                return "Intercontinental";

            // Por defecto basarse en duración
            if (duracionMinutos > 360) // Más de 6 horas
                return "Intercontinental";
            
            return "Internacional";
        }

        private static string ObtenerRegion(string? pais, string[] caribe, string[] norteamerica, string[] centroamerica, string[] sudamerica, string[] europa)
        {
            if (string.IsNullOrEmpty(pais)) return "Desconocido";
            
            if (caribe.Contains(pais)) return "Caribe";
            if (norteamerica.Contains(pais)) return "Norteamerica";
            if (centroamerica.Contains(pais)) return "Centroamerica";
            if (sudamerica.Contains(pais)) return "Sudamerica";
            if (europa.Contains(pais)) return "Europa";
            
            return "Otro";
        }

        /// <summary>
        /// Formatea la duración en formato legible
        /// </summary>
        private static string FormatearDuracion(int minutos)
        {
            var horas = minutos / 60;
            var mins = minutos % 60;

            if (horas > 0 && mins > 0)
                return $"{horas}h {mins}m";
            else if (horas > 0)
                return $"{horas}h";
            else
                return $"{mins}m";
        }

        /// <summary>
        /// Formatea hora en formato 12h con AM/PM
        /// </summary>
        private static string FormatearHora(TimeSpan hora)
        {
            var hh = hora.Hours;
            var mm = hora.Minutes;
            var periodo = hh >= 12 ? "PM" : "AM";
            var displayHour = hh == 0 ? 12 : (hh > 12 ? hh - 12 : hh);
            return $"{displayHour}:{mm:D2} {periodo}";
        }

        /// <summary>
        /// Mapea entidad Ruta a RutaDto
        /// </summary>
        private static RutaDto MapToDto(Ruta ruta)
        {
            return new RutaDto
            {
                Id = ruta.Id,
                OrigenCodigo = ruta.OrigenCodigo,
                DestinoCodigo = ruta.DestinoCodigo,
                DuracionMinutos = ruta.DuracionMinutos,
                DistanciaKm = ruta.DistanciaKm,
                Activa = ruta.Activa,
                DuracionFormato = FormatearDuracion(ruta.DuracionMinutos),
                OrigenNombre = ruta.Origen?.Nombre,
                OrigenCiudad = ruta.Origen?.Ciudad,
                DestinoNombre = ruta.Destino?.Nombre,
                DestinoCiudad = ruta.Destino?.Ciudad
            };
        }

        #endregion
    }
}
