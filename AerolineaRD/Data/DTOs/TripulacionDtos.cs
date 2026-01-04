namespace AerolineaRD.Data.DTOs
{
    // ========== PERSONAL DTOs ==========
    
    public class PersonalDto
    {
   public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
 public string Rol { get; set; } = null!;
        public string? Licencia { get; set; }
        public string? CertificacionesAeronave { get; set; }
     public int TiempoDescansoMinutos { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? UltimoVueloFin { get; set; }
        public DateTime FechaContratacion { get; set; }
        public bool Activo { get; set; }
    public string NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class CrearPersonalDto
    {
    public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
  public string Rol { get; set; } = null!;
        // "Piloto", "Copiloto", "Sobrecargo Jefe", "Sobrecargo"
        public string? Licencia { get; set; }
        public string? CertificacionesAeronave { get; set; }
      public int TiempoDescansoMinutos { get; set; } = 480;
    }

    public class ActualizarPersonalDto
    {
    public int Id { get; set; }
     public string? Nombre { get; set; }
        public string? Apellido { get; set; }
     public string? Rol { get; set; }
        public string? Licencia { get; set; }
 public string? CertificacionesAeronave { get; set; }
   public int? TiempoDescansoMinutos { get; set; }
   public string? Estado { get; set; }
  }

    // ========== EQUIPO DTOs ==========
    
    public class EquipoDto
    {
     public int Id { get; set; }
   public string Nombre { get; set; } = null!;
 public string Codigo { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoVueloFin { get; set; }
        public DateTime? DisponibleDesde { get; set; }
        public bool Activo { get; set; }
        public List<PersonalDto> Miembros { get; set; } = new();
      public AsignacionAeronaveDto? AsignacionActual { get; set; }
    }

    public class EquipoDetalleDto : EquipoDto
    {
  public PersonalDto? Piloto { get; set; }
     public PersonalDto? Copiloto { get; set; }
        public PersonalDto? SobrecargoJefe { get; set; }
        public List<PersonalDto> Sobrecargos { get; set; } = new();
        public bool EsEquipoCompleto { get; set; }
    public string? MensajeValidacion { get; set; }
    }

 public class CrearEquipoDto
   {
        public string Nombre { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public List<int> IdsPersonal { get; set; } = new();
    }

 public class ActualizarEquipoDto
    {
        public int Id { get; set; }
  public string? Nombre { get; set; }
        public string? Codigo { get; set; }
  public List<int>? IdsPersonal { get; set; }
    }

 // ========== ASIGNACIÓN DTOs ==========
    
    public class AsignacionAeronaveDto
    {
    public int Id { get; set; }
        public int IdEquipo { get; set; }
 public string Matricula { get; set; } = null!;
     public DateTime FechaAsignacion { get; set; }
        public DateTime? FechaDesasignacion { get; set; }
  public bool Activa { get; set; }
  public string? Observaciones { get; set; }
        public EquipoDto? Equipo { get; set; }
     public AeronaveInfoDto? Aeronave { get; set; }
    }

    public class AsignarEquipoAeronaveDto
    {
    public int IdEquipo { get; set; }
        public string Matricula { get; set; } = null!;
  public string? Observaciones { get; set; }
    }

    public class DesasignarEquipoDto
    {
      public int IdAsignacion { get; set; }
        public string? Observaciones { get; set; }
 }

// ========== VALIDACIÓN DTOs ==========
    
    public class ValidacionEquipoDto
    {
 public bool EsValido { get; set; }
        public List<string> Errores { get; set; } = new();
      public ComposicionEquipoDto? Composicion { get; set; }
    }

    public class ComposicionEquipoDto
    {
        public int TotalPilotos { get; set; }
  public int TotalCopilotos { get; set; }
        public int TotalSobrecargosJefe { get; set; }
        public int TotalSobrecargos { get; set; }
        public int TotalMiembros { get; set; }
    }
}
