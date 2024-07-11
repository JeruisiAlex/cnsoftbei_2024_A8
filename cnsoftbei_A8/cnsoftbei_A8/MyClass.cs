﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MyClass
{
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

    public class AppInfo
    {
        public string Name { get; set; }

        // 应用程序 Logo
        public Bitmap Logo { get; set; }

        // 启动程序的路径
        public string OpenPath { get; set; }

        // 卸载程序的路径
        public string UninstallPath { get; set; }

        public AppInfo(string name, Bitmap logo)
        {
            Name = name;
            Logo = logo;
        }

        // 构造函数
        public AppInfo(string name, Bitmap logo, string openPath, string uninstallPath)
        {
            Name = name;
            Logo = logo;
            OpenPath = openPath;
            UninstallPath = uninstallPath;
        }
    }
}