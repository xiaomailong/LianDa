using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                lock (NonCommunicationTrain.LoseTrain)
                {
                    if (NonCommunicationTrain.LoseTrain.Keys.Contains(DATA[8]))
                    {
                        CancelNonComTrain(DATA[8]);
                    }
                }
                HandleVOBC(DataType,DATA);
            }
            if (DataType == 2)
            {
                HandleCI2Data HandleCI2Data = new HandleCI2Data(DATA);
                InfoSendToCI2 SendToCI = new InfoSendToCI2();
                byte[] Head = WriteCIHead(NumToCI2, DataType, 17);
                Array.Copy(Head, SendToCI.DataSendToCI, 8);
                Send(SendToCI.DataSendToCI, GetIPByDataType(DataType), GetPortByDataType(DataType));
                if (NumToCI2 < 65536)
                {
                    NumToCI2++;
                }
                else
                {
                    NumToCI2 = 1;
                }
            }
            if (DataType == 1)
            {
                HandleCI1Data HandleCI1Data = new HandleCI1Data(DATA);
                InfoSendToCI1 SendToCI = new InfoSendToCI1();
                byte[] Head = WriteCIHead(NumToCI1, DataType, 21);
                Array.Copy(Head, SendToCI.DataSendToCI, 8);
                Send(SendToCI.DataSendToCI, GetIPByDataType(DataType), GetPortByDataType(DataType));
                if (NumToCI1 < 65536)
                {
                    NumToCI1++;
                }
                else
                {
                    NumToCI1 = 1;
                }
            }
        }

        public void HandleVOBC(int DataType, byte[] DATA)
        {
            int num = VOBCNonCom.IndexOf(DATA[8]);
            if (num == -1)
            {
                VOBCNonCom.Add(DATA[8]);
            }
            HandleVOBCData HandleVOBCData = new HandleVOBCData(DATA);
            VOBCData VOBCData = new VOBCData(DATA, HandleVOBCData);
            UpdateInfo UpdateInfo = new UpdateInfo(HandleVOBCData,DATA);
            byte[] DataToVOBC = new byte[8 + VOBCData.InfoSendToVOBC.Obstacle.Length + 36];
            WriteVOBCHead(DataType).CopyTo(DataToVOBC,0);
            VOBCData.InfoSendToVOBC.ReplyMessageToZC.CopyTo(DataToVOBC, 8);
            VOBCData.InfoSendToVOBC.Obstacle.CopyTo(DataToVOBC, 8 + VOBCData.InfoSendToVOBC.ReplyMessageToZC.Length);
            Send(DataToVOBC, GetIPByDataType(DataType), GetPortByDataType(DataType));
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

        public void Send(byte[] Data, IPAddress IP, int port)
        {
            ReceiveData.DIP = IP;
            ReceiveData.Dport = port;
            ReceiveData.SendControlData(Data, Data.Length);
        }

        public IPAddress GetIPByDataType(int DataType)
        {
            foreach (var item in IPConfigure.IPList)
            {
                if (item.DeviceID == DataType)
                {
                    return item.IP;
                }
            }
            return null;
        }

        public int GetPortByDataType(int DataType)
        {
            foreach (var item in IPConfigure.IPList)
            {
                if (item.DeviceID == DataType)
                {
                    return item.Port;
                }
            }
            return 0;
        }

        public static void CancelNonComTrain(byte TrainID)
        {
            string PreNonComTrainPosition;
            string PreNonComTrainPositionSectionName;
            lock (NonCommunicationTrain.LoseTrain)
            {
                PreNonComTrainPosition = (Convert.ToInt16(NonCommunicationTrain.LoseTrain[TrainID][1]) * 256 + Convert.ToInt16(NonCommunicationTrain.LoseTrain[TrainID][0])).ToString();
                PreNonComTrainPositionSectionName = (Convert.ToInt16(NonCommunicationTrain.LoseTrain[TrainID][2])).ToString();
                NonCommunicationTrain.LoseTrain.Remove(TrainID);
            }
            if (UpdateInfo.TraverseSection(PreNonComTrainPosition) != null)
            {
                Section section = UpdateInfo.TraverseSection(PreNonComTrainPosition);
                section.HasNonComTrain.Remove(TrainID);
                System.Windows.Application.Current.Dispatcher.Invoke(
                 new Action(
                 delegate
                 {
                     section.InvalidateVisual();
                 }));
            }
            else if (UpdateInfo.TraverseRailSwitch(PreNonComTrainPosition, PreNonComTrainPositionSectionName) != null)
            {
                RailSwitch railswitch = UpdateInfo.TraverseRailSwitch(PreNonComTrainPosition, PreNonComTrainPositionSectionName);
                railswitch.HasNonComTrain.Remove(TrainID);
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    railswitch.InvalidateVisual();
                }));
            }
        }
    }
}
