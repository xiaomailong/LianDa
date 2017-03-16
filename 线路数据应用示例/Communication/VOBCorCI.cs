using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class VOBCorCI
    {
        public static List<byte> VOBCNonCom = new List<byte>();
        public static int NumToVOBC = 1;
        public static int NumToCI1 = 1;
        public static int NumToCI2 = 1;

        public VOBCorCI(byte[] DATA)
        {
            int DataType = Convert.ToInt16(DATA[6]);
            if (DataType == 4 || DataType == 5 || DataType == 6 || DataType == 7)
            {
                int num = VOBCNonCom.IndexOf(DATA[11]);
                if (num == -1)
                {
                    VOBCNonCom.Add(DATA[11]);
                }
                HandleVOBCData HandleVOBCData = new HandleVOBCData(DATA);
                VOBCData VOBCData = new VOBCData(DATA, HandleVOBCData);
                UpdateInfo UpdateInfo = new UpdateInfo(HandleVOBCData);
            }
            if (DataType == 1)
            {
                HandleCI1Data HandleCI1Data = new HandleCI1Data(DATA);
                InfoSendToCI SendToCI = new InfoSendToCI();
                byte[] Head = WriteHead(NumToCI1, DataType, SendToCI.DataLength);
                Array.Copy(Head, SendToCI.DataSendToCI, 10);
                Send(SendToCI.DataSendToCI, "127.0.0.1", 8003);
                NumToCI1++;
            }
            if (DataType == 2)
            {
                HandleCI2Data HandleCI2Data = new HandleCI2Data(DATA);
                InfoSendToCI SendToCI = new InfoSendToCI();
                byte[] Head = WriteHead(NumToCI2, DataType, SendToCI.DataLength);
                Array.Copy(Head, SendToCI.DataSendToCI, 10);
                Send(SendToCI.DataSendToCI, "127.0.0.1", 8005);
                NumToCI2++;
            }
        }

        public byte[] WriteHead(int Num,int DataType,int DataLength)
        {
            byte[] Head = new byte[10];
            byte[] NumByte = System.BitConverter.GetBytes(Num);
            Array.Copy(NumByte, Head, 4);
            Head[4] = 0;
            Head[5] = 2;
            Head[6] = 3;
            Head[7] = Convert.ToByte(DataType);
            Head[8] = Convert.ToByte(DataLength);
            Head[9] = 0;
            return Head;
        }
        public void Send(byte[] Data, string IP, int port)
        {
            SendMessage Send = new SendMessage();
            Send.DIP = IP;
            Send.Dport = port;
            Send.SendControlData(Data, Data.Length);
        }
    }
}
