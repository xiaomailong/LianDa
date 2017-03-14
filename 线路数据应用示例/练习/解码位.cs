using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class 解码位
    {
        public void adsf()
        {
            byte b = 5;//0x14  
            Boolean[] bzw = new Boolean[8];
            for (int i = 0, n = 1; i < 8; i++)
            {
                bzw[i] = ((b & n) == 0 ? false : true);
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
            Console.WriteLine(a);
            Console.ReadKey();
        }
    }
}