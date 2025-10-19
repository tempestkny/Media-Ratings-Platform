using Media_Ratings_Platform.Models;
using System.Text.Json.Serialization;

namespace Media_Ratings_Platform.Models
{
    public class MediaEntry
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaType MediaType { get; set; }
        public int ReleaseYear { get; set; }
        public string[] Genres { get; set; } = Array.Empty<string>();
        public int AgeRestriction { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
