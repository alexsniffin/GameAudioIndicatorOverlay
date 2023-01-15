using Timer = System.Windows.Forms.Timer;

namespace GameAudioIndicatorOverlay;

public class IndicatorPanel : Panel
{
    private double _degree;

    public IndicatorPanel(double degree)
    {
        _degree = degree;
        var screen = Screen.PrimaryScreen;
        Size = new Size(screen.Bounds.Width, screen.Bounds.Height);
    }

    public double Degree
    {
        get { return _degree; }
        set
        {
            _degree = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var screen = Screen.PrimaryScreen;
        var diameter = Math.Min(screen.Bounds.Width, screen.Bounds.Height);
        var radius = diameter / 2;
        var center = new Point(screen.Bounds.Width / 2, screen.Bounds.Height / 2);

        // Draw the circle
        e.Graphics.DrawEllipse(Pens.RoyalBlue, center.X - radius, center.Y - radius, diameter, diameter);

        // Calculate the starting and ending angles for the highlighted section
        var startAngle = _degree - 10;
        var endAngle = _degree + 10;

        // Draw the highlighted section
        e.Graphics.FillPie(Brushes.Red, center.X - radius, center.Y - radius, diameter, diameter, (float)startAngle, (float)(endAngle - startAngle));
    }
}