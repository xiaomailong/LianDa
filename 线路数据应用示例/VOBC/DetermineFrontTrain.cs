﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class DetermineFrontTrain
    {
        public string MASection = "";
        public int MAOffset = 0;
        public int MADir = 0;
        public int NumOfBarrier = 0;
        public List<CItable> CurTrainIn = new List<CItable>();
        public List<string> Route = new List<string>();

        public byte[] DetermineMA(HandleVOBCData Handle)
        {
            int TrainSectionInt = Convert.ToInt16(HandleVOBCData.TrainPosition[Handle.NID_Train][1]) * 256 + Convert.ToInt16(HandleVOBCData.TrainPosition[Handle.NID_Train][0]);//纯数字
            int RailSwitchInt = Convert.ToInt16(HandleVOBCData.TrainPosition[Handle.NID_Train][2]);
            string RailSwitchString = RailSwitchInt.ToString();
            string TrainSectionString = TrainSectionInt.ToString();
            int TrainDir = Handle.Q_TrainRealDirection;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                try
                {
                    if(item is Section)
                    {
                        if (item.Name.Substring(0, 3) == TrainSectionString)
                        {
                            FindCurTrainIn(TrainSectionString + "-0", TrainDir);
                        }
                    }
                    if (item is RailSwitch)
                    {
                        if ((item as RailSwitch).SectionName.Substring(0,3) == TrainSectionString)
                        {
                            FindCurTrainIn(TrainSectionString + "-" + RailSwitchString, TrainDir);

                        }
                    }
                }
                catch(Exception e)
                {

                }
            }
            CItable NextAccess = null;
            if (CurTrainIn.Count != 0)
            {
                foreach (var item in CurTrainIn)
                {
                    int num = item.Section.IndexOf(TrainSectionString + "-" + RailSwitchString);
                    for (int i = num; i < item.Section.Count; i++)
                    {
                        if (!Route.Contains(item.Section[i]))
                        {
                            Route.Add(item.Section[i]);
                        }
                        byte[] a = SectionAxleOccpy(item.Section[i],TrainDir);
                        if (a != null)
                        {
                            return a;
                        }
                        if (IsApproachSection(item.Section[i], TrainDir) != null)
                        {
                            NextAccess = IsApproachSection(item.Section[i], TrainDir);
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
                        if (NextAccess.Section[i] == TrainSectionString + "-" + RailSwitchString)
                        {

                        }
                        else
                        {
                            if (!Route.Contains(NextAccess.Section[i]))
                            {
                                Route.Add(NextAccess.Section[i]);
                            }
                            byte[] a = SectionOccpy(NextAccess.Section[i], TrainDir);
                            if (a != null)
                            {
                                return a;
                            }
                            Next = IsApproachSection(NextAccess.Section[i], TrainDir);
                        }
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
            else
            {
                NextAccess = CurTrainIn[CurTrainIn.Count - 1];
            }

            MASection = NextAccess.EndSection;
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
	        {
                try
                {
                    if (item.NodeDevice is Section)
                    {
                        if ((item.NodeDevice as Section).Name.Substring(0, 3) == MASection.Substring(0, 3))
                        {
                            MAOffset = 100;
                            MADir = TrainDir;
                        }
                    }
                    else if (item.NodeDevice is RailSwitch)
                    {
                        if ((item.NodeDevice as RailSwitch).SectionName.Substring(0, 3) == MASection.Substring(0, 3)
                            && (item.NodeDevice as RailSwitch).Name == MASection.Substring(4))
                        {
                            MAOffset = SetMAOffset(item.NodeDevice);
                            MADir = TrainDir;
                        }
                    }
                }
                catch(Exception e)
                {

                }
	        }
            foreach (var item in MainWindow.stationTopoloty_1_.Nodes)
            {
                try
                {
                    if (item.NodeDevice is Section)
                    {
                        if ((item.NodeDevice as Section).Name.Substring(0, 3) == MASection.Substring(0, 3))
                        {
                            MAOffset = 100;
                            MADir = TrainDir;
                        }
                    }
                    else if (item.NodeDevice is RailSwitch)
                    {
                        if ((item.NodeDevice as RailSwitch).SectionName.Substring(0, 3) == MASection.Substring(0, 3)
                            && (item.NodeDevice as RailSwitch).Name == MASection.Substring(4))
                        {
                            MAOffset = SetMAOffset(item.NodeDevice);
                            MADir = TrainDir;
                        }
                    }
                }
                catch(Exception e)
                {

                }
            }
            return ConvertToByte(MASection, MAOffset, MADir); 
        }

        private int SetMAOffset(线路绘图工具.Device RailSwitch)
        {
            if ((RailSwitch as RailSwitch).SectionName.Substring(0,3) == "110" || (RailSwitch as RailSwitch).SectionName.Substring(0,3) == "111"
                || (RailSwitch as RailSwitch).SectionName.Substring(0, 3) == "118" || (RailSwitch as RailSwitch).SectionName.Substring(0, 3) == "119")
            {
                if ((RailSwitch as RailSwitch).IsPositionNormal == true && (RailSwitch as RailSwitch).IsPositionReverse == false)
                {
                    return 45;
                }
                else
                {
                    return 20;
                }
            }
            else
            {
                return 20;
            }
        }

        private void FindCurTrainIn(string TrainSectionString, int TrainDir)
        {
            foreach (var item in AddCIAccess.CITableListDown)
            {
                if (item.Section.Contains(TrainSectionString) && item.Direction == TrainDir && item.AccessState == 1)
                {
                    CurTrainIn.Add(item);
                }
            }
            foreach (var item in AddCIAccess.CITableListTop)
            {
                if (item.Section.Contains(TrainSectionString) && item.Direction == TrainDir && item.AccessState == 1)
                {
                    CurTrainIn.Add(item);
                }
            }
        }

        public byte[] SectionAxleOccpy(string CurSection,int TrainDir)
        {
            byte[] OccupyMA = SectionAxleOccpyJudge(CurSection, MainWindow.stationElements_.Elements,TrainDir);
            if (OccupyMA == null)
            {
                SectionAxleOccpyJudge(CurSection, MainWindow.stationElements_1_.Elements,TrainDir);
            }
            return OccupyMA;
        }

        private byte[] SectionAxleOccpyJudge(string CurSection, List<线路绘图工具.GraphicElement> Elements,int TrainDir)
        {
            foreach (var item in Elements)
            {
                if (item is Section)
	            {
                    if ((item as Section).Name.Substring(0,3) == CurSection.Substring(0,3))
	                {
		                if ((item as Section).AxleOccupy == 0)
                        {
                            MASection = item.Name.Substring(0,3);
                            MAOffset = (item as Section).Offset;/////////////////////////////////
                            MADir = TrainDir;
                            return ConvertToByte(MASection, MAOffset, MADir);
                        }
	                }
	            }
                else if (item is RailSwitch)
	            {
                    if ((item as RailSwitch).SectionName.Substring(0, 3) == CurSection.Substring(0, 3) 
                        && (item as RailSwitch).Name == CurSection.Substring(4))
	                {
		                if ((item as RailSwitch).AxleOccupy == 0)
                        {
                            MASection = (item as RailSwitch).SectionName.Substring(0,3);
                            MADir = TrainDir;
                            MAOffset = (item as RailSwitch).Offset;/////////////////////////////////////
                            return ConvertToByte(MASection, MAOffset, MADir);
                        }
	                }
	            }
            }
            return null;
        }


        public byte[] SectionOccpy(string CurSection, int TrainDir)
        {
            byte[] OccupyMA = SectionOccpyJudge(CurSection, MainWindow.stationElements_.Elements, TrainDir);
            if (OccupyMA == null)
            {
                SectionOccpyJudge(CurSection, MainWindow.stationElements_1_.Elements, TrainDir);
            }
            return OccupyMA;
        }

        private byte[] SectionOccpyJudge(string CurSection, List<线路绘图工具.GraphicElement> Elements, int TrainDir)
        {
            foreach (var item in Elements)
            {
                if (item is Section)
                {
                    if ((item as Section).Name.Substring(0, 3) == CurSection.Substring(0, 3))
                    {
                        if ((item as Section).TrainOccupy == 0 || (item as Section).AxleOccupy == 0)
                        {
                            MASection = item.Name.Substring(0, 3);
                            MAOffset = (item as Section).Offset;/////////////////////////////////
                            MADir = TrainDir;
                            return ConvertToByte(MASection, MAOffset, MADir);
                        }
                    }
                }
                else if (item is RailSwitch)
                {
                    if ((item as RailSwitch).SectionName.Substring(0, 3) == CurSection.Substring(0, 3)
                        && (item as RailSwitch).Name == CurSection.Substring(4))
                    {
                        if ((item as RailSwitch).TrainOccupy == 0 || (item as RailSwitch).AxleOccupy == 0)
                        {
                            MASection = (item as RailSwitch).SectionName.Substring(0, 3);
                            MADir = TrainDir;
                            MAOffset = (item as RailSwitch).Offset;/////////////////////////////////////
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
            if (MAInt > 256)
            {
                MAEnd[0] = Convert.ToByte(MAInt - 256);
                MAEnd[1] = 1;
            }
            else
            {
                MAEnd[0] = Convert.ToByte(MAInt);
                MAEnd[1] = 0;
            }
            MAEnd[2] = Convert.ToByte(MAOffset);
            MAEnd[3] = 0;
            MAEnd[4] = 0;
            MAEnd[5] = 0;
            if (MADir == 0)
            {
                MAEnd[6] = 0xaa;
            }
            else if (MADir == 1)
            {
                MAEnd[6] = 0x55;
            }
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
            foreach (var item in AddCIAccess.CITableListTop)
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
		                if ((element as RailSwitch).SectionName.Substring(0,3) == item.Substring(0,3))
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
                        if ((element as RailSwitch).SectionName.Substring(0,3) == item.Substring(0,3))
	                    {
                            NumOfBarrier++;
                            break;
	                    }
	                }
                }
            }
            return NumOfBarrier;
        }
      
        public void SetMAObstacle(byte[] Obstacle, List<string> Route)
        {
            List<byte[]> ObstacleCollection = new List<byte[]>();
            foreach (var item in Route)
            {
                AddObstacleCollection(MainWindow.stationElements_, ObstacleCollection, item);
                AddObstacleCollection(MainWindow.stationElements_1_, ObstacleCollection, item);
            }
            if (ObstacleCollection.Count != 0)
            {
                for (int i = 0; i < ObstacleCollection.Count; i++)
                {
                    ObstacleCollection[i].CopyTo(Obstacle,i*5);
                }
            }

        }

        public void AddObstacleCollection(StationElements StationElements, List<byte[]> ObstacleCollection, string Section)
        {
            foreach (var element in StationElements.Elements)
            {
                if (element is RailSwitch && (element as RailSwitch).SectionName.Substring(0, 3) == Section.Substring(0, 3) && (element as RailSwitch).Name == Section.Substring(4))
                {
                    byte[] obstacle = new byte[5];
                    obstacle[0] = 0x04;
                    byte[] ID = System.BitConverter.GetBytes(Convert.ToInt16((element as RailSwitch).SectionName.Substring(0, 3)));
                    Array.Copy(ID, 0, obstacle, 1, 2);
                    obstacle[3] = ((element as RailSwitch).IsPositionNormal == true ? Convert.ToByte(1) : Convert.ToByte(2));
                    obstacle[4] = obstacle[3];
                    ObstacleCollection.Add(obstacle);
                }
            }
        }
    }
}