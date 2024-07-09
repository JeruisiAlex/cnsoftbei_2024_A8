﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MyClass;
using MouseActionFactory;


namespace cnsoftbei_A8
{
    public partial class Form1 : Form
    {
        public static Panel sidePanel;
        public static Button lastClickedButton; // 用于跟踪最后点击的按钮
        public static Panel ipPortPanel;
        public static Panel connectionPanel;
        public static Panel contentPanel;
        public static Panel appInfoPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        // 命名空間.类名 变量名
        private MouseActionFactory.MouseActionFactory mouseAction;
        private List<AppInfo> appListInfo; //应用程序列表
        public Form1()
        {
            InitializeComponent();
            initializeCustomComponents();
        }


        private void initializeCustomComponents()
        {
            // 获取 MouseActionFactory 的唯一实例
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;

            // 左侧面板
            sidePanel = createSidePanel();
            this.Controls.Add(sidePanel);

            // 内容面板
            contentPanel = createContentPanel();
            this.Controls.Add(contentPanel);

            //createAllAppInfo();
            //appInfoPanel.Visible = false;
        }

        public Panel createSidePanel()
        {
            Panel sidePanel = new Panel
            {
                BackColor = Color.LightGray,
                Size = new Size(150, this.ClientSize.Height),
                Dock = DockStyle.Left
            };

            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(0, 10, 0, 0),
                AutoScroll = true,
                WrapContents = false,
                Margin = new Padding(0),
            };

            flowLayoutPanel.Controls.Add(CreateButton("主机信息"));
            flowLayoutPanel.Controls.Add(CreateButton("应用程序"));
            flowLayoutPanel.Controls.Add(CreateButton("发布程序"));
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
        }
        public Panel createContentPanel()
        {
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            this.Controls.Add(contentPanel);


            // IP 和 Port 面板
            ipPortPanel = new Panel();
            addPanel(ipPortPanel, new Point(180, 100));
            ipPortPanel.MouseEnter += mouseAction.IpPortPanel_MouseEnter;
            ipPortPanel.MouseLeave += mouseAction.IpPortPanel_MouseLeave;


            // 添加内容控件
            Label lblIP = new Label();
            lblIP.Text = "主机名:";
            lblIP.Location = new Point(20, 15);
            lblIP.AutoSize = true;
            lblIP.Font = new Font("华文中宋", 15); // 设置字体名称、大小和样式
            ipPortPanel.Controls.Add(lblIP);

            Label lblPort = new Label();
            lblPort.Text = "Port:";
            lblPort.Location = new Point(320, 15);
            lblPort.AutoSize = true;
            lblPort.Font = new Font("华文中宋", 13);
            ipPortPanel.Controls.Add(lblPort);

            String hostName = getHostName();

            // 添加 IP 和端口显示控件
            addInfoLabel(ipPortPanel, hostName, new Point(60, 15), "Times New Roman");
            addInfoLabel(ipPortPanel, "6789", new Point(370, 15), "Times New Roman");

            // 开关控件
            connectionPanel = new Panel();
            addPanel(connectionPanel, new Point(180, 170));
            connectionPanel.MouseEnter += mouseAction.connectionPanel_MouseEnter;
            connectionPanel.MouseLeave += mouseAction.connectionPanel_MouseLeave;
            addInfoLabel(connectionPanel, "允许局域网连接", new Point(20, 15), "华文中宋");

            ToggleButton chkAllowNetwork = new ToggleButton();
            chkAllowNetwork.Location = new Point(320, 15);
            connectionPanel.Controls.Add(chkAllowNetwork);


            broadPanel = new Panel();
            addPanel(broadPanel, new Point(180, 240));
            broadPanel.MouseEnter += mouseAction.broadPanel_MouseEnter;
            broadPanel.MouseLeave += mouseAction.broadPanel_MouseLeave;
            addInfoLabel(broadPanel, "是否发送广播", new Point(20, 15), "华文中宋");

            ToggleButton chkBroadcast = new ToggleButton();
            chkBroadcast.Location = new Point(320, 15);
            broadPanel.Controls.Add(chkBroadcast);

            startupPanel = new Panel();
            addPanel(startupPanel, new Point(180, 310));
            startupPanel.MouseEnter += mouseAction.startupPanel_MouseEnter;
            startupPanel.MouseLeave += mouseAction.startupPanel_MouseLeave;
            addInfoLabel(startupPanel, "开机启动", new Point(20, 15), "华文中宋");

            ToggleButton chkStartup = new ToggleButton();
            chkStartup.Location = new Point(320, 15);
            //chkStartup.AutoSize = true;
            startupPanel.Controls.Add(chkStartup);

            // 顶部logo和连接状态
            Label lblLogo = new Label();
            lblLogo.Text = "logo";
            lblLogo.Location = new Point(450, 30);
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font(lblLogo.Font, FontStyle.Bold);
            lblLogo.ForeColor = Color.Blue;
            contentPanel.Controls.Add(lblLogo);

            Label lblStatus = new Label();
            lblStatus.Text = "(Un)Connected";
            lblStatus.Location = new Point(600, 30);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Black;
            contentPanel.Controls.Add(lblStatus);

            // 增加paint事件
            contentPanel.Paint += mouseAction.ContentPanel_Paint;

            return contentPanel;
        }

        private string getHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        // 增加左侧按钮
        private Button CreateButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(150, 50);
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.LightGray;
            button.FlatAppearance.BorderSize = 0;
            button.Margin = new Padding(0, 0, 0, 15); // 设置底部间距为 10 像素
            button.TextAlign = ContentAlignment.MiddleCenter;

            button.Font = new Font("等线", 13);

            button.Click += mouseAction.BtnSidePanel_Click;
            return button;
        }

        private void addPanel(Panel panel, Point location)
        {
            panel.Location = location;
            panel.Size = new Size(540, 50);
            panel.BackColor = Color.Transparent;
            contentPanel.Controls.Add(panel);
        }

        // 增加信息标签
        private void addInfoLabel(Panel panel, string text, Point location, String font)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.AutoSize = true;
            label.Font = new Font(font, 13, FontStyle.Regular);
            panel.Controls.Add(label);

        }

    }



}
