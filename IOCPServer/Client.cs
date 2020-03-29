using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IOCPServer
{
//    [Serializable]
//    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//    struct UserData
//    {
//        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
//        public string name;
//        public int age;
//    };

    class Client : SocketAsyncEventArgs
    {
//        static List<UserData> userList = new List<UserData>();
        private Socket sock;
        private IPEndPoint remoteAddr;
        private ILog logger;
        private StringBuilder sb = new StringBuilder();

        public Client(Socket sock)
        {
            logger = LogManager.GetLogger(this.GetType());

            this.sock = sock;
            base.SetBuffer(new byte[1024], 0, 1024);
            base.UserToken = sock;
            base.Completed += Client_Completed;
            this.sock.ReceiveAsync(this);

            remoteAddr = (IPEndPoint)sock.RemoteEndPoint;

            logger.Info($"Client : (From: {remoteAddr.Address.ToString()}:{remoteAddr.Port}, Connection time: {DateTime.Now})");
            this.Send("server connected !");
        }

        private void Client_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (sock.Connected == true &&
                base.BytesTransferred > 0)
            {
                logger.Info("Client connected");

                byte[] data = e.Buffer;
                base.SetBuffer(new byte[1024], 0, 1024);

//                UserData user;
//                unsafe
//                {
//                    fixed (byte* fixed_data = data)
//                    {
//                        user = (UserData)Marshal.PtrToStructure((IntPtr)fixed_data, typeof(UserData));
//                    }
//                }
                string msg = Encoding.UTF8.GetString(data);
                sb.Append(msg.Trim('\0'));

                if (msg.Length > 0)
                {
                    logger.Info("Echo - " + msg);
                    if (msg.Equals("exit"))
                    {
                        logger.Info("Disconnected : (From : " + remoteAddr.Address.ToString() +
                                    " : " + remoteAddr.Port + ", Connection time : " + DateTime.Now);
                        sock.DisconnectAsync(this);
                        return;
                    }

                    sb.Clear();
                }

//                if (data.Length > 0)
//                {
//                    // data 처리
//                    if (user.name.Equals("null") ||
//                        user.age.Equals(-1))
//                    {
//                        sock.DisconnectAsync(this);
//                        return;
//                    }
//                    else
//                    {
//                        lock (userList)
//                        {
//                            userList.Add(user);
//                        }
//                    }
//                }

                this.sock.ReceiveAsync(this);
            }
            else
            {
                logger.Info("Disconnected : (From : " + remoteAddr.Address.ToString() +
                            " : " + remoteAddr.Port + ", Connection time : " + DateTime.Now);
            }
        }

        private void Send(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            sock.Send(data, data.Length, SocketFlags.None);
        }
    }
}
