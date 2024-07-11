using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnsoftbei_A8
{
    internal class RemoteApp
    {
        private string name;
        private string fullName;
        private string path;
        private string iconPath;

        public RemoteApp(string fullName, string path)
        {
            this.name = fullName;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = path;
        }
        public RemoteApp(string name, string fullName, string path)
        {
            this.name = name;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = path;
        }
        public RemoteApp(string name, string fullName, string path, string iconPath)
        {
            this.name = name;
            this.fullName = fullName;
            this.path = path;
            this.iconPath = iconPath;
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
    }
}
