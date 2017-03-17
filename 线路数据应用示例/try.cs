using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using 线路绘图工具;

namespace 线路数据应用示例
{
    class @try
    {
        StationElements element = new StationElements();

        public Device FindSectionByName(string sectionName)
        {
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
            {
                if (item.NodeDevice is Section)
                {
                    if (item.NodeDevice.Name == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
                else if (item.NodeDevice is RailSwitch)
                {
                    if ((item.NodeDevice as RailSwitch).SectionName == sectionName)
                    {
                        return item.NodeDevice as Device;
                    }
                }
            }
            return null;
        }
        public Device FindSignalByName(string sectionName)
        {
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Signal)
                {
                    if ((item as Signal).Name == sectionName)
                    {
                        return item as Device;
                    }
                }
            }
            return null;
        }

        public Device Left()
        {
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
            {
                if (item.NodeDevice.Name == "112G")
                {
                    foreach (var items in item.LeftNodes)
                    {
                        if (items.NodeDevice is RailSwitch)
	                    {
		                    return (items.NodeDevice as Device);
	                    }
                    }
                }
            }
            return null;
        }

        public void a()
        {
            foreach (var item in MainWindow.stationTopoloty_.Nodes)
            {
                if (item.NodeDevice.Name == "315G")
                {
                    foreach (var items in item.RightNodes)
                    {
                        MessageBox.Show(items.NodeDevice.Name);
                    }
                }
            }
            foreach (var item in MainWindow.stationTopoloty_1_.Nodes)
            {
                if (item.NodeDevice.Name == "315G")
                {
                    foreach (var items in item.RightNodes)
                    {
                        MessageBox.Show(items.NodeDevice.Name);
                    }
                }
            }
        }


        public void GetElement()
        {
            StationTopoloty topo = (App.Current.MainWindow as MainWindow).Topo;
            TopolotyNode startNode = topo.Nodes[0];

            线路绘图工具.Device device = startNode.FindDeviceByDistance(81.0);
            if (device != null)
            {
                MessageBox.Show(device.Name);
            }
            else
            {
                int a = device.ID;
            }
        }

        public void FindAllRail()
        {
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is RailSwitch)
                {
                    MessageBox.Show((item as RailSwitch).SectionName);
                }
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

        public byte[] f = { 1,2,3,4};
        public string Conbine(byte[] Data)
        {
            StringBuilder MyStringBuilder = new StringBuilder();
            foreach (var item in Data)
            {
                MyStringBuilder.Append(ByteToString(item));
            }
            return MyStringBuilder.ToString();
        }
        private string DataString = "";
        private int num = 0;
        public void Begin()
        {
            DataString = Conbine(f);
            SetRailSwithPosition(DataString);
            SetRailSwithLock(DataString);
            SetSectionRunDirection(DataString);
            SetSectionAxleState(DataString);
            SetAxleSectionAccessLock(DataString);
            SetSignalState(DataString);
            SetAccessState(DataString);
        }
        private void SetRailSwithPosition(string Data)
        {
            foreach (var item in MainWindow.stationElements_1_.Elements)
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
            foreach (var item in MainWindow.stationElements_1_.Elements)
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
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).Direction = Convert.ToInt16(Data.Substring(num, 1));
                    num++;
                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                Dictionary<string, int> RailSwitchDir = new Dictionary<string, int>();
                if (item is RailSwitch)
                {
                    if (RailSwitchDir.Keys.Contains((item as RailSwitch).SectionName))
                    {
                        (item as RailSwitch).Direction = RailSwitchDir[(item as RailSwitch).SectionName];
                    }
                    else
                    {
                        (item as RailSwitch).Direction = Convert.ToInt16(Data.Substring(num, 1));
                        RailSwitchDir.Add((item as RailSwitch).SectionName, Convert.ToInt16(Data.Substring(num, 1)));
                        num++;
                    }
                }
            }
        }

        public void SetSectionAxleState(string Data)
        {
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).AxleOccupy = Convert.ToInt16(Data.Substring(num, 1));
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(
                     delegate
                     {
                         item.InvalidateVisual();
                     })); num++;
                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
	        {
                if (item is RailSwitch)
                {
                    Dictionary<string, int> RailSwitchAxleState = new Dictionary<string, int>();
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
                            item.InvalidateVisual();
                            num++;
                        }
                    }
                }
	        }
        }

        public void SetAxleSectionAccessLock(string Data)
        {
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Section)
                {
                    (item as Section).IsAccessLock = (Data.Substring(num, 1) == "1" ? true : false);
                    num++;
                }
            }
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is RailSwitch)
                {
                    Dictionary<string, bool> RailSwitchAccessLock = new Dictionary<string, bool>();
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
            foreach (var item in MainWindow.stationElements_1_.Elements)
            {
                if (item is Signal)
                {
                    (item as Signal).IsSignalOpen = (Data.Substring(num, 1) == "1" ? true : false);
                    item.InvalidateVisual();
                    num++;
                }
            }
        }

        public void SetAccessState(string Data)
        {
            num = num + 8;
            foreach (var item in AddCIAccess.CITableListDown)
            {
                item.AccessState = Convert.ToInt16(Data.Substring(num,1));
                num++;
            }
        }
    }
}
