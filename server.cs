using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpListener server;
    static List<TcpClient> clients = new List<TcpClient>();

    static void Main()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();

        Console.WriteLine("==============================");
        Console.WriteLine(" Multi User Chat Server");
        Console.WriteLine("==============================");
        Console.WriteLine("Listening on Port 5000...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            lock (clients)
            {
                clients.Add(client);
            }

            Console.WriteLine("Client Connected.");

            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytes);

                Console.WriteLine(message);

                Broadcast(message, client);
            }
        }
        catch
        {

        }

        Console.WriteLine("Client Disconnected.");

        lock (clients)
        {
            clients.Remove(client);
        }

        client.Close();
    }

    static void Broadcast(string message, TcpClient sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        lock (clients)
        {
            foreach (TcpClient client in clients)
            {
                if (client != sender)
                {
                    try
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                    catch { }
                }
            }
        }
    }
}
