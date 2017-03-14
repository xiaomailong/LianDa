using System;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using 线路绘图工具;
using System.Net.Sockets;
using System.Net;

namespace 线路数据应用示例
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static StationElements stationElements_;
        public static StationElements stationElements_1_;
        public static StationTopoloty stationTopoloty_;
        public static StationTopoloty stationTopoloty_1_;

        ReceiveData Receive;
        Socket socketMain = null;
        byte[] sendControlPacket;
        public StationTopoloty Topo { get { return stationTopoloty_; } }

        public MainWindow()
        {
            InitializeComponent();
            LoadGraphicElements("StationElements.xml");
            LoadStationTopo("StationTopoloty.xml");

            LoadSecondStation();

            AddCIAccess a = new AddCIAccess();

            //@try t = new @try();
            //t.Begin();
            //解码位 I = new 解码位();
            //I.adsf();
            //Signal s = new Signal();
            //s = t.FindSignalByName("4-XC") as Signal;
            //s.IsSignalOpen = true;
            //s.InvalidateVisual();
            //t.Begin();

            //if (t.FindSectionByName("203G") is Section)
            //{
            //    Section s = t.FindSectionByName("203G") as Section;
            //    s.TrainOccupy = 0;
            //    s.InvalidateVisual();
            //}
            //if (t.FindSectionByName("110DG") is RailSwitch)
            //{
            //    RailSwitch s = t.FindSectionByName("110DG") as RailSwitch;
            //    s.IsPositionReverse = true;
            //    s.IsPositionNormal = false;
            //    s.InvalidateVisual();
            //}

            //RailSwitch u = t.Left() as RailSwitch;
            //u.IsPositionNormal = false;
            //u.InvalidateVisual();



            int MainSport = 8000;
            IPEndPoint IPEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), MainSport);
            EndPoint EP = (EndPoint)IPEP;
            socketMain = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socketMain.Blocking = false;
            socketMain.Bind(EP);
            Receive = new ReceiveData();
            Receive.socketMain = socketMain;
            Receive.Start();
        }

        private void LoadSecondStation()
        {
            stationElements_1_ = StationElements.Open("StationElements1.xml");
            stationTopoloty_1_ = new StationTopoloty();
            stationTopoloty_1_.Open("StationTopoloty1.xml", stationElements_1_.Elements);

            foreach (var item in stationElements_1_.Elements)
            {
                item.Top += 45;
                item.Left += 2000;
            }

            stationElements_1_.AddElementsToCanvas(MainCanvas);
            ConnectNodes(stationTopoloty_, stationTopoloty_1_, "201G", "301G");
        }

        private void ConnectNodes(StationTopoloty topo1, StationTopoloty topo2, string deviceName1, string deviceName2)
        {
            TopolotyNode leftNode = null;
            foreach (var item in topo1.Nodes)
            {
                if (item.NodeDevice.Name == deviceName1)
                {
                    leftNode = item;
                }
            }

            TopolotyNode rightNode = null;
            foreach (var item in topo2.Nodes)
            {
                if (item.NodeDevice.Name == deviceName2)
                {
                    rightNode = item;
                }
            }

            if (leftNode != null && rightNode != null)
            {
                leftNode.RightNodes.Add(rightNode);
                rightNode.LeftNodes.Add(leftNode);
            }
        }

        private void LoadStationTopo(string path)
        {
            stationTopoloty_ = new StationTopoloty();
            stationTopoloty_.Open(path, stationElements_.Elements);
        }
        private void LoadStationTopo1(string path)
        {
            stationTopoloty_1_ = new StationTopoloty();
            stationTopoloty_1_.Open(path, stationElements_.Elements);
        }

        private void LoadGraphicElements(string path)
        {
            stationElements_ = StationElements.Open(path);
            stationElements_.AddElementsToCanvas(MainCanvas);
        }
        private void NonComTrain()
        {
            var obj = new NonCommunicationTrain();
            var NonComTrainThread = new Thread(obj.JudgeLostTrain);
            NonComTrainThread.Start();
        }
    }
}
