using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 线路绘图工具;

namespace 线路数据应用示例
{
    class UpdateInfo
    {
        List<Device> NeedChange = new List<Device>();
        HandleVOBCData VOBCInfo;
        public static Dictionary<byte, byte[]> PreTrainPosition = new Dictionary<byte, byte[]>();
        byte[] Data;

        public UpdateInfo(HandleVOBCData Handle, byte[] Data)
        {
            this.Data = Data;
            this.VOBCInfo = Handle;
            CancelPreTrainPosition();
            UpDataTrainOccupy();
            UpdataLine();
            UpdatePrePosition();
        }

        private void UpdataLine()
        {
            foreach (var item in NeedChange)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                new Action(
                delegate
                {
                    item.InvalidateVisual();
                }));
            }
        }

        public void UpdatePrePosition()
        {
            if (PreTrainPosition.Keys.Contains(VOBCInfo.NID_Train))
            {
                //byte[] pre = new byte[12];
                Array.Copy(Data, 20, PreTrainPosition[VOBCInfo.NID_Train], 0, 12);
                //PreTrainPosition[VOBCInfo.NID_Train] = HandleVOBCData.TrainPosition[VOBCInfo.NID_Train];
            }
            else
            {
                byte[] pre = new byte[12];
                Array.Copy(Data, 20, pre, 0, 12);
                PreTrainPosition.Add(VOBCInfo.NID_Train, pre);
            }
        }

        private void CancelPreTrainPosition()
        {
            if (PreTrainPosition.Keys.Contains(VOBCInfo.NID_Train))
            {
                string PreTrainHeadPosition = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][1]) * 256 + Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][0])).ToString();
                string PreTrainHeadRailSwitchName = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][2])).ToString();
                Cancel(PreTrainHeadPosition, PreTrainHeadRailSwitchName);
                string PreTrainTailPosition = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][7]) * 256 + Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][6])).ToString();
                string PreTrainTailRailSwitchName = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][8])).ToString();
                Cancel(PreTrainTailPosition, PreTrainTailRailSwitchName);
            }
        }

        private void Cancel(string PreTrainPosition,string PreTrainHeadSwitchName)
        {
            if (TraverseSection(PreTrainPosition) != null)
            {
                TraverseSection(PreTrainPosition).TrainOccupy = 1;
                TraverseSection(PreTrainPosition).IsFrontLogicOccupy = false;
                TraverseSection(PreTrainPosition).IsLastLogicOccupy = false;
                if (!NeedChange.Contains(TraverseSection(PreTrainPosition) as Device))
                {
                    NeedChange.Add(TraverseSection(PreTrainPosition));
                }
            }
            else if(TraverseRailSwitch(PreTrainPosition, PreTrainHeadSwitchName) != null)
            {
                TraverseRailSwitch(PreTrainPosition, PreTrainHeadSwitchName).TrainOccupy = 1;
                if (!NeedChange.Contains(TraverseRailSwitch(PreTrainPosition, PreTrainHeadSwitchName) as Device))
                {
                    NeedChange.Add(TraverseRailSwitch(PreTrainPosition, PreTrainHeadSwitchName));
                }
            }

        }

        private Section TraverseSection(string TrainPosition)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                try
                {
                    if (item is Section)
                    {
                        if ((item as Section).Name.Substring(0, 3) == TrainPosition)
                        {
                            return (item as Section);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                try
                {
                    if (item is Section)
                    {
                        if ((item as Section).Name.Substring(0, 3) == TrainPosition)
                        {
                            return (item as Section);
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }

        private RailSwitch TraverseRailSwitch(string TrainPosition,string TrainRailSwitchName)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                try
                {
                    if (item is RailSwitch)
                    {
                        if ((item as RailSwitch).SectionName.Substring(0, 3) == TrainPosition && (item as RailSwitch).Name == TrainRailSwitchName)
                        {
                            return (item as RailSwitch);
                        }
                    }
                }
                catch ( Exception e)
                {

                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                try
                {
                    if (item is RailSwitch)
                    {
                        if ((item as RailSwitch).SectionName.Substring(0, 3) == TrainPosition && (item as RailSwitch).Name == TrainRailSwitchName)
                        {
                            return (item as RailSwitch);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return null;
        }

        private void UpDataTrainOccupy()
        {
            string CurTrainHeadSectionName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][1]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][0])).ToString();
            string CurTrainHeadName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][2])).ToString();
            int CurTrainHeadOffset = Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][4]);

            string CurTrainTailSectionName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][7]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][6])).ToString();
            string CurTrainTailName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][8])).ToString();
            int CurTrainTailOffset = Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][10]);

            Updata(CurTrainHeadSectionName, CurTrainHeadName, CurTrainHeadOffset);
            Updata(CurTrainTailSectionName, CurTrainTailName, CurTrainTailOffset);
        }

        public void Updata(string CurTrainSectionName,string CurTrainName,int CurTrainOffset)
        {
            if (TraverseSection(CurTrainSectionName) != null)
            {
                if (!NeedChange.Contains(TraverseSection(CurTrainSectionName) as Device))
                {
                    NeedChange.Add(TraverseSection(CurTrainSectionName));
                }
                TraverseSection(CurTrainSectionName).TrainOccupy = 0;
                if (VOBCInfo.Q_TrainRealDirection == 1)
                {
                    if (CurTrainOffset <= 60)
                    {
                        TraverseSection(CurTrainSectionName).IsFrontLogicOccupy = true;
                    }
                    else
                    {
                        TraverseSection(CurTrainSectionName).IsLastLogicOccupy = true;
                    }
                }
                if (VOBCInfo.Q_TrainRealDirection == 0)
                {
                    if (CurTrainOffset <= 60)
                    {
                        TraverseSection(CurTrainSectionName).IsLastLogicOccupy = true;
                    }
                    else
                    {
                        TraverseSection(CurTrainSectionName).IsFrontLogicOccupy = true;
                    }
                }
            }
            else if (TraverseRailSwitch(CurTrainSectionName, CurTrainName) != null)
            {
                if (!NeedChange.Contains(TraverseRailSwitch(CurTrainSectionName, CurTrainName) as Device))
                {
                    NeedChange.Add(TraverseRailSwitch(CurTrainSectionName, CurTrainName));
                }
                TraverseRailSwitch(CurTrainSectionName, CurTrainName).TrainOccupy = 0;
            }
        }
    }
}
