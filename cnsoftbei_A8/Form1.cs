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

namespace cnsoftbei_A8
{
    public partial class Form1 : Form
    {
        private Button lastClickedButton; // 用于跟踪最后点击的按钮
        private Panel ipPortPanel;
        private Panel connectionPanel;
        private Panel contentPanel;
        private Panel broadPanel;
        private Panel startupPanel;
        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        static string GetLocalIPAddress()
        {
            string localIP = null;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 过滤掉非活动网络接口
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                var ipProperties = ni.GetIPProperties();
                foreach (var ip in ipProperties.UnicastAddresses)
                {
                    // 过滤掉IPv6地址和本地回环地址
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                    {
                        // 检查是否是局域网地址
                        if (IsPrivateIP(ip.Address))
                        {
                            localIP += ip.Address.ToString() + ";\n";

                        }
                    }
                }
            }

            return localIP;
        }

        static bool IsPrivateIP(IPAddress ipAddress)
        {
            // 解析IP地址为字节数组
            byte[] ipBytes = ipAddress.GetAddressBytes();

            // 10.0.0.0/8
            if (ipBytes[0] == 10)
                return true;

            // 172.16.0.0/12
            if (ipBytes[0] == 172 && ipBytes[1] >= 16 && ipBytes[1] <= 31)
                return true;

            // 192.168.0.0/16
            if (ipBytes[0] == 192 && ipBytes[1] == 168)
                return true;

            return false;
        }


        private void InitializeCustomComponents()
        {
            // 设置Form属性
            this.Text = "Remote-operation";
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;

            // 左侧面板
            Panel sidePanel = new Panel();
            sidePanel.BackColor = Color.LightGray;
            sidePanel.Size = new Size(150, this.ClientSize.Height);
            sidePanel.Dock = DockStyle.Left;
            this.Controls.Add(sidePanel);

            // 添加侧边栏按钮
            // 创建 FlowLayoutPanel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Padding = new Padding(0, 10, 0, 0); // 设置顶部内边距
            flowLayoutPanel.AutoScroll = true; // 启用滚动条
            flowLayoutPanel.WrapContents = false; // 防止自动换行
            flowLayoutPanel.Margin = new Padding(0); // FlowLayoutPanel 外边距
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.Controls.Add(CreateButton("主机信息"));
            flowLayoutPanel.Controls.Add(CreateButton("应用程序"));
            flowLayoutPanel.Controls.Add(CreateButton("发布程序"));
            sidePanel.Controls.Add(flowLayoutPanel);

            // 内容面板
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            this.Controls.Add(contentPanel);


            // IP 和 Port 面板
            ipPortPanel = new Panel();
            AddPanel(ipPortPanel, new Point(180, 100));
            ipPortPanel.MouseEnter += IpPortPanel_MouseEnter;
            ipPortPanel.MouseLeave += IpPortPanel_MouseLeave;

            // 添加内容控件
            Label lblIP = new Label();
            lblIP.Text = "IP:";
            lblIP.Location = new Point(20, 15);
            lblIP.AutoSize = true;
            lblIP.Font = new Font("华文中宋", 15); // 设置字体名称、大小和样式
            lblIP.Click += lblIP_Click;
            ipPortPanel.Controls.Add(lblIP);

            Label lblPort = new Label();
            lblPort.Text = "Port:";
            lblPort.Location = new Point(320, 15);
            lblPort.AutoSize = true;
            lblPort.Font = new Font("华文中宋", 13);
            ipPortPanel.Controls.Add(lblPort);



            // 添加 IP 和端口显示控件
            //AddInfoLabel(ipPortPanel, "122", new Point(60, 15), "Times New Roman");
            AddInfoLabel(ipPortPanel, "1234", new Point(370, 15), "Times New Roman");

            // 开关控件
            connectionPanel = new Panel();
            AddPanel(connectionPanel, new Point(180, 170));
            connectionPanel.MouseEnter += connectionPanel_MouseEnter;
            connectionPanel.MouseLeave += connectionPanel_MouseLeave;
            AddInfoLabel(connectionPanel, "允许局域网连接", new Point(20, 15), "华文中宋");

            ToggleButton chkAllowNetwork = new ToggleButton();
            chkAllowNetwork.Location = new Point(320, 15);
            connectionPanel.Controls.Add(chkAllowNetwork);


            broadPanel = new Panel();
            AddPanel(broadPanel, new Point(180, 240));
            broadPanel.MouseEnter += broadPanel_MouseEnter;
            broadPanel.MouseLeave += broadPanel_MouseLeave;
            AddInfoLabel(broadPanel, "是否发送广播", new Point(20, 15), "华文中宋");

            ToggleButton chkBroadcast = new ToggleButton();
            chkBroadcast.Location = new Point(320, 15);
            broadPanel.Controls.Add(chkBroadcast);

            startupPanel = new Panel();
            AddPanel(startupPanel, new Point(180, 310));
            startupPanel.MouseEnter += startupPanel_MouseEnter;
            startupPanel.MouseLeave += startupPanel_MouseLeave;
            AddInfoLabel(startupPanel, "开机启动", new Point(20, 15), "华文中宋");

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
            contentPanel.Paint += ContentPanel_Paint;

        }
        private void lblIP_Click(object sender, EventArgs e)
        {
            string localIPAddress = GetLocalIPAddress();
            if (!string.IsNullOrEmpty(localIPAddress))
            {
                Console.WriteLine("Local IP Address: " + localIPAddress);
            }
            // 执行点击后的逻辑
            MessageBox.Show(localIPAddress);
        }


