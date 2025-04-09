using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebSocketServer.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> users = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Usuário conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Usuário desconectado: {Context.ConnectionId}");
            users.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SetUsername(string username)
        {
            users[Context.ConnectionId] = username;
            Console.WriteLine($"Usuário {Context.ConnectionId} definiu o nome: {username}");
        }

        public async Task SendMessage(string message)
        {
            var author = users.GetValueOrDefault(Context.ConnectionId, "Anônimo");
            var data = new { author, text = message, authorId = Context.ConnectionId };
            Console.WriteLine($"Mensagem recebida de {author}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", data);
        }
    }
}