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
        public StationTopoloty Topo { get { return stationTopoloty_; } }

        public MainWindow()
        {
            InitializeComponent();
            LoadGraphicElements("StationElements.xml");
            LoadStationTopo("StationTopoloty.xml");

            LoadSecondStation();

            AddCIAccess LoadInterlockTable = new AddCIAccess();
            IPConfigure LoadIPConfig = new IPConfigure();

            Receive = new ReceiveData();
            Receive.Start();
            NonComTrain();
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
