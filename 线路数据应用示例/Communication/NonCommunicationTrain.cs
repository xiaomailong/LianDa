using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class NonCommunicationTrain
    {
        public static Dictionary<byte, byte[]> LoseTrain = new Dictionary<byte, byte[]>();

        public void JudgeLostTrain()
        {
            while (true)
            {
                HiPerfTimer hip = new HiPerfTimer();
                hip.Start();
                hip.Interval(900);
                lock (VOBCorCI.VOBCNonCom)
                {
                    Judge(VOBCorCI.VOBCNonCom);
                    VOBCorCI.VOBCNonCom.Clear();
                }
                UpdateLostTrain();
            }
        }

        private void Judge(List<byte> VOBCList)
        {
            lock (HandleVOBCData.TrainPosition)
            {
                foreach (var item in HandleVOBCData.TrainPosition.Keys)
                {
                    if (!VOBCList.Contains(item))
                    {
                        if (!LoseTrain.Keys.Contains(item))
                        {
                            LoseTrain.Add(item, HandleVOBCData.TrainPosition[item]);
                        }
                    }
                }
            }
        }

        private void UpdateLostTrain()
        {
            foreach (var item in LoseTrain.Keys)
            {
                string TrainSection = (Convert.ToInt16(LoseTrain[item][1]) * 256 + Convert.ToInt16(LoseTrain[item][0])).ToString();
                string RailSwitch = (Convert.ToInt16(LoseTrain[item][2])).ToString();
                if (TraverseSection(TrainSection) != null)
                {
                    Section section = TraverseSection(TrainSection);
                    if (!section.HasNonComTrain.Contains(item))
                    {
                        section.HasNonComTrain.Add(item);
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(
                     new Action(
                     delegate
                     {
                         section.InvalidateVisual();
                     }));
                }
                else if (TraverseRailSwitch(TrainSection,RailSwitch) != null)
                {
                    RailSwitch railswitch = TraverseRailSwitch(TrainSection,RailSwitch);
                    if (railswitch.HasNonComTrain.Contains(item))
                    {
                        railswitch.HasNonComTrain.Add(item);
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
                     {
                         railswitch.InvalidateVisual();
                     }));
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

        private RailSwitch TraverseRailSwitch(string TrainPosition, string TrainRailSwitchName)
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
                catch (Exception e)
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
    }
}
