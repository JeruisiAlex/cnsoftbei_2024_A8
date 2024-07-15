using System;
using System.Diagnostics;
using MouseActionFactory;
using System.Windows.Forms;
using MyClass;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net;
using System.Drawing;
namespace Server
{
    public partial class Form1 : Form
    {
        public static Panel sidePanel;
        public static Button infoButton;
        public static Button historyButton;
        public static Button lastClickedButton; // ���ڸ���������İ�ť
        public static Panel ipPortPanel;
        public static Panel connectionPanel;
        public static Panel contentPanel;
        public static Panel broadPanel;
        public static FlowLayoutPanel historyPanel;
        public static Label lblStatus;
        public static Panel statusPanel;
        public static Label remoteLabel; 
        //��ɫ
        public static Brush brush;
        private Kernel kernel;
        public static double rWidth;
        private static double rHeight;
        private MouseActionFactory.MouseActionFactory mouseAction;

        public Form1()
        {
            InitializeComponent();
            kernel = Kernel.getKernel();
            kernel.init();
            kernel.histories.Add(new History("123", "123", "123"));
            kernel.writeHistories();
            kernel.readHistories();
            initializeCustomComponents();

        }

        private void initializeCustomComponents()
        {
            // ��ȡ MouseActionFactory ��Ψһʵ��
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;
            // ������
            sidePanel = createSidePanel();
            this.Controls.Add(sidePanel);

            // �������
            contentPanel = createContentPanel();
            this.Controls.Add(contentPanel);

            //��ʷ�������
            historyPanel = createHistoryPanel();
            this.Controls.Add(historyPanel);
            this.Resize += Form1_Resize;

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            //appInfoPanel.Visible = false;
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            historyPanel.Size = new Size(this.ClientSize.Width - 150, this.ClientSize.Height);
            historyPanel.Location = new Point(150, 0);
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

            infoButton = CreateButton("������Ϣ");
            flowLayoutPanel.Controls.Add(infoButton);
            historyButton = CreateButton("��ʷ����");
            flowLayoutPanel.Controls.Add(historyButton);
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
        }

        // ������ఴť
        private Button CreateButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(150,50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 15),
                Font = new Font("����", 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.FlatAppearance.BorderSize = 0; // ���õײ����Ϊ 10 ����

            button.Click += mouseAction.BtnSidePanel_Click;
            return button;
        }

