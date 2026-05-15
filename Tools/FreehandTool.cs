using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class FreehandTool : DrawingTool
    {
        public override string DisplayName => "Ceruza";

        private readonly List<PointF> _points = new();
        private bool _isDrawing;

        public FreehandTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _points.Clear();
            _isDrawing = true;
            _points.Add(GetRawWorldPoint(e));
            Canvas.Invalidate();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing || (e.Button & MouseButtons.Left) == 0) return;

            var world = GetRawWorldPoint(e);
            if (_points.Count == 0)
            {
                _points.Add(world);
            }
            else
            {
                var last = _points[^1];
                float dx = world.X - last.X;
                float dy = world.Y - last.Y;
                float minStep = Math.Max(Transform.ScreenToWorldDistance(2f), 0.02f);
                if (dx * dx + dy * dy >= minStep * minStep)
                    _points.Add(world);
            }

            Canvas.Invalidate();
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (!_isDrawing || e.Button != MouseButtons.Left) return;

            _isDrawing = false;

            var end = GetRawWorldPoint(e);
            if (_points.Count == 0 || _points[^1] != end)
                _points.Add(end);

            if (_points.Count >= 2)
            {
                var shape = new CurveShape { Points = new List<PointF>(_points) };
                Document.AddShape(shape);
            }

            _points.Clear();
            Canvas.Invalidate();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape || !_isDrawing) return;

            _isDrawing = false;
            _points.Clear();
            Canvas.Invalidate();
            e.Handled = true;
        }

        public override void OnPaint(Graphics g)
        {
            if (_points.Count < 2) return;

            var screenPts = _points.Select(p => Transform.WorldToScreen(p)).ToArray();
            using var pen = new Pen(Color.FromArgb(150, Color.Black), 1.5f) { DashStyle = DashStyle.Dash };
            g.DrawLines(pen, screenPts);
        }

        public override void Deactivate()
        {
            _isDrawing = false;
            _points.Clear();
        }

        // freehand uses raw world coordinates
        private PointF GetRawWorldPoint(MouseEventArgs e) => Transform.ScreenToWorld(e.Location);
    }
}