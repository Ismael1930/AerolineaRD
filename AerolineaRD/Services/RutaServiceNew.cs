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
        private readonly IVueloRepository _vueloRepository;

        private const decimal PRECIO_BASE_POR_MINUTO = 1.5m;
        private const decimal PRECIO_MINIMO = 100m;
        private const decimal CARGO_INTERNACIONAL = 50m;
        private const decimal CARGO_INTERCONTINENTAL = 150m;
        private const decimal INCREMENTO_EJECUTIVA = 100m;
        private const decimal INCREMENTO_PRIMERA = 200m;

        public RutaService(
            IRutaRepository rutaRepository, 
            IAeropuertoRepository aeropuertoRepository,
            IVueloRepository vueloRepository)
        {
            _rutaRepository = rutaRepository;
            _aeropuertoRepository = aeropuertoRepository;
            _vueloRepository = vueloRepository;
        }

        public async Task<RutaDuracionDto> ObtenerDuracionRutaAsync(string origenCodigo, string destinoCodigo)
        {
            return await ObtenerInfoRutaCompletaAsync(origenCodigo, destinoCodigo, null);
        }

        public async Task<RutaDuracionDto> ObtenerInfoRutaCompletaAsync(string origenCodigo, string destinoCodigo, TimeSpan? horaSalida = null)
        {
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

            var tipoRuta = DeterminarTipoRuta(ruta.Origen, ruta.Destino, ruta.DuracionMinutos);
            var precioBase = CalcularPrecioSugerido(ruta.DuracionMinutos, tipoRuta);

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
                TipoRuta = tipoRuta,
                CruzaMedianoche = false
            };

            if (horaSalida.HasValue)
            {
                var horaLlegada = CalcularHoraLlegada(horaSalida.Value, ruta.DuracionMinutos);
                resultado.HoraLlegadaCalculada = horaLlegada;
                resultado.HoraLlegadaFormato = FormatearHora(horaLlegada);
                
                bool cruzaMedianoche = horaLlegada < horaSalida.Value;
                resultado.CruzaMedianoche = cruzaMedianoche;
                
                if (cruzaMedianoche)
                {
                    resultado.NotaMedianoche = "?? Este vuelo llega al día siguiente (cruza medianoche)";
                    resultado.Mensaje = $"Duración: {FormatearDuracion(ruta.DuracionMinutos)} | Llegada: {FormatearHora(horaLlegada)} (+1 día)";
                }
                else
                {
                    resultado.Mensaje = $"Duración: {FormatearDuracion(ruta.DuracionMinutos)} | Llegada: {FormatearHora(horaLlegada)}";
                }
            }

            return resultado;
        }

        public async Task<HorasDisponiblesDto> ObtenerHorasDisponiblesAsync(string origenCodigo, string destinoCodigo, DateTime fecha)
        {
            var resultado = new HorasDisponiblesDto
            {
                OrigenCodigo = origenCodigo,
                DestinoCodigo = destinoCodigo,
                Fecha = fecha.Date,
                FechaFormato = fecha.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")),
                HorasDisponibles = new List<HoraDisponibleDto>(),
                HorasOcupadas = new List<HoraOcupadaDto>()
            };

            var aeropuerto = await _aeropuertoRepository.GetByIdAsync(origenCodigo);
            if (aeropuerto == null)
            {
                resultado.Mensaje = $"Aeropuerto de origen '{origenCodigo}' no encontrado";
                return resultado;
            }

            resultado.OrigenNombre = aeropuerto.Nombre;
            resultado.CapacidadPorHora = aeropuerto.CapacidadVuelosPorHora > 0 
                ? aeropuerto.CapacidadVuelosPorHora 
                : 10;

            var ruta = await _rutaRepository.ObtenerRutaAsync(origenCodigo, destinoCodigo);
            if (ruta != null)
            {
                resultado.InfoRuta = await ObtenerInfoRutaCompletaAsync(origenCodigo, destinoCodigo, null);
            }

            var vuelosProgramados = await _vueloRepository.ObtenerVuelosPorOrigenYFechaAsync(origenCodigo, fecha);

            var vuelosPorHora = vuelosProgramados
                .GroupBy(v => v.HoraSalida.Hours)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ? NUEVO: Calcular hora mínima permitida si es hoy
            var ahora = DateTime.Now;
            var esHoy = fecha.Date == ahora.Date;
            TimeSpan horaMinima = TimeSpan.Zero;
            
            if (esHoy)
            {
                // Agregar 2 horas de margen para preparación del vuelo
                const int HORAS_PREPARACION = 2;
                horaMinima = ahora.TimeOfDay.Add(TimeSpan.FromHours(HORAS_PREPARACION));
                
                // Redondear hacia arriba a la siguiente media hora
                var minutosActuales = (int)horaMinima.TotalMinutes;
                var minutosRedondeados = ((minutosActuales / 30) + 1) * 30;
                horaMinima = TimeSpan.FromMinutes(minutosRedondeados);
            }

            for (int hora = 5; hora <= 23; hora++)
            {
                var horaTimeSpan = new TimeSpan(hora, 0, 0);
                var vuelosEnEstaHora = vuelosPorHora.GetValueOrDefault(hora, new List<Vuelo>());
                var cantidadVuelos = vuelosEnEstaHora.Count;
                var espaciosDisponibles = resultado.CapacidadPorHora - cantidadVuelos;

                if (cantidadVuelos > 0)
                {
                    resultado.HorasOcupadas.Add(new HoraOcupadaDto
                    {
                        Hora = horaTimeSpan,
                        HoraFormato = FormatearHora(horaTimeSpan),
                        VuelosProgramados = cantidadVuelos,
                        CapacidadMaxima = resultado.CapacidadPorHora,
                        Saturada = espaciosDisponibles <= 0,
                        VuelosEnHora = vuelosEnEstaHora.Select(v => v.NumeroVuelo ?? "N/A").ToList()
                    });
                }

                if (espaciosDisponibles > 0)
                {
                    // ? NUEVO: Verificar hora en punto
                    if (!esHoy || horaTimeSpan >= horaMinima)
                    {
                        var horaDisponible = new HoraDisponibleDto
                        {
                            Hora = horaTimeSpan,
                            HoraFormato = FormatearHora(horaTimeSpan),
                            Valor = $"{hora:D2}:00",
                            EspaciosDisponibles = espaciosDisponibles
                        };

                        if (ruta != null)
                        {
                            var horaLlegada = CalcularHoraLlegada(horaTimeSpan, ruta.DuracionMinutos);
                            horaDisponible.HoraLlegada = horaLlegada;
                            horaDisponible.HoraLlegadaFormato = FormatearHora(horaLlegada);
                            horaDisponible.CruzaMedianoche = horaLlegada < horaTimeSpan;
                        }

                        resultado.HorasDisponibles.Add(horaDisponible);
                    }

                    // ? NUEVO: Verificar hora y media
                    var horaMediaTimeSpan = new TimeSpan(hora, 30, 0);
                    if (!esHoy || horaMediaTimeSpan >= horaMinima)
                    {
                        var horaDisponibleMedia = new HoraDisponibleDto
                        {
                            Hora = horaMediaTimeSpan,
                            HoraFormato = FormatearHora(horaMediaTimeSpan),
                            Valor = $"{hora:D2}:30",
                            EspaciosDisponibles = espaciosDisponibles
                        };

                        if (ruta != null)
                        {
                            var horaLlegadaMedia = CalcularHoraLlegada(horaMediaTimeSpan, ruta.DuracionMinutos);
                            horaDisponibleMedia.HoraLlegada = horaLlegadaMedia;
                            horaDisponibleMedia.HoraLlegadaFormato = FormatearHora(horaLlegadaMedia);
                            horaDisponibleMedia.CruzaMedianoche = horaLlegadaMedia < horaMediaTimeSpan;
                        }

                        resultado.HorasDisponibles.Add(horaDisponibleMedia);
                    }
                }
            }

            resultado.HorasDisponibles = resultado.HorasDisponibles.OrderBy(h => h.Hora).ToList();
            resultado.HorasOcupadas = resultado.HorasOcupadas.OrderBy(h => h.Hora).ToList();

            // ? NUEVO: Mensaje informativo si es hoy
            if (esHoy && horaMinima > TimeSpan.Zero)
            {
                var horaMinFormateada = FormatearHora(horaMinima);
                resultado.Mensaje = resultado.HorasDisponibles.Any()
                    ? $"{resultado.HorasDisponibles.Count} horarios disponibles. Para vuelos de hoy, la hora mínima es {horaMinFormateada} (2 horas de preparación)."
                    : $"No hay horarios disponibles para hoy. Los vuelos requieren al menos 2 horas de preparación (hora mínima: {horaMinFormateada}).";
            }
            else
            {
                resultado.Mensaje = resultado.HorasDisponibles.Any()
                    ? $"{resultado.HorasDisponibles.Count} horarios disponibles encontrados"
                    : "No hay horarios disponibles para esta fecha. Todas las horas están saturadas.";
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
            var origen = await _aeropuertoRepository.GetByIdAsync(dto.OrigenCodigo);
            var destino = await _aeropuertoRepository.GetByIdAsync(dto.DestinoCodigo);

            if (origen == null)
                throw new KeyNotFoundException($"Aeropuerto de origen '{dto.OrigenCodigo}' no encontrado");

            if (destino == null)
                throw new KeyNotFoundException($"Aeropuerto de destino '{dto.DestinoCodigo}' no encontrado");

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

            ruta.Activa = false;
            _rutaRepository.Update(ruta);
            await _rutaRepository.SaveAsync();

            return true;
        }

        #region Helpers privados

        private static TimeSpan CalcularHoraLlegada(TimeSpan horaSalida, int duracionMinutos)
        {
            var llegada = horaSalida.Add(TimeSpan.FromMinutes(duracionMinutos));
            if (llegada.TotalHours >= 24)
                llegada = TimeSpan.FromHours(llegada.TotalHours - 24);
            return llegada;
        }

        private decimal CalcularPrecioSugerido(int duracionMinutos, string tipoRuta)
        {
            var precio = duracionMinutos * PRECIO_BASE_POR_MINUTO;
            precio += tipoRuta switch
            {
                "Internacional" => CARGO_INTERNACIONAL,
                "Intercontinental" => CARGO_INTERCONTINENTAL,
                _ => 0m
            };
            precio = Math.Max(precio, PRECIO_MINIMO);
            precio = Math.Ceiling(precio / 5) * 5;
            return precio;
        }

        private static string DeterminarTipoRuta(Aeropuerto? origen, Aeropuerto? destino, int duracionMinutos)
        {
            if (origen == null || destino == null) return "Desconocido";
            if (origen.Pais == destino.Pais) return "Nacional";

            var regionCaribe = new[] { "Republica Dominicana", "Cuba", "Puerto Rico", "Jamaica", "Haiti" };
            var regionNorteamerica = new[] { "Estados Unidos", "Canada", "Mexico" };
            var regionCentroamerica = new[] { "Panama", "Costa Rica", "Guatemala", "Honduras", "El Salvador", "Nicaragua" };
            var regionSudamerica = new[] { "Colombia", "Venezuela", "Ecuador", "Peru", "Brasil", "Argentina", "Chile" };
            var regionEuropa = new[] { "Espana", "Francia", "Alemania", "Italia", "Reino Unido", "Portugal" };

            var origenRegion = ObtenerRegion(origen.Pais, regionCaribe, regionNorteamerica, regionCentroamerica, regionSudamerica, regionEuropa);
            var destinoRegion = ObtenerRegion(destino.Pais, regionCaribe, regionNorteamerica, regionCentroamerica, regionSudamerica, regionEuropa);

            if (origenRegion == destinoRegion) return "Regional";
            if ((origenRegion == "Caribe" || origenRegion == "Norteamerica" || origenRegion == "Sudamerica") && destinoRegion == "Europa") return "Intercontinental";
            if ((destinoRegion == "Caribe" || destinoRegion == "Norteamerica" || destinoRegion == "Sudamerica") && origenRegion == "Europa") return "Intercontinental";
            if (duracionMinutos > 360) return "Intercontinental";
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

        private static string FormatearDuracion(int minutos)
        {
            var horas = minutos / 60;
            var mins = minutos % 60;
            if (horas > 0 && mins > 0) return $"{horas}h {mins}m";
            else if (horas > 0) return $"{horas}h";
            else return $"{mins}m";
        }

        private static string FormatearHora(TimeSpan hora)
        {
            var hh = hora.Hours;
            var mm = hora.Minutes;
            var periodo = hh >= 12 ? "PM" : "AM";
            var displayHour = hh == 0 ? 12 : (hh > 12 ? hh - 12 : hh);
            return $"{displayHour}:{mm:D2} {periodo}";
        }

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
