using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace remote_connection.Model
{
    [Serializable]
    internal class Client
    {
        public string Username { get; set; }
        public string Message { get; set; }

        [JsonIgnore]
        public Socket ClientSocket { get; set; }
        [JsonIgnore]
        public IPEndPoint IPEndPoint { get; set; }
        
        public Client() 
        {
            IPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            ClientSocket = new(IPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
       
        
        public async Task connectToServerAsync()
        {
            try
            {
                await ClientSocket.ConnectAsync(IPEndPoint);
                Console.WriteLine($"Client socket connected to {IPEndPoint.Address}:{IPEndPoint.Port}");
                await sendStream();
                
            }

            catch (SocketException se)
            {
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }
        

        public async Task sendStream(string message = "")
        {
            
           Message = message;
           string jsonString = JsonSerializer.Serialize(this);
           var messageBytes = Encoding.UTF8.GetBytes(jsonString);
           await ClientSocket.SendAsync(messageBytes, SocketFlags.None);
        }


        public async Task receiveMessageAsync()
        {

            var buffer = new byte[1_024];
            int bytesRead = await ClientSocket.ReceiveAsync(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Client c = JsonSerializer.Deserialize<Client>(message);
            Console.WriteLine($"{c.Username}: {c.Message}");

        }

    }
}
