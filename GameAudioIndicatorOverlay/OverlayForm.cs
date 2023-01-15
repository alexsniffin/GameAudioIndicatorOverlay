namespace GameAudioIndicatorOverlay;

public partial class OverlayForm : Form
{
    public OverlayForm()
    {
        Text = "Audio Indicator Overlay";

        // hacks, beautiful hacks
        BackColor = Color.Magenta;
        TransparencyKey = Color.Magenta;

        TopMost = true;
        AllowTransparency = true;
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x00000020;
            return cp;
        }
    }
}