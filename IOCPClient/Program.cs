using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOCPClient
{
    class Program : Socket
    {
        public Program() :
        base
        (
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        )
        {
            Client client = new Client(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000));
            base.ConnectAsync(client);
            while (true)
            {
                string k = Console.ReadLine();
                client.Send(k);
                if (k.Equals("exit"))
                {
                    break;
                }
            }
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
