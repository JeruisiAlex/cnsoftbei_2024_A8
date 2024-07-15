using System;
using System.Windows.Forms;

namespace MouseActionFactory
{
    public interface IMouseActionFactory
    {

        void BtnSidePanel_Click(object sender, EventArgs e);
        void showContent(string buttonText);
        void ContentPanel_Paint(object sender, PaintEventArgs e);
        // 窗口大小改变时，元素位置自适应
    }
}

