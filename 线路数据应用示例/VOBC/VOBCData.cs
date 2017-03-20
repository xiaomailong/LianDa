using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class VOBCData
    {
        HandleVOBCData Handle;
        public InfoSendToVOBC InfoSendToVOBC = new InfoSendToVOBC();
        DetermineFrontTrain Determine = new DetermineFrontTrain();
        private byte[] Data;
        public VOBCData(byte[] data, HandleVOBCData handle)
        {
            this.Data = data;
            this.Handle = handle;
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
            SetMAObstacle();
        }
        
        private void SetIDofZC()
        {
            InfoSendToVOBC.NID_ZC = 0x01;
            InfoSendToVOBC.ReplyMessageToZC[0] = InfoSendToVOBC.NID_ZC;
            InfoSendToVOBC.ReplyMessageToZC[1] = 0x00;
        }

        private void SetIDofVOBC()
        {
            InfoSendToVOBC.NID_Train = Handle.NID_Train;
            InfoSendToVOBC.ReplyMessageToZC[2] = InfoSendToVOBC.NID_Train;
            InfoSendToVOBC.ReplyMessageToZC[3] = 0x00;
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
            InfoSendToVOBC.ReplyMessageToZC[4] = InfoSendToVOBC.NC_ZC;
        }

        private void SetMAHead()
        {
            byte[] MAHead = new byte[6];
            Array.Copy(Data, 20, MAHead, 0, 2);
            Array.Copy(Data, 24, MAHead, 2, 2);
            MAHead[4] = 0;
            MAHead[5] = 0;
            Array.Copy(MAHead, 0, InfoSendToVOBC.ReplyMessageToZC, 21, 6);
            InfoSendToVOBC.ReplyMessageToZC[27] = Data[33];
        }

        private void SetMATail()
        {
            byte[] MATail = Determine.DetermineMA(Handle);
            Array.Copy(MATail, 0, InfoSendToVOBC.ReplyMessageToZC, 28, 7);
        }

        private void SetNumberOfBarrier()
        {
            int NumOfBarrier = Determine.GetNumOfBarrier(Determine.Route);
            byte BNum = Convert.ToByte(NumOfBarrier);
            InfoSendToVOBC.ReplyMessageToZC[35] = BNum;
        }

        private void SetMALength()
        {
            int NumOfBarrier = Determine.NumOfBarrier;
            byte MALength = Convert.ToByte(17 + 5 * NumOfBarrier);
            InfoSendToVOBC.ReplyMessageToZC[19] = MALength;
        }
        private void SetMAEndType()
        {
            InfoSendToVOBC.ReplyMessageToZC[20] = 0x01;
        }

        private void SetMAObstacle()
        {
            InfoSendToVOBC.Obstacle = new byte[Determine.NumOfBarrier * 5];
            Determine.SetMAObstacle(InfoSendToVOBC.Obstacle, Determine.Route);
        }
    }
}
