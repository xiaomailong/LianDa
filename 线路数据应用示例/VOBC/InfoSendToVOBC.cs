using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class InfoSendToVOBC
    {
        #region 属性
        public byte[] Obstacle { get; set; }

        private byte[] _ReplyMessageToZC = new byte[36];
        public byte[] ReplyMessageToZC
        {
            get { return _ReplyMessageToZC; }
            set { _ReplyMessageToZC = value; }
        }

        private byte _NID_ZC;
        public byte NID_ZC
        {
            get { return _NID_ZC; }
            set { _NID_ZC = value; }
        }
        private byte _NID_Train;
        public byte NID_Train
        {
            get { return _NID_Train; }
            set { _NID_Train = value; }
        }
        private byte _NC_ZC;
        public byte NC_ZC
        {
            get { return _NC_ZC; }
            set { _NC_ZC = value; }
        }
        #endregion
        public InfoSendToVOBC()
        {
            this._ReplyMessageToZC[5] = 0xaa;
            this._ReplyMessageToZC[6] = 0x71;
            this._ReplyMessageToZC[7] = 0xdb;
            this._ReplyMessageToZC[8] = 0x11;
            this._ReplyMessageToZC[9] = 0x4c;
            this._ReplyMessageToZC[10] = 0x00;
            this._ReplyMessageToZC[11] = 0x01;
            this._ReplyMessageToZC[12] = 0x01;
            this._ReplyMessageToZC[13] = 0x02;
            this._ReplyMessageToZC[14] = 0x00;
            this._ReplyMessageToZC[15] = 0x00;
            this._ReplyMessageToZC[16] = 0xaa;
            this._ReplyMessageToZC[17] = 0xff;
            this._ReplyMessageToZC[18] = 0xff;
        }
    }
}
