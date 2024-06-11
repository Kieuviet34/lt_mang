using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static List<Socket> client_socket = new List<Socket>();
        static Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static byte[] buffer = new byte[1024];
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Server starting....");
            SetUpServer();
            Console.ReadLine();
        }

        static void SetUpServer() { 
            server_socket.Bind(new IPEndPoint(IPAddress.Any, 5000));
            server_socket.Listen(10);
            server_socket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        static void AcceptCallBack(IAsyncResult ar)
        {
            Socket socket = server_socket.EndAccept(ar);
            client_socket.Add(socket);
            foreach(var user in client_socket)
            {
                Console.WriteLine($"Client {user} connected");
            }
            socket.BeginReceive(buffer, 0, buffer.Length,
                SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            server_socket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int recv = socket.EndReceive(ar);
            byte[] databuffer = new byte[recv];
            Array.Copy(buffer, databuffer, recv);
            string text = Encoding.UTF8.GetString(databuffer);
            Console.WriteLine("Receive: " + text);

            byte[] data= Encoding.UTF8.GetBytes(text);
            foreach (Socket client_soc in client_socket)
            {
                if(client_soc != socket) client_soc.Send(data);
            }
            socket.BeginReceive(buffer, 0, buffer.Length,
                SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }
    }
}
