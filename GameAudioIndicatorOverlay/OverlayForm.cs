namespace GameAudioIndicatorOverlay;

public class OverlayForm : Form
{
    private Label _label;
    private IndicatorPanel _panel;
    
    public OverlayForm()
    {
        Text = "Audio Indicator Overlay";
        
        _label = new Label();
        _label.Text = "Hello World!";
        _label.Font = new Font("Arial", 10);
        
        var screen = Screen.PrimaryScreen;
        _label.Size = new Size(500, 200);
        _label.ForeColor = Color.Transparent;

        _panel = new IndicatorPanel();
        
        Controls.Add(_label);
        Controls.Add(_panel);

        // hacks, beautiful hacks
        BackColor = Color.Magenta;
        TransparencyKey = Color.Magenta;

        TopMost = true;
        AllowTransparency = true;
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
    }

    public void UpdateText(string text)
    {
        _label.Text = text;
    }

    public void UpdateIndicator(float panValue)
    {
        _panel.UpdatePanValue(panValue);
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
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && (_panel != null))
        {
            _panel.Dispose();
        }

        base.Dispose(disposing);
    }
}