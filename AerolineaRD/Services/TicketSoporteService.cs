using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class TicketSoporteService : ITicketSoporteService
    {
        private readonly ITicketSoporteRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketSoporteService(ITicketSoporteRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<TicketSoporteResponseDto> CrearTicketAsync(CrearTicketDto dto)
        {
            var ticket = _mapper.Map<TicketSoporte>(dto);
            ticket.FechaCreacion = DateTime.Now;

            await _ticketRepository.AddAsync(ticket);
            await _ticketRepository.SaveAsync();

            return _mapper.Map<TicketSoporteResponseDto>(ticket);
        }

        public async Task<List<TicketSoporteResponseDto>> ObtenerTicketsPorClienteAsync(int idCliente)
        {
            var tickets = await _ticketRepository.ObtenerPorClienteAsync(idCliente);
            return _mapper.Map<List<TicketSoporteResponseDto>>(tickets);
        }

        public async Task<List<TicketSoporteResponseDto>> ObtenerTicketsAbiertosAsync()
        {
            var tickets = await _ticketRepository.ObtenerTicketsAbiertosiAsync();
            return _mapper.Map<List<TicketSoporteResponseDto>>(tickets);
        }

        public async Task<bool> ActualizarEstadoTicketAsync(ActualizarTicketDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(dto.IdTicket);
            if (ticket == null)
                return false;

            ticket.Estado = dto.Estado;
            if (dto.Estado == "Cerrado" || dto.Estado == "Resuelto")
            {
                ticket.FechaCierre = DateTime.Now;
            }

            _ticketRepository.Update(ticket);
            await _ticketRepository.SaveAsync();

            return true;
        }
    }
}