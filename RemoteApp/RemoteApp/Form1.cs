using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MouseActionFactory;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RemoteApp
{
    public partial class Form1 : Form
    {
        public static Panel sidePanel;
        public static Button lastClickedButton; // 用于跟踪最后点击的按钮
        public static FlowLayoutPanel appInfoPanel;
        public static Panel appInstallPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        public static Button buttonAppList;
        public static Button buttonAddApp;
        public static Panel contentPanel;
        public static Label title; 
        public static Kernel kernel;
        public Size size;
        // 命名空间.类名 变量名
        private MouseActionFactory.MouseActionFactory mouseAction;


        //加载初始化窗口
        public Form1()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            Kernel.getKernel().init();
            Network.getNetwork().init();
            kernel = Kernel.getKernel();
            InitializeComponent();
            initializeCustomComponents();
        }

        private void initializeCustomComponents()
        {
            // 获取 MouseActionFactory 的唯一实例
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;

            // 初始化 contentPanel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            // 应用程序面板
            createAppInfoPanel();
            contentPanel.Controls.Add(appInfoPanel);
            this.Controls.Add(contentPanel);
            this.Resize += Form_Resize;
            mouseAction.flushAppPanel(kernel.getRemoteAppList(),new Size(screenWidth,screenHeight));
            //appInfoPanel.Visible = false;

        }


        private void Form_Resize(object? sender, EventArgs e)
        {
            screenWidth = this.ClientSize.Width;
            screenHeight= this.ClientSize.Height;
            appInfoPanel.Size = new Size(screenWidth, (int)(0.8 * screenHeight));
            mouseAction.flushAppPanel(kernel.getRemoteAppList(),new Size(screenWidth,screenHeight));
        }

        public void createAppInfoPanel()
        {
            appInfoPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, // 上下
                WrapContents = false,
                Size = new Size(screenWidth,(int)(0.75*screenHeight)),
                Location = new Point(10,100),
                BackColor = Color.White
            };

            //title
            Label titleLabel = new Label
            {
                Text = "已发布程序",
                Font = new Font("华文中宋", 20, FontStyle.Bold),
                Location=new Point(0,20),
                BackColor= Color.White,     
                AutoSize = true,
                Size = new Size(screenWidth,100),
            };
            contentPanel.Controls.Add(titleLabel);
            // 添加按钮
            // 创建按钮容器面板
            Panel buttonPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = (int)(0.1*screenHeight), // 设置容器面板的高度
                Padding = new Padding(0, 0, 0, 0) // 设置按钮容器的内边距
            };
            Button addButton = new Button
            {
                Text = "发布应用",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(120, 50),
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
                Location = new Point(screenWidth - 200, 15),
            };

            Button installButton = new Button
            {
                Text = "选择安装程序",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(200,50),
                Location = new Point(screenWidth-500,15),
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat
            };
            // 添加按钮到按钮容器面板
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(installButton);

            // 将按钮容器面板添加到 FlowLayoutPanel
            contentPanel.Controls.Add(buttonPanel);

            addButton.MouseClick += mouseAction.SelectExeButton_Click;
            installButton.MouseClick += mouseAction.SelectInstallButton_Click;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Kernel.getKernel().removeUninstallAndInstall();
            Network.getNetwork().stop();
        }
    }
}
