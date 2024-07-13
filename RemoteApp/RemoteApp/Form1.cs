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
        // 命名空间.类名 变量名
        private MouseActionFactory.MouseActionFactory mouseAction;

        //加载初始化窗口
        public Form1()
        {
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

            // 左侧面板
            sidePanel = createSidePanel();
            this.Controls.Add(sidePanel);

            // 初始化 contentPanel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            // 应用程序面板
            appInfoPanel = createAppInfoPanel();
            contentPanel.Controls.Add(appInfoPanel);
            this.Controls.Add(contentPanel);

            mouseAction.flushAppPanel(kernel.getRemoteAppList());
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

            buttonAppList = CreateButton("发布程序");
            flowLayoutPanel.Controls.Add(buttonAppList);
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
        }

        // 增加左侧按钮
        private Button CreateButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(150, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 0, 15), // 设置底部间距为 10 像素
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("等线", 15)
            };

            button.Click += mouseAction.BtnSidePanel_Click;
            return button;
        }

        public FlowLayoutPanel createAppInfoPanel()
        {
            FlowLayoutPanel allAppInfoPanel = new FlowLayoutPanel
            {
                Size = new Size(this.ClientSize.Width,this.ClientSize.Height-100),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, // 上下
                WrapContents = false,
                Location = new Point(0,50),
            };

            //title
            Label titleLabel = new Label
            {
                Text = "已发布程序",
                Font = new Font("华文中宋", 20, FontStyle.Bold),
                Location=new Point(180,15),
                BackColor= Color.White,     
                AutoSize = true
            };
            contentPanel.Controls.Add(titleLabel);
            // 添加按钮
            // 创建按钮容器面板
            Panel buttonPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = 70, // 设置容器面板的高度
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
                //Location = new Point(0),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            Button installButton = new Button
            {
                Text = "选择安装程序",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(180,50),
                Location = new Point(540,0),
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
            };
            // 添加按钮到按钮容器面板
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(installButton);

            // 将按钮容器面板添加到 FlowLayoutPanel
            contentPanel.Controls.Add(buttonPanel);

            addButton.MouseClick += mouseAction.SelectExeButton_Click;
            installButton.MouseClick += mouseAction.SelectInstallButton_Click;
            return allAppInfoPanel;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Kernel.getKernel().removeUninstallAndInstall();
            Network.getNetwork().stop();
        }
    }
}
