using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;

namespace remote_connection.Model
{
    internal class Server
    {
        public Socket ServerSocket { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public List<Socket> Clients { get; set; }


        public Server()
        {
            Clients = new List<Socket>();
        }

        //Start server async
        public async Task startServer()
        {
            try
            {
                //Bind server socket to the endpoint and Listen for connections
                ServerSocket.Bind(IPEndPoint);
                ServerSocket.Listen(100);
                Console.WriteLine("Waiting for connection...");

                while (true)
                {
                    //Accept a new client connection
                    Socket Handler = await ServerSocket.AcceptAsync();
                    
                    //Get client details
                    Client c = await getClient(Handler);

                    //Notify all clients of the new connection
                    await notifyClientConnected(c);

                    //Add the new client to the list of clients
                    Clients.Add(Handler);
                    Console.WriteLine($"Accepted connection from {c.Username}");
                   
                    
                    _ = handleClient(Handler); //handles send and receive from clients
                }

            }
            catch (SocketException se)
            {
                //Handle error
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }

        //Method to notify all clients of the new client
        private async Task notifyClientConnected(Client c)
        {
            if (Clients.Count > 0) 
            {
                foreach (Socket clientSocket in Clients)
                {
                    //Verifying that its not the sender
                    c.Message = $"{c.Username} has connected to the server";
                    c.Username = "";
                        string jsonString = JsonSerializer.Serialize(c);
                        var content = Encoding.UTF8.GetBytes(jsonString);
                        await clientSocket.SendAsync(content);
                    
                }
            }
        }

        //Method to handle communication between clients
        private async Task handleClient(Socket socket)
        {

            while (true)
            {
                Client client = await receiveMessageAsync(socket);
                await sendMessageAsyncToAll(socket, client);
            }
        }

        //Method to send message to all clients except self 
        private async Task sendMessageAsyncToAll(Socket socket, Client client)
        {
            //Sending current client message to all connected clients
            foreach (Socket clientSocket in Clients)
            {
                //Verifying that its not the sender
                if (!clientSocket.Equals(socket))
                {
                    string jsonString = JsonSerializer.Serialize(client);
                    var content = Encoding.UTF8.GetBytes(jsonString);
                    await clientSocket.SendAsync(content);
                }
            }
        }

        //Method to get client details from socket
        private async Task<Client> getClient(Socket socket)
        {       
            
                var buffer = new byte[1_024];
                int bytesRead = await socket.ReceiveAsync(buffer);
                return JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            
        }

        //Method to receive a message from client
        private async Task<Client> receiveMessageAsync(Socket socket)
        {
            
            var buffer = new byte[1_024];
            int bytesRead =  await socket.ReceiveAsync(buffer);
            Client client = JsonSerializer.Deserialize<Client>(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            Console.WriteLine($"{client.Username}: {client.Message}");
            return client;

        }

        //I wonder what this method does
        public void endSession()
        {
            ServerSocket.Close();
        }
    
}
}
