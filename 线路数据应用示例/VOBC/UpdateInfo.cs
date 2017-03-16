using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class UpdateInfo
    {
        HandleVOBCData VOBCInfo;
        public static Dictionary<byte, byte[]> PreTrainPosition = new Dictionary<byte, byte[]>();

        public UpdateInfo(HandleVOBCData Handle)
        {
            this.VOBCInfo = Handle;
        }

        private void CancelPreTrainPosition()
        {
            if (PreTrainPosition.Keys.Contains(VOBCInfo.NID_Train))
            {

            }
        }



        private void UpDataTrainOccupy()
        {

        }
    }
}
