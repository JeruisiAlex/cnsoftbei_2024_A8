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
        public static Button lastClickedButton; // ���ڸ���������İ�ť
        public static FlowLayoutPanel appInfoPanel;
        public static Panel appInstallPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        public static Button buttonAppList;
        public static Button buttonAddApp;
        public static Panel contentPanel;
        public static Label title; 
        public static Kernel kernel;
        // �����ռ�.���� ������
        private MouseActionFactory.MouseActionFactory mouseAction;

        //���س�ʼ������
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
            // ��ȡ MouseActionFactory ��Ψһʵ��
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;

            // ������
            sidePanel = createSidePanel();
            this.Controls.Add(sidePanel);

            // ��ʼ�� contentPanel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            // Ӧ�ó������
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

            buttonAppList = CreateButton("��������");
            flowLayoutPanel.Controls.Add(buttonAppList);
            sidePanel.Controls.Add(flowLayoutPanel);

            return sidePanel;
        }

        // ������ఴť
        private Button CreateButton(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(150, 50),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 0, 15), // ���õײ����Ϊ 10 ����
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("����", 15)
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
                FlowDirection = FlowDirection.TopDown, // ����
                WrapContents = false,
                Location = new Point(0,50),
            };

            //title
            Label titleLabel = new Label
            {
                Text = "�ѷ�������",
                Font = new Font("��������", 20, FontStyle.Bold),
                Location=new Point(180,15),
                BackColor= Color.White,     
                AutoSize = true
            };
            contentPanel.Controls.Add(titleLabel);
            // ��Ӱ�ť
            // ������ť�������
            Panel buttonPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = 70, // �����������ĸ߶�
                Padding = new Padding(0, 0, 0, 0) // ���ð�ť�������ڱ߾�
            };
            Button addButton = new Button
            {
                Text = "����Ӧ��",
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
                Text = "ѡ��װ����",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(180,50),
                Location = new Point(540,0),
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat,
            };
            // ��Ӱ�ť����ť�������
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(installButton);

            // ����ť���������ӵ� FlowLayoutPanel
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