        private void IpPortPanel_MouseEnter(object sender, EventArgs e)
        {
            ipPortPanel.BackColor = Color.LightGray;
        }

        private void IpPortPanel_MouseLeave(object sender, EventArgs e)
        {
            ipPortPanel.BackColor = Color.Transparent;
        }

        private void connectionPanel_MouseEnter(object sender, EventArgs e)
        {
            connectionPanel.BackColor = Color.LightGray;
        }

        private void connectionPanel_MouseLeave(object sender, EventArgs e)
        {
            connectionPanel.BackColor = Color.Transparent;
        }

        private void BtnSidePanel_Click(object sender, EventArgs e)
        {
            // 获取按钮控件
            Button btn = sender as Button;

            if (btn != null)
            {
                // 恢复上一个按钮的背景颜色
                if (lastClickedButton != null)
                {
                    lastClickedButton.BackColor = Color.LightGray;
                }

                // 设置点击的按钮背景颜色为白色
                btn.BackColor = Color.White;
                lastClickedButton = btn; // 更新最后点击的按钮
            }
        }

        private void broadPanel_MouseEnter(object sender, EventArgs e)
        {
            broadPanel.BackColor = Color.LightGray;
        }
        private void broadPanel_MouseLeave(object sender, EventArgs e)
        {
            broadPanel.BackColor = Color.Transparent;
        }

        private void startupPanel_MouseEnter(object sender, EventArgs e)
        {
            startupPanel.BackColor = Color.LightGray;
        }

        private void startupPanel_MouseLeave(object sender, EventArgs e)
        {
            startupPanel.BackColor = Color.Transparent;
        }

        // 绘制虚线
        private void ContentPanel_Paint(object sender, PaintEventArgs e)
        {
            // 获取Graphics对象
            Graphics g = e.Graphics;

            // 创建虚线画笔
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                pen.DashStyle = DashStyle.Custom;
                pen.DashPattern = new float[] { 5, 5 };

                // 绘制虚线
                g.DrawLine(pen, 240, 117, 330, 137); // IP下方虚线
                g.DrawLine(pen, 550, 117, 600, 137); // Port下方虚线
            }
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

            button.Click += BtnSidePanel_Click;
            return button;
        }

        private void AddPanel(Panel panel, Point location)
        {
            panel.Location = location;
            panel.Size = new Size(540, 50);
            panel.BackColor = Color.Transparent;
            contentPanel.Controls.Add(panel);
        }

        // 增加信息标签
        private void AddInfoLabel(Panel panel, string text, Point location, String font)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.AutoSize = true;
            label.Font = new Font(font, 13, FontStyle.Regular);
            panel.Controls.Add(label);

        }
    }

    // 定义一个选择器
    public class ToggleButton : CheckBox
    {
        public ToggleButton()
        {
            this.Appearance = Appearance.Button;
            this.AutoSize = false;
            this.Size = new Size(40, 20);
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.Transparent;
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(this.Parent.BackColor);

            // 圆角矩形
            int radius = 10;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            Brush toggleBrush = Checked ? Brushes.LightGreen : Brushes.LightGray;
            g.FillPath(toggleBrush, path);

            // 增加选择器（圆形）的大小
            int circleDiameter = this.Height - 5;
            int circleX = Checked ? this.Width - circleDiameter - 3 : 3;
            Rectangle circleRect = new Rectangle(circleX, 3, circleDiameter, circleDiameter);
            Brush circleBrush = Checked ? Brushes.Green : Brushes.Gray;
            g.FillEllipse(circleBrush, circleRect);

            using (Pen pen = new Pen(Color.DarkGray, 2))
            {
                g.DrawPath(pen, path);
                g.DrawEllipse(pen, circleRect);
            }
        }

    }
}
