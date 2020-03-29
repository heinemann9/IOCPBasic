using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOCPClient
{
    class Client : SocketAsyncEventArgs
    {
        private Socket sock;
        private StringBuilder sb = new StringBuilder();

        public Client(EndPoint ep)
        {
            RemoteEndPoint = ep;
            base.Completed += Connected_Completed;
        }

        private void Connected_Completed(object sender, SocketAsyncEventArgs e)
        {
            base.Completed -= Connected_Completed;
            this.sock = e.ConnectSocket;
            base.UserToken = this.sock;
            base.SetBuffer(new byte[1024], 0, 1024);
            base.Completed += Client_Completed;
            this.sock.ReceiveAsync(this);
        }

        private void Client_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (sock.Connected == true &&
                base.BytesTransferred > 0)
            {
                byte[] data = e.Buffer;
                string msg = Encoding.UTF8.GetString(data);
                base.SetBuffer(new byte[1024], 0, 1024);
                sb.Append(msg.Trim('\0'));

                if (sb.Length >= 0)
                {
                    msg = sb.ToString();
                    Console.Write(msg);
                    sb.Clear();
                }
                this.sock.ReceiveAsync(this);
            }
            else
            {
                IPEndPoint remoteAddr = (IPEndPoint)sock.RemoteEndPoint;
                // disconnected
                Console.WriteLine("Disconnected : (From : " + remoteAddr.Address.ToString() +
                                  " : " + remoteAddr.Port + ", Connection time : " + DateTime.Now);
            }
        }

        public void Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            sock.Send(data, data.Length, SocketFlags.None);
        }
    }
}
