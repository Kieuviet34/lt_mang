using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client3
{
    internal class Program
    {
        static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static byte[] buffer = new byte[1024];
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.Title = $"{name}";
            ConnectToServer();
            Thread receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
            while (true)
            {
                string msg = Console.ReadLine();
                SendData($"{name}: {msg}");
            }
        }

        static void SendData(string v)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(v);
            client.Send(buffer);
        }

        static void ReceiveData()
        {
            while (true)
            {
                int recv = client.Receive(buffer);
                byte[] databuf = new byte[recv];
                Array.Copy(buffer, databuf, recv);
                string text = Encoding.UTF8.GetString(databuf);
                Console.WriteLine(text);
            }
        }

        static void ConnectToServer()
        {
            int attemps = 0;
            while (!client.Connected)
            {
                try
                {
                    attemps++;
                    client.Connect(IPAddress.Loopback, 5000);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine(attemps);
                }
            }
            Console.Clear();
            Console.WriteLine("Connected to server");
        }
    }
}
