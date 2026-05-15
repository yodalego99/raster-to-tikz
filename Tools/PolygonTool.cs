using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{

    public class PolygonTool : DrawingTool
    {
        public override string DisplayName => "Sokszög";

        private readonly List<PointF> _points = new();
        private PointF _cursorWorld;
        private bool _isDrawing;

        // snap-to-close radius in world units
        private const float CloseSnapWorldRadius = 0.3f;

        public PolygonTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var wp = GetWorldPoint(e);

                // if we're close to the first vertex, snap-close and finish
                if (_isDrawing && _points.Count >= 3 && IsNearFirst(wp))
                {
                    Finish(closed: true);
                    return;
                }

                _points.Add(wp);
                _cursorWorld = wp;
                _isDrawing = true;
                Canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                Finish(closed: _points.Count >= 3);
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDrawing) return;
            _cursorWorld = GetWorldPoint(e);
            Canvas.Invalidate();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _points.Clear();
                _isDrawing = false;
                Canvas.Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back && _points.Count > 0)
            {
                _points.RemoveAt(_points.Count - 1);
                if (_points.Count == 0) _isDrawing = false;
                Canvas.Invalidate();
                e.Handled = true;
            }
        }

        public override void OnPaint(Graphics g)
        {
            if (!_isDrawing || _points.Count == 0) return;

            using var pen = new Pen(Color.FromArgb(160, Color.Black), 1.5f) { DashStyle = DashStyle.Dash };
            using var dotBrush = new SolidBrush(Color.DarkGray);

            bool nearFirst = _points.Count >= 3 && IsNearFirst(_cursorWorld);
            var cursor = nearFirst ? _points[0] : _cursorWorld; // snap cursor to first vertex

            // build the preview polyline: placed points + cursor position
            var preview = _points.Select(p => Transform.WorldToScreen(p)).ToList();
            var screenCursor = Transform.WorldToScreen(cursor);

            // draw lines
            var all = new List<PointF>(preview) { screenCursor };
            for (int i = 0; i < all.Count - 1; i++)
                g.DrawLine(pen, all[i], all[i + 1]);

            // if near first vertex, draw the closing segment with a solid hint
            if (nearFirst && preview.Count >= 1)
            {
                using var closePen = new Pen(Color.FromArgb(200, 0, 100, 200), 1.5f);
                g.DrawLine(closePen, screenCursor, preview[0]);
            }

            // vertex dots
            foreach (var sp in preview)
            {
                g.FillEllipse(dotBrush, sp.X - 3f, sp.Y - 3f, 6f, 6f);
                g.DrawEllipse(Pens.Black, sp.X - 3f, sp.Y - 3f, 6f, 6f);
            }

            // highlight first vertex when snap is active
            if (nearFirst)
            {
                using var snapBrush = new SolidBrush(Color.FromArgb(200, 0, 100, 200));
                g.FillEllipse(snapBrush, preview[0].X - 5f, preview[0].Y - 5f, 10f, 10f);
                g.DrawEllipse(Pens.Navy, preview[0].X - 5f, preview[0].Y - 5f, 10f, 10f);
            }
        }

        public override void Deactivate()
        {
            _points.Clear();
            _isDrawing = false;
        }

        private void Finish(bool closed)
        {
            if (_points.Count >= 2)
            {
                var shape = new PolygonShape
                {
                    Points = new List<PointF>(_points),
                    IsClosed = closed
                };
                Document.AddShape(shape);
            }

            _points.Clear();
            _isDrawing = false;
            Canvas.Invalidate();
        }

        private bool IsNearFirst(PointF worldPt)
        {
            if (_points.Count == 0) return false;
            float dx = worldPt.X - _points[0].X;
            float dy = worldPt.Y - _points[0].Y;
            return MathF.Sqrt(dx * dx + dy * dy) <= CloseSnapWorldRadius;
        }
    }
}
