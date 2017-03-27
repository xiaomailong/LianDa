using System;

namespace 线路数据应用示例
{
    class Unpack
    {
        int byteFlag_ = 12;
        int bitFlag_ = 0;
        byte[] buf_;

        public void Reset(byte[] buf)
        {
            buf_ = buf;
            byteFlag_ = 12;
            bitFlag_ = 0;
        }

        public UInt16 GetUint16()
        {
            UInt16 value = (UInt16)(buf_[byteFlag_ + 1] << 8);
            value |= buf_[byteFlag_];
            byteFlag_ += 2;
            return value;
        }

        public bool GetBit()
        {
            bool result = ((buf_[byteFlag_] >> (7 - bitFlag_++)) & 1) == 1;
            if (bitFlag_ == 8)
            {
                Skip();
            }
            return result;
        }

        public void Skip()
        {
            if (bitFlag_ != 0)
            {
                byteFlag_++;
                bitFlag_ = 0;
            }
        }
    }
}
