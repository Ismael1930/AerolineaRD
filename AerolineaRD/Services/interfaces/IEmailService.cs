namespace AerolineaRD.Services.interfaces
{
    /// <summary>
    /// Servicio para envío de correos electrónicos
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpoHtml);

        /// <summary>
        /// Envía un correo a múltiples destinatarios
        /// </summary>
        Task<int> EnviarEmailMasivoAsync(IEnumerable<string> destinatarios, string asunto, string cuerpoHtml);

        /// <summary>
        /// Notifica a los clientes sobre cambios en un vuelo
        /// </summary>
        Task<NotificacionVueloResultado> NotificarCambioVueloAsync(int idVuelo, CambioVueloInfo cambios);
    }

    /// <summary>
    /// Información sobre los cambios realizados a un vuelo
    /// </summary>
    public class CambioVueloInfo
    {
        public string NumeroVuelo { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        
        // Fecha anterior y nueva
        public DateTime? FechaAnterior { get; set; }
        public DateTime? FechaNueva { get; set; }
        
        // Hora de salida anterior y nueva
        public TimeSpan? HoraSalidaAnterior { get; set; }
        public TimeSpan? HoraSalidaNueva { get; set; }
        
        // Hora de llegada anterior y nueva
        public TimeSpan? HoraLlegadaAnterior { get; set; }
        public TimeSpan? HoraLlegadaNueva { get; set; }
        
        // Estado del vuelo
        public string? EstadoAnterior { get; set; }
        public string? EstadoNuevo { get; set; }
        
        // Tipo de cambio
        public TipoCambioVuelo TipoCambio { get; set; }
        
        // Mensaje adicional del administrador
        public string? MensajeAdicional { get; set; }

        public bool HayCambioFecha => FechaAnterior.HasValue && FechaNueva.HasValue && FechaAnterior != FechaNueva;
        public bool HayCambioHoraSalida => HoraSalidaAnterior.HasValue && HoraSalidaNueva.HasValue && HoraSalidaAnterior != HoraSalidaNueva;
        public bool HayCambioHoraLlegada => HoraLlegadaAnterior.HasValue && HoraLlegadaNueva.HasValue && HoraLlegadaAnterior != HoraLlegadaNueva;
        public bool HayCambioEstado => !string.IsNullOrEmpty(EstadoAnterior) && !string.IsNullOrEmpty(EstadoNuevo) && EstadoAnterior != EstadoNuevo;
    }

    public enum TipoCambioVuelo
    {
        Reprogramacion,    // Cambio de fecha/hora
        Retraso,           // Solo retraso en la hora
        Cancelacion,       // Vuelo cancelado
        Adelanto,          // Vuelo adelantado
        CambioEstado       // Cambio de estado general
    }

    /// <summary>
    /// Resultado de la notificación masiva
    /// </summary>
    public class NotificacionVueloResultado
    {
        public bool Exitoso { get; set; }
        public int TotalClientes { get; set; }
        public int EmailsEnviados { get; set; }
        public int EmailsFallidos { get; set; }
        public List<string> Errores { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;
    }
}
