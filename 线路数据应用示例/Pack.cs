using System;

namespace 线路数据应用示例
{
    class Pack
    {
        int byteFlag_;
        int bitFlag_;
        public byte[] buf_;

        public Pack(int Num)
        {
            this.buf_ = new byte[Num];
            byteFlag_ = 0;
            bitFlag_ = 0;
        }

        public void SetBit(bool flag)
        {
            int c = (buf_[byteFlag_] << bitFlag_);
            buf_[byteFlag_] = flag ? Convert.ToByte(((c | 128) >> bitFlag_)) : Convert.ToByte((c & ~128) >> bitFlag_);
            bitFlag_++;
            if (bitFlag_ == 8)
            {
                Skip();
            }
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