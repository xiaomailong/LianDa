using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class HandleCI1Data
    {
        MyStruct struct_ = new MyStruct();
        List<bool> Info = new List<bool>();
        int Num;

        public HandleCI1Data(byte[] CIData)
        {
            struct_.Reset(CIData);
            UnpackSwitchStatus();
            UnPackSection();
            int nSignal = UnpackSignal();
            UnPackProtectionSection(nSignal);
            UnPackAccessState();
        }

        private void UnPackAccessState()
        {
            int nAccess = 34;
            Info.Clear();
            for (int i = 0; i < nAccess; i++)
            {
                bool IsAccessOpen = struct_.GetBit();
                Info.Add(IsAccessOpen);
            }
            SetAccessState();
            struct_.Skip();
        }

        private void UnPackProtectionSection(int nSignal)
        {
            for (int i = 0; i < nSignal; i++)
            {
                bool InfoOfProtectSection = struct_.GetBit();
            }
            struct_.Skip();
        }

        private int UnpackSignal()
        {
            int nSignal = struct_.GetUint16();
            Info.Clear();
            for (int i = 0; i < nSignal; i++)
            {
                bool isSignalOpen = struct_.GetBit();
                Info.Add(isSignalOpen);
            }
            SetSignalState();
            struct_.Skip();
            return nSignal;
        }

        private void UnPackSection()
        {
            Info.Clear();
            int nSection = struct_.GetUint16();
            for (int i = 0; i < nSection; i++)
            {
                bool isUpward = struct_.GetBit();
                Info.Add(isUpward);
                bool isDownward = struct_.GetBit();
                Info.Add(isDownward);
            }
            SetSectionRunDirection();
            struct_.Skip();

            Info.Clear();
            for (int i = 0; i < nSection; i++)
            {
                bool isOccupied = struct_.GetBit();
                Info.Add(isOccupied);
            }
            SetSectionAxleState();
            struct_.Skip();

            Info.Clear();
            for (int i = 0; i < nSection; i++)
            {
                bool isRouteLocked = struct_.GetBit();
                Info.Add(isRouteLocked);
            }
            SetAxleSectionAccessLock();
            struct_.Skip();
        }

        private void UnpackSwitchStatus()
        {
            int nSwitch = struct_.GetUint16();
            Info.Clear();
            for (int i = 0; i < nSwitch; i++)
            {
                bool isNormal = struct_.GetBit();
                Info.Add(isNormal);
                bool isReverse = struct_.GetBit();
                Info.Add(isReverse);
            }
            struct_.Skip();
            SetRailSwithPosition();

            Info.Clear();
            for (int i = 0; i < nSwitch; i++)
            {
                bool isLocked = struct_.GetBit();
                Info.Add(isLocked);
            }
            struct_.Skip();
            SetRailSwithLock();
        }

        private void SetRailSwithPosition()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    (item as RailSwitch).IsPositionNormal = Info[Num];
                    Num++;
                    (item as RailSwitch).IsPositionReverse = Info[Num];
                    Num++;
                }
            }
        }

        public void SetRailSwithLock()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    (item as RailSwitch).Islock = Info[Num];
                    Num++;
                }
            }
        }

        public void SetSectionRunDirection()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).Direction = Info[Num] == true ? 0 : 1;
                    Num = Num + 2;
                }
            }
            Dictionary<string, int> RailSwitchDir = new Dictionary<string, int>();
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    if (RailSwitchDir.Keys.Contains((item as RailSwitch).SectionName))
                    {
                        (item as RailSwitch).Direction = RailSwitchDir[(item as RailSwitch).SectionName];
                    }
                    else
                    {
                        (item as RailSwitch).Direction = Info[Num] == true ? 0 : 1;
                        RailSwitchDir.Add((item as RailSwitch).SectionName, (item as RailSwitch).Direction);
                        Num = Num + 2;
                    }
                }
            }
        }

        public void SetSectionAxleState()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).AxleOccupy = Info[Num] == true ? 1 : 0;
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                (item as Section).InvalidateVisual();
                            }));
                    Num++;
                }
            }
            Dictionary<string, int> RailSwitchAxleState = new Dictionary<string, int>();
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    if (item is RailSwitch)
                    {
                        if (RailSwitchAxleState.Keys.Contains((item as RailSwitch).SectionName))
                        {
                            (item as RailSwitch).AxleOccupy = RailSwitchAxleState[(item as RailSwitch).SectionName];
                            System.Windows.Application.Current.Dispatcher.Invoke(
                            new Action(
                            delegate
                            {
                                item.InvalidateVisual();
                            }));
                        }
                        else
                        {
                            (item as RailSwitch).AxleOccupy = Info[Num] == true ? 1 : 0;
                            RailSwitchAxleState.Add((item as RailSwitch).SectionName, (item as RailSwitch).AxleOccupy);
                            System.Windows.Application.Current.Dispatcher.Invoke(
                             new Action(
                             delegate
                             {
                                 item.InvalidateVisual();
                             }));
                            Num++;
                        }
                    }
                }
            }
        }

        public void SetAxleSectionAccessLock()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).IsAccessLock = Info[Num];
                    Num++;
                }
            }
            Dictionary<string, bool> RailSwitchAccessLock = new Dictionary<string, bool>();
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    if (item is RailSwitch)
                    {
                        if (RailSwitchAccessLock.Keys.Contains((item as RailSwitch).SectionName))
                        {
                            (item as RailSwitch).IsAccessLock = RailSwitchAccessLock[(item as RailSwitch).SectionName];
                        }
                        else
                        {
                            (item as RailSwitch).IsAccessLock = Info[Num];
                            RailSwitchAccessLock.Add((item as RailSwitch).SectionName, (item as RailSwitch).IsAccessLock);
                            Num++;
                        }
                    }
                }
            }
        }

        public void SetSignalState()
        {
            Num = 0;
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Signal)
                {
                    (item as Signal).IsSignalOpen = Info[Num];
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(
                     delegate
                     {
                         item.InvalidateVisual();
                     }));
                    Num++;
                }
            }
        }

        public void SetAccessState()
        {
            Num = 0;
            foreach (var item in AddCIAccess.CITableListTop)
            {
                item.AccessState = Info[Num] == true ? 1 : 0;
                Num++;
            }
        }
    }
}
