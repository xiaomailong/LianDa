using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 线路绘图工具;

namespace 线路数据应用示例
{
    class HandleVOBCData
    {
        public static Dictionary<byte, byte> TrainDirection = new Dictionary<byte, byte>();
        public static Dictionary<byte, byte[]> TrainPosition = new Dictionary<byte, byte[]>();
        public static List<byte> Train = new List<byte>();
        public delegate void GetInfo(byte[] Data);
        #region 属性
        private byte _NID_Train;
        public byte NID_Train
        {
            private set { _NID_Train = value; }
            get { return _NID_Train; }
        }
        private byte _NID_ZC;
        public byte NID_ZC
        {
            private set { _NID_ZC = value; }
            get { return _NID_ZC; }
        }
        private byte _NC_Train;
        public byte NC_Train
        {
            private set { _NC_Train = value; }
            get { return _NC_Train; }
        }
        private byte _Q_TrainRealDirection;
        public byte Q_TrainRealDirection
        {
            private set { _Q_TrainRealDirection = value; }
            get { return _Q_TrainRealDirection; }
        }
        #endregion
        public void AddProgress(byte[] Data)
        {
            GetInfo GetInfo = new GetInfo(GetTrainID);
            GetInfo += GetZCID;
            GetInfo += GetNCofTrain;
            GetInfo += GetTrainPosition;
            GetInfo += GetDirection;
            GetInfo += AddTrain;
            HandleProgress(Data, GetInfo);
        }
        private void AddTrain(byte[] Data)
        {
            if (!Train.Contains(Data[9]))
            {
                Train.Add(Data[9]);
            }
        }
        private void HandleProgress(byte[] Data, GetInfo HandleProcess)
        {
            HandleProcess(Data);
        }
        private void GetTrainID(byte[] Data)
        {
            this._NID_Train = Data[9];
        }
        private void GetZCID(byte[] Data)
        {
            this._NID_ZC = Data[10];
        }

        private void GetNCofTrain(byte[] Data)
        {
            this._NID_ZC = Data[12];
        }

        private void GetTrainPosition(byte[] Data)
        {
            if (TrainPosition.Keys.Contains(Data[10]))
            {
                Array.Copy(Data, 20, TrainPosition[Data[10]], 0, 12);
            }
            else
            {
                byte[] Position = new byte[12];
                Array.Copy(Data, 20, Position, 0, 12);
                TrainPosition.Add(Data[11], Position);
            }
        }

        private void GetDirection(byte[] Data)
        {
            _Q_TrainRealDirection = Data[33];
            if (TrainDirection.Keys.Contains(Data[9]))
            {
                TrainDirection[Data[9]] = _Q_TrainRealDirection;
            }
            else
            {
                TrainDirection.Add(Data[9], _Q_TrainRealDirection);
            }
        }
    }
}