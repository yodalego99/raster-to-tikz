using System.Drawing;
using System.Drawing.Drawing2D;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class CurveShape : Shape
    {
        public List<PointF> Points { get; set; } = new();

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible || Points.Count < 2) return;

            var screenPts = Points.Select(p => transform.WorldToScreen(p)).ToArray();
            var beziers = ComputeCatmullRomBeziers(screenPts);

            using var pen = CreatePen(transform);

            if (beziers.Count > 0)
            {
                using var path = new GraphicsPath();
                foreach (var seg in beziers)
                    path.AddBezier(seg.P0, seg.C1, seg.C2, seg.P3);
                g.DrawPath(pen, path);
            }
            else
            {
                g.DrawLine(pen, screenPts[0], screenPts[1]);
            }

            // draw key points as small dots
            foreach (var sp in screenPts)
                g.FillEllipse(Brushes.Gray, sp.X - 2.5f, sp.Y - 2.5f, 5f, 5f);

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            if (Points.Count < 2) return false;

            var beziers = ComputeCatmullRomBeziers(Points.ToArray());

            if (beziers.Count == 0 && Points.Count == 2)
                return DistanceToSegment(worldPoint, Points[0], Points[1]) <= tolerance;

            foreach (var seg in beziers)
            {
                const int steps = 20;
                for (int i = 0; i < steps; i++)
                {
                    var a = EvalBezier(seg, i / (float)steps);
                    var b = EvalBezier(seg, (i + 1) / (float)steps);
                    if (DistanceToSegment(worldPoint, a, b) <= tolerance)
                        return true;
                }
            }
            return false;
        }

        public override RectangleF GetBounds()
        {
            if (Points.Count == 0) return RectangleF.Empty;
            float minX = Points.Min(p => p.X), minY = Points.Min(p => p.Y);
            float maxX = Points.Max(p => p.X), maxY = Points.Max(p => p.Y);
            return new RectangleF(minX, minY, Math.Max(maxX - minX, 0.01f), Math.Max(maxY - minY, 0.01f));
        }

        public override string ToTikZ()
        {
            if (Points.Count < 2) return "";

            string opts = GetTikZStyleOptions();
            string optStr = opts.Length > 0 ? $"[{opts}]" : "";

            var beziers = ComputeCatmullRomBeziers(Points.ToArray());

            if (beziers.Count == 0 && Points.Count == 2)
                return $"\\draw{optStr} {TikZCoord(Points[0])} -- {TikZCoord(Points[1])};";

            var sb = new System.Text.StringBuilder();
            sb.Append($"\\draw{optStr} {TikZCoord(beziers[0].P0)}");
            foreach (var seg in beziers)
                sb.Append($"\n  .. controls {TikZCoord(seg.C1)} and {TikZCoord(seg.C2)} .. {TikZCoord(seg.P3)}");
            sb.Append(';');
            return sb.ToString();
        }

        public override Shape Clone() => new CurveShape
        {
            Points = Points.Select(p => new PointF(p.X, p.Y)).ToList(),
            StrokeColor = StrokeColor, StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle, FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new(Points);

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            if (handleIndex >= 0 && handleIndex < Points.Count)
                Points[handleIndex] = newWorldPos;
        }

        public override void Translate(float dx, float dy)
        {
            for (int i = 0; i < Points.Count; i++)
                Points[i] = new PointF(Points[i].X + dx, Points[i].Y + dy);
        }

        // catmull conversion

        public struct BezierSegment
        {
            public PointF P0, C1, C2, P3;
        }

        public static List<BezierSegment> ComputeCatmullRomBeziers(PointF[] pts)
        {
            var result = new List<BezierSegment>();
            if (pts.Length < 3) return result;

            // mirror endpoints to create virtual boundary points
            var ext = new PointF[pts.Length + 2];
            ext[0] = new PointF(2 * pts[0].X - pts[1].X, 2 * pts[0].Y - pts[1].Y);
            for (int i = 0; i < pts.Length; i++) ext[i + 1] = pts[i];
            ext[^1] = new PointF(2 * pts[^1].X - pts[^2].X, 2 * pts[^1].Y - pts[^2].Y);

            for (int i = 1; i < ext.Length - 2; i++)
            {
                var p0 = ext[i - 1]; var p1 = ext[i]; var p2 = ext[i + 1]; var p3 = ext[i + 2];
                const float a = 1f / 6f;
                var c1 = new PointF(p1.X + a * (p2.X - p0.X), p1.Y + a * (p2.Y - p0.Y));
                var c2 = new PointF(p2.X - a * (p3.X - p1.X), p2.Y - a * (p3.Y - p1.Y));
                result.Add(new BezierSegment { P0 = p1, C1 = c1, C2 = c2, P3 = p2 });
            }
            return result;
        }

        private static PointF EvalBezier(BezierSegment seg, float t)
        {
            float u = 1 - t;
            return new PointF(
                u * u * u * seg.P0.X + 3 * u * u * t * seg.C1.X + 3 * u * t * t * seg.C2.X + t * t * t * seg.P3.X,
                u * u * u * seg.P0.Y + 3 * u * u * t * seg.C1.Y + 3 * u * t * t * seg.C2.Y + t * t * t * seg.P3.Y);
        }
    }
}
