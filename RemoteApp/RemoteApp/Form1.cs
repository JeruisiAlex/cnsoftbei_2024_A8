using System;
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
using MouseActionFactory;
using Microsoft.Win32;
using System.Diagnostics;
using MouseActionFactory;

namespace RemoteApp
{
    public partial class Form1 : Form
    {
        public static Panel sidePanel;
        public static Button lastClickedButton; // 用于跟踪最后点击的按钮
        public static Panel appInfoPanel;
        public static FlowLayoutPanel appInstallPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        public static Button buttonAppList;
        public static Button buttonInstallApp;
        public static List<App> appList=new List<App>();
        //public static Kernel kernel;
        // 命名空間.类名 变量名
        private MouseActionFactory.MouseActionFactory mouseAction;
        //加载初始化窗口
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


            //应用程序面板
            appInfoPanel = createAppInfoPanel();
            this.Controls.Add(appInfoPanel);


            //mouseAction.flushAppPanel(kernel.getRemoteAppList());
            mouseAction.flushAppPanel(appList);
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

            buttonAppList = CreateButton("应用程序");
            flowLayoutPanel.Controls.Add(buttonAppList);
            buttonInstallApp = CreateButton("安装程序");
            flowLayoutPanel.Controls.Add(buttonInstallApp);
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
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

            button.Font = new Font("等线", 15);

            button.Click += mouseAction.BtnSidePanel_Click;
            return button;
        }

        public FlowLayoutPanel createAppInfoPanel()
        {
            FlowLayoutPanel allAppInfoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, //上下布局
                WrapContents = false,
                BackColor = Color.White
            };

            Panel panel = new Panel
            {
                Margin = new Padding(500, 20, 10, 0), // 调整Margin确保不会超出范围
                Size = new Size(500, 50),
                BackColor = Color.White,
                AutoSize = true
            };
            //title
            Label titleLabel = new Label
            {
                Text = "已发布程序",
                Location = new Point(200,15),
                Font = new Font("华文中宋", 20, FontStyle.Bold),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);
            this.Controls.Add(panel);
            // 添加按钮
            Button addButton = new Button
            {
                Text = "+",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(50, 50),
                Location = new Point(this.ClientSize.Width - 70, this.ClientSize.Height - 70),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            this.Controls.Add(addButton);

            addButton.MouseClick += mouseAction.SelectExeButton_Click;

            return allAppInfoPanel;
        }
    }
}
