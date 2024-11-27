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

        //Properties can be ignore during Json serialization
        [JsonIgnore]
        public Socket ClientSocket { get; set; }
        [JsonIgnore]
        public IPEndPoint IPEndPoint { get; set; }
        

        //Initialization for the IPEnpoint and socket of Client 
        public Client() 
        {
            IPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
            ClientSocket = new(IPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
       
        //Method to connect to the server ASYNC
        public async Task connectToServerAsync()
        {
            try
            {
                //Attempting to connect to the server
                await ClientSocket.ConnectAsync(IPEndPoint);
                Console.WriteLine($"Client socket connected to {IPEndPoint.Address}:{IPEndPoint.Port}");
                await sendStream();
                
            }

            catch (SocketException se)
            {
                //Handling connection error  
                Console.WriteLine($"Socket error: {se.Message}");
            }
        }
        

        //Method to send message ASYNC
        public async Task sendStream(string message = "")
        {
            //Set the message property and convert it to JSON
           Message = message;
           string jsonString = JsonSerializer.Serialize(this);

           //Convert JSON(Client) string to bytes
           var messageBytes = Encoding.UTF8.GetBytes(jsonString);
           await ClientSocket.SendAsync(messageBytes, SocketFlags.None);
        }


        //Method to receive a message from server ASYNC
        public async Task receiveMessageAsync()
        {

            var buffer = new byte[1_024];

            //Receive data from socket
            int bytesRead = await ClientSocket.ReceiveAsync(buffer);

            //Convert receive data to string
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            //Turn the string to my client object
            Client c = JsonSerializer.Deserialize<Client>(message);
            Console.WriteLine($"{c.Username}: {c.Message}");

        }

    }
}
