using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class CircleTool : DrawingTool
    {
        public override string DisplayName => "Kör";

        private bool _isDrawing;
        private PointF _centerWorld;
        private float _currentRadius;

        public CircleTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (!_isDrawing)
            {
                // first click: set center
                _centerWorld = GetWorldPoint(e);
                _currentRadius = 0;
                _isDrawing = true;
            }
            else
            {
                // second click: confirm radius
                var worldPt = GetWorldPoint(e);
                float dx = worldPt.X - _centerWorld.X;
                float dy = worldPt.Y - _centerWorld.Y;
                _currentRadius = MathF.Sqrt(dx * dx + dy * dy);
                _isDrawing = false;

                // dont create zero-radius circles
                if (_currentRadius < 0.05f) return;

                var circle = new CircleShape { Center = _centerWorld, Radius = _currentRadius };
                Document.AddShape(circle);
            }

            Canvas.Invalidate();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing) return;

            var worldPt = GetWorldPoint(e);
            float dx = worldPt.X - _centerWorld.X;
            float dy = worldPt.Y - _centerWorld.Y;
            _currentRadius = MathF.Sqrt(dx * dx + dy * dy);
            Canvas.Invalidate();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _isDrawing)
            {
                _isDrawing = false;
                Canvas.Invalidate();
                e.Handled = true;
            }
        }

        public override void OnPaint(Graphics g)
        {
            if (!_isDrawing || _currentRadius < 0.001f) return;

            var screenCenter = Transform.WorldToScreen(_centerWorld);
            float screenRadius = Transform.WorldToScreenDistance(_currentRadius);

            using var pen = new Pen(Color.FromArgb(150, Color.Black), 1.5f)
            {
                DashStyle = DashStyle.Dash
            };
            g.DrawEllipse(pen, screenCenter.X - screenRadius, screenCenter.Y - screenRadius,
                screenRadius * 2, screenRadius * 2);
        }

        public override void Deactivate()
        {
            _isDrawing = false;
        }
    }
}
