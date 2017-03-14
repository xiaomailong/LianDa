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
        public Socket socketMain = null;
        private string HostIP = "127.0.0.1";
        private int HostPort = 8000;  //暂定写死，以后根据情况加配置文件
        public bool runningFlag;

        byte[] receiveDataArray = new byte[1024];
        Thread thread;

        private byte[] DATA;
        public void Start()
        {
            runningFlag = true;
            thread = new Thread(new ThreadStart(this.ListenControlData));
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
                catch
                {

                }
            }
        }

        private void SaveData(byte[] receiveDataArray)
        {
            DATA = receiveDataArray;
            if (DATA != null)
            {
                receiveDataArray = new byte[1024];//目前做的认为接收到的都是VOBC发来的数据
                VOBCorCI VOBCorCI = new VOBCorCI(DATA);
            }
            receiveDataArray = new byte[1024];
        }























    }
}
