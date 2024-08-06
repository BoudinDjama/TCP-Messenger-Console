using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace remote_connection.Model
{
    internal class Server
    {
        public Socket ServerSocket { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        List <Client> Clients { get; set; }

        public int userCount { get; set; }
        public Server()
        {
            Clients = new List <Client>();
        }

        public async Task startServer()
        {
            try
            {
                ServerSocket.Bind(IPEndPoint);
                ServerSocket.Listen(100);
                Console.WriteLine("Waiting for connection...");
                while (true)
                {
                    Socket Handler = await ServerSocket.AcceptAsync();
                    Client c = await getClient(Handler);
                    Clients.Add(c);

                    Console.WriteLine($"Accepted connection from {c.Username}");
                   
                    
                    _ = handleClient(Handler);
                }

            }
            catch (SocketException se)
            {
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }

        

        public async Task handleClient(Socket socket)
        {

            while (true)
            {
                await receiveMessageAsync(socket);
            }
        }

        public async Task sendMessageAsync(string message, Socket socket)
        {
            var content = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(content);

        }
        public async Task<Client> getClient(Socket socket)
        {       
            
                var buffer = new byte[1_024];
                int bytesRead = await socket.ReceiveAsync(buffer);
                return JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            
        }
        public async Task receiveMessageAsync(Socket socket)
        {
            
            var buffer = new byte[1_024];
            int bytesRead =  await socket.ReceiveAsync(buffer);
            Client client = JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            Console.WriteLine($"{client.Username}: {client.Message}");

        }

        public void endSession()
        {
            ServerSocket.Close();
        }
    
}
}
