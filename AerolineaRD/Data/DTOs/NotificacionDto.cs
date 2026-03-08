namespace AerolineaRD.Data.DTOs
{
    public class NotificacionResponseDto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leida { get; set; }
    }

    public class MarcarNotificacionLeidaDto
    {
        public int IdNotificacion { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de actualización de vuelo con información de notificaciones
    /// </summary>
    public class VueloActualizadoConNotificacionesDto
    {
        public bool Success { get; set; }
        public VueloDetalleDto? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Información sobre los cambios realizados
        /// </summary>
        public CambiosRealizadosDto? Cambios { get; set; }
        
        /// <summary>
        /// Resultado de las notificaciones enviadas
        /// </summary>
        public NotificacionResultadoDto? Notificaciones { get; set; }
    }

    /// <summary>
    /// Detalle de los cambios realizados al vuelo
    /// </summary>
    public class CambiosRealizadosDto
    {
        public bool CambioFecha { get; set; }
        public bool CambioHoraSalida { get; set; }
        public bool CambioHoraLlegada { get; set; }
        public bool CambioEstado { get; set; }
        public string TipoCambio { get; set; } = string.Empty;
        
        // Valores anteriores
        public DateTime? FechaAnterior { get; set; }
        public string? HoraSalidaAnterior { get; set; }
        public string? HoraLlegadaAnterior { get; set; }
        public string? EstadoAnterior { get; set; }
        
        // Valores nuevos
        public DateTime? FechaNueva { get; set; }
        public string? HoraSalidaNueva { get; set; }
        public string? HoraLlegadaNueva { get; set; }
        public string? EstadoNuevo { get; set; }
    }

    /// <summary>
    /// Resultado del envío de notificaciones por email
    /// </summary>
    public class NotificacionResultadoDto
    {
        public bool Enviadas { get; set; }
        public int TotalClientes { get; set; }
        public int EmailsEnviados { get; set; }
        public int EmailsFallidos { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string>? ClientesNotificados { get; set; }
    }
}