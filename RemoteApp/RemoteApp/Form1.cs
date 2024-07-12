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
        public static Button lastClickedButton; // ���ڸ���������İ�ť
        public static Panel appInfoPanel;
        public static FlowLayoutPanel appInstallPanel;
        public static Panel broadPanel;
        public static Panel startupPanel;
        public static Button buttonAppList;
        public static Button buttonInstallApp;
        public static List<App> appList=new List<App>();
        //public static Kernel kernel;
        // �������g.���� ������
        private MouseActionFactory.MouseActionFactory mouseAction;
        //���س�ʼ������
        public Form1()
        {
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


            //Ӧ�ó������
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

            buttonAppList = CreateButton("Ӧ�ó���");
            flowLayoutPanel.Controls.Add(buttonAppList);
            buttonInstallApp = CreateButton("��װ����");
            flowLayoutPanel.Controls.Add(buttonInstallApp);
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

        public FlowLayoutPanel createAppInfoPanel()
        {
            FlowLayoutPanel allAppInfoPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown, //���²���
                WrapContents = false,
                BackColor = Color.White
            };

            Panel panel = new Panel
            {
                Margin = new Padding(500, 20, 10, 0), // ����Marginȷ�����ᳬ����Χ
                Size = new Size(500, 50),
                BackColor = Color.White,
                AutoSize = true
            };
            //title
            Label titleLabel = new Label
            {
                Text = "�ѷ�������",
                Location = new Point(200,15),
                Font = new Font("��������", 20, FontStyle.Bold),
                AutoSize = true
            };
            panel.Controls.Add(titleLabel);
            this.Controls.Add(panel);
            // ��Ӱ�ť
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
