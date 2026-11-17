using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpClient client;
    static NetworkStream stream;

    static void Main()
    {
        client = new TcpClient();

        client.Connect("127.0.0.1", 5000);

        stream = client.GetStream();

        Console.Write("Enter Username: ");
        string username = Console.ReadLine();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        while (true)
        {
            string message = Console.ReadLine();

            if (message == "/exit")
                break;

            string output = username + ": " + message;

            byte[] data = Encoding.UTF8.GetBytes(output);

            stream.Write(data, 0, data.Length);
        }

        client.Close();
    }

    static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes == 0)
                    break;

                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
            catch
            {
                break;
            }
        }
    }
}
