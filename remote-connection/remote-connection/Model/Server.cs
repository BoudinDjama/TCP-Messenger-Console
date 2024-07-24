using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace remote_connection.Model
{
    internal class Server
    {
        public Socket ServerSocket { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public Socket Handler {  get; set; }
        public Server()
        {
            
        }

        public async Task connectWithClientAsync()
        {
            try
            {
                ServerSocket.Bind(IPEndPoint);
                ServerSocket.Listen(100);
                Console.WriteLine("Waiting for connection...");

                Handler =  await ServerSocket.AcceptAsync();
                Console.WriteLine($"Accepted connection from {Handler.RemoteEndPoint}");
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }

        public async Task sendMessageAsync(string message)
        {
            var content = Encoding.UTF8.GetBytes(message);
            await Handler.SendAsync(content);

        }

        public async Task receiveMessageAsync()
        {
          
            var buffer = new byte[1_024];
            int bytesRead =  await Handler.ReceiveAsync(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received message from client: {message}");
        }

        public void endSession()
        {
            ServerSocket.Close();
        }
    
}
}
