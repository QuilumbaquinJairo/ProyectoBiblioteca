using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class RabbitMqService
{
    private readonly ConnectionFactory _factory;

    public RabbitMqService(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost"
        };
    }

    public async Task PublishFacturaAsync(object mensaje)
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        string queueName = "facturacion_to_contabilidad";

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensaje));

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: body
        );
    }
}
