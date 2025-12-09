using System.Text.Json.Serialization;

namespace MauiApp4.DTO
{
    public class ContactDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
        
        [JsonIgnore]
        public string DisplayImage => "user.png";
    }
}