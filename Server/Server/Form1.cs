using System;
using System.Diagnostics;
using MouseActionFactory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        public static Panel startupPanel;
        private MouseActionFactory.MouseActionFactory mouseAction;

        public Form1()
        {
            InitializeComponent();
            Kernel kernel = Kernel.getKernel();
            kernel.histories.Add(new History("123", "123", "123"));
            kernel.writeHistories();
            kernel.readHistories();
            if (kernel.histories == null)
            {
                Debug.WriteLine("++++++++++++++++");
            }
            else
            {
                Debug.WriteLine(kernel.histories[0].getIp());
            }
            kernel.init();
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

            //Ӧ�ó������
            /*appInfoPanel = createAddAppInfoPanel();
            this.Controls.Add(appInfoPanel);


            mouseAction.flushAppPanel(kernel.getRemoteAppList());*/
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
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(150, 50);
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = Color.LightGray;
            button.FlatAppearance.BorderSize = 0;
            button.Margin = new Padding(0, 0, 0, 15); // ���õײ����Ϊ 10 ����
            button.TextAlign = ContentAlignment.MiddleCenter;

            button.Font = new Font("����", 15);

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


            // IP �� Port ���
            ipPortPanel = new Panel();
            addPanel(ipPortPanel, new Point(180, 100));
            ipPortPanel.MouseEnter += mouseAction.IpPortPanel_MouseEnter;
            ipPortPanel.MouseLeave += mouseAction.IpPortPanel_MouseLeave;
            ipPortPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


            // ������ݿؼ�
            addInfoLabel(ipPortPanel, "������:", new Point(20, 15), "��������");
            addInfoLabel(ipPortPanel, "Port:", new Point(320, 15), "��������");

            String hostName = getHostName();

            // ��� IP �Ͷ˿���ʾ�ؼ�
            addInfoLabel(ipPortPanel, hostName, new Point(60, 15), "Times New Roman");
            addInfoLabel(ipPortPanel, "6789", new Point(370, 15), "Times New Roman");

            // ���ؿؼ�
            connectionPanel = new Panel();
            addPanel(connectionPanel, new Point(180, 170));
            addInfoLabel(connectionPanel, "�������������", new Point(20, 15), "��������");
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
            addInfoLabel(broadPanel, "�Ƿ��͹㲥", new Point(20, 15), "��������");

            ToggleButton chkBroadcast = new ToggleButton();
            chkBroadcast.Location = new Point(320, 15);
            broadPanel.Controls.Add(chkBroadcast);

            startupPanel = new Panel();
            addPanel(startupPanel, new Point(180, 310));
            startupPanel.MouseEnter += mouseAction.startupPanel_MouseEnter;
            startupPanel.MouseLeave += mouseAction.startupPanel_MouseLeave;
            addInfoLabel(startupPanel, "��������", new Point(20, 15), "��������");

            ToggleButton chkStartup = new ToggleButton();
            chkStartup.Location = new Point(320, 15);
            //chkStartup.AutoSize = true;
            startupPanel.Controls.Add(chkStartup);

            // ����logo������״̬
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

            // ����paint�¼�
            contentPanel.Paint += mouseAction.ContentPanel_Paint;

            return contentPanel;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("������ѱ����ӣ���Ͽ����Ӻ��ٹرգ�");
            e.Cancel = true;
        }
    }
}
