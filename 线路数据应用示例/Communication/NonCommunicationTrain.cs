using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class NonCommunicationTrain
    {
        HandleVOBCData VOBCInfo = new HandleVOBCData();

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
            }
        }

        private void Judge(List<byte> VOBCList)
        {
            for (int i = 0; i < HandleVOBCData.Train.Count; i++)
            {
                int index = VOBCorCI.VOBCNonCom.IndexOf(HandleVOBCData.Train[i]);
                if (index == -1)
                {
                    byte[] LostTrainPosition = HandleVOBCData.TrainPosition[HandleVOBCData.Train[i]];

                }
            }
        }
    }
}
