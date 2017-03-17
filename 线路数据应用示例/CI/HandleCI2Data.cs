using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace 线路数据应用示例
{
    class HandleCI2Data
    {
        private string DataString; 
        private int num = 64;

        public HandleCI2Data(byte[] CIData)
        {
            DataString = Conbine(CIData);
            SetRailSwithPosition(DataString);
            SetRailSwithLock(DataString);
            SetSectionRunDirection(DataString);
            SetSectionAxleState(DataString);
            SetAxleSectionAccessLock(DataString);
            SetSignalState(DataString);
            SetAccessState(DataString);
        }

        public string Conbine(byte[] Data)
        {
            StringBuilder MyStringBuilder = new StringBuilder();
            foreach (var item in Data)
            {
                MyStringBuilder.Append(ByteToString(item));
            }
            return MyStringBuilder.ToString();
        }

        private void SetRailSwithPosition(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    (item as RailSwitch).IsPositionNormal = (Data.Substring(num, 1) == "1" ? true : false);
                    num++;
                    (item as RailSwitch).IsPositionReverse = (Data.Substring(num, 1) == "1" ? true : false);
                    num++;
                }
            }
        }

        public void SetRailSwithLock(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is RailSwitch)
                {
                    (item as RailSwitch).Islock = (Data.Substring(num, 1) == "1" ? true : false);
                    num++;
                }
            }
        }

        public void SetSectionRunDirection(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).Direction = Convert.ToInt16(Data.Substring(num, 1)) == 1 ? 0 : 1;
                    num = num + 2;
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
                        (item as RailSwitch).Direction = Convert.ToInt16(Data.Substring(num, 1)) == 1 ? 0 : 1;
                        RailSwitchDir.Add((item as RailSwitch).SectionName, (item as RailSwitch).Direction);
                        num = num + 2;
                    }
                }
            }
        }

        public void SetSectionAxleState(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).AxleOccupy = Convert.ToInt16(Data.Substring(num, 1));

                    System.Windows.Application.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                (item as Section).InvalidateVisual();
                            }));
                    num++;
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
                            (item as RailSwitch).AxleOccupy = Convert.ToInt16(Data.Substring(num, 1));
                            RailSwitchAxleState.Add((item as RailSwitch).SectionName, Convert.ToInt16(Data.Substring(num, 1)));
                            System.Windows.Application.Current.Dispatcher.Invoke(
                             new Action(
                             delegate
                             {
                                 item.InvalidateVisual();
                             }));
                            num++;
                        }
                    }
                }
            }
        }

        public void SetAxleSectionAccessLock(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).IsAccessLock = (Data.Substring(num, 1) == "1" ? true : false);
                    num++;
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
                            (item as RailSwitch).IsAccessLock = (Data.Substring(num, 1) == "1" ? true : false);
                            RailSwitchAccessLock.Add((item as RailSwitch).SectionName, (item as RailSwitch).IsAccessLock);
                            num++;
                        }
                    }
                }
            }
        }

        public void SetSignalState(string Data)
        {
            foreach (var item in MainWindow.stationElements_.Elements)
            {
                if (item is Signal)
                {
                    (item as Signal).IsSignalOpen = (Data.Substring(num, 1) == "1" ? true : false);
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(
                     delegate
                     {
                         item.InvalidateVisual();
                     })); num++;
                }
            }
        }

        public void SetAccessState(string Data)
        {
            num = num + 8;
            foreach (var item in AddCIAccess.CITableListTop)
            {
                item.AccessState = Convert.ToInt16(Data.Substring(num, 1));
                num++;
            }
        }

        public string ByteToString(byte Data)
        {
            Boolean[] bzw = new Boolean[8];
            for (int i = 0, n = 1; i < 8; i++)
            {
                bzw[i] = ((Data & n) == 0 ? false : true);
                n = n << 1;
            }
            string a = null;
            for (int i = 7; i >= 0; i--)
            {
                if (bzw[i] == true)
                {
                    a += "1";
                }
                else
                {
                    a += "0";
                }
            }
            return a;
        }
    }
}
