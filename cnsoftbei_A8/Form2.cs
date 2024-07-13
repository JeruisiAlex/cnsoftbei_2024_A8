
using System;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using IWshRuntimeLibrary; // Reference: Windows Script Host Object Model

namespace cnsoftbei_A8
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadApplications();
        }

        private void LoadApplications()
        {
            // Clear existing items
            listBox1.Items.Clear();

            // Load applications from WMI
            listBox1.Items.Add("Applications (WMI):");
            ListApplicationsWMI();

            // Load applications from Registry
            listBox1.Items.Add("\nApplications (Registry):");
            ListApplicationsRegistry();

            // Load applications from Shell
            listBox1.Items.Add("\nApplications (Start Menu):");
            ListApplicationsShell();
        }

        private void ListApplicationsWMI()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
                foreach (ManagementObject obj in searcher.Get())
                {
                    string name = obj["Name"] as string;
                    if (!string.IsNullOrEmpty(name))
                    {
                        listBox1.Items.Add(name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ListApplicationsRegistry()
        {
            try
            {
                string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(uninstallKey);

                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey != null)
                            {
                                string displayName = subKey.GetValue("DisplayName") as string;
                                if (!string.IsNullOrEmpty(displayName))
                                {
                                    listBox1.Items.Add(displayName);
                                }
                            }
                        }
                    }
                }
                else
                {
                    listBox1.Items.Add("Registry key not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ListApplicationsShell()
        {
            try
            {
                string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                var directories = Directory.GetDirectories(startMenuPath);

                foreach (string directory in directories)
                {
                    var files = Directory.GetFiles(directory, "*.lnk");
                    foreach (string file in files)
                    {
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(file);
                        listBox1.Items.Add(shortcut.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}