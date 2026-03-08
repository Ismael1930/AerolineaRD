using System.Net;
using System.Net.Mail;
using AerolineaRD.Data;
using AerolineaRD.Services.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AerolineaRD.Services
{
    /// <summary>
    /// Configuración SMTP para el envío de correos
    /// </summary>
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "AerolineaRD";
        public bool EnableSsl { get; set; } = true;
    }

    /// <summary>
    /// Servicio de envío de correos electrónicos usando SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly AppDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<SmtpSettings> smtpSettings,
            AppDbContext context,
            ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                using var client = CrearSmtpClient();
                using var mensaje = CrearMensaje(destinatario, asunto, cuerpoHtml);

                await client.SendMailAsync(mensaje);
                _logger.LogInformation($"?? Email enviado exitosamente a {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"? Error al enviar email a {destinatario}: {ex.Message}");
                return false;
            }
        }

        public async Task<int> EnviarEmailMasivoAsync(IEnumerable<string> destinatarios, string asunto, string cuerpoHtml)
        {
            int enviados = 0;

            foreach (var destinatario in destinatarios)
            {
                if (await EnviarEmailAsync(destinatario, asunto, cuerpoHtml))
                {
                    enviados++;
                }
                
                // Pequeña pausa para no saturar el servidor SMTP
                await Task.Delay(100);
            }

            return enviados;
        }

        public async Task<NotificacionVueloResultado> NotificarCambioVueloAsync(int idVuelo, CambioVueloInfo cambios)
        {
            var resultado = new NotificacionVueloResultado();

            try
            {
                _logger.LogInformation($"?? Iniciando notificación para vuelo ID: {idVuelo}");

                // Obtener todas las reservas activas del vuelo con información del cliente
                var reservasConClientes = await _context.Reservas
                    .Include(r => r.Cliente)
                    .Include(r => r.Pasajero)
                    .Where(r => r.IdVuelo == idVuelo && r.Estado != "Cancelada")
                    .ToListAsync();

                _logger.LogInformation($"?? Reservas encontradas para vuelo {idVuelo}: {reservasConClientes.Count}");

                if (!reservasConClientes.Any())
                {
                    resultado.Exitoso = true;
                    resultado.Mensaje = "No hay reservas activas para este vuelo. No se enviaron notificaciones.";
                    _logger.LogWarning($"?? Vuelo {idVuelo}: {resultado.Mensaje}");
                    return resultado;
                }

                // Obtener emails únicos de los clientes
                var clientesConEmail = reservasConClientes
                    .Where(r => r.Cliente != null && !string.IsNullOrEmpty(r.Cliente.Email))
                    .Select(r => new
                    {
                        Email = r.Cliente!.Email!,
                        NombreCliente = r.Cliente.Nombre ?? "Estimado cliente",
                        NombrePasajero = r.Pasajero != null ? $"{r.Pasajero.Nombre} {r.Pasajero.Apellido}" : null,
                        CodigoReserva = r.Codigo
                    })
                    .DistinctBy(c => c.Email)
                    .ToList();

                resultado.TotalClientes = clientesConEmail.Count;
                _logger.LogInformation($"?? Clientes con email para notificar: {resultado.TotalClientes}");

                if (resultado.TotalClientes == 0)
                {
                    resultado.Exitoso = true;
                    resultado.Mensaje = "Los clientes con reservas no tienen email registrado.";
                    _logger.LogWarning($"?? Vuelo {idVuelo}: {resultado.Mensaje}");
                    return resultado;
                }

                // Generar el asunto según el tipo de cambio
                var asunto = GenerarAsuntoEmail(cambios);
                _logger.LogInformation($"?? Asunto del email: {asunto}");

                // Enviar email a cada cliente
                foreach (var cliente in clientesConEmail)
                {
                    try
                    {
                        _logger.LogInformation($"?? Enviando email a: {cliente.Email}");
                        var cuerpoHtml = GenerarCuerpoEmailCambioVuelo(cliente.NombreCliente, cliente.CodigoReserva, cambios);
                        
                        if (await EnviarEmailAsync(cliente.Email, asunto, cuerpoHtml))
                        {
                            resultado.EmailsEnviados++;
                            _logger.LogInformation($"? Email enviado exitosamente a {cliente.Email}");
                        }
                        else
                        {
                            resultado.EmailsFallidos++;
                            resultado.Errores.Add($"Fallo al enviar a {cliente.Email}");
                            _logger.LogError($"? Fallo al enviar email a {cliente.Email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado.EmailsFallidos++;
                        resultado.Errores.Add($"Error con {cliente.Email}: {ex.Message}");
                        _logger.LogError(ex, $"? Error enviando email a {cliente.Email}: {ex.Message}");
                    }
                }

                resultado.Exitoso = resultado.EmailsEnviados > 0;
                resultado.Mensaje = $"Se enviaron {resultado.EmailsEnviados} de {resultado.TotalClientes} notificaciones. " +
                                   (resultado.EmailsFallidos > 0 ? $"Fallaron {resultado.EmailsFallidos}." : "");

                _logger.LogInformation($"?? Resultado notificación vuelo {cambios.NumeroVuelo}: {resultado.Mensaje}");
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = $"Error al procesar notificaciones: {ex.Message}";
                resultado.Errores.Add(ex.Message);
                _logger.LogError(ex, $"? Error crítico al notificar cambio de vuelo {idVuelo}: {ex.Message}");
            }

            return resultado;
        }

        #region Métodos privados

        private SmtpClient CrearSmtpClient()
        {
            var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            return client;
        }

        private MailMessage CrearMensaje(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            return mensaje;
        }

        private static string GenerarAsuntoEmail(CambioVueloInfo cambios)
        {
            return cambios.TipoCambio switch
            {
                TipoCambioVuelo.Cancelacion => $"?? IMPORTANTE: Vuelo {cambios.NumeroVuelo} Cancelado",
                TipoCambioVuelo.Retraso => $"?? Aviso: Cambio de horario - Vuelo {cambios.NumeroVuelo}",
                TipoCambioVuelo.Adelanto => $"?? Aviso: Vuelo {cambios.NumeroVuelo} Adelantado",
                TipoCambioVuelo.Reprogramacion => $"?? Vuelo {cambios.NumeroVuelo} Reprogramado",
                _ => $"?? Actualización de su vuelo {cambios.NumeroVuelo}"
            };
        }

        private static string GenerarCuerpoEmailCambioVuelo(string nombreCliente, string codigoReserva, CambioVueloInfo cambios)
        {
            var colorPrincipal = "#1e3a5f"; // Azul aerolínea
            var colorSecundario = "#e74c3c"; // Rojo para alertas
            var colorExito = "#27ae60"; // Verde

            var mensajePrincipal = cambios.TipoCambio switch
            {
                TipoCambioVuelo.Cancelacion => "Lamentamos informarle que su vuelo ha sido <strong style='color: #e74c3c;'>CANCELADO</strong>.",
                TipoCambioVuelo.Retraso => "Le informamos que su vuelo ha sufrido un <strong>cambio de horario</strong>.",
                TipoCambioVuelo.Adelanto => "Le informamos que su vuelo ha sido <strong>adelantado</strong>.",
                TipoCambioVuelo.Reprogramacion => "Le informamos que su vuelo ha sido <strong>reprogramado</strong>.",
                _ => "Le informamos que hay actualizaciones importantes sobre su vuelo."
            };

            var detallesCambios = new System.Text.StringBuilder();

            if (cambios.HayCambioFecha)
            {
                detallesCambios.AppendLine($@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'><strong>?? Fecha:</strong></td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-decoration: line-through; color: #999;'>{cambios.FechaAnterior:dddd, dd 'de' MMMM 'de' yyyy}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; color: {colorSecundario}; font-weight: bold;'>{cambios.FechaNueva:dddd, dd 'de' MMMM 'de' yyyy}</td>
                </tr>");
            }

            if (cambios.HayCambioHoraSalida)
            {
                detallesCambios.AppendLine($@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'><strong>?? Hora de Salida:</strong></td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-decoration: line-through; color: #999;'>{FormatearHora(cambios.HoraSalidaAnterior!.Value)}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; color: {colorSecundario}; font-weight: bold;'>{FormatearHora(cambios.HoraSalidaNueva!.Value)}</td>
                </tr>");
            }

            if (cambios.HayCambioHoraLlegada)
            {
                detallesCambios.AppendLine($@"
                <tr>
                    <td style='padding: 10px; border-bottom: 1px solid #eee;'><strong>?? Hora de Llegada:</strong></td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; text-decoration: line-through; color: #999;'>{FormatearHora(cambios.HoraLlegadaAnterior!.Value)}</td>
                    <td style='padding: 10px; border-bottom: 1px solid #eee; color: {colorSecundario}; font-weight: bold;'>{FormatearHora(cambios.HoraLlegadaNueva!.Value)}</td>
                </tr>");
            }

            var mensajeAdicionalHtml = !string.IsNullOrEmpty(cambios.MensajeAdicional)
                ? $@"<div style='background-color: #fff3cd; border: 1px solid #ffc107; border-radius: 5px; padding: 15px; margin-top: 20px;'>
                        <strong>?? Mensaje de la aerolínea:</strong><br/>
                        {cambios.MensajeAdicional}
                     </div>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: #f8f9fa;'>
        
        <!-- Header -->
        <div style='background-color: {colorPrincipal}; padding: 20px; text-align: center;'>
            <h1 style='color: white; margin: 0; font-size: 24px;'>?? AerolineaRD</h1>
            <p style='color: #ccc; margin: 5px 0 0 0; font-size: 14px;'>Notificación de cambio de vuelo</p>
        </div>

        <!-- Contenido -->
        <div style='padding: 30px; background-color: white;'>
            <p style='font-size: 16px;'>Estimado/a <strong>{nombreCliente}</strong>,</p>
            
            <p style='font-size: 16px;'>{mensajePrincipal}</p>

            <!-- Información del vuelo -->
            <div style='background-color: #f8f9fa; border-radius: 8px; padding: 20px; margin: 20px 0;'>
                <h3 style='color: {colorPrincipal}; margin-top: 0;'>?? Detalles del Vuelo</h3>
                <table style='width: 100%;'>
                    <tr>
                        <td style='padding: 5px 0;'><strong>Número de Vuelo:</strong></td>
                        <td style='padding: 5px 0;'>{cambios.NumeroVuelo}</td>
                    </tr>
                    <tr>
                        <td style='padding: 5px 0;'><strong>Ruta:</strong></td>
                        <td style='padding: 5px 0;'>{cambios.Origen} ? {cambios.Destino}</td>
                    </tr>
                    <tr>
                        <td style='padding: 5px 0;'><strong>Código de Reserva:</strong></td>
                        <td style='padding: 5px 0;'><span style='background-color: {colorPrincipal}; color: white; padding: 3px 8px; border-radius: 4px;'>{codigoReserva}</span></td>
                    </tr>
                </table>
            </div>

            <!-- Tabla de cambios -->
            {(detallesCambios.Length > 0 ? $@"
            <h3 style='color: {colorPrincipal};'>?? Cambios Realizados</h3>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                <thead>
                    <tr style='background-color: #f8f9fa;'>
                        <th style='padding: 10px; text-align: left;'>Campo</th>
                        <th style='padding: 10px; text-align: left;'>Anterior</th>
                        <th style='padding: 10px; text-align: left;'>Nuevo</th>
                    </tr>
                </thead>
                <tbody>
                    {detallesCambios}
                </tbody>
            </table>" : "")}

            {mensajeAdicionalHtml}

            <!-- Acciones -->
            <div style='margin-top: 30px; padding: 20px; background-color: #e8f4fd; border-radius: 8px;'>
                <h4 style='margin-top: 0; color: {colorPrincipal};'>¿Necesita ayuda?</h4>
                <p style='margin-bottom: 0;'>
                    Si tiene alguna pregunta o necesita asistencia, por favor contacte nuestro servicio al cliente:
                    <br/>?? <a href='mailto:soporte@aerolineard.com'>soporte@aerolineard.com</a>
                    <br/>?? +1 (809) 555-0100
                </p>
            </div>
        </div>

        <!-- Footer -->
        <div style='background-color: #2c3e50; padding: 20px; text-align: center;'>
            <p style='color: #bdc3c7; margin: 0; font-size: 12px;'>
                Este es un correo automático. Por favor no responda a este mensaje.
                <br/>© {DateTime.Now.Year} AerolineaRD. Todos los derechos reservados.
            </p>
        </div>
    </div>
</body>
</html>";
        }

        private static string FormatearHora(TimeSpan hora)
        {
            var hh = hora.Hours;
            var mm = hora.Minutes;
            var periodo = hh >= 12 ? "PM" : "AM";
            var displayHour = hh == 0 ? 12 : (hh > 12 ? hh - 12 : hh);
            return $"{displayHour}:{mm:D2} {periodo}";
        }

        #endregion
    }
}
