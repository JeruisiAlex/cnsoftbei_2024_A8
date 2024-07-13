using System;
using System.Windows.Forms;
namespace MouseActionFactory
{
    public interface IMouseActionFactory
    {

        void BtnSidePanel_Click(object sender, EventArgs e);
        void SelectExeButton_Click(object sender, EventArgs e);
        void SelectInstallButton_Click(object sender, EventArgs e);

    }
}

