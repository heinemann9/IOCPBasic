using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOCPServer
{
    class Server :SocketAsyncEventArgs
    {
        private Socket sock;
        private ILog logger;

        public Server(Socket sock)
        {
            logger.Info("Accept Client.");

            this.sock = sock;
            base.UserToken = sock;
            base.Completed += Server_Completed;
        }

        private void Server_Completed(object sender, SocketAsyncEventArgs e)
        {
            // to make Client_completed event
            Client client = new Client(e.AcceptSocket);
            e.AcceptSocket = null;
            this.sock.AcceptAsync(e);
        }
    }
}
