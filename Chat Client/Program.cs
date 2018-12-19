using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            Int32 port = 5667;

            TcpClient client = new TcpClient("127.0.0.1", port);

            byte[] data;
            String message;

            NetworkStream stream = client.GetStream();

            Console.Write("Please enter your username: ");
            string username = Console.ReadLine();

            Console.Write("Please enter the username of the person you would like to talk to: ");
            string friend = Console.ReadLine();

            data = Encoding.ASCII.GetBytes("CONNECTIONSTART:" + username + ":" + friend);
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine("Enter messages to send to your friend:");
            Console.Write("Me: ");

            Thread t = new Thread(new ThreadStart(() => GetMessage(stream, friend)));
            t.Start();

            while (!((message = Console.ReadLine()).Equals("exit")))
            {
                data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Console.Write("Me: ");
            }

            data = Encoding.ASCII.GetBytes("exit");

            stream.Write(data, 0, data.Length);

            t.Abort();
            stream.Close();
            client.Close();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void GetMessage(NetworkStream stream, string friend)
        {
            byte[] data = new byte[256];
            string message = null;
            int i;

            while ((i = stream.Read(data, 0, data.Length)) != 0)
            {
                message = Encoding.ASCII.GetString(data, 0, i);

                /* Split into seperate messages if the user had pending messages since sometimes the
                   stream is written to too quickly for multiple WriteLine()s to handle. */
                if (message.Contains("ENDOFLINE")) {
                    foreach (string line in message.Split(new[] { "ENDOFLINE" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Console.Write("\b \b");
                        }
                        Console.WriteLine(friend + ": " + line);
                        Console.Write("Me: ");
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Console.Write("\b \b");
                    }
                    Console.WriteLine(friend + ": " + message);
                    Console.Write("Me: ");
                }

            }
        }

    }
}
