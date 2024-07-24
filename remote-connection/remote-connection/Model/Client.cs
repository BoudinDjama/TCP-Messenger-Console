using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace remote_connection.Model
{
    internal class Client
    {
        public Socket ClientSocket { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public bool EndSession { get; set; }
        public Client() 
        {
        
        }
       

        public async Task connectToServerAsync()
        {
            try
            {
                    await ClientSocket.ConnectAsync(IPEndPoint);
                    Console.WriteLine($"Client socket connected to {IPEndPoint.Address}:{IPEndPoint.Port}");            

            }

            catch (SocketException se)
            {
                Console.WriteLine($"Socket error: {se.Message}");

            }
        }

        public async Task sendMessageAsync(string message)
        {
            
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await ClientSocket.SendAsync(messageBytes);
            Console.WriteLine($"Socket client sent message: \"{message}\"");


        }

        public async Task receiveMessageAsync()
        {
            var buffer = new byte[1_024];
            int bytesRead = await ClientSocket.ReceiveAsync(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received message from server: {message}");
        }

    }
}
