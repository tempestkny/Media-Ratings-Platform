namespace Media_Ratings_Platform.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int MediaId { get; set; }      
        public int UserId { get; set; }       
        public int Stars { get; set; }        
        public string? Comment { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int Likes { get; set; } = 0;
        public bool IsConfirmedByAuthor { get; set; } = true; 
    }
}