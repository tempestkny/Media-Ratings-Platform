using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Media_Ratings_Platform.Helper
{
    public static class HelperFunctions
    {
        public static string HashPassword(string text)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes);
        }

        public static async Task<T?> ReadJsonBodyAsync<T>(HttpListenerRequest req)
        {
            try { return await JsonSerializer.DeserializeAsync<T>(req.InputStream); }
            catch { return default; }
        }

        public static void WriteJson(HttpListenerResponse res, object obj, int status = 200)
        {
            res.StatusCode = status;
            res.ContentType = "application/json";
            JsonSerializer.Serialize(res.OutputStream, obj);
            res.OutputStream.Flush();
            res.Close();
        }
    }
}
