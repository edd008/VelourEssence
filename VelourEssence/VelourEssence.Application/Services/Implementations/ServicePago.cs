using AutoMapper;
using VelourEssence.Application.DTOs;
using VelourEssence.Application.Services.Interfaces;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Application.Services.Implementations
{
    public class ServicePago : IServicePago
    {
        private readonly IPedidoService _pedidoService;
        private readonly IRepositoryCarrito _repositoryCarrito;
        private readonly IMapper _mapper;

        public ServicePago(IPedidoService pedidoService, IRepositoryCarrito repositoryCarrito, IMapper mapper)
        {
            _pedidoService = pedidoService;
            _repositoryCarrito = repositoryCarrito;
            _mapper = mapper;
        }

        public async Task<ValidacionTarjetaDto> ValidarTarjetaAsync(PagoTarjetaDto pagoTarjeta)
        {
            var validacion = new ValidacionTarjetaDto { EsValida = true };

            // Validar número de tarjeta
            if (string.IsNullOrWhiteSpace(pagoTarjeta.NumeroTarjeta) || pagoTarjeta.NumeroTarjeta.Length != 16)
            {
                validacion.EsValida = false;
                validacion.Errores.Add("El número de tarjeta debe tener 16 dígitos");
            }
            else if (!ValidarAlgoritmoLuhn(pagoTarjeta.NumeroTarjeta))
            {
                validacion.EsValida = false;
                validacion.Errores.Add("El número de tarjeta no es válido");
            }
            else
            {
                validacion.TipoTarjeta = ObtenerTipoTarjeta(pagoTarjeta.NumeroTarjeta);
            }

            // Validar fecha de expiración
            if (!ValidarFechaExpiracion(pagoTarjeta.FechaExpiracion))
            {
                validacion.EsValida = false;
                validacion.Errores.Add("La fecha de expiración no es válida o ya ha vencido");
            }

            // Validar CVV
            if (!ValidarCVV(pagoTarjeta.CodigoCVV, validacion.TipoTarjeta))
            {
                validacion.EsValida = false;
                validacion.Errores.Add("El código CVV no es válido");
            }

            // Validar nombre del titular
            if (string.IsNullOrWhiteSpace(pagoTarjeta.NombreTitular) || pagoTarjeta.NombreTitular.Length < 2)
            {
                validacion.EsValida = false;
                validacion.Errores.Add("El nombre del titular es requerido");
            }

            return validacion;
        }

        public async Task<ResultadoPagoDto> ProcesarPagoTarjetaAsync(PagoTarjetaDto pagoTarjeta)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] ProcesarPagoTarjetaAsync iniciado");
                Console.WriteLine($"[DEBUG SERVICE] IdCarrito={pagoTarjeta.IdCarrito}, TotalPedido={pagoTarjeta.TotalPedido}, MetodoPago={pagoTarjeta.MetodoPago}");
                
                // PASO 1: Validar tarjeta primero (igual que hacía antes)
                Console.WriteLine("[DEBUG SERVICE] Validando tarjeta...");
                var validacion = await ValidarTarjetaAsync(pagoTarjeta);
                if (!validacion.EsValida)
                {
                    Console.WriteLine($"[DEBUG SERVICE] Validación de tarjeta falló: {string.Join(", ", validacion.Errores)}");
                    return new ResultadoPagoDto
                    {
                        Exitoso = false,
                        Mensaje = string.Join(", ", validacion.Errores),
                        MetodoPago = pagoTarjeta.MetodoPago,
                        TotalPagado = 0,
                        FechaPago = DateTime.Now
                    };
                }
                
                Console.WriteLine("[DEBUG SERVICE] Tarjeta válida, procediendo...");
                
                // PASO 2: USAR EXACTAMENTE LA MISMA LÓGICA QUE EFECTIVO
                
                // Obtener o crear el pedido desde el carrito con estado "Pagado" (ID = 2)
                var pedidoId = await ObtenerIdPedidoDelCarrito(pagoTarjeta.IdCarrito);
                if (pedidoId <= 0)
                {
                    Console.WriteLine("[DEBUG SERVICE] Error al crear pedido desde carrito");
                    return new ResultadoPagoDto
                    {
                        Exitoso = false,
                        Mensaje = "Error al crear el pedido desde el carrito",
                        MetodoPago = pagoTarjeta.MetodoPago,
                        TotalPagado = 0,
                        FechaPago = DateTime.Now
                    };
                }

                Console.WriteLine($"[DEBUG SERVICE] Pedido creado exitosamente con ID: {pedidoId}");
                
                var numeroTransaccion = await GenerarNumeroTransaccionAsync();

                // El pedido ya se creó con estado "Pagado" (ID = 2), no necesitamos tabla Pago

                return new ResultadoPagoDto
                {
                    Exitoso = true,
                    Mensaje = "Pago con tarjeta procesado exitosamente",
                    NumeroTransaccion = numeroTransaccion,
                    MetodoPago = pagoTarjeta.MetodoPago,
                    TotalPagado = pagoTarjeta.TotalPedido,
                    FechaPago = DateTime.Now,
                    IdPedido = pedidoId,
                    IdPago = pedidoId // Usar IdPedido como IdPago (EXACTAMENTE IGUAL QUE EFECTIVO)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error al procesar el pago: {ex.Message}");
                return new ResultadoPagoDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al procesar el pago: {ex.Message}",
                    MetodoPago = pagoTarjeta.MetodoPago,
                    TotalPagado = 0,
                    FechaPago = DateTime.Now
                };
            }
        }

        public async Task<ResultadoPagoDto> ProcesarPagoEfectivoAsync(PagoEfectivoDto pagoEfectivo)
        {
            try
            {
                // Validar monto suficiente
                if (pagoEfectivo.MontoPagado < pagoEfectivo.TotalPedido)
                {
                    return new ResultadoPagoDto
                    {
                        Exitoso = false,
                        Mensaje = "El monto pagado es insuficiente",
                        MetodoPago = pagoEfectivo.MetodoPago,
                        TotalPagado = pagoEfectivo.MontoPagado,
                        FechaPago = DateTime.Now
                    };
                }

                // Obtener o crear el pedido desde el carrito
                var pedidoId = await ObtenerIdPedidoDelCarrito(pagoEfectivo.IdCarrito);
                if (pedidoId <= 0)
                {
                    return new ResultadoPagoDto
                    {
                        Exitoso = false,
                        Mensaje = "Error al crear el pedido desde el carrito",
                        MetodoPago = pagoEfectivo.MetodoPago,
                        TotalPagado = 0,
                        FechaPago = DateTime.Now
                    };
                }

                Console.WriteLine($"[DEBUG SERVICE] Pedido creado exitosamente con ID: {pedidoId}");
                
                var numeroTransaccion = await GenerarNumeroTransaccionAsync();

                // El pedido ya se creó con estado "Pagado" (ID = 2), no necesitamos tabla Pago

                return new ResultadoPagoDto
                {
                    Exitoso = true,
                    Mensaje = "Pago en efectivo procesado exitosamente",
                    NumeroTransaccion = numeroTransaccion,
                    MetodoPago = pagoEfectivo.MetodoPago,
                    TotalPagado = pagoEfectivo.TotalPedido,
                    Vuelto = pagoEfectivo.Vuelto,
                    FechaPago = DateTime.Now,
                    IdPedido = pedidoId,
                    IdPago = pedidoId // Usar IdPedido como IdPago
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error al procesar el pago: {ex.Message}");
                return new ResultadoPagoDto
                {
                    Exitoso = false,
                    Mensaje = $"Error al procesar el pago: {ex.Message}",
                    MetodoPago = pagoEfectivo.MetodoPago,
                    TotalPagado = 0,
                    FechaPago = DateTime.Now
                };
            }
        }

        public async Task<PagoInfoDto?> ObtenerPagoPorIdAsync(int idPago)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] Obteniendo información de 'pago' para pedido ID: {idPago}");
                
                // En este nuevo enfoque, idPago es realmente idPedido
                var pedido = await _pedidoService.ObtenerPorIdAsync(idPago);
                if (pedido == null)
                {
                    Console.WriteLine($"[DEBUG SERVICE] No se encontró el pedido con ID: {idPago}");
                    return null;
                }
                
                Console.WriteLine($"[DEBUG SERVICE] Pedido encontrado: {pedido.IdPedido}, Estado: {pedido.Estado}");
                
                // Crear PagoInfoDto desde PedidoDetalleDto
                var pagoInfo = new PagoInfoDto
                {
                    IdPago = pedido.IdPedido,
                    IdPedido = pedido.IdPedido,
                    TipoPago = pedido.MetodoPago ?? "Efectivo",
                    MontoTotal = pedido.Total ?? 0,
                    FechaPago = pedido.Fecha ?? DateTime.Now,
                    EstadoPago = "Completado",
                    FechaPedido = pedido.Fecha ?? DateTime.Now,
                    EstadoPedido = pedido.Estado ?? "Pagado",
                    CantidadProductos = pedido.Productos?.Count ?? 0
                };
                
                Console.WriteLine($"[DEBUG SERVICE] PagoInfoDto creado exitosamente");
                return pagoInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error al obtener el pago por ID: {ex.Message}");
                throw new Exception($"Error al obtener el pago: {ex.Message}", ex);
            }
        }

        public async Task<List<PagoInfoDto>> ObtenerPagosPorUsuarioAsync(int idUsuario)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] Obteniendo pedidos pagados (estado 2) para usuario {idUsuario}");
                
                // Obtener todos los pedidos del usuario
                var todosPedidos = await _pedidoService.ObtenerPorUsuarioAsync(idUsuario);
                Console.WriteLine($"[DEBUG SERVICE] Total pedidos obtenidos: {todosPedidos.Count}");
                
                // Filtrar solo los pedidos con estado "Pagado"
                var pedidosPagados = todosPedidos.Where(p => 
                    p.NombreEstado != null && 
                    (p.NombreEstado.Contains("Pagado") || p.NombreEstado.Contains("Completado"))
                ).ToList();
                Console.WriteLine($"[DEBUG SERVICE] Pedidos pagados filtrados: {pedidosPagados.Count}");
                
                var pagosInfo = new List<PagoInfoDto>();
                
                foreach (var pedido in pedidosPagados)
                {
                    Console.WriteLine($"[DEBUG SERVICE] Procesando pedido ID: {pedido.IdPedido}, Total: {pedido.Total}, Fecha: {pedido.FechaPedido}");
                    
                    var pagoInfo = new PagoInfoDto
                    {
                        IdPago = pedido.IdPedido, // Usar IdPedido como identificador
                        IdPedido = pedido.IdPedido,
                        TipoPago = "Efectivo", // Por defecto efectivo
                        MontoTotal = pedido.Total,
                        FechaPago = pedido.FechaPedido,
                        EstadoPago = "Completado",
                        // Información del pedido
                        FechaPedido = pedido.FechaPedido,
                        EstadoPedido = pedido.NombreEstado ?? "Pagado",
                        CantidadProductos = 1 // Por defecto 1, se puede mejorar después
                    };
                    
                    pagosInfo.Add(pagoInfo);
                }
                
                Console.WriteLine($"[DEBUG SERVICE] DTOs de pago creados: {pagosInfo.Count}");
                return pagosInfo.OrderByDescending(p => p.FechaPago).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error al obtener los pedidos pagados del usuario: {ex.Message}");
                Console.WriteLine($"[ERROR SERVICE] StackTrace: {ex.StackTrace}");
                throw new Exception($"Error al obtener los pedidos pagados del usuario: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerarNumeroTransaccionAsync()
        {
            await Task.CompletedTask;
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }

        public bool ValidarFechaExpiracion(string fechaExpiracion)
        {
            if (string.IsNullOrWhiteSpace(fechaExpiracion) || !fechaExpiracion.Contains('/'))
                return false;

            var partes = fechaExpiracion.Split('/');
            if (partes.Length != 2) return false;

            if (!int.TryParse(partes[0], out int mes) || !int.TryParse(partes[1], out int año))
                return false;

            if (mes < 1 || mes > 12) return false;

            // Convertir año de 2 dígitos a 4 dígitos
            if (año < 100) año += 2000;

            var fechaExp = new DateTime(año, mes, DateTime.DaysInMonth(año, mes));
            return fechaExp >= DateTime.Now.Date;
        }

        public bool ValidarCVV(string cvv, string tipoTarjeta = "")
        {
            if (string.IsNullOrWhiteSpace(cvv)) return false;

            // American Express requiere 4 dígitos, otros requieren 3
            var longitudRequerida = tipoTarjeta.ToUpper() == "AMEX" ? 4 : 3;
            
            return cvv.Length == longitudRequerida && cvv.All(char.IsDigit);
        }

        public string ObtenerTipoTarjeta(string numeroTarjeta)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta)) return "Unknown";

            // Visa: empieza con 4
            if (numeroTarjeta.StartsWith("4")) return "Visa";
            
            // Mastercard: empieza con 5 o entre 2221-2720
            if (numeroTarjeta.StartsWith("5") || 
                (numeroTarjeta.Length >= 4 && int.TryParse(numeroTarjeta.Substring(0, 4), out int prefix) && 
                 prefix >= 2221 && prefix <= 2720))
                return "Mastercard";
            
            // American Express: empieza con 34 o 37
            if (numeroTarjeta.StartsWith("34") || numeroTarjeta.StartsWith("37"))
                return "Amex";

            return "Unknown";
        }

        public bool ValidarAlgoritmoLuhn(string numeroTarjeta)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta) || !numeroTarjeta.All(char.IsDigit))
                return false;

            int suma = 0;
            bool alternar = false;

            // Procesar desde el último dígito hacia el primero
            for (int i = numeroTarjeta.Length - 1; i >= 0; i--)
            {
                int digito = int.Parse(numeroTarjeta[i].ToString());

                if (alternar)
                {
                    digito *= 2;
                    if (digito > 9)
                        digito = (digito % 10) + 1;
                }

                suma += digito;
                alternar = !alternar;
            }

            return suma % 10 == 0;
        }

        private async Task<int> ObtenerIdPedidoDelCarrito(int idCarrito)
        {
            try
            {
                Console.WriteLine($"[DEBUG SERVICE] ObtenerIdPedidoDelCarrito iniciado para carrito: {idCarrito}");
                
                // Obtener el carrito para obtener el ID del usuario
                var carrito = await _repositoryCarrito.GetByIdAsync(idCarrito);
                if (carrito == null || carrito.IdUsuario == null)
                {
                    Console.WriteLine($"[ERROR SERVICE] Carrito no encontrado o sin usuario: {idCarrito}");
                    throw new InvalidOperationException("No se pudo encontrar el carrito o el usuario asociado");
                }

                Console.WriteLine($"[DEBUG SERVICE] Carrito encontrado para usuario: {carrito.IdUsuario.Value}");

                // Crear el pedido desde el carrito con estado "Pagado" (ID = 2)
                Console.WriteLine($"[DEBUG SERVICE] Creando pedido con estado 2 (Pagado)...");
                var pedidoId = await _pedidoService.CrearPedidoDesdeCarritoAsync(idCarrito, carrito.IdUsuario.Value, "", 2);
                
                Console.WriteLine($"[DEBUG SERVICE] ¡PEDIDO CREADO EXITOSAMENTE! ID: {pedidoId}");
                return pedidoId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR SERVICE] Error al crear pedido desde carrito: {ex.Message}");
                Console.WriteLine($"[ERROR SERVICE] StackTrace: {ex.StackTrace}");
                throw new InvalidOperationException($"Error al crear pedido desde carrito: {ex.Message}", ex);
            }
        }
    }
}
