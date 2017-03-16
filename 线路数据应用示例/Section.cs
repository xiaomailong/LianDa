using System.Windows.Media;
using 线路绘图工具;

namespace 线路数据应用示例
{
    public class Section : 线路绘图工具.Section, ICheckDistance
    {
        static Pen DefaultPen_ = new Pen(Brushes.Cyan, 3);
        static Pen AxleOccupyPen_ = new Pen(Brushes.Red, 3);
        static Pen TrainOccpyPen_ = new Pen(Brushes.Yellow, 3);

        public static Pen DefaultPen { get { return DefaultPen_; } }
        public static Pen AxleOccupyPen { get { return AxleOccupyPen_; } }
        public static Pen TrainOccpyPen { get { return TrainOccpyPen_; } }
        public bool IsAccessLock { get; set; }

        public double LeftDistance { get; set; }
        public double RightDistance { get; set; }
        private int _Offset = 100;
        public int Offset 
        { 
            get {return _Offset; }
            set
            {
                if (_Offset != value)
                {
                    _Offset = value;
                }
            }
        }
        public int Direction { get; set; } //1表示上行，0表示下行

        private int _AxleOccpy = 0;
        public int AxleOccupy 
        {
            get {return _AxleOccpy; } 
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

        public bool IsDistanceIn(double distance)
        {
            return distance <= LeftDistance && distance >= RightDistance;
        }

        bool isRouteLock_ = false;
        bool IsRouteLock
        {
            get { return isRouteLock_; }
            set
            {
                if (isRouteLock_ != value)
                {
                    isRouteLock_ = value;
                }
            }
        }

        bool isFrontLogicOccupy_ = false;
        public  bool IsFrontLogicOccupy
        {
            get { return isFrontLogicOccupy_; }
            set
            {
                if (isFrontLogicOccupy_ != value)
                {
                    isFrontLogicOccupy_ = value;
                }
            }
        }

        bool isLastLogicOccupy_ = true;
        public bool IsLastLogicOccupy
        {
            get { return isLastLogicOccupy_; }
            set
            {
                if (isLastLogicOccupy_ != value)
                {
                    isLastLogicOccupy_ = value;
                }
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            foreach (Line line in graphics_)
            {
                System.Windows.Point Middle = new System.Windows.Point((line.Points[0].X + line.Points[1].X)/2,(line.Points[0].Y + line.Points[1].Y)/2);
                if (AxleOccupy == 0)
	            {
		            dc.DrawLine(AxleOccupyPen_,line.Points[0],line.Points[1]);
	            }
                else
	            {
                    dc.DrawLine(DefaultPen_,line.Points[0],line.Points[1]);
	            }
                if (IsFrontLogicOccupy && IsLastLogicOccupy)
	            {
		            dc.DrawLine(TrainOccpyPen_,line.Points[0],line.Points[1]);
	            }
                else if (IsFrontLogicOccupy && !IsLastLogicOccupy)
	            {
		            dc.DrawLine(TrainOccpyPen_,line.Points[0],Middle);
	            }
                else if (!IsFrontLogicOccupy && IsLastLogicOccupy)
                {
                    dc.DrawLine(TrainOccpyPen_, Middle, line.Points[1]);
                }
            }
            dc.DrawText(formattedName_, namePoint_);
        }
    }
}
