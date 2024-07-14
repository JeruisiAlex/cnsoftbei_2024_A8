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
        public Size size;
        // �����ռ�.���� ������
        private MouseActionFactory.MouseActionFactory mouseAction;


        //���س�ʼ������
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
            // ��ȡ MouseActionFactory ��Ψһʵ��
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;

            // ��ʼ�� contentPanel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            // Ӧ�ó������
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
                FlowDirection = FlowDirection.TopDown, // ����
                WrapContents = false,
                Size = new Size(screenWidth,(int)(0.75*screenHeight)),
                Location = new Point(10,100),
                BackColor = Color.White
            };

            //title
            Label titleLabel = new Label
            {
                Text = "�ѷ�������",
                Font = new Font("��������", 20, FontStyle.Bold),
                Location=new Point(0,20),
                BackColor= Color.White,     
                AutoSize = true,
                Size = new Size(screenWidth,100),
            };
            contentPanel.Controls.Add(titleLabel);
            // ��Ӱ�ť
            // ������ť�������
            Panel buttonPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = (int)(0.1*screenHeight), // �����������ĸ߶�
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
                Location = new Point(screenWidth - 200, 15),
            };

            Button installButton = new Button
            {
                Text = "ѡ��װ����",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(200,50),
                Location = new Point(screenWidth-500,15),
                BackColor = Color.LightGray,
                FlatAppearance = { BorderSize = 0 },
                FlatStyle = FlatStyle.Flat
            };
            // ��Ӱ�ť����ť�������
            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(installButton);

            // ����ť���������ӵ� FlowLayoutPanel
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
