using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class LineTool : DrawingTool
    {
        public override string DisplayName => "Szakasz";

        private bool _isDrawing;
        private PointF _startWorld;
        private PointF _currentWorld;

        public LineTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var wp = GetWorldPoint(e);

            if (!_isDrawing)
            {
                // first click: set start point
                _startWorld = wp;
                _currentWorld = wp;
                _isDrawing = true;
            }
            else
            {
                // second click: place end point and create the line
                _currentWorld = wp;
                _isDrawing = false;

                // dont create zero-length lines
                float dx = _currentWorld.X - _startWorld.X;
                float dy = _currentWorld.Y - _startWorld.Y;
                if (dx * dx + dy * dy < 0.001f) return;

                var line = new LineSegment { Start = _startWorld, End = _currentWorld };
                Document.AddShape(line);
            }

            Canvas.Invalidate();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing) return;

            _currentWorld = GetWorldPoint(e);
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
            if (!_isDrawing) return;

            var s = Transform.WorldToScreen(_startWorld);
            var e = Transform.WorldToScreen(_currentWorld);

            using var pen = new Pen(Color.FromArgb(150, Color.Black), 1.5f)
            {
                DashStyle = DashStyle.Dash
            };
            g.DrawLine(pen, s, e);
        }

        public override void Deactivate()
        {
            _isDrawing = false;
        }
    }
}
