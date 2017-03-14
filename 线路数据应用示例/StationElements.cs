using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace 线路数据应用示例
{
    public class StationElements
    {
        [XmlElement("Section", typeof(Section))]
        [XmlElement("RailSwitch", typeof(RailSwitch))]
        [XmlElement("Signal", typeof(Signal))]
        [XmlElement("CommandButton", typeof(线路绘图工具.CommandButton))]
        [XmlElement("SmallButton", typeof(线路绘图工具.SmallButton))]
        [XmlElement("GraphicElement", typeof(线路绘图工具.GraphicElement))]
        public List<线路绘图工具.GraphicElement> Elements { get; set; }

        //internal Device FindDeviceByName(string deviceName, int stationID)
        //{
        //    foreach (var element in Elements)
        //    {
        //        if (element is Device)
        //        {
        //            Device device = element as Device;
        //            if (device.Name.Equals(deviceName) && device.StationID == stationID)
        //            {
        //                return device;
        //            }
        //        }
        //    }

        //    return null;
        //}

        public static StationElements Open(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return new XmlSerializer(typeof(StationElements)).Deserialize(sr) as StationElements;
            }
        }

        public void AddElementsToCanvas(Canvas canvas)
        {
            foreach (线路绘图工具.GraphicElement element in Elements)
            {
                canvas.Children.Add(element);
            }
        }
    }
}
