using Google.Cloud.Firestore;

public class FirestoreService
{
    private readonly FirestoreDb _firestoreDb;

    public FirestoreService(IConfiguration configuration)
    {
        // Initialize Firestore DB with project ID
        string projectId = configuration["Firebase:ProjectId"];
        _firestoreDb = FirestoreDb.Create(projectId);
    }

    // Retrieve chat messages from Firestore
    public async Task<List<Dictionary<string, object>>> GetChatMessagesAsync()
    {
        try
        {
            // Use OrderByDescending to sort by timestamp in descending order
            Query chatMessagesQuery = _firestoreDb.Collection("chatMessages").OrderByDescending("timestamp");
            QuerySnapshot chatMessagesSnapshot = await chatMessagesQuery.GetSnapshotAsync();

            List<Dictionary<string, object>> chatMessages = new List<Dictionary<string, object>>();
            foreach (DocumentSnapshot documentSnapshot in chatMessagesSnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    Dictionary<string, object> message = documentSnapshot.ToDictionary();
                    chatMessages.Add(message);
                }
            }
            return chatMessages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving chat messages: {ex.Message}");
            return new List<Dictionary<string, object>>(); // Return an empty list in case of an error
        }
    }

    // Add a chat message to Firestore
    public async Task AddChatMessageAsync(string user, string content)
    {
        try
        {
            CollectionReference chatMessagesRef = _firestoreDb.Collection("chatMessages");

            Dictionary<string, object> newMessage = new Dictionary<string, object>
            {
                { "user", user },
                { "content", content },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            await chatMessagesRef.AddAsync(newMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding chat message: {ex.Message}");
        }
    }
}
