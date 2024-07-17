﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using RemoteApp;
using System.Runtime.InteropServices;

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
            }

        }



        public void SelectExeButton_Click(object sender, EventArgs e)
        {
            Kernel kernel = Kernel.getKernel();
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
                    string fileName = file.FileDescription;

                    
                    // 添加到应用程序信息列表
                    kernel.addRemoteApp(fileName, selectedFilePath);


                    flushAppPanel(kernel.getRemoteAppList(),new Size(Form1.screenWidth,Form1.screenHeight));
                    // 显示文件信息
                    /*string message = $"文件路径: {selectedFilePath}\n" +
                                     $"文件名: {fileName}\n";*/
                    //MessageBox.Show(message, "选择的exe程序");
                    //MessageBox.Show("你选择的文件路径: " + selectedFilePath, "选择的exe程序");
                }
            }
        }

        public void SelectInstallButton_Click(object sender, EventArgs e)
        {
            Kernel kernel = Kernel.getKernel();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "安装程序 (*.exe;*.msi)|*.exe;*.msi|All files (*.*)|*.*";
                openFileDialog.Title = "选择安装程序";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = "";

                    // 获取文件扩展名
                    string fileExtension = Path.GetExtension(selectedFilePath).ToLower();

                    if (fileExtension == ".exe")
                    {
                        // 获取 .exe 文件的信息
                        FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(selectedFilePath);
                        fileName = fileInfo.FileDescription;
                        if (string.IsNullOrEmpty(fileName))
                        {
                            // 如果 FileDescription 为空，则使用文件名
                            fileName = Path.GetFileNameWithoutExtension(selectedFilePath);
                        }
                    }
                    else if (fileExtension == ".msi")
                    {
                        // 获取 .msi 文件的信息
                        fileName = GetMsiSubject(selectedFilePath);
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //MessageBox.Show($"安装程序名称: {fileName}");
                        // 添加到应用程序信息列表
                        kernel.installApp(fileName, selectedFilePath);
                    }
                    else
                    {
                        MessageBox.Show("无法获取安装程序名称");
                    }

                }
            }
        }
        private string GetMsiSubject(string msiFilePath)
        {
            string subject = Path.GetFileNameWithoutExtension(msiFilePath);


            return subject;
        }
        public void flushAppPanel(List<App> appList,Size size)
        {
            Form1.appInfoPanel.Controls.Clear();
            foreach (App app in appList)
            {
                Panel appPanel = createAppPanel(app,size);
                Form1.appInfoPanel.Controls.Add(appPanel);
            }
        }
        public Panel createAppPanel(App app,Size size)
        {
            //Kernel kernel = Kernel.getKernel();
            // 创建应用程序面板
            Panel appPanel = new Panel
            {
                Size = new Size((int)(0.9*size.Width), 50),
                Margin = new Padding(10, 10, 0, 0)
            };

            Icon icon = Icon.ExtractAssociatedIcon(app.getIconPath());
            Bitmap bitmap = icon.ToBitmap();
            // 创建显示图标的 PictureBox
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(5, 12),
                Image = bitmap,
                SizeMode = PictureBoxSizeMode.StretchImage,
            };
            appPanel.Controls.Add(pictureBox);

            // 创建显示名称的 Label
            Label nameLabel = new Label
            {
                Text = app.getFullName(), // 设置名称文本
                Location = new Point(60, 15), // 设置位置
                AutoSize = true, // 自动调整大小以适应内容
                Font = new Font("华文中宋", 16, FontStyle.Bold), // 设置字体
            };
            appPanel.Controls.Add(nameLabel); // 将 Label 添加到面板

            // 创建 ContextMenuStrip 并添加菜单项
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem runMenuItem = new ToolStripMenuItem("运行");
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("移除");
            ToolStripMenuItem unistallMenuItem = new ToolStripMenuItem("卸载");

            // 添加菜单项到 ContextMenuStrip
            contextMenu.Items.Add(runMenuItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.Items.Add(unistallMenuItem);
            appPanel.ContextMenuStrip = contextMenu;
            // 处理菜单项的点击事件
            runMenuItem.Click += (sender, e) => runMenuItem_Click(sender, e, app);
            deleteMenuItem.Click += (sender, e) => deleteMenuItem_Click(sender, e, app);
            unistallMenuItem.Click += (sender, e) => unistallMenuItem_Click(sender, e, app);

            // 绑定鼠标移入和移出事件
            appPanel.MouseEnter += (sender, e) => appPanel.BackColor = Color.WhiteSmoke;
            appPanel.MouseLeave += (sender, e) => appPanel.BackColor = Color.Transparent;
            appPanel.DoubleClick += (sender, e) => runMenuItem_Click(sender, e, app);
            nameLabel.MouseEnter += (sender, e) => appPanel.BackColor = Color.WhiteSmoke;
            nameLabel.MouseLeave += (sender, e) => appPanel.BackColor = Color.Transparent;

            pictureBox.MouseEnter += (sender, e) => appPanel.BackColor = Color.WhiteSmoke;
            pictureBox.MouseLeave += (sender, e) => appPanel.BackColor = Color.Transparent;

            return appPanel;
        }


        private void deleteMenuItem_Click(object sender, EventArgs e, App app)
        {
            Kernel kernel = Kernel.getKernel();
            kernel.removeApp(app.getFullName());
            flushAppPanel(kernel.getRemoteAppList(),new Size(Form1.screenWidth,Form1.screenHeight));
        }
        private void runMenuItem_Click(Object sender, EventArgs e, App app)
        {
            Kernel kernel = Kernel.getKernel();
            kernel.openApp(app.getFullName());
        }
        private void unistallMenuItem_Click(Object sender, EventArgs e, App app)
        {
            Kernel kernel = Kernel.getKernel();
            kernel.uninstallApp(app.getFullName());
        }

        public void copyFileButton_Click(object sender, EventArgs e)
        {
            string[] sourcePath = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择文件";
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    sourcePath = openFileDialog.FileNames;
                }
            }
            string destinationPath = "";
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Tag = "选择目标文件夹";
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    destinationPath = folderBrowserDialog.SelectedPath;
                }
            }
            if (!string.IsNullOrEmpty(destinationPath) && sourcePath != null)
            {
                foreach(string path in sourcePath)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        string destination = Path.Combine(destinationPath, Path.GetFileName(path));
                        File.Copy(path, destination, true);
                    }
                }
            }
        }

        public void deleteFileButton_click(object sender, EventArgs e)
        {
            string[] filePath = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "选择要删除的文件";
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileNames;
                }
            }
            if (filePath != null)
            {
                foreach (string path in filePath)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        File.Delete(path);
                    }
                }
            }
        }
    }
}