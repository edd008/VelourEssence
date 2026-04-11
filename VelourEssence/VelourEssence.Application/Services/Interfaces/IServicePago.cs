using VelourEssence.Application.DTOs;

namespace VelourEssence.Application.Services.Interfaces
{
    public interface IServicePago
    {
        Task<ValidacionTarjetaDto> ValidarTarjetaAsync(PagoTarjetaDto pagoTarjeta);
        Task<ResultadoPagoDto> ProcesarPagoTarjetaAsync(PagoTarjetaDto pagoTarjeta);
        Task<ResultadoPagoDto> ProcesarPagoEfectivoAsync(PagoEfectivoDto pagoEfectivo);
        
        // Obtiene información de un pago específico (ahora basado en pedidos)
        Task<PagoInfoDto?> ObtenerPagoPorIdAsync(int idPago);

        // Obtiene todos los pagos de un usuario específico (ahora basado en pedidos pagados)
        Task<List<PagoInfoDto>> ObtenerPagosPorUsuarioAsync(int idUsuario);
        
        // Métodos de validación de tarjetas
        Task<string> GenerarNumeroTransaccionAsync();
        bool ValidarFechaExpiracion(string fechaExpiracion);
        bool ValidarCVV(string cvv, string tipoTarjeta = "");
        string ObtenerTipoTarjeta(string numeroTarjeta);
        bool ValidarAlgoritmoLuhn(string numeroTarjeta);
    }
}
