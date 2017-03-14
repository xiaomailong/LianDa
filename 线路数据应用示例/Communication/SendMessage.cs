using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace 线路数据应用示例
{
    class SendMessage
    {
        public string DIP = "";
        public int Dport = 5001;
        public Socket socketMain = null;

        public void SendControlData(byte[] sendControlPacket, int packetLength)
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(DIP), Dport);
            EndPoint ep = (EndPoint)ipep;
            socketMain.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 0);
            try
            {
                socketMain.SendTo(sendControlPacket, 0, packetLength, SocketFlags.None, ipep);
            }
            catch
            {
            }

        }
    }
}
