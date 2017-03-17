using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class InfoSendToCI
    {
        public byte[] LogicSectionState = new byte[200];
        public byte[] TrainAccessInfo = new byte[49];
        public byte[] DataSendToCI;
        public int DataLength = 0;

        StringBuilder SendInfoBuilder;

        public InfoSendToCI()
        {
            SendInfoBuilder = new StringBuilder();
            SetLogicState(MainWindow.stationElements_.Elements);
            SetLogicState(MainWindow.stationElements_1_.Elements);
            SetTrainAccessInfo(AddCIAccess.CITableListTop);
            SetTrainAccessInfo(AddCIAccess.CITableListDown);
            string DataString = SendInfoBuilder.ToString();
            StringToByte(DataString);
        }

        private void StringToByte(string DataString)
        {
            StringBuilder b = new StringBuilder(DataString.Length / 8 + 1);
            int mod4Len = DataString.Length % 8;
            if (mod4Len != 0)
            {
                DataString = DataString.PadRight(((DataString.Length / 8) + 1) * 8, '0');
            }
            DataSendToCI = new byte[DataString.Length / 8 + 8];
            DataLength = DataString.Length / 8 + 8;
            int j = 8;
            for (int i = 0; i < DataString.Length; i += 8)
            {
                string b8 = DataString.Substring(i, 8);
                byte o = Convert.ToByte(b8, 2);
                DataSendToCI[j] = o;
                j++;
            }
        }

        private void SetTrainAccessInfo(List<CItable> CITable)
        {
            foreach (var item in CITable)
            {
                string bit = (item.IsNonComTrainAccess == true ? "0" : "1");
                SendInfoBuilder.Append(bit);
            }
        }

        public void SetLogicState(List<线路绘图工具.GraphicElement> Element)
        {
            foreach (var item in Element)
            {
                if (item is Section)
                {
                    string bitFront = ((item as Section).IsFrontLogicOccupy == true ?"0":"1");
                    SendInfoBuilder.Append(bitFront);
                    string bitLast = ((item as Section).IsLastLogicOccupy == true ? "0" : "1");
                    SendInfoBuilder.Append(bitLast);
                }
            }
            foreach (var item in Element)
            {
                if (item is RailSwitch)
                {
                    string bit = (item as RailSwitch).TrainOccupy.ToString();
                    SendInfoBuilder.Append(bit);
                }
            }
        }
    }
}
