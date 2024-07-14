using System;
using System.Diagnostics;
using MouseActionFactory;
using System.Windows.Forms;
using MyClass;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net;
namespace Server
{
    public partial class Form1 : Form
    {
        public static Panel sidePanel;
        public static Button infoButton;
        public static Button historyButton;
        public static Button lastClickedButton; // 用于跟踪最后点击的按钮
        public static Panel ipPortPanel;
        public static Panel connectionPanel;
        public static Panel contentPanel;
        public static Panel broadPanel;
        public static FlowLayoutPanel historyPanel;
        private MouseActionFactory.MouseActionFactory mouseAction;

        public Form1()
        {
            InitializeComponent();
            Kernel kernel = Kernel.getKernel();
            kernel.init();
            Network.getNetwork().init();
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

            //历史连接面板
            historyPanel = createHistoryPanel();
            this.Controls.Add(historyPanel);

            
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

            infoButton = CreateButton("主机信息");
            flowLayoutPanel.Controls.Add(infoButton);
            historyButton = CreateButton("历史连接");
            flowLayoutPanel.Controls.Add(historyButton);
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

        public Panel createContentPanel()
        {
            version = new Version(1, 0, 0);
            contentPanel = new Panel();
            contentPanel.BackColor = Color.White;
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

            //String hostName = getHostName();

            // 添加 IP 和端口显示控件
            addInfoLabel(ipPortPanel, "hostName", new Point(100, 15), "Times New Roman");
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

            // 顶部logo和连接状态
            Label lblLogo = new Label();
            lblLogo.Text = "SKRO";
            lblLogo.Location = new Point(200, 15);
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Times New Roman", 42, FontStyle.Bold);
            lblLogo.ForeColor = Color.DarkBlue;
            contentPanel.Controls.Add(lblLogo);

            string ver = version.ToString();
            addInfoLabel(contentPanel, "v" + ver, new Point(400, 47), "Times New Roman");

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

        private FlowLayoutPanel createHistoryPanel()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Location = new Point(180, 0),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right, // 锚定到底部和右侧
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll  = true,
                BackColor = Color.White,

            };
            History his = new History("123", "nanananna", "woshipinpin");
            for(int i = 0; i < 5; i++)
            {
                Panel panel;
                panel = createConnectionPanel(his);
                flowLayoutPanel.Controls.Add(panel);
            }

            return flowLayoutPanel;
        }

        private Panel createConnectionPanel(History his)
        {
            Panel panel = new Panel
            {
                Size = new Size(300,150),
                Margin = new Padding(180,10,0,10),
                BackColor = Color.WhiteSmoke,
            };
            // 创建并配置 Label 控件
            Label ipLabel = new Label
            {
                Text = "IP地址: " + his.ip,
                AutoSize = true,
                Location = new Point(10, 20),
                Font = new Font("华文中宋",16)
            };

            Label hostLabel = new Label
            {
                Text = "主机名: " + his.hostName,
                AutoSize = true,
                Location = new Point(10, 50),
                Font = new Font("华文中宋", 16)
            };

            Label userLabel = new Label
            {
                Text = "用户名: " + his.userName,
                AutoSize = true,
                Location = new Point(10, 80),
                Font = new Font("华文中宋", 16)
            };

            // 将 Label 添加到 Panel
            panel.Controls.Add(ipLabel);
            panel.Controls.Add(hostLabel);
            panel.Controls.Add(userLabel);
            return panel;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Network network = Network.getNetwork();
            network.isConnectMutex.WaitOne();
            if (network.isConnect)
            {
                MessageBox.Show("服务端已被连接，请断开连接后再关闭！");
                e.Cancel = true;
            }
            else
            {
                network.stop();
                network.release();
            }
            network.isConnectMutex.ReleaseMutex();
        }
    }
}
