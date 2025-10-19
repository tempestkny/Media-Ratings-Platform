using System.Net;

namespace Media_Ratings_Platform.Server
{
    public class TokenManager
    {
        private Dictionary<string, int> tokens = new Dictionary<string, int>();

        public string CreateToken(int userId, string username)
        {
            string token = username + "-mrpToken"; // required format
            tokens[token] = userId;
            return token;
        }

        public int? ValidateToken(HttpListenerRequest req)
        {
            string? auth = req.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer ")) return null;
            string token = auth.Substring("Bearer ".Length).Trim();
            if (tokens.TryGetValue(token, out int id))
                return id;
            return null;
        }
    }
}
