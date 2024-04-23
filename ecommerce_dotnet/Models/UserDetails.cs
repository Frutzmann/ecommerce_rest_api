using System.Text.Json.Serialization;

namespace quest_web.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole? Role { get; set; }
            }
}
