namespace AerolineaRD.Data.DTOs
{
    /// <summary>
    /// Resultado de una operación con validaciones
    /// </summary>
    /// <typeparam name="T">Tipo de datos devueltos en caso de éxito</typeparam>
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<ValidationError> Errors { get; set; } = new();

        public static OperationResult<T> SuccessResult(T data, string? message = null)
        {
            return new OperationResult<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operación exitosa"
            };
        }

        public static OperationResult<T> FailureResult(string message, List<ValidationError>? errors = null)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<ValidationError>()
            };
        }

        public static OperationResult<T> ValidationFailure(List<ValidationError> errors)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = "La validación falló. Por favor, revise los errores.",
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Representa un error de validación específico
    /// </summary>
    public class ValidationError
    {
        public string Campo { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public object? Detalles { get; set; } // Información adicional para el frontend
    
        public static ValidationError Create(string campo, string tipo, string mensaje, object? detalles = null)
        {
            return new ValidationError
            {
                Campo = campo,
                Tipo = tipo,
                Mensaje = mensaje,
                Detalles = detalles
            };
        }
    }

    /// <summary>
    /// Tipos de errores de validación
    /// </summary>
    public static class ValidationErrorType
    {
        public const string AeronaveNoDisponible = "AERONAVE_NO_DISPONIBLE";
        public const string AeropuertoSinCapacidad = "AEROPUERTO_SIN_CAPACIDAD";
        public const string TripulanteNoDisponible = "TRIPULANTE_NO_DISPONIBLE";
        public const string TripulanteSinCertificacion = "TRIPULANTE_SIN_CERTIFICACION";
        public const string EntidadNoEncontrada = "ENTIDAD_NO_ENCONTRADA";
        public const string DatosInvalidos = "DATOS_INVALIDOS";
        public const string EquipoNoDisponible = "EQUIPO_NO_DISPONIBLE"; // ? NUEVO
        public const string AeronaveSinEquipo = "AERONAVE_SIN_EQUIPO"; // ? NUEVO
        public const string AeronaveNoOperativa = "AERONAVE_NO_OPERATIVA"; // ? NUEVO
        public const string FechaInvalida = "FECHA_INVALIDA"; // ? NUEVO
        public const string DuracionInvalida = "DURACION_INVALIDA"; // ? NUEVO
    }
}
