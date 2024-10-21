using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


[Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;
        private readonly FirestoreService _firestoreService;
        private readonly IHubContext<ChatBackend.Services.ChatHub> _chatHub;

        public ChatController(FirebaseService firebaseService, FirestoreService firestoreService, IHubContext<ChatBackend.Services.ChatHub> chatHub)
        {
            _firebaseService = firebaseService;
            _firestoreService = firestoreService;
            _chatHub = chatHub;
        }

        // Retrieve chat messages from Firebase Firestore
        [HttpGet("messages")]
        public async Task<IActionResult> GetChatMessages([FromHeader(Name = "Authorization")] string token)
        {

        var result = await _firebaseService.VerifyFirebaseTokenAsync(token);
        if (!result.IsValid)
        {
            return Unauthorized(result.ErrorMessage); // Return 401 Unauthorized if the token is invalid
        }

        var messages = await _firestoreService.GetChatMessagesAsync();
            return Ok(messages);
        }

        // Send a new message via SignalR and save it to Firestore
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatBackend.Models.ChatMessageModel model, [FromHeader(Name = "Authorization")] string token)
        {
        var result = await _firebaseService.VerifyFirebaseTokenAsync(token);
        if (!result.IsValid)
        {
            return Unauthorized(result.ErrorMessage); // Return 401 Unauthorized if the token is invalid
        }

        // Ensure message is valid
        if (string.IsNullOrEmpty(model.User) || string.IsNullOrEmpty(model.Content))
            {
                return BadRequest("Invalid message");
            }

            // Save message to Firestore
            await _firestoreService.AddChatMessageAsync(model.User, model.Content);

            // Send message to all connected SignalR clients
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", model.User, model.Content);

            return Ok();
        }
    }

    
