using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class ArcTool : DrawingTool
    {
        public override string DisplayName => "Ív";

        private enum State { Idle, HaveStart, HaveEnd }
        private State _state = State.Idle;

        private PointF _start;
        private PointF _end;
        private PointF _cursor;

        public ArcTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var wp = GetWorldPoint(e);

            switch (_state)
            {
                case State.Idle:
                    _start = wp;
                    _cursor = wp;
                    _state = State.HaveStart;
                    break;

                case State.HaveStart:
                    _end = wp;
                    _cursor = wp;
                    _state = State.HaveEnd;
                    break;

                case State.HaveEnd:
                    // third click finalize
                    var arc = new ArcShape { Start = _start, End = _end, MidPoint = wp };
                    Document.AddShape(arc);
                    _state = State.Idle;
                    break;
            }

            Canvas.Invalidate();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            if (_state == State.Idle) return;
            _cursor = GetWorldPoint(e);
            Canvas.Invalidate();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _state = State.Idle;
                Canvas.Invalidate();
                e.Handled = true;
            }
        }

        public override void OnPaint(Graphics g)
        {
            using var pen = new Pen(Color.FromArgb(150, Color.Black), 1.5f) { DashStyle = DashStyle.Dash };
            using var dotBrush = new SolidBrush(Color.DarkGray);
            const float r = 3f;

            switch (_state)
            {
                case State.HaveStart:
                {
                    // preview line from start to cursor
                    var s = Transform.WorldToScreen(_start);
                    var c = Transform.WorldToScreen(_cursor);
                    g.DrawLine(pen, s, c);
                    DrawDot(g, dotBrush, s, r);
                    DrawDot(g, dotBrush, c, r);
                    break;
                }

                case State.HaveEnd:
                {
                    // preview arc from start to end passing through cursor
                    var sStart = Transform.WorldToScreen(_start);
                    var sEnd = Transform.WorldToScreen(_end);
                    var sCursor = Transform.WorldToScreen(_cursor);

                    // try to draw the arc preview
                    if (ArcShape.ComputeCircle(_start, _cursor, _end, out var center, out float radius))
                    {
                        float startAngle = MathF.Atan2(_start.Y - center.Y, _start.X - center.X);
                        float midAngle = MathF.Atan2(_cursor.Y - center.Y, _cursor.X - center.X);
                        float endAngle = MathF.Atan2(_end.Y - center.Y, _end.X - center.X);

                        float mid = NormalizeAngle(midAngle - startAngle);
                        float end = NormalizeAngle(endAngle - startAngle);
                        float sweep = mid < end ? end : end - 2 * MathF.PI;

                        const int steps = 60;
                        var pts = new PointF[steps + 1];
                        for (int i = 0; i <= steps; i++)
                        {
                            float t = i / (float)steps;
                            float angle = startAngle + sweep * t;
                            var wp = new PointF(center.X + radius * MathF.Cos(angle),
                                                center.Y + radius * MathF.Sin(angle));
                            pts[i] = Transform.WorldToScreen(wp);
                        }
                        for (int i = 0; i < steps; i++)
                            g.DrawLine(pen, pts[i], pts[i + 1]);
                    }
                    else
                    {
                        // collinear — just draw a straight line
                        g.DrawLine(pen, sStart, sEnd);
                    }

                    DrawDot(g, dotBrush, sStart, r);
                    DrawDot(g, dotBrush, sEnd, r);
                    DrawDot(g, dotBrush, sCursor, r);
                    break;
                }
            }
        }

        public override void Deactivate()
        {
            _state = State.Idle;
        }

        private static void DrawDot(Graphics g, Brush brush, PointF center, float radius)
        {
            g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
            g.DrawEllipse(Pens.Black, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }

        private static float NormalizeAngle(float a)
        {
            a %= (2 * MathF.PI);
            if (a < 0) a += 2 * MathF.PI;
            return a;
        }
    }
}
