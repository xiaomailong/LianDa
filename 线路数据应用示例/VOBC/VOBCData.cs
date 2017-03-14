using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class VOBCData
    {
        HandleVOBCData Handle = new HandleVOBCData();
        InfoSendToVOBC InfoSendToVOBC = new InfoSendToVOBC();
        DetermineFrontTrain Determine = new DetermineFrontTrain();
        public event EventHandler<VOBCEvent> NewVOBCData;
        private byte[] Data;
        public VOBCData(byte[] data)
        {
            this.Data = data;
        }
        public void DataHandle()
        {
            EventHandler<VOBCEvent> HandleEvent = NewVOBCData;
            if (HandleEvent != null)
            {
                HandleVOBCData(this, new VOBCEvent(this.Data));
            }


            //写发给VOBC的数据
            SetInfoToVOBC();


        }

        private void SetInfoToVOBC()
        {
            SetIDofZC();
            SetIDofVOBC();
            SetNCofZC();
            SetMAHead();
            SetMATail();
            SetNumberOfBarrier();
            SetMALength();
            SetMAEndType();
        }

        public void HandleVOBCData(object sender, VOBCEvent TrainData)
        {
            Handle.AddProgress(this.Data);
        }

        private void SetIDofZC()
        {
            InfoSendToVOBC.NID_ZC = 0x01;
            InfoSendToVOBC.ReplyMessageToZC[8] = InfoSendToVOBC.NID_ZC;
            InfoSendToVOBC.ReplyMessageToZC[9] = 0x00;
        }

        private void SetIDofVOBC()
        {
            InfoSendToVOBC.NID_Train = Handle.NID_Train;
            InfoSendToVOBC.ReplyMessageToZC[10] = InfoSendToVOBC.NID_Train;
            InfoSendToVOBC.ReplyMessageToZC[11] = 0x00;
        }

        private void SetNCofZC()
        {
            switch (Handle.NC_Train)
            {
                case 0x01:
                    InfoSendToVOBC.NC_ZC = 0x01;
                    break;
                case 0x02:
                    InfoSendToVOBC.NC_ZC = 0x02;
                    break;
                case 0x03:
                    InfoSendToVOBC.NC_ZC = 0x03;
                    break;
                case 0x04:
                    InfoSendToVOBC.NC_ZC = 0x04;
                    break;
                default:
                    break;
            }
            InfoSendToVOBC.ReplyMessageToZC[12] = InfoSendToVOBC.NC_ZC;
        }
        private void SetMAHead()
        {
            byte[] MAHead = new byte[6];
            Array.Copy(Data, 28, MAHead, 0, 6);
            Array.Copy(Data, 28, InfoSendToVOBC.ReplyMessageToZC, 29, 6);
            InfoSendToVOBC.ReplyMessageToZC[35] = Handle.Q_TrainRealDirection;
        }
        private void SetMATail()
        {
            byte[] MATail = Determine.DetermineMA(Handle);
            Array.Copy(MATail, 0, InfoSendToVOBC.ReplyMessageToZC, 36, 7);
        }

        private void SetNumberOfBarrier()
        {
            int NumOfBarrier = Determine.GetNumOfBarrier(Determine.Route);
            byte BNum = Convert.ToByte(NumOfBarrier);
            InfoSendToVOBC.ReplyMessageToZC[43] = BNum;
        }

        private void SetMALength()
        {
            int NumOfBarrier = Determine.NumOfBarrier;
            byte MALength = Convert.ToByte(17 + 5 * NumOfBarrier);
            InfoSendToVOBC.ReplyMessageToZC[27] = MALength;
        }
        private void SetMAEndType()
        {
            InfoSendToVOBC.ReplyMessageToZC[28] = 0x01;
        }

        private void SetMAObstacle()
        {
            InfoSendToVOBC.Obstacle = new byte[Determine.NumOfBarrier * 5];
            Determine.SetMAObstacle(InfoSendToVOBC.Obstacle, Determine.Route);
        }
    }
}
