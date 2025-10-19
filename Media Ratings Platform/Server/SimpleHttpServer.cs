using Media_Ratings_Platform.Helper;
using Media_Ratings_Platform.InMemory;
using Media_Ratings_Platform.Models;
using mrp_beginner.InMemory;
using System.Net;

namespace Media_Ratings_Platform.Server
{
    public class SimpleHttpServer
    {
        private HttpListener listener = new HttpListener();
        private TokenManager tokenManager = new TokenManager();

        // plug in-memory stores here
        private InMemoryUserStore userStore = new InMemoryUserStore();
        private InMemoryMediaStore mediaStore = new InMemoryMediaStore();

        public SimpleHttpServer(string address)
        {
            listener.Prefixes.Add(address);
        }

        public void StartServer()
        {
            listener.Start();
            Console.WriteLine("Server is running on http://localhost:8080/");
            Task.Run(() => ListenForRequests());
        }

        public void StopServer()
        {
            listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async Task ListenForRequests()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
        }

        private async Task HandleRequest(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var res = ctx.Response;
            var path = req.Url!.AbsolutePath;
            var method = req.HttpMethod.ToUpperInvariant();

            Console.WriteLine($"[{DateTime.Now}] {method} {path}");

            try
            {
                // REGISTER
                if (path == "/api/users/register" && method == "POST")
                {
                    var data = await HelperFunctions.ReadJsonBodyAsync<Dictionary<string, string>>(req);
                    if (data == null || !data.ContainsKey("username") || !data.ContainsKey("password"))
                    {
                        HelperFunctions.WriteJson(res, new { error = "Invalid input" }, 400);
                        return;
                    }

                    var existing = await userStore.GetByUsername(data["username"]);
                    if (existing != null)
                    {
                        HelperFunctions.WriteJson(res, new { error = "Username already exists" }, 409);
                        return;
                    }

                    var id = await userStore.CreateUser(data["username"], HelperFunctions.HashPassword(data["password"]));
                    HelperFunctions.WriteJson(res, new { id = id, username = data["username"] }, 201);
                    return;
                }

                // LOGIN
                if (path == "/api/users/login" && method == "POST")
                {
                    var data = await HelperFunctions.ReadJsonBodyAsync<Dictionary<string, string>>(req);
                    if (data == null)
                    {
                        HelperFunctions.WriteJson(res, new { error = "Invalid input" }, 400);
                        return;
                    }

                    var user = await userStore.GetByUsername(data["username"]);
                    if (user == null || user.PasswordHash != HelperFunctions.HashPassword(data["password"]))
                    {
                        HelperFunctions.WriteJson(res, new { error = "Invalid credentials" }, 401);
                        return;
                    }

                    var token = tokenManager.CreateToken(user.Id, user.Username);
                    HelperFunctions.WriteJson(res, new { token = token }, 200);
                    return;
                }

                // CREATE MEDIA
                if (path == "/api/media" && method == "POST")
                {
                    int? uid = tokenManager.ValidateToken(req);
                    if (uid == null) { res.StatusCode = 401; res.Close(); return; }

                    var media = await HelperFunctions.ReadJsonBodyAsync<MediaEntry>(req);
                    if (media == null) { res.StatusCode = 400; res.Close(); return; }
                    media.CreatedByUserId = uid.Value;

                    var id = await mediaStore.Create(media);
                    HelperFunctions.WriteJson(res, new { id = id }, 201);
                    return;
                }

                // READ MEDIA
                if (path.StartsWith("/api/media/") && method == "GET")
                {
                    if (int.TryParse(path.Substring("/api/media/".Length), out var id))
                    {
                        var media = await mediaStore.Get(id);
                        if (media == null) { res.StatusCode = 404; res.Close(); return; }
                        HelperFunctions.WriteJson(res, media, 200);
                        return;
                    }
                }

                // UPDATE MEDIA
                if (path.StartsWith("/api/media/") && method == "PUT")
                {
                    int? uid = tokenManager.ValidateToken(req);
                    if (uid == null) { res.StatusCode = 401; res.Close(); return; }
                    if (!int.TryParse(path.Substring("/api/media/".Length), out var id)) { res.StatusCode = 400; res.Close(); return; }

                    var media = await HelperFunctions.ReadJsonBodyAsync<MediaEntry>(req);
                    if (media == null) { res.StatusCode = 400; res.Close(); return; }
                    media.Id = id;
                    media.CreatedByUserId = uid.Value;

                    try { await mediaStore.Update(media); HelperFunctions.WriteJson(res, new { updated = id }, 200); }
                    catch { res.StatusCode = 403; res.Close(); }
                    return;
                }

                // DELETE MEDIA
                if (path.StartsWith("/api/media/") && method == "DELETE")
                {
                    int? uid = tokenManager.ValidateToken(req);
                    if (uid == null) { res.StatusCode = 401; res.Close(); return; }
                    if (!int.TryParse(path.Substring("/api/media/".Length), out var id)) { res.StatusCode = 400; res.Close(); return; }

                    try { await mediaStore.Delete(id, uid.Value); res.StatusCode = 204; res.Close(); }
                    catch { res.StatusCode = 403; res.Close(); }
                    return;
                }

                // Optional: list all (intermediate hint only)
                if (path == "/api/media" && method == "GET")
                {
                    var list = await mediaStore.GetAll();
                    HelperFunctions.WriteJson(res, list, 200);
                    return;
                }

                HelperFunctions.WriteJson(res, new { error = "Not Found" }, 404);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                HelperFunctions.WriteJson(res, new { error = "Server error" }, 500);
            }
        }
    }
}
