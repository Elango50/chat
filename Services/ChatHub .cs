using Microsoft.AspNetCore.SignalR;

namespace ChatBackend.Services
{
    public class ChatHub : Hub
    {
        // Method for sending messages
        public async Task SendMessage(string user, string message)
        {
            // Broadcast message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Optional: You can override methods to handle user connection events
        public override async Task OnConnectedAsync()
        {
            // Logic when a user connects to the hub
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "Welcome to the chat!");
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            // Logic when a user disconnects from the hub
            await base.OnDisconnectedAsync(exception);
            await Clients.All.SendAsync("ReceiveMessage", "System", "A user has left the chat.");
        }
    }
}
