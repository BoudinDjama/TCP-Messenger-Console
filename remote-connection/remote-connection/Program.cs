using remote_connection.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;



//Server IP and port
string serverIp = "127.0.0.1";
int serverPort = 1234;


//End point and socket creation
IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
Socket socketClient = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
Socket socketServer = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);



//Setting up client side
Client client = new Client();
client.ClientSocket = socketClient;
client.IPEndPoint = ipEndPoint;

//Setting up server side
Server server = new Server();
server.ServerSocket = socketServer;
server.IPEndPoint = ipEndPoint;





Console.WriteLine("New chat session...");
Console.WriteLine("Are you a client or server");

string role = Console.ReadLine() ?? "";

//Reading if its a client or server
if (role == "client")
{
    await client.connectToServerAsync();

    while (true)
    {
        
        var message = Console.ReadLine() ?? "";
        await client.sendMessageAsync(message);
        await client.receiveMessageAsync();
        
        
        break;
        
    }

    
    client.ClientSocket.Shutdown(SocketShutdown.Both);
}


else if (role == "server")
{   
    await server.connectWithClientAsync();

    while (true)
    {
        // Receive message.
       await server.receiveMessageAsync();
       await server.sendMessageAsync(Console.ReadLine() ?? "");
       break;
    }

    server.Handler.Shutdown(SocketShutdown.Both);
    
}

