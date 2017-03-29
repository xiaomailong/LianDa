using System.Collections.Generic;
using System.Windows.Media;
using 线路绘图工具;

namespace 线路数据应用示例
{
    public class RailSwitch : 线路绘图工具.RailSwitch
    {
        static Pen YellowPen_ = new Pen(Brushes.Yellow,3);
        static Pen RedPen_ = new Pen(Brushes.Red, 3);
        static Pen DefaultPen_ = new Pen(Brushes.Cyan, 3);
        static Pen PurplePen_ = new Pen(Brushes.Purple, 3);

        public bool IsPositionNormal { get; set; }
        public bool IsPositionReverse { get; set; }
        public bool Islock { get; set; }
        public bool IsAccessLock { get; set; }
        public int Offset { get; set; }
        public int Direction { get; set; }

        private int _AxleOccpy = 0;
        public int AxleOccupy
        {
            get { return _AxleOccpy; }
            set
            {
                if (_AxleOccpy != value)
                {
                    _AxleOccpy = value;
                }
            }
        } //1表示计轴检测为空闲，0表示计轴检测为占用

        int TrainOccupy_ = 1;
        public int TrainOccupy
        {
            get { return TrainOccupy_; }
            set
            {

                if (TrainOccupy_ != value)
                {
                    TrainOccupy_ = value;
                }
            }

        }//1表示空闲，0表示占用

        List<byte> hasNonComTrain_ = new List<byte>();
        public List<byte> HasNonComTrain
        {
            get { return hasNonComTrain_; }
            set
            {
                if (hasNonComTrain_ != value)
                {
                    hasNonComTrain_ = value;
                }
            }
        }

        RailSwitch()
        {
            IsPositionNormal = false;
            IsPositionReverse = false;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            List<int> sectionIndexs = this.SectionIndexList[0];
            List<int> normalIndexs = SectionIndexList[1];
            List<int> reverseIndexs = SectionIndexList[2];

            if (HasNonComTrain.Count != 0)
            {
                foreach (int index in normalIndexs)
                {
                    Line line = graphics_[index] as Line;
                    dc.DrawLine(PurplePen_, line.Points[0], line.Points[1]);
                }

                foreach (int index in reverseIndexs)
                {
                    Line line = graphics_[index] as Line;
                    dc.DrawLine(PurplePen_, line.Points[0], line.Points[1]);
                }
            }
            else
            {
                if (TrainOccupy_ == 0)
                {
                    foreach (int index in sectionIndexs)
                    {
                        Line line = graphics_[index] as Line;
                        dc.DrawLine(YellowPen_, line.Points[0], line.Points[1]);
                    }
                    if (IsPositionNormal == true && IsPositionReverse == false)
                    {
                        foreach (int index in normalIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(YellowPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in reverseIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                        }
                    }
                    else if (IsPositionNormal == false && IsPositionReverse == true)
                    {
                        foreach (int index in reverseIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(YellowPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in normalIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                        }
                    }
                }
                else if (_AxleOccpy == 0)
                {
                    foreach (int index in sectionIndexs)
                    {
                        Line line = graphics_[index] as Line;
                        dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                    }
                    if (IsPositionNormal == true && IsPositionReverse == false)
                    {
                        foreach (int index in normalIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in reverseIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                        }
                    }
                    else if (IsPositionNormal == false && IsPositionReverse == true)
                    {
                        foreach (int index in reverseIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in normalIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                        }
                    }
                    else
                    {
                        foreach (int index in sectionIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in normalIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                        }

                        foreach (int index in reverseIndexs)
                        {
                            Line line = graphics_[index] as Line;
                            dc.DrawLine(RedPen_, line.Points[0], line.Points[1]);
                        }
                    }
                }
                else
                {
                    foreach (int index in sectionIndexs)
                    {
                        Line line = graphics_[index] as Line;
                        dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                    }

                    foreach (int index in normalIndexs)
                    {
                        Line line = graphics_[index] as Line;
                        dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                    }

                    foreach (int index in reverseIndexs)
                    {
                        Line line = graphics_[index] as Line;
                        dc.DrawLine(DefaultPen_, line.Points[0], line.Points[1]);
                    }
                }
            }
        }
    }
}