        public Panel createContentPanel()
        {
            brush = Brushes.Red;
            version = new Version(1, 0, 0);
            contentPanel = new Panel();
            contentPanel.BackColor = Color.White;
            contentPanel.Dock = DockStyle.Fill;
            this.Controls.Add(contentPanel);


            // IP �� Port ���
            ipPortPanel = new Panel();
            addPanel(ipPortPanel, new Point(180, 105));
            ipPortPanel.MouseEnter += mouseAction.IpPortPanel_MouseEnter;
            ipPortPanel.MouseLeave += mouseAction.IpPortPanel_MouseLeave;

            // ������ݿؼ�
            addInfoLabel(ipPortPanel, "������:", new Point(20, 15), "��������");
            addInfoLabel(ipPortPanel, "Port:", new Point(320, 15), "��������");

            String hostName = kernel.getHostName();

            // ��� IP �Ͷ˿���ʾ�ؼ�
            addInfoLabel(ipPortPanel, hostName, new Point(90, 15), "Times New Roman");
            addInfoLabel(ipPortPanel, "6789", new Point(370, 15), "Times New Roman");
            //MessageBox.Show($"{hostName}");

            // ���ؿؼ�
            connectionPanel = new Panel();
            addPanel(connectionPanel, new Point(180, 170));
            addInfoLabel(connectionPanel, "�������������", new Point(20, 15), "��������");
            connectionPanel.MouseEnter += mouseAction.connectionPanel_MouseEnter;
            connectionPanel.MouseLeave += mouseAction.connectionPanel_MouseLeave;
            connectionPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            ToggleButton chkAllowNetwork = new ToggleButton();
            chkAllowNetwork.Location = new Point((int)(0.85*size.Width), 15);
            connectionPanel.Controls.Add(chkAllowNetwork);


            broadPanel = new Panel();
            addPanel(broadPanel, new Point(180, 240));
            broadPanel.MouseEnter += mouseAction.broadPanel_MouseEnter;
            broadPanel.MouseLeave += mouseAction.broadPanel_MouseLeave;
            addInfoLabel(broadPanel, "�Ƿ��͹㲥", new Point(20, 15), "��������");

            ToggleButton chkBroadcast = new ToggleButton();
            chkBroadcast.Location = new Point((int)(0.85 * size.Width), 15);
            broadPanel.Controls.Add(chkBroadcast);

            // ����logo������״̬
            Label lblLogo = new Label
            {
                Text = "SKRO",
                Location = new Point(200, 15),
                Font = new Font("Times New Roman", 42, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true
            };
            contentPanel.Controls.Add(lblLogo);

            string ver = version.ToString();
            Label versionLabel = new Label
            {
                Text = "v" + ver,
                Location = new Point((int)(200 + 0.25 * size.Width), 47),
                Font = new Font("Times New Roman", 14),
                ForeColor = Color.Black,
                AutoSize = true
            };

            contentPanel.Controls.Add(versionLabel);

            statusPanel = new Panel
            {
                Location = new Point((int)(0.85 * size.Width), 42),
                Size = new Size(180,50),
                BackColor = Color.White,
            };
            status = "δ����";
            lblStatus = new Label
            {
                Text = status,
                Location = new Point(0,0),
                AutoSize = true,
                Font = new Font("��������", 14),
                ForeColor = Color.Black,
            };
            statusPanel.Controls.Add(lblStatus);
            contentPanel.Controls.Add(statusPanel);

            // ����paint�¼�
            contentPanel.Paint += mouseAction.ContentPanel_Paint;
            return contentPanel;
        }
        private void addPanel(Panel panel, Point location)
        {
            panel.Location = location;
            panel.Size = new Size(size.Width-180, 50);
            panel.BackColor = Color.Transparent;
            panel.MouseEnter +=(sender,e)=>panel.BackColor = Color.LightGray;
            panel.MouseLeave += (sender, e) => panel.BackColor = Color.Transparent;
            contentPanel.Controls.Add(panel);
        }

        // ������Ϣ��ǩ
        private Label addInfoLabel(Panel panel, string text, Point location, String font)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = location;
            label.AutoSize = true;
            label.Font = new Font(font, 13, FontStyle.Regular);
            label.MouseEnter += (sender, e) => panel.BackColor = Color.LightGray;
            label.MouseLeave += (sender, e) => panel.BackColor = Color.Transparent;
            panel.Controls.Add(label);
            return label;

        }

        private FlowLayoutPanel createHistoryPanel()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Location = new Point(150, 0),
                Size= new Size(this.ClientSize.Width - 150, this.ClientSize.Height),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll  = true,
                BackColor = Color.White,

            };
            foreach(History his in kernel.histories)
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
                Margin = new Padding(20,10,0,10),
                BackColor = Color.WhiteSmoke,
            };
            // ���������� Label �ؼ�
            Label ipLabel = new Label
            {
                Text = "IP��ַ: " + his.ip,
                AutoSize = true,
                Location = new Point(10, 20),
                Font = new Font("��������",16)
            };

            Label hostLabel = new Label
            {
                Text = "������: " + his.hostName,
                AutoSize = true,
                Location = new Point(10, 50),
                Font = new Font("��������", 16)
            };

            Label userLabel = new Label
            {
                Text = "�û���: " + his.userName,
                AutoSize = true,
                Location = new Point(10, 80),
                Font = new Font("��������", 16)
            };

            // �� Label ��ӵ� Panel
            panel.Controls.Add(ipLabel);
            panel.Controls.Add(hostLabel);
            panel.Controls.Add(userLabel);
            return panel;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ������ִ�йر�ǰ�Ĳ��������磺
            // ѯ���û��Ƿ����Ҫ�ر�
            var result = MessageBox.Show("ȷ�Ϲر�", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // ȡ���رղ���
            }
            else
            {
                // ִ�йر�ǰ����������
                // ���磬�������û�������Դ
            }
        }
    }
}
