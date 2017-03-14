using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class DetermineFrontTrain
    {
        public List<string> Route = new List<string>();
        public string MASection = "";
        public int MAOffset = 0;
        public int MADir = 0;
        public int NumOfBarrier = 0;
        public List<CItable> CurTrainIn = new List<CItable>();


        public byte[] DetermineMA(HandleVOBCData Handle)
        {
            int TrainSectionInt = Convert.ToInt16(HandleVOBCData.TrainPosition[Handle.NID_Train][0]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[Handle.NID_Train][1]);//纯数字
            string TrainSectionString = TrainSectionInt.ToString();
            Route.Add(TrainSectionString);
            int SectionDir = GetSectionDir(TrainSectionString);
            FindCurTrainIn(TrainSectionString, SectionDir);
            CItable NextAccess = null;
            if (CurTrainIn.Count != 0)
            {
                foreach (var item in CurTrainIn)
                {
                    int num = item.Section.IndexOf(TrainSectionString);
                    for (int i = num; i < item.Section.Count; i++)
                    {
                        if (!Route.Contains(item.Section[i]))
                        {
                            Route.Add(item.Section[i]);
                        }
                        byte[] a = SectionOccpy(item.Section[i]);
                        if (a != null)
                        {
                            return a;
                        }
                        if (IsApproachSection(item.Section[i], GetSectionDir(item.Section[i])) != null)
                        {
                            NextAccess = IsApproachSection(item.Section[i], GetSectionDir(item.Section[i]));
                        }
                    }
                }
            }
            if (NextAccess != null)
            {
                bool HasNextAccessOpen = true;
                while (HasNextAccessOpen)
                {
                    CItable Next = null;
                    for (int i = 0; i < NextAccess.Section.Count; i++)
                    {
                        if (!Route.Contains(NextAccess.Section[i]))
                        {
                            Route.Add(NextAccess.Section[i]);
                        }
                        byte[] a = SectionOccpy(NextAccess.Section[i]);
                        if (a != null)
                        {
                            return a;
                        }
                        Next = IsApproachSection(NextAccess.Section[i], GetSectionDir(Next.Section[i]));
                    }
                    if (Next != null)
                    {
                        NextAccess = Next;
                    }
                    else
                    {
                        HasNextAccessOpen = false;
                    }
                }
            }
            MASection = NextAccess.EndSection;
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
	        {
                if (item.NodeDevice is Section)
                {
                    if ((item.NodeDevice as Section).Name.Substring(0,3) == MASection)
                    {
                        MAOffset = 120;
                        MADir = (item.NodeDevice as Section).Direction;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName.Substring(0,3) == MASection)
                    {
                        MAOffset = SetMAOffset(NextAccess, MainWindow.stationTopoloty_.Nodes);
                        MADir = (item.NodeDevice as RailSwitch).Direction;
                    }
                }
	        }
            foreach (var item in MainWindow.stationTopoloty_1_.Nodes)
            {
                if (item.NodeDevice is Section)
                {
                    if ((item.NodeDevice as Section).Name.Substring(0, 3) == MASection)
                    {
                        MAOffset = 120;
                        MADir = (item.NodeDevice as Section).Direction;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName.Substring(0, 3) == MASection)
                    {
                        MAOffset = SetMAOffset(NextAccess, MainWindow.stationTopoloty_.Nodes);
                        MADir = (item.NodeDevice as RailSwitch).Direction;
                    }
                }
            }
            return ConvertToByte(MASection, MAOffset, MADir); 
        }

        private int SetMAOffset(CItable EndAccess,List<TopolotyNode> StationTopoloty)
        {
            string Last = EndAccess.Section[EndAccess.Section.Count - 2];
            foreach (var item in StationTopoloty)
            {
                if (item.NodeDevice is Section)
                {
                    if ((item.NodeDevice as Section).Name.Substring(0, 3) == Last)
                    {
                        return 50;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName.Substring(0, 3) == Last && (item.NodeDevice as RailSwitch).IsPositionReverse == true)
                    {
                        return 25;
                    }
                }
            }
            return 50;
        }

        private void FindCurTrainIn(string TrainSectionString, int SectionDir)
        {
            foreach (var item in AddCIAccess.CITableListDown)
            {
                if (item.Section.Contains(TrainSectionString) && item.Direction == SectionDir && item.AccessState == 1)
                {
                    CurTrainIn.Add(item);
                }
            }
            foreach (var item in AddCIAccess.CITableListTop)
            {
                if (item.Section.Contains(TrainSectionString) && item.Direction == SectionDir && item.AccessState == 1)
                {
                    CurTrainIn.Add(item);
                }
            }
        }

        private int GetSectionDir(string TrainSection)
        {
            int Dir = FindSectionDirection(MainWindow.stationElements_.Elements, TrainSection);
            if (Dir == 2)
            {
                Dir = FindSectionDirection(MainWindow.stationElements_1_.Elements, TrainSection);
            }
            return Dir;
        }

        private int FindSectionDirection(List<线路绘图工具.GraphicElement> Elements, string TrainSection)
        {
            foreach (var item in Elements)
            {
                if (item is Section)
                {
                    if ((item as Section).Name.Substring(0, 3) == TrainSection)
                    {
                        return (item as Section).Direction;
                    }
                }
                else if (item is RailSwitch)
                {
                    if ((item as RailSwitch).SectionName.Substring(0, 3) == TrainSection)
                    {
                        return (item as RailSwitch).Direction;
                    }
                }
            }
            return 2;
        }

        public byte[] SectionOccpy(string CurSection)
        {
            byte[] OccupyMA = SectionOccpyJudge(CurSection, MainWindow.stationElements_.Elements);
            if (OccupyMA == null)
            {
                SectionOccpyJudge(CurSection, MainWindow.stationElements_1_.Elements);
            }
            return OccupyMA;
        }

        private byte[] SectionOccpyJudge(string CurSection, List<线路绘图工具.GraphicElement> Elements)
        {
            foreach (var item in Elements)
            {
                if (item is Section)
	            {
                    if ((item as Section).Name.Substring(0,3) == CurSection)
	                {
		                if ((item as Section).TrainOccupy == 0 || (item as Section).AxleOccupy == 0)
                        {
                            MASection = item.Name;
                            MAOffset = (item as Section).Offset;
                            MADir = (item as Section).Direction;
                            return ConvertToByte(MASection, MAOffset, MADir);
                        }
	                }
	            }
                else if (item is RailSwitch)
	            {
                    if ((item as RailSwitch).SectionName.Substring(0,3) == CurSection)
	                {
		                if ((item as RailSwitch).TrainOccupy == 0 || (item as RailSwitch).AxleOccupy == 0)
                        {
                            MASection = item.Name;
                            MADir = (item as RailSwitch).Direction;
                            MAOffset = (item as RailSwitch).Offset;
                            return ConvertToByte(MASection, MAOffset, MADir);
                        }
	                }
	            }
            }
            return null;
        }

        public byte[] ConvertToByte(string MASection, int MAOffset, int MADir)
        {
            byte[] MAEnd = new byte[7];
            int MAInt = Convert.ToInt16(MASection.Substring(0,3));
            if (MAInt > 255)
            {
                MAEnd[0] = 1;
                MAEnd[1] = Convert.ToByte(MAInt - 256);
            }
            else
            {
                MAEnd[0] = 0;
                MAEnd[1] = Convert.ToByte(MAInt);
            }
            MAEnd[2] = 0;
            MAEnd[3] = 0;
            MAEnd[4] = 0;
            MAEnd[5] = Convert.ToByte(MAOffset);
            MAEnd[6] = Convert.ToByte(MADir);
            return MAEnd;
        }

        public CItable IsApproachSection(string SectionName, int Direction)
        {
            foreach (var item in AddCIAccess.CITableListDown)
            {
                if (item.StartSection == SectionName && item.AccessState == 1 && item.Direction == Direction)
                {
                    return item;
                }
            }
            return null;
        }

        public int GetNumOfBarrier(List<string> Route)
        {
            foreach (var item in Route)
            {
                foreach (var element in MainWindow.stationElements_.Elements)
                {
                    if (element is RailSwitch)
	                {
		                if ((element as RailSwitch).SectionName.Substring(0,3) == item)
	                    {
		                    NumOfBarrier++;
                            break;
	                    }
	                }
                }
                foreach (var element in MainWindow.stationElements_1_.Elements)
                {
                    if (element is RailSwitch)
	                {
                        if ((element as RailSwitch).SectionName.Substring(0,3) == item)
	                    {
                            NumOfBarrier++;
                            break;
	                    }
	                }
                }
            }
            return NumOfBarrier;
        }
      
        public byte[] SetMAObstacle(byte[] Obstacle, List<string> Route)
        {
            List<byte[]> ObstacleCollection = new List<byte[]>();
            foreach (var item in Route)
            {
                foreach (var element in MainWindow.stationElements_.Elements)
                {

                }
                foreach (var element in MainWindow.stationElements_1_.Elements)
                {

                }
            }
            return null;
        }
    }
}
