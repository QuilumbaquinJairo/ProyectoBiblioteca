using ContabilidadBackend.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class FacturaConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _factory;
    private readonly ILogger<FacturaConsumerService> _logger;

    public FacturaConsumerService(IConfiguration config, IServiceProvider serviceProvider, ILogger<FacturaConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:HostName"] ?? "localhost",
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = await _factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "facturacion_to_contabilidad",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var factura = JsonSerializer.Deserialize<AsientoMensaje>(message);
                if (factura.Monto <= 0)
                {
                    _logger.LogWarning("Se recibió un monto inválido para la factura #{NumeroFactura}: {Total}", factura.Fecha, factura.Monto);
                    return;
                }

                _logger.LogInformation("Mensaje recibido de RabbitMQ: Factura #{NumeroFactura}, Total: {Total}", factura.Fecha, factura.Monto);

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
                db.AsientoContables.Add(new AsientoContable
                {
                    Fecha = factura.Fecha,
                    Referencia = factura.Referencia,
                    TipoOperacion = "Factura",
                    CuentaDebito = "1101-Cuentas por Cobrar",
                    CuentaCredito = "4101-Ventas",
                    Monto = factura.Monto,
                    
                });
                await db.SaveChangesAsync();

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando el mensaje recibido desde RabbitMQ.");
            }
        };

        await channel.BasicConsumeAsync(
            queue: "facturacion_to_contabilidad",
            autoAck: false,
            consumer: consumer);
    }
}

public class AsientoMensaje
{
    public DateTime Fecha { get; set; }

    public string TipoOperacion { get; set; } = null!;

    public int Referencia { get; set; }

    public string CuentaDebito { get; set; } = null!;

    public string CuentaCredito { get; set; } = null!;

    public decimal Monto { get; set; }
}
