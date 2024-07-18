using System;
using System.Diagnostics;
using MouseActionFactory;
using System.Windows.Forms;
using MyClass;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
namespace SKRO_server
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
        public static FlowLayoutPanel historyPanel;
        public static Panel titlePanel;
        public static Panel portPanel;
        //��ɫ
        public static Brush brush;
        private Kernel kernel;
        public static double rWidth;
        private static double rHeight;
        public static ToggleButton chkAllowNetwork;
        private MouseActionFactory.MouseActionFactory mouseAction;

        public Form1()
        {
            InitializeComponent();
            kernel = Kernel.getKernel();
            kernel.init();
            Network.getNetwork().init();
            initializeCustomComponents();
        }

        private void initializeCustomComponents()
        {
            // ��ȡ MouseActionFactory ��Ψһʵ��
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;

            Panel wholePanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(wholePanel);
            // ������
            sidePanel = createSidePanel();
            wholePanel.Controls.Add(sidePanel);

            createTitlePanel();
            wholePanel.Controls.Add(titlePanel);
            // �������
            contentPanel = createContentPanel();
            wholePanel.Controls.Add(contentPanel);

            //��ʷ�������
            historyPanel = createHistoryPanel();
            wholePanel.Controls.Add(historyPanel);
            historyPanel.Visible = false;
            this.Resize += Form1_Resize;

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            //appInfoPanel.Visible = false;
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            if (this.ClientSize.Width > 1190)
            {
                contentPanel.Location = new Point((int)(200 + (this.ClientSize.Width - 1190) / 2), 200);
            }
            else
            {
                contentPanel.Location = new Point(200, 200);
            }
            historyPanel.Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height);
            historyPanel.Location = new Point(200, 0);
        }

        public void createTitlePanel()
        {
            titlePanel = new Panel
            {
                Size = new Size(900, 120),
                Location = new Point(200, 0),
                BackColor = Color.White
            };
            // ����logo������״̬
            Label lblLogo = new Label
            {
                Text = "SKRO",
                Location = new Point(200, 15),
                Font = new Font("Times New Roman", 42, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true
            };
            titlePanel.Controls.Add(lblLogo);


  
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(50, 0),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            string currentDirectory = Directory.GetCurrentDirectory();
            String imgPath = Path.GetFullPath(Path.Combine(currentDirectory, "./img.ico"));
            pictureBox.Image = Image.FromFile(imgPath);
            titlePanel.Controls.Add(pictureBox);

            version = new Version(1, 0, 0);
            string ver = version.ToString();
            Label versionLabel = new Label
            {
                Text = "v" + ver,
                Location = new Point(700, 60),
                Font = new Font("Times New Roman", 14),
                ForeColor = Color.Black,
                AutoSize = true
            };

            titlePanel.Controls.Add(versionLabel);
        }
        public Panel createSidePanel()
        {
            Panel sidePanel = new Panel
            {
                BackColor = Color.LightGray,
                Size = new Size(180, this.ClientSize.Height),
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
                Size = new Size(180,50),
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
            
            contentPanel = new Panel
            {
                Size = new Size(1000,700),
                Location = new Point(200,200),
                BackColor = Color.White,
            };
            this.Controls.Add(contentPanel);


            // IP �� Port ���
            ipPortPanel = new Panel();
            addPanel(ipPortPanel, new Point(0, 0));

            // ������
            Label ipNameLabel = new Label
            {
                Text = "������",
                Location = new Point(10,15),
                Font = new Font("��������", 16, FontStyle.Regular),
                Size = new Size(200,100)
            };
            ipNameLabel.MouseEnter += (sender, e) => ipPortPanel.BackColor = Color.WhiteSmoke;
            ipNameLabel.MouseLeave += (sender, e) => ipPortPanel.BackColor = Color.Transparent;
            ipPortPanel.Controls.Add(ipNameLabel);
            //addInfoLabel(ipPortPanel, "Port:", new Point(420, 15), "��������");

            // ����������
            String hostName = kernel.getHostName();
            Label hostNameLabel = new Label
            {
                Text = hostName,
                Location = new Point(550, 15),
                Font = new Font("Times New Roman", 16, FontStyle.Regular),
                Size = new Size(500, 100)
            };
            hostNameLabel.MouseEnter += (sender, e) => ipPortPanel.BackColor = Color.WhiteSmoke;
            hostNameLabel.MouseLeave += (sender, e) => ipPortPanel.BackColor = Color.Transparent;
            ipPortPanel.Controls.Add(hostNameLabel);

            //port
            portPanel = new Panel();
            addPanel(portPanel, new Point(0, 80));
            Label portNameLabel = new Label
            {
                Text = "�˿�",
                Location = new Point(10, 15),
                Font = new Font("��������", 16, FontStyle.Regular),
                Size = new Size(200, 100)
            };
            portNameLabel.MouseEnter += (sender, e) => portPanel.BackColor = Color.WhiteSmoke;
            portNameLabel.MouseLeave += (sender, e) => portPanel.BackColor = Color.Transparent;
            portPanel.Controls.Add(portNameLabel);

            //6789
            Label portNumLable = new Label
            {
                Text = "6789",
                Location = new Point(550, 15),
                Font = new Font("Times New Roman", 16, FontStyle.Regular),
                Size = new Size(200, 100)
            };
            portNumLable.MouseEnter += (sender, e) => portPanel.BackColor = Color.WhiteSmoke;
            portNumLable.MouseLeave += (sender, e) => portPanel.BackColor = Color.Transparent;
            portPanel.Controls.Add(portNumLable);

            // ����paint�¼�
            contentPanel.Paint += mouseAction.ContentPanel_Paint;
            return contentPanel;
        }

        
        private void addPanel(Panel panel, Point location)
        {
            panel.Location = location;
            panel.Size = new Size(1000, 70);
            panel.BackColor = Color.White;
            panel.MouseEnter += (sender, e) => panel.BackColor = Color.WhiteSmoke;
            panel.MouseLeave += (sender, e) => panel.BackColor = Color.Transparent;
            contentPanel.Controls.Add(panel);
        }

        // ������Ϣ��ǩ
        private Label addInfoLabel(Panel panel, string text, Font font)
        {
            Label label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Font = font;
            label.Dock = DockStyle.Right;
            label.MouseEnter += (sender, e) => panel.BackColor = Color.WhiteSmoke;
            label.MouseLeave += (sender, e) => panel.BackColor = Color.Transparent;
            panel.Controls.Add(label);
            return label;

        }

        private FlowLayoutPanel createHistoryPanel()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Location = new Point(200, 0),
                Size= new Size(this.ClientSize.Width - 200, this.ClientSize.Height),
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
                Size = new Size(520,250),
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
                Location = new Point(10, 70),
                Font = new Font("��������", 16)
            };

            Label userLabel = new Label
            {
                Text = "�û���: " + his.userName,
                AutoSize = true,
                Location = new Point(10, 120),
                Font = new Font("��������", 16)
            };

            Label timeLabel = new Label
            {
                Text = "����ʱ��: " + his.connectTime,
                AutoSize = true,
                Location = new Point(10, 170),
                Font = new Font("��������", 14)
            };
            // �� Label ��ӵ� Panel
            panel.Controls.Add(ipLabel);
            panel.Controls.Add(hostLabel);
            panel.Controls.Add(userLabel);
            panel.Controls.Add(timeLabel);
            return panel;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Network network = Network.getNetwork();
            Debug.WriteLine("��ʼ�ر�");
            network.isConnectMutex.WaitOne();
            Debug.WriteLine("�����ر�");
            if (network.isConnect)
            {
                MessageBox.Show("������ѱ����ӣ���Ͽ����Ӻ��ٹرգ�");
                e.Cancel = true;
            }
            else
            {
                kernel.writeHistories();
                Debug.WriteLine("�ر������߳�");
                network.stop();
            }
            network.isConnectMutex.ReleaseMutex();
        }
    }
}
