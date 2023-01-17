using System.Drawing.Drawing2D;

namespace GameAudioIndicatorOverlay
{
    public class IndicatorPanel : Panel
    {
        private float _panValue = 0.0f;
        private float[] _channelPercentages = new float[]{};
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
        
        public void UpdateChannelPercentages(float[] channelPercentages)
        {
            _channelPercentages = channelPercentages;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            var yPos = Height * 0.55f;
            var lineStart = new PointF(Width * 0.45f, yPos);
            var lineEnd = new PointF(Width * 0.55f, yPos);
            g.DrawLine(_linePen, lineStart, lineEnd);
            var offset = _panValue * (lineEnd.X - lineStart.X - _highlightWidth) / 2 + (lineStart.X + lineEnd.X - _highlightWidth) / 2;
            var height = _linePen.Width + _linePen.Width * 0.5f;
            var yRectangle = yPos - height / 2;
            g.FillRectangle(_highlightBrush, new RectangleF(offset, yRectangle, _highlightWidth, height));
            
            var center = new PointF(Width/2, Height/2);
            float radius = Width/5;
            var angleIncrement = 2 * (float)Math.PI / _channelPercentages.Length;
            const float minSize = 0;
            const float maxSize = 125;
            for (var i = 0; i < _channelPercentages.Length; i++)
            {
                var angle = i switch
                {
                    0 => (float)(-0.75 * Math.PI),
                    1 => (float)(-0.25 * Math.PI),
                    2 => (float)(-0.50 * Math.PI),
                    3 => (float)(0.50 * Math.PI),
                    4 => (float)(0.75 * Math.PI),
                    5 => (float)(0.25 * Math.PI),
                    _ => 0
                };
                var size = Math.Min(maxSize, Math.Max(minSize, _channelPercentages[i] * maxSize));
                var x = center.X + (float)Math.Cos(angle) * radius - size/2;
                var y = center.Y + (float)Math.Sin(angle) * radius - size/2;
                g.FillEllipse(_highlightBrush, x, y, size, size);
            }
        }
    }
}