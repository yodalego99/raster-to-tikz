using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class CurveTool : DrawingTool
    {
        public override string DisplayName => "Bézier-görbe";

        private readonly List<PointF> _points = new();
        private PointF _cursorWorld;
        private bool _isDrawing;

        public CurveTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _points.Add(GetWorldPoint(e));
                _cursorWorld = GetWorldPoint(e);
                _isDrawing = true;
                Canvas.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                Finish();
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

            var preview = new List<PointF>(_points) { _cursorWorld };
            var screenPts = preview.Select(p => Transform.WorldToScreen(p)).ToArray();

            using var pen = new Pen(Color.FromArgb(150, Color.Black), 1.5f) { DashStyle = DashStyle.Dash };

            if (screenPts.Length >= 3)
            {
                var beziers = CurveShape.ComputeCatmullRomBeziers(screenPts);
                if (beziers.Count > 0)
                {
                    using var path = new GraphicsPath();
                    foreach (var seg in beziers)
                        path.AddBezier(seg.P0, seg.C1, seg.C2, seg.P3);
                    g.DrawPath(pen, path);
                }
            }
            else if (screenPts.Length == 2)
            {
                g.DrawLine(pen, screenPts[0], screenPts[1]);
            }

            foreach (var sp in screenPts)
            {
                g.FillEllipse(Brushes.DarkGray, sp.X - 3f, sp.Y - 3f, 6f, 6f);
                g.DrawEllipse(Pens.Black, sp.X - 3f, sp.Y - 3f, 6f, 6f);
            }
        }

        public override void Deactivate()
        {
            _points.Clear();
            _isDrawing = false;
        }

        private void Finish()
        {
            if (_points.Count >= 2)
                Document.AddShape(new CurveShape { Points = new List<PointF>(_points) });

            _points.Clear();
            _isDrawing = false;
            Canvas.Invalidate();
        }
    }
}
