namespace AerolineaRD.Data.DTOs
{
    public class CrearReservaDto
    {
        public int IdPasajero { get; set; }
        public int IdVuelo { get; set; }
        public int IdCliente { get; set; }
        public string? NumAsiento { get; set; }
        public string? Clase { get; set; } // "Economica", "Ejecutiva", "Primera"
        public string? MetodoPago { get; set; }
        public decimal? PrecioTotal { get; set; } 
    }

    public class ModificarReservaDto
    {
        public string CodigoReserva { get; set; } = null!;
        public int? NuevoIdVuelo { get; set; }
        public string? NuevoNumAsiento { get; set; }
    }

    public class ReservaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string? PasajeroNombre { get; set; }
        public string? PasajeroApellido { get; set; }
        public string? NumeroVuelo { get; set; }
        public DateTime FechaVuelo { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        
        // ? NUEVO: Propiedades formateadas con AM/PM
        public string HoraSalidaFormato => FormatearHora(HoraSalida);
        public string HoraLlegadaFormato => FormatearHora(HoraLlegada);

        public string? Origen { get; set; }
        public string? Destino { get; set; }
        public string? NumAsiento { get; set; }
        public string? Clase { get; set; }
        public DateTime FechaReserva { get; set; }
        public string? Estado { get; set; }
        public decimal PrecioTotal { get; set; }
        public FacturaResponseDto? Factura { get; set; }

        // ? Método auxiliar para formatear horas
        private static string FormatearHora(TimeSpan tiempo)
        {
            var hora = tiempo.Hours;
            var minutos = tiempo.Minutes;
            var periodo = hora >= 12 ? "PM" : "AM";
            
            // Convertir a formato 12 horas
            if (hora == 0)
                hora = 12; // Medianoche = 12 AM
            else if (hora > 12)
                hora -= 12; // 13:00 = 1 PM
            
            return $"{hora}:{minutos:D2} {periodo}";
        }
    }
}