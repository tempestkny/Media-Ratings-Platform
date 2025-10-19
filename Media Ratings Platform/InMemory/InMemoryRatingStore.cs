using Media_Ratings_Platform.Models;

namespace Media_Ratings_Platform.InMemory
{
    public class InMemoryRatingStore
    {
        private readonly List<Rating> ratings = new List<Rating>();
        private int ratingCounter = 1;

        public Task<int> Create(Rating r)
        {
            r.Id = ratingCounter++;
            ratings.Add(r);
            return Task.FromResult(r.Id);
        }

        public Task<Rating?> Get(int id)
        {
            return Task.FromResult(ratings.FirstOrDefault(x => x.Id == id));
        }

        public Task Update(Rating r, int userId)
        {
            var ex = ratings.FirstOrDefault(x => x.Id == r.Id);
            if (ex == null || ex.UserId != userId) throw new Exception("Not found or not owner");
            ex.Stars = r.Stars;
            ex.Comment = r.Comment;
            return Task.CompletedTask;
        }

        public Task Delete(int id, int userId)
        {
            var ex = ratings.FirstOrDefault(x => x.Id == id);
            if (ex == null || ex.UserId != userId) throw new Exception("Not found or not owner");
            ratings.Remove(ex);
            return Task.CompletedTask;
        }
    }
}
