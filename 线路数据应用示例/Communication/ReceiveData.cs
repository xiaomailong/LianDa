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
        private string HostIP = "127.0.0.1";
        private int HostPort = 4001;  //暂定写死，以后根据情况加配置文件
        public static string DIP;
        public static int Dport;
        public bool runningFlag;

        public ReceiveData()
        {
            IPEndPoint IPEP = new IPEndPoint(IPAddress.Parse(HostIP), HostPort);
            EndPoint EP = (EndPoint)IPEP;
            socketMain = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socketMain.Blocking = true;
            socketMain.Bind(EP);
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
            IPEndPoint ipEP = new IPEndPoint(IPAddress.Parse(HostIP), HostPort);
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
                receiveDataArray = new byte[1024];
                VOBCorCI VOBCorCI = new VOBCorCI(DATA);
            }
            receiveDataArray = new byte[1024];
        }

        public static void SendControlData(byte[] sendControlPacket, int packetLength)
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(DIP), Dport);
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
