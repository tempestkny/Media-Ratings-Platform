using Media_Ratings_Platform.Models;

namespace Media_Ratings_Platform.InMemory
{
    public class InMemoryMediaStore
    {
        private readonly List<MediaEntry> media = new List<MediaEntry>();
        private int mediaCounter = 1;

        public Task<int> Create(MediaEntry m)
        {
            m.Id = mediaCounter++;
            media.Add(m);
            return Task.FromResult(m.Id);
        }

        public Task<MediaEntry?> Get(int id)
        {
            var m = media.FirstOrDefault(x => x.Id == id);
            return Task.FromResult<MediaEntry?>(m);
        }

        public Task Update(MediaEntry updated)
        {
            var existing = media.FirstOrDefault(m => m.Id == updated.Id);
            if (existing == null || existing.CreatedByUserId != updated.CreatedByUserId)
                throw new Exception("Not found or not owner");

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.MediaType = updated.MediaType;
            existing.ReleaseYear = updated.ReleaseYear;
            existing.Genres = updated.Genres;
            existing.AgeRestriction = updated.AgeRestriction;
            return Task.CompletedTask;
        }

        public Task Delete(int id, int userId)
        {
            var existing = media.FirstOrDefault(m => m.Id == id);
            if (existing == null || existing.CreatedByUserId != userId)
                throw new Exception("Not found or not owner");

            media.Remove(existing);
            return Task.CompletedTask;
        }

        public Task<List<MediaEntry>> GetAll()
        {
            return Task.FromResult(media.ToList());
        }
    }
}
