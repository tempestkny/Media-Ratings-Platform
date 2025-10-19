using Media_Ratings_Platform.Models;

namespace mrp_beginner.InMemory
{
    // very simple "repository" that just keeps things in lists
    public class InMemoryUserStore
    {
        private readonly List<User> users = new List<User>();
        private int userCounter = 1;

        public Task<User?> GetByUsername(string username)
        {
            var found = users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult<User?>(found);
        }

        public Task<int> CreateUser(string username, string passwordHash)
        {
            var u = new User
            {
                Id = userCounter++,
                Username = username,
                PasswordHash = passwordHash
            };
            users.Add(u);
            return Task.FromResult(u.Id);
        }
    }
}
