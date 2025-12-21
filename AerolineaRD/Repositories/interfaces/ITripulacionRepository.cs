using AerolineaRD.Entity;

namespace AerolineaRD.Repositories.interfaces
{
    public interface ITripulacionRepository : IGenericRepository<Tripulacion>
    {
        Task<List<Tripulacion>> ObtenerPorRolAsync(string rol);
        
        /// <summary>
        /// Verifica si un tripulante está disponible en un horario específico
        /// </summary>
        Task<bool> EstaTripulacionDisponibleAsync(int idTripulacion, DateTime fecha, TimeSpan horaSalida, TimeSpan horaLlegada, int? vueloIdExcluir = null);
        
        /// <summary>
        /// Verifica si un tripulante tiene la certificación para un modelo de aeronave
        /// </summary>
        Task<bool> TieneCertificacionParaAeronaveAsync(int idTripulacion, string modeloAeronave);
    }
}