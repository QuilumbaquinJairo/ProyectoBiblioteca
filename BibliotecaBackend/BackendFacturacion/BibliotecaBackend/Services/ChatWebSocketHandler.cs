using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BibliotecaBackend.Services;

public class ChatWebSocketHandler
{
    class ChatClient
    {
        public WebSocket Socket { get; set; } = null!;
        public string Rol { get; set; } = null!; // "cliente" o "agente"
        public string Id { get; set; } = null!;   // clienteId o agenteId
    }

    private static readonly ConcurrentDictionary<string, ChatClient> _clientes = new(); // clienteId -> ChatClient
    private static readonly ConcurrentDictionary<string, ChatClient> _agentes = new();  // agenteId -> ChatClient
    private static readonly ConcurrentDictionary<string, string> _asignaciones = new(); // clienteId -> agenteId

    // Agrega estos logs a tu ChatWebSocketHandler.cs

    public async Task HandleAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var rol = context.Request.Query["rol"].ToString();
        var id = context.Request.Query["id"].ToString();

        // === DEBUG LOGS ===
        Console.WriteLine($"=== WEBSOCKET CONNECTION ===");
        Console.WriteLine($"Rol recibido: '{rol}'");
        Console.WriteLine($"ID recibido: '{id}'");
        Console.WriteLine($"Rol es vacío: {string.IsNullOrWhiteSpace(rol)}");
        Console.WriteLine($"ID es vacío: {string.IsNullOrWhiteSpace(id)}");
        Console.WriteLine($"============================");

        if (string.IsNullOrWhiteSpace(rol) || string.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("ERROR: Rol o ID vacíos, cerrando conexión");
            context.Response.StatusCode = 400;
            return;
        }

        var client = new ChatClient
        {
            Socket = socket,
            Rol = rol,
            Id = id
        };

        if (rol == "Administrador")
        {
            Console.WriteLine($"Registrando AGENTE: {id}");
            _agentes.TryAdd(id, client);
            Console.WriteLine($"Total agentes: {_agentes.Count}");

            // NUEVO: asignar a clientes pendientes
            foreach (var cliente in _clientes)
            {
                if (!_asignaciones.ContainsKey(cliente.Key))
                {
                    _asignaciones.TryAdd(cliente.Key, id);
                    Console.WriteLine($"Cliente {cliente.Key} asignado a nuevo agente {id}");
                }
            }
        }

        else if (rol == "cliente")
        {
            Console.WriteLine($"Registrando CLIENTE: {id}");
            _clientes.TryAdd(id, client);

            var agenteDisponible = _agentes.Values.FirstOrDefault(a => a.Socket.State == WebSocketState.Open);
            if (agenteDisponible != null)
            {
                _asignaciones.TryAdd(id, agenteDisponible.Id);
                Console.WriteLine($"Cliente {id} asignado a agente {agenteDisponible.Id}");
            }
            else
            {
                Console.WriteLine($"No hay agentes disponibles para cliente {id}");
            }
            Console.WriteLine($"Total clientes: {_clientes.Count}");
            Console.WriteLine($"Total asignaciones: {_asignaciones.Count}");
        }

        await ReceiveMessages(client);

        // Cleanup
        if (rol == "Administrador")
        {
            _agentes.TryRemove(id, out _);
            Console.WriteLine($"Agente {id} desconectado");
        }
        if (rol == "cliente")
        {
            _clientes.TryRemove(id, out _);
            _asignaciones.TryRemove(id, out _);
            Console.WriteLine($"Cliente {id} desconectado");
        }
    }

    private async Task EnviarMensaje(ChatClient remitente, ChatMessage mensaje)
    {
        ChatClient? receptor = null;

        Console.WriteLine($"=== ENVIANDO MENSAJE ===");
        Console.WriteLine($"Remitente: {remitente.Id} (Rol: {remitente.Rol})");
        Console.WriteLine($"Mensaje: {mensaje.Mensaje}");
        Console.WriteLine($"DestinoId: {mensaje.DestinoId}");

        if (remitente.Rol == "cliente" && _asignaciones.TryGetValue(remitente.Id, out var agenteId))
        {
            Console.WriteLine($"Cliente {remitente.Id} tiene asignado agente {agenteId}");
            receptor = _agentes.GetValueOrDefault(agenteId);
            Console.WriteLine($"Agente encontrado: {receptor != null}");
        }
        else if (remitente.Rol == "Administrador")
        {
            Console.WriteLine($"Agente quiere enviar a: {mensaje.DestinoId}");
            receptor = _clientes.GetValueOrDefault(mensaje.DestinoId ?? "");
            Console.WriteLine($"Cliente encontrado: {receptor != null}");
        }

        if (receptor is not null && receptor.Socket.State == WebSocketState.Open)
        {
            var payload = JsonSerializer.Serialize(new
            {
                de = remitente.Id,
                mensaje = mensaje.Mensaje
            });

            Console.WriteLine($"Enviando payload: {payload}");

            var buffer = Encoding.UTF8.GetBytes(payload);
            await receptor.Socket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            Console.WriteLine("Mensaje enviado exitosamente");
        }
        else
        {
            Console.WriteLine("ERROR: No se pudo enviar el mensaje - receptor no encontrado o desconectado");
        }
        Console.WriteLine("=======================");
    }

    // En tu ChatWebSocketHandler.cs, reemplaza el método ReceiveMessages:

    private async Task ReceiveMessages(ChatClient client)
    {
        var buffer = new byte[4096];

        while (client.Socket.State == WebSocketState.Open)
        {
            var result = await client.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrado por el cliente", CancellationToken.None);
                return;
            }

            var raw = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // === DEBUG LOGS ===
            Console.WriteLine($"=== MENSAJE RECIBIDO ===");
            Console.WriteLine($"Cliente: {client.Id} (Rol: {client.Rol})");
            Console.WriteLine($"Raw data: '{raw}'");
            Console.WriteLine($"Length: {raw.Length}");
            Console.WriteLine("========================");

            var parsed = JsonSerializer.Deserialize<ChatMessage>(raw);
            if (parsed is not null)
            {
                Console.WriteLine($"=== MENSAJE PARSEADO ===");
                Console.WriteLine($"Mensaje: '{parsed.Mensaje}'");
                Console.WriteLine($"DestinoId: '{parsed.DestinoId}'");
                Console.WriteLine("========================");

                await EnviarMensaje(client, parsed);
            }
            else
            {
                Console.WriteLine("ERROR: No se pudo parsear el mensaje JSON");
            }
        }
    }


    public bool HayAgentesConectados()
    {
        return _agentes.Values.Any(a => a.Socket.State == WebSocketState.Open);
    }

    private class ChatMessage
    {
        [JsonPropertyName("destinoId")]
        public string? DestinoId { get; set; } // solo lo llena el agente
        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }
    }
}
