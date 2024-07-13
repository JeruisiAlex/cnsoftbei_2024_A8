using System.Diagnostics;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Kernel kernel = Kernel.getKernel();
            kernel.init();
        }
    }
}
