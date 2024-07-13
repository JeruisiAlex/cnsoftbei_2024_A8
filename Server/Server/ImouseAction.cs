using System;
using System.Windows.Forms;

namespace MouseActionFactory
{
    public interface IMouseActionFactory
    {

        void IpPortPanel_MouseEnter(object sender, EventArgs e);
        void IpPortPanel_MouseLeave(object sender, EventArgs e);
        void connectionPanel_MouseEnter(object sender, EventArgs e);
        void connectionPanel_MouseLeave(object sender, EventArgs e);
        void BtnSidePanel_Click(object sender, EventArgs e);
        void showContent(string buttonText);
        void broadPanel_MouseEnter(object sender, EventArgs e);

        void ContentPanel_Paint(object sender, PaintEventArgs e);
        // 窗口大小改变时，元素位置自适应
    }
}

