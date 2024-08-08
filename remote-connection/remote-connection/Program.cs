using remote_connection.Model;
using System.Net;
using System.Net.Sockets;




//Server IP and port
string serverIp = "127.0.0.1";
int serverPort = 1234;


//End point and socket creation
IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
Socket socketServer = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);



//Setting up client side
Client client = new Client();



//Setting up server side
Server server = new Server();
server.ServerSocket = socketServer;
server.IPEndPoint = ipEndPoint;





Console.WriteLine("New chat session...");
Console.WriteLine("Are you a client or server");

//Reading if its a client or server
string role = Console.ReadLine() ?? "";
if (role == "client")
{
    //Getting the Username
    Console.WriteLine("Please enter a username: ");
    client.Username = Console.ReadLine() ?? "Default User";


    //connecting to the server
    await client.connectToServerAsync();

    //thread to listen for incomming messages
    new Thread(async() => { while(true) await client.receiveMessageAsync(); }).Start();


    //Sending user messages to the server
    while (true)
    {
        
        var message = Console.ReadLine() ?? "";
        await client.sendStream(message);       
    }

}

//Logic is done on the server class
else if (role == "server")
{   
    await server.startServer();
    
}



