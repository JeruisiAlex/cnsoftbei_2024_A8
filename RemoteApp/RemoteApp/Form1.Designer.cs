namespace RemoteApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public static int screenWidth;
        public static int screenHeight;
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            Size minSize = new Size(800, 600);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // 设置自动缩放基准尺寸
            this.AutoScaleDimensions = new SizeF(96F, 96F); // 默认的 DPI 是 96 DPI
            getResolution();

            screenWidth = (int)(screenWidth*0.5);
            screenHeight = (int)(screenHeight*0.6);
            if (screenHeight < minSize.Height)
            {
                screenHeight = minSize.Height;
            }
            if (screenWidth < minSize.Width)
            {
                screenWidth = minSize.Width;
            }

            ClientSize = new Size(screenWidth, screenHeight);
            Margin = new Padding(4);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
        }

        private void getResolution()
        {
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;
        }

        #endregion
    }
}
