namespace GameAudioIndicatorOverlay;

partial class OverlayForm
{
    private IndicatorPanel panel;
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && (panel != null))
        {
            panel.Dispose();
        }

        base.Dispose(disposing);
    }

    public void TriggerOverlayIndicator(double degree)
    {
        if (this.panel != null)
        {
            this.Controls.Remove(panel); 
            this.panel.Dispose();
        }

        this.panel = new IndicatorPanel(degree);
        this.Controls.Add(this.panel);
    }
}