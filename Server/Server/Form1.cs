using System.Diagnostics;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Kernel kernel = Kernel.getKernel();

            kernel.histories.Add(new History("123", "123", "123"));
            kernel.writeHistories();
            kernel.readHistories();
            if(kernel.histories == null)
            {
                Debug.WriteLine("++++++++++++++++");
            }
            else
            {
                Debug.WriteLine(kernel.histories[0].getIp());
            }
        }
    }
}
