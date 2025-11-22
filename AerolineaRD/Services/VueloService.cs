using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;
using System.Globalization;
using System.Text;

namespace AerolineaRD.Services
{
    public class VueloService : IVueloService
    {
        private readonly IVueloRepository _vueloRepository;
        private readonly IMapper _mapper;

        public VueloService(IVueloRepository vueloRepository, IMapper mapper)
        {
            _vueloRepository = vueloRepository;
            _mapper = mapper;
        }

        public async Task<List<VueloResponseDto>> BuscarVuelosAsync(BuscarVueloDto filtros)
        {
            var vuelos = await _vueloRepository.BuscarVuelosConFiltrosAsync(
                filtros.Origen,
                filtros.Destino,
                filtros.FechaSalida,
                filtros.FechaRegreso,
                filtros.Clase,
                filtros.TipoViaje
            );

            var resultados = vuelos
                .Where(v => v.Aeronave?.Asientos != null && v.Aeronave.Asientos.Any())
                .Select(vuelo =>
                {
                    var vueloDto = _mapper.Map<VueloResponseDto>(vuelo);
                    vueloDto.ClasesDisponibles = CalcularClasesDisponibles(vuelo, filtros.Clase);
                    return vueloDto;
                })
                .ToList();

            return resultados;
        }

        public async Task<VueloResponseDto?> ObtenerVueloPorIdAsync(int id)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(id);

            if (vuelo == null) return null;

            var vueloDto = _mapper.Map<VueloResponseDto>(vuelo);
            vueloDto.ClasesDisponibles = vuelo.Aeronave?.Asientos != null
                ? CalcularClasesDisponibles(vuelo, null)
                : new List<ClaseDisponibilidadDto>();

            return vueloDto;
        }

        public async Task<List<AsientoDisponibleDto>> ObtenerAsientosDisponiblesAsync(int idVuelo, string clase)
        {
            var vuelo = await _vueloRepository.ObtenerVueloConDetallesAsync(idVuelo);

            if (vuelo?.Aeronave?.Asientos == null)
                return new List<AsientoDisponibleDto>();

            // Normalizar la clase para comparación
            var claseNormalizada = NormalizarTexto(clase);

            // Obtener asientos ocupados
            var asientosOcupados = (vuelo.Reservas ?? Enumerable.Empty<Reserva>())
                .Where(r => r.Estado == "Confirmada" && !string.IsNullOrEmpty(r.NumAsiento))
                .Select(r => r.NumAsiento!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Filtrar asientos por clase y mapear a DTO
            var asientos = vuelo.Aeronave.Asientos
                .Where(a => NormalizarTexto(a.Clase ?? "Economica") == claseNormalizada)
                .OrderBy(a => a.NumeroAsiento)
                .Select(a => new AsientoDisponibleDto
                {
                    Numero = a.NumeroAsiento ?? "",
                    Clase = a.Clase ?? "Economica",
                    Disponible = !asientosOcupados.Contains(a.NumeroAsiento ?? ""),
                    Fila = ExtraerFila(a.NumeroAsiento ?? ""),
                    Columna = ExtraerColumna(a.NumeroAsiento ?? "")
                })
                .ToList();

            return asientos;
        }

        private static int ExtraerFila(string numeroAsiento)
        {
            if (string.IsNullOrEmpty(numeroAsiento))
                return 0;

            var numeros = new string(numeroAsiento.Where(char.IsDigit).ToArray());
            return int.TryParse(numeros, out var fila) ? fila : 0;
        }

        private static string ExtraerColumna(string numeroAsiento)
        {
            if (string.IsNullOrEmpty(numeroAsiento))
                return "";

            return new string(numeroAsiento.Where(char.IsLetter).ToArray());
        }

        private List<ClaseDisponibilidadDto> CalcularClasesDisponibles(Vuelo vuelo, string? filtroClase)
        {
            if (vuelo.Aeronave?.Asientos == null || !vuelo.Aeronave.Asientos.Any())
                return new List<ClaseDisponibilidadDto>();

            var asientosOcupados = (vuelo.Reservas ?? Enumerable.Empty<Reserva>())
                .Where(r => r.Estado == "Confirmada" && !string.IsNullOrEmpty(r.NumAsiento))
                .Select(r => r.NumAsiento!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var clasesCalculadas = vuelo.Aeronave.Asientos
                .GroupBy(a => a.Clase ?? "Economica")
                .Select(g => new ClaseDisponibilidadDto
                {
                    Clase = g.Key,
                    AsientosDisponibles = g.Count(a => !asientosOcupados.Contains(a.NumeroAsiento ?? "")),
                    Precio = CalcularPrecioPorClase(vuelo.PrecioBase, g.Key)
                })
                .Where(c => c.AsientosDisponibles > 0)
                .ToList();

            if (!string.IsNullOrEmpty(filtroClase))
            {
                var filtroNormalizado = NormalizarTexto(filtroClase);
                clasesCalculadas = clasesCalculadas
                    .Where(c => NormalizarTexto(c.Clase) == filtroNormalizado)
                    .ToList();
            }

            return clasesCalculadas;
        }

        private static decimal CalcularPrecioPorClase(decimal precioBase, string clase)
        {
            return clase switch
            {
                "Primera" => precioBase * 3.5m,
                "Ejecutiva" => precioBase * 2.0m,
                "Economica" => precioBase,
                _ => precioBase
            };
        }

        private static string NormalizarTexto(string? texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in textoNormalizado)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();
        }
    }
}