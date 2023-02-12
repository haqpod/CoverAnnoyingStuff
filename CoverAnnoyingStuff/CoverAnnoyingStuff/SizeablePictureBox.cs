using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;

class SizeablePictureBox : PictureBox
{
    public SizeablePictureBox()
    {
        this.ResizeRedraw = true;
        this.Size = new Size(300, 300);
        this.Location = new Point((Screen.FromControl(this).Bounds.Width - this.Width) / 2, (Screen.FromControl(this).Bounds.Height - this.Height) / 2);
    }
}