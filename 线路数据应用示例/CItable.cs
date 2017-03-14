using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace 线路数据应用示例
{
    class AddCIAccess
    {
        public static List<CItable> CITableListTop = new List<CItable>();
        public static List<CItable> CITableListDown = new List<CItable>();
        public AddCIAccess()
        {
            AddAccess("CITableTop.csv", CITableListTop);
            AddAccess("CITableDown.csv", CITableListDown);
        }
        public void AddAccess(string path, List<CItable> CITableList)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            List<string> list = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }
            sr.Close();
            foreach (var item in list)
            {
                string[] strs = item.Split(new char[] { ',' });
                CItable access = new CItable();
                access.Number = Convert.ToInt16(strs[0]);
                access.StartSection = strs[1];
                access.EndSection = strs[2];
                access.AccessState = 0;
                access.Direction = Convert.ToInt16(strs[6]);
                string[] section = strs[5].Split(new char[] { '#' });
                access.Section = new List<string>();
                foreach (var sec in section)
                {
                    access.Section.Add(sec);
                }
                CITableList.Add(access);
            }
        }
    }
    class CItable
    {
        private List<string> _Section = new List<string>();
        public int Number { get; set; }
        public string StartSection { get; set; }
        public string EndSection { get; set; }
        public string StartSignal { get; set; }
        public string EndSignal{ get; set; }
        bool isNonComTrainAccess_ = false;
        public bool IsNonComTrainAccess
        {
            get { return isNonComTrainAccess_; }
            set
            {
                if (isNonComTrainAccess_ != value)
                {
                    isNonComTrainAccess_ = value;
                }
            }
        }
        public List<string> Section
        {
            get { return _Section; }
            set { _Section = value; }
        }
        public int AccessState { get; set; }  //1表示进路开放，0表示未开放
        public int Direction { get; set; } //1上行，0下行
    }
    
}
