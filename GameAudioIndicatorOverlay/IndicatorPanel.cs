using System.Drawing.Drawing2D;

namespace GameAudioIndicatorOverlay;

public class IndicatorPanel : Panel
{
    private float _panValue;
    private Brush _highlightBrush = new SolidBrush(Color.Red);
    private Pen _linePen = new Pen(Color.FromArgb(255, Color.Black),5);
    private float _highlightWidth = 20;

    public IndicatorPanel()
    {
        var screen = Screen.PrimaryScreen;
        Size = new Size(screen.Bounds.Width, screen.Bounds.Height);
        _linePen.EndCap = LineCap.Round;
        _linePen.StartCap = LineCap.Round;
    }

    public void UpdatePanValue(float panValue)
    {
        _panValue = panValue;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        float yPos = Height * 0.55f;
        var lineStart = new PointF(Width * 0.45f, yPos);
        var lineEnd = new PointF(Width * 0.55f, yPos);
        g.DrawLine(_linePen, lineStart, lineEnd);
        float offset = _panValue * (lineEnd.X - lineStart.X - _highlightWidth) / 2 + (lineStart.X + lineEnd.X - _highlightWidth) / 2;
        float height = _linePen.Width + _linePen.Width * 0.5f;
        float yRectangle = yPos - height / 2;
        g.FillRectangle(_highlightBrush, new RectangleF(offset, yRectangle, _highlightWidth, height));
    }
}