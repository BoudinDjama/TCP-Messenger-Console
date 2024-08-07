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
        public List<Socket> Clients { get; set; }
        public Client Client { get; set; }

        public Server()
        {
            Clients = new List<Socket>();
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
                    Clients.Add(Handler);
                    Console.WriteLine($"Accepted connection from {c.Username}");
                   
                    
                    _ = handleClient(Handler); //handles send and receive from clients
                }

            }
            catch (SocketException se)
            {
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }

        

        private async Task handleClient(Socket socket)
        {

            while (true)
            {
                Client client = await receiveMessageAsync(socket);
                await sendMessageAsyncToAll(socket, client);
            }
        }

        private async Task sendMessageAsyncToAll(Socket socket, Client client)
        {
            //Displaying message to all connected clients
            foreach (Socket clientSocket in Clients)
            {
                //Avoiding client sender receving his own message
                if (!clientSocket.Equals(socket))
                {
                    string jsonString = JsonSerializer.Serialize(client);
                    var content = Encoding.UTF8.GetBytes(jsonString);
                    await clientSocket.SendAsync(content);
                }
            }
        }

        
        private async Task<Client> getClient(Socket socket)
        {       
            
                var buffer = new byte[1_024];
                int bytesRead = await socket.ReceiveAsync(buffer);
                return JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            
        }
        private async Task<Client> receiveMessageAsync(Socket socket)
        {
            
            var buffer = new byte[1_024];
            int bytesRead =  await socket.ReceiveAsync(buffer);
            Client client = JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            Console.WriteLine($"{client.Username}: {client.Message}");
            return client;

        }

        public void endSession()
        {
            ServerSocket.Close();
        }
    
}
}
