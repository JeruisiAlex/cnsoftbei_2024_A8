namespace RemoteApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Network.getNetwork().init();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Network.getNetwork().stop();
        }
    }
}
