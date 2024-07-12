using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using RemoteApp;
namespace MouseActionFactory
{
    public class MouseActionFactory : IMouseActionFactory
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
            Form1.appInfoPanel.Visible = false;
            //appPanel.Visible = false;

            // 根据按钮文本显示相关面板
            switch (buttonText)
            {
                case "应用程序":
                    Form1.appInfoPanel.Visible = true;
                    break;
                case "安装程序":
                    Form1.appInstallPanel.Visible = true;
                    break;
            }
        }


        public void SelectExeButton_Click(object sender, EventArgs e)
        {
            //Kernel kernel = Kernel.getKernel();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.Title = "选择exe程序";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    // 获取文件信息
                    FileInfo fileInfo = new FileInfo(selectedFilePath);
                    FileVersionInfo file = FileVersionInfo.GetVersionInfo(selectedFilePath);
                    string fileName = file.ProductName;
                    long fileSize = fileInfo.Length; // 文件大小，单位为字节
                    DateTime creationTime = fileInfo.CreationTime; // 创建时间
                    DateTime lastAccessTime = fileInfo.LastAccessTime; // 上次访问时间
                    DateTime lastWriteTime = fileInfo.LastWriteTime; // 上次写入时间

                    // 添加到应用程序信息列表
                    //kernel.addRemoteApp(fileName, selectedFilePath);
                    Form1.appList.Add(new App(fileName, selectedFilePath));

                    //flushAppPanel(kernel.getRemoteAppList());
                    flushAppPanel(Form1.appList);
                    // 显示文件信息
                    /*string message = $"文件路径: {selectedFilePath}\n" +
                                     $"文件名: {fileName}\n" +
                                     $"文件大小: {fileSize} 字节\n" +
                                     $"创建时间: {creationTime}\n" +
                                     $"上次访问时间: {lastAccessTime}\n" +
                                     $"上次写入时间: {lastWriteTime}";*/
                    //MessageBox.Show(message, "选择的exe程序");
                    //MessageBox.Show("你选择的文件路径: " + selectedFilePath, "选择的exe程序");
                }
            }
        }
        public void flushAppPanel(List<App> appList)
        {
            Form1.appInfoPanel.Controls.Clear();
            foreach (App app in appList)
            {
                Panel appPanel = createAppPanel(app);
                Form1.appInfoPanel.Controls.Add(appPanel);
                //MessageBox.Show($"{app.Name}\n");
            }
        }
        public Panel createAppPanel(App app)
        {
            //Kernel kernel = Kernel.getKernel();
            // 创建应用程序面板
            Panel appPanel = new Panel
            {
                Size = new Size(600, 100),
                //BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(200, 60, 0, 0)
            };

            Icon icon = Icon.ExtractAssociatedIcon(app.getIconPath());
            Bitmap bitmap = icon.ToBitmap();
            // 创建显示图标的 PictureBox
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(50, 50),
                Location = new Point(15, 25),
                Image = bitmap,
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            appPanel.Controls.Add(pictureBox);

            // 创建显示名称的 Label
            Label nameLabel = new Label
            {
                Text = "程序名：" + app.getFullName(), // 设置名称文本
                Location = new Point(100, 20), // 设置位置
                AutoSize = true, // 自动调整大小以适应内容
                Font = new Font("Arial", 12, FontStyle.Bold), // 设置字体
            };
            appPanel.Controls.Add(nameLabel); // 将 Label 添加到面板
            Label otherNameLabel = new Label
            {
                Text = "别名：" + app.getName(), // 设置名称文本
                Location = new Point(100, 60), // 设置位置
                AutoSize = true, // 自动调整大小以适应内容
                Font = new Font("Arial", 12, FontStyle.Bold), // 设置字体
            };
            appPanel.Controls.Add(otherNameLabel); // 将 Label 添加到面板

            Button deleteButton = new Button
            {
                Text = "删除",
                Font = new Font("华文中宋", 16, FontStyle.Bold),
                BackColor = Color.White,
                Size = new Size(80, 40),
                Location = new Point(400, 20),
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += (s, e) =>
            {
                // 显示确认对话框
                DialogResult result = MessageBox.Show(
                    "确认要删除此应用程序吗？",
                    "确认删除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // 从 appListInfo 中删除相应的项
                    //kernel.removeApp(app.getFullName());
                    Form1.appList.Remove(app);

                    // 从面板中移除当前的 appPanel
                    appPanel.Parent.Controls.Remove(appPanel);

                    // 重新渲染面板
                    //flushAppPanel(kernel.getRemoveList());
                    flushAppPanel(Form1.appList);
                }
            };
            appPanel.Controls.Add(deleteButton);
            return appPanel;
        }
    }
}