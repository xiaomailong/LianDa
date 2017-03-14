using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class VOBCEvent : EventArgs
    {
        public VOBCEvent(byte[] trainData)
        {
            this.Traindata = trainData;
        }
        public byte[] Traindata { get; private set; }
    }
}