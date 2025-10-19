using Media_Ratings_Platform.Server;

Console.WriteLine("Starting MRP Server...");

SimpleHttpServer server = new SimpleHttpServer("http://localhost:8080/");
server.StartServer();

Console.WriteLine("Press CTRL + C to stop.");
Console.CancelKeyPress += (s, e) =>
{
    server.StopServer();
    Environment.Exit(0);
};
await Task.Delay(-1);