using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using cnsoftbei_A8;
namespace MouseActionFactory
{
    public class MouseActionFactory: IMouseActionFactory
    {
        public static MouseActionFactory mouseAction;

        // 私有构造函数防止外部实例化
        private MouseActionFactory() { }

        // 公共方法提供唯一实例
        public static MouseActionFactory Instance
        {
            get
            {
                if (mouseAction == null)
                {
                    mouseAction = new MouseActionFactory();
                }
                return mouseAction;
            }
        }
        public void IpPortPanel_MouseEnter(object sender, EventArgs e)
        {
            Form1.ipPortPanel.BackColor = Color.LightGray;
        }

        public void IpPortPanel_MouseLeave(object sender, EventArgs e)
        {
            Form1.ipPortPanel.BackColor = Color.Transparent;
        }

        public void connectionPanel_MouseEnter(object sender, EventArgs e)
        {
            Form1.connectionPanel.BackColor = Color.LightGray;
        }

        public void connectionPanel_MouseLeave(object sender, EventArgs e)
        {
            Form1.connectionPanel.BackColor = Color.Transparent;
        }

        public void BtnSidePanel_Click(object sender, EventArgs e)
        {
            // 获取按钮控件
            Button btn = sender as Button;

            if (btn != null)
            {
                // 恢复上一个按钮的背景颜色
                if (Form1.lastClickedButton != null)
                {
                    Form1.lastClickedButton.BackColor = Color.LightGray;
                }

                // 设置点击的按钮背景颜色为白色
                btn.BackColor = Color.White;
                Form1.lastClickedButton = btn; // 更新最后点击的按钮

                showContent(btn.Text);
            }

        }

        // 用于显示相关内容
        public void showContent(string buttonText)
        {
            // 隐藏所有内容面板
            Form1.contentPanel.Visible = false;
            //appPanel.Visible = false;

            // 根据按钮文本显示相关面板
            switch (buttonText)
            {
                case "主机信息":
                    Form1.contentPanel.Visible = true;
                    break;
                case "发布程序":
                    Form1.appInfoPanel.Visible = true;
                    break;
            }
        }

        public void broadPanel_MouseEnter(object sender, EventArgs e)
        {
            Form1.broadPanel.BackColor = Color.LightGray;
        }
        public void broadPanel_MouseLeave(object sender, EventArgs e)
        {
            Form1.broadPanel.BackColor = Color.Transparent;
        }

        public void startupPanel_MouseEnter(object sender, EventArgs e)
        {
            Form1.startupPanel.BackColor = Color.LightGray;
        }

        public void startupPanel_MouseLeave(object sender, EventArgs e)
        {
            Form1.startupPanel.BackColor = Color.Transparent;
        }


        // 绘制虚线
        public void ContentPanel_Paint(object sender, PaintEventArgs e)
        {
            // 获取Graphics对象
            Graphics g = e.Graphics;

            // 创建虚线画笔
            using (Pen pen = new Pen(Color.LightGray, 1))
            {
                pen.DashStyle = DashStyle.Custom;
                pen.DashPattern = new float[] { 5, 5 };

                // 绘制虚线
                g.DrawLine(pen, 270, 137, 430, 137); // IP下方虚线
                g.DrawLine(pen, 550, 137, 600, 137); // Port下方虚线
            }
        }

        public void SelectExeButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.Title = "选择exe程序";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    Icon icon = Icon.ExtractAssociatedIcon(selectedFilePath);
                    Bitmap bitmap = icon.ToBitmap();
                    //Form1.appListInfo.Add(new MyClass.AppInfo(selectedFilePath,bitmap));
                    // 获取文件信息
                    FileInfo fileInfo = new FileInfo(selectedFilePath);
                    string fileName = fileInfo.Name;
                    long fileSize = fileInfo.Length; // 文件大小，单位为字节
                    DateTime creationTime = fileInfo.CreationTime; // 创建时间
                    DateTime lastAccessTime = fileInfo.LastAccessTime; // 上次访问时间
                    DateTime lastWriteTime = fileInfo.LastWriteTime; // 上次写入时间

                    // 添加到应用程序信息列表
                    //Form1.appListInfo.Add(new MyClass.AppInfo(fileName, bitmap));

                    // 显示文件信息
                    string message = $"文件路径: {selectedFilePath}\n" +
                                     $"文件名: {fileName}\n" +
                                     $"文件大小: {fileSize} 字节\n" +
                                     $"创建时间: {creationTime}\n" +
                                     $"上次访问时间: {lastAccessTime}\n" +
                                     $"上次写入时间: {lastWriteTime}";

                    MessageBox.Show(message, "选择的exe程序");
                    //MessageBox.Show("你选择的文件路径: " + selectedFilePath, "选择的exe程序");
                }
            }
        }

    }
}