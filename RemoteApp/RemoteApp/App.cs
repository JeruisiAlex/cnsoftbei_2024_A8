using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteApp
{
    public class App
    {
        private string name;
        private string fullName;
        private string path;
        private string iconPath;
        private App uninstall; // 卸载程序

        public App(string fullName, string path)
        {
            this.name = fullName;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = path;
            this.uninstall = null;
        }
        public App(string name, string fullName, string path)
        {
            this.name = name;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = path;
            this.uninstall = null;
        }
        public App(string name, string fullName, string path, string iconPath)
        {
            this.name = name;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = iconPath;
            this.uninstall = null;
        }
        public App(string name, string fullName, string path, string iconPath,App uninstall)
        {
            this.name = name;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = iconPath;
            this.uninstall = uninstall;
        }



        public string getName()
        {
            return name;
        }
        public string getFullName()
        {
            return fullName;
        }
        public string getPath()
        {
            return path;
        }
        public string getIconPath()
        {
            return iconPath;
        }
        public App getUninstall()
        {
            return uninstall;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void setPath(string path)
        {
            this.path = path;
        }

        public void setIconPath(string iconPath)
        {
            this.iconPath = iconPath;
        }
    }
}
