using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线路数据应用示例
{
    class SectionPara
    {
        public static List<SectionPara> SectionParameter = new List<SectionPara>();
        public string Name { get; set; }
        public int Direction { get; set; } // 1表示上行，0表示下行
        public int AxleOccupy { get; set; } //1表示计轴检测为空闲，0表示计轴检测为占用
        public int TrainOccupy { get; set; }//1表示空闲，0表示占用
        public bool IsSwitch { get; set; }
        public int TrainOffset { get; set; }
    }
}
