using AerolineaRD.Data.DTOs;
using AerolineaRD.Entity;
using AerolineaRD.Repositories.interfaces;
using AerolineaRD.Services.interfaces;
using AutoMapper;

namespace AerolineaRD.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<List<ClienteDto>> ObtenerTodosAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return _mapper.Map<List<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
        }

        public async Task<ClienteConReservasDto?> ObtenerClienteConReservasAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return null;

            var dto = _mapper.Map<ClienteConReservasDto>(cliente);
            dto.TotalReservas = cliente.Reservas?.Count ?? 0;
            return dto;
        }

        public async Task<ClienteDto?> ObtenerPorUserIdAsync(string userId)
        {
            var cliente = await _clienteRepository.ObtenerClientePorUserIdAsync(userId);
            return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
        }

        public async Task<ClienteDto?> ObtenerPorEmailAsync(string email)
        {
            var cliente = await _clienteRepository.ObtenerClientePorEmailAsync(email);
            return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
        }

        public async Task<ClienteDto> CrearAsync(CrearClienteDto dto)
        {
            // Validar que el email no exista
            var existe = await _clienteRepository.ObtenerClientePorEmailAsync(dto.Email);
            if (existe != null)
                throw new InvalidOperationException("Ya existe un cliente con ese email.");

            var cliente = _mapper.Map<Cliente>(dto);
            await _clienteRepository.AddAsync(cliente);
            await _clienteRepository.SaveAsync();

            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<ClienteDto> ActualizarAsync(int id, ActualizarClienteDto dto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException("Cliente no encontrado.");

            // Validar email único si se está actualizando
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != cliente.Email)
            {
                var existe = await _clienteRepository.ObtenerClientePorEmailAsync(dto.Email);
                if (existe != null)
                    throw new InvalidOperationException("Ya existe un cliente con ese email.");
            }

            _mapper.Map(dto, cliente);
            _clienteRepository.Update(cliente);
            await _clienteRepository.SaveAsync();

            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return false;

            _clienteRepository.Delete(cliente);
            await _clienteRepository.SaveAsync();
            return true;
        }
    }
}