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
        public static FlowLayoutPanel appInfoPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        public static Button buttonInfo;
        public static Button buttonAddApp;
        // 命名空間.类名 变量名
        private MouseActionFactory.MouseActionFactory mouseAction;
        public static List<AppInfo> appListInfo= new List<AppInfo>(); //应用程序列表
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

            //应用程序面板
            appInfoPanel = createAddAppInfoPanel();
            this.Controls.Add(appInfoPanel);

            flushAppPanel();

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

            buttonInfo = CreateButton("主机信息");
            flowLayoutPanel.Controls.Add(buttonInfo);
            buttonAddApp = CreateButton("发布程序");
            flowLayoutPanel.Controls.Add(buttonAddApp);
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
        }
        public Panel createContentPanel()
        {
            version = new Version(1,0, 0);
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            this.Controls.Add(contentPanel);


            // IP 和 Port 面板
            ipPortPanel = new Panel();
            addPanel(ipPortPanel, new Point(180, 100));
            ipPortPanel.MouseEnter += mouseAction.IpPortPanel_MouseEnter;
            ipPortPanel.MouseLeave += mouseAction.IpPortPanel_MouseLeave;
            ipPortPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


            // 添加内容控件
            addInfoLabel(ipPortPanel, "主机名:", new Point(20, 15), "华文中宋");
            addInfoLabel(ipPortPanel, "Port:", new Point(320, 15), "华文中宋");

            String hostName = getHostName();

            // 添加 IP 和端口显示控件
            addInfoLabel(ipPortPanel, hostName, new Point(60, 15), "Times New Roman");
            addInfoLabel(ipPortPanel, "6789", new Point(370, 15), "Times New Roman");

            // 开关控件
            connectionPanel = new Panel();
            addPanel(connectionPanel, new Point(180, 170));
            addInfoLabel(connectionPanel, "允许局域网连接", new Point(20, 15), "华文中宋");
            connectionPanel.MouseEnter += mouseAction.connectionPanel_MouseEnter;
            connectionPanel.MouseLeave += mouseAction.connectionPanel_MouseLeave;
            connectionPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            ToggleButton chkAllowNetwork = new ToggleButton();
            chkAllowNetwork.Location = new Point(320, 15);
            connectionPanel.Controls.Add(chkAllowNetwork);


            broadPanel = new Panel();
            addPanel(broadPanel, new Point(180, 240));
            broadPanel.MouseEnter += mouseAction.broadPanel_MouseEnter;
            broadPanel.MouseLeave += mouseAction.broadPanel_MouseLeave;
            broadPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            lblLogo.Text = "SKRO";
            lblLogo.Location = new Point(200, 15);
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Times New Roman",42, FontStyle.Bold);
            lblLogo.ForeColor = Color.DarkBlue;
            contentPanel.Controls.Add(lblLogo);

            string ver = version.ToString();
            addInfoLabel(contentPanel, "v"+ver, new Point(400, 47), "Times New Roman");

            status = "Connected";
            Label lblStatus = new Label();
            lblStatus.Text = status;
            lblStatus.Location = new Point(600, 47);
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Times New Roman", 14);
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

            button.Font = new Font("等线", 15);

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
        private Label addInfoLabel(Panel panel, string text, Point location, String font)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.AutoSize = true;
            label.Font = new Font(font, 13, FontStyle.Regular);
            panel.Controls.Add(label);
            return label;

        }

        public FlowLayoutPanel createAddAppInfoPanel()
        {
            FlowLayoutPanel allAppInfoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, // 从左到右布局
                WrapContents = false, 
            };

            //title
            Label titleLabel = new Label
            {
                Text = "已发布程序",
                Font = new Font("华文中宋", 20, FontStyle.Bold),
                Margin = new Padding(180, 15, 0, 10),
                AutoSize = true
            };
            allAppInfoPanel.Controls.Add(titleLabel);

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

        private Panel createAppPanel(String name,Bitmap bitmap)
        {
            // 创建应用程序面板
            Panel appPanel = new Panel
            {
                Size = new Size(500, 200),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(200,100,0,10)
            };

            // 创建显示图标的 PictureBox
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(15, 10),
                Image = bitmap,
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            appPanel.Controls.Add(pictureBox); 

            // 创建显示名称的 Label
            Label nameLabel = new Label
            {
                Text = name, // 设置名称文本
                Location = new Point(15, 120), // 设置位置
                AutoSize = true, // 自动调整大小以适应内容
                Font = new Font("Arial", 12, FontStyle.Bold), // 设置字体
            };
            appPanel.Controls.Add(nameLabel); // 将 Label 添加到面板
            return appPanel;
        }

    }



}
