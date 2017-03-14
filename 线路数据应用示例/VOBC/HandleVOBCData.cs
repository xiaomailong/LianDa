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
            if (!Train.Contains(Data[11]))
            {
                Train.Add(Data[11]);
            }
        }
        private void HandleProgress(byte[] Data, GetInfo HandleProcess)
        {
            HandleProcess(Data);
        }
        private void GetTrainID(byte[] Data)
        {
            this._NID_Train = Data[11];
        }
        private void GetZCID(byte[] Data)
        {
            this._NID_ZC = Data[12];
        }
        private void GetNCofTrain(byte[] Data)
        {
            this._NID_ZC = Data[14];
        }
        private void GetTrainPosition(byte[] Data)
        {
            if (TrainPosition.Keys.Contains(Data[10]))
            {
                if (Convert.ToInt16(Data[22]) * 256 + Convert.ToInt16(Data[23]) == Convert.ToInt16(TrainPosition[Data[10]][0]) * 256 + Convert.ToInt16(TrainPosition[Data[10]][1]))
                {
                    if (Convert.ToInt16(Data[28]) * 256 + Convert.ToInt16(Data[29]) == Convert.ToInt16(TrainPosition[Data[10]][6]) * 256 + Convert.ToInt16(TrainPosition[Data[10]][7]))
                    {
                        
                    }
                    else
                    {

                    }
                }












                Array.Copy(Data, 22, TrainPosition[Data[10]], 0, 12);
            }
            else
            {
                int TrainHead = Convert.ToInt16(Data[22] * 256 + Data[23]);
                SetOccupyLine(TrainHead);
                int TrainEnd = Convert.ToInt16(Data[28] * 256 + Data[29]);
                SetOccupyLine(TrainEnd);
                byte[] Position = new byte[12];
                Array.Copy(Data, 22, Position, 0, 12);
                TrainPosition.Add(Data[11], Position);
            }
        }
        private void SetOccupyLine(int TrainIn)
        {
            if (FindSectionByName(TrainIn.ToString()) is Section)
            {
                Section s = FindSectionByName(TrainIn.ToString()) as Section;
                s.TrainOccupy = 0;
                s.InvalidateVisual();
            }
            if (FindSectionByName(TrainIn.ToString()) is RailSwitch)
            {
                RailSwitch s = FindSectionByName(TrainIn.ToString()) as RailSwitch;


                ////////////////////////////////
                //设置道岔有车没做
                ////////////////////////////////
                s.InvalidateVisual();
            }
        }
        private void GetDirection(byte[] Data)
        {
            _Q_TrainRealDirection = Data[35];
            if (TrainDirection.Keys.Contains(Data[11]))
            {
                TrainDirection[Data[11]] = _Q_TrainRealDirection;
            }
            else
            {
                TrainDirection.Add(Data[11], _Q_TrainRealDirection);
            }
        }

        public Device FindSectionByName(string sectionName)
        {
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
            {
                if (item.NodeDevice is Section)
                {
                    if (item.NodeDevice.Name.Substring(0,3) == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName.Substring(0,3) == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
            }
            foreach (var item in MainWindow.stationTopoloty_1_.Nodes)
            {
                if (item.NodeDevice is Section)
                {
                    if (item.NodeDevice.Name.Substring(0,3) == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName.Substring(0,3) == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
            }

            return null;
        }
    }
}
