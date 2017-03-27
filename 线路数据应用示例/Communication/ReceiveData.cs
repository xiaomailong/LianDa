using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows;


namespace 线路数据应用示例
{
    class ReceiveData
    {
        public static Socket socketMain = null;
        private IPAddress HostIP;
        private int HostPort;
        public static IPAddress DIP;
        public static int Dport;
        public bool runningFlag;

        public ReceiveData()
        {
            SetHostIPAndPort();
            IPEndPoint IPEP = new IPEndPoint(HostIP, HostPort);
            EndPoint EP = (EndPoint)IPEP;
            socketMain = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socketMain.Blocking = true;
            socketMain.Bind(EP);
        }

        public void SetHostIPAndPort()
        {
            foreach (var item in IPConfigure.IPList)
            {
                if (item.DeviveName == "ZC")
                {
                    HostIP = item.IP;
                    HostPort = item.Port;
                }
            }
        }

        byte[] receiveDataArray = new byte[1024];
        Thread thread;

        private byte[] DATA;
        public void Start()
        {
            runningFlag = true;
            thread = new Thread(ListenControlData);
            thread.IsBackground = true;
            thread.Start();
            
        }

        public void ListenControlData()
        {
            IPEndPoint ipEP = new IPEndPoint(HostIP, HostPort);
            EndPoint EP = (EndPoint)ipEP;

            while (runningFlag)
            {
                try
                {
                    int nRecv = socketMain.ReceiveFrom(receiveDataArray, ref EP);
                    SaveData(receiveDataArray);
                }
                catch(Exception e)
                {
                    
                }
            }
        }
        
        private void SaveData(byte[] receiveDataArray)
        {
            DATA = receiveDataArray;
            if (DATA != null)
            {
                VOBCorCI VOBCorCI = new VOBCorCI(DATA);
            }
        }

        public static void SendControlData(byte[] sendControlPacket, int packetLength)
        {

            IPEndPoint ipep = new IPEndPoint(DIP, Dport);
            EndPoint ep = (EndPoint)ipep;
            socketMain.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 0);
            try
            {
                socketMain.SendTo(sendControlPacket, 0, packetLength, SocketFlags.None, ipep);
            }
            catch (Exception e)
            {
            }

        }

    }
}
