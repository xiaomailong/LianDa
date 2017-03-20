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
            int DataType = Convert.ToInt16(DATA[4]);
            if (DataType == 4 || DataType == 5 || DataType == 6 || DataType == 7)
            {
               HandleVOBC(DataType,DATA);
            }
            if (DataType == 1)
            {
                HandleCI1Data HandleCI1Data = new HandleCI1Data(DATA);
                InfoSendToCI SendToCI = new InfoSendToCI();
                byte[] Head = WriteCIHead(NumToCI1, DataType, SendToCI.DataLength);
                Array.Copy(Head, SendToCI.DataSendToCI, 8);
                Send(SendToCI.DataSendToCI, "127.0.0.1", 8003);
                if (NumToCI1 < 65536)
                {
                    NumToCI1++;
                }
                else
                {
                    NumToCI1 = 1;
                }
            }
            if (DataType == 2)
            {
                HandleCI2Data HandleCI2Data = new HandleCI2Data(DATA);
                InfoSendToCI SendToCI = new InfoSendToCI();
                byte[] Head = WriteCIHead(NumToCI2, DataType, SendToCI.DataLength);
                Array.Copy(Head, SendToCI.DataSendToCI, 8);
                Send(SendToCI.DataSendToCI, "127.0.0.1", 8005);
                if (NumToCI2 < 65536)
                {
                    NumToCI2++;
                }
                else
                {
                    NumToCI2 = 1;
                }
            }
        }

        public void HandleVOBC(int DataType, byte[] DATA)
        {
            int num = VOBCNonCom.IndexOf(DATA[11]);
            if (num == -1)
            {
                VOBCNonCom.Add(DATA[11]);
            }
            HandleVOBCData HandleVOBCData = new HandleVOBCData(DATA);
            VOBCData VOBCData = new VOBCData(DATA, HandleVOBCData);
            UpdateInfo UpdateInfo = new UpdateInfo(HandleVOBCData,DATA);
            byte[] DataToVOBC = new byte[8 + VOBCData.InfoSendToVOBC.Obstacle.Length + 36];
            WriteVOBCHead(DataType).CopyTo(DataToVOBC,0);
            VOBCData.InfoSendToVOBC.ReplyMessageToZC.CopyTo(DataToVOBC, 8);
            VOBCData.InfoSendToVOBC.Obstacle.CopyTo(DataToVOBC, 8 + VOBCData.InfoSendToVOBC.ReplyMessageToZC.Length);
            Send(DataToVOBC, "121.194.59.122", 7000);
        }

        public byte[] WriteCIHead(int Num, int DataType, int DataLength)
        {
            byte[] Head = new byte[8];
            byte[] NumByte = System.BitConverter.GetBytes(Num);
            Array.Copy(NumByte, Head, 2);
            Head[2] = 1;
            Head[3] = 0;
            Head[4] = 3;
            Head[5] = Convert.ToByte(DataType);
            Head[6] = Convert.ToByte(DataLength);
            Head[7] = 0;
            return Head;
        }

        public byte[] WriteVOBCHead(int DataType)
        {
            byte[] Head = new byte[8];
            byte[] NumByte = System.BitConverter.GetBytes(0);
            Array.Copy(NumByte, Head, 2);
            Head[2] = 9;
            Head[3] = 0;
            Head[4] = 3;
            Head[5] = Convert.ToByte(DataType);
            Head[6] = 0;
            Head[7] = 0;
            return Head;
        }

        public void Send(byte[] Data, string IP, int port)
        {
            ReceiveData.DIP = IP;
            ReceiveData.Dport = port;
            ReceiveData.SendControlData(Data, Data.Length);
        }
    }
}
