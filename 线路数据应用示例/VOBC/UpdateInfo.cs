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

        public UpdateInfo(HandleVOBCData Handle)
        {
            this.VOBCInfo = Handle;
            CancelPreTrainPosition();
            UpDataTrainOccupy();
            UpdataLine();
        }

        private void UpdataLine()
        {
            foreach (var item in NeedChange)
            {
                item.InvalidateVisual();
            }
        }

        private void CancelPreTrainPosition()
        {
            if (PreTrainPosition.Keys.Contains(VOBCInfo.NID_Train))
            {
                string PreTrainHeadPosition = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][0]) * 256 + Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][1])).ToString();
                string PreTrainHeadRailSwitchName = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][2])).ToString();
                Cancel(PreTrainHeadPosition, PreTrainHeadRailSwitchName);
                string PreTrainTailPosition = (Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][6]) * 256 + Convert.ToInt16(PreTrainPosition[VOBCInfo.NID_Train][7])).ToString();
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
                TraverseRailSwitch(PreTrainPosition, PreTrainHeadSwitchName).TrainOccupy = 0;
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
                if (item is Section)
                {
                    if ((item as Section).Name.Substring(0, 3) == TrainPosition)
                    {
                        return (item as Section);
                    }
                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Section)
                {
                    if ((item as Section).Name.Substring(0, 3) == TrainPosition)
                    {
                        return (item as Section);
                    }
                }
            }
            return null;
        }

        private RailSwitch TraverseRailSwitch(string TrainPosition,string TrainRailSwitchName)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    if ((item as RailSwitch).SectionName.Substring(0,3) == TrainPosition && (item as RailSwitch).Name == TrainRailSwitchName)
                    {
                        return (item as RailSwitch);
                    }
                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is RailSwitch)
                {
                    if ((item as RailSwitch).SectionName.Substring(0, 3) == TrainPosition && (item as RailSwitch).Name == TrainRailSwitchName)
                    {
                        return (item as RailSwitch);
                    }
                }
            }
            return null;
        }

        private void UpDataTrainOccupy()
        {
            string CurTrainHeadSectionName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][0]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][1])).ToString();
            string CurTrainHeadName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][2])).ToString();
            int CurTrainHeadOffset = Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][4]);

            string CurTrainTailSectionName = (Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][6]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[VOBCInfo.NID_Train][7])).ToString();
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
