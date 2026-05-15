using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class ArcShape : Shape
    {
        public PointF Start { get; set; }
        public PointF End { get; set; }
        public PointF MidPoint { get; set; }

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible) return;

            using var pen = CreatePen(transform);
            var path = BuildScreenPath(transform);
            if (path != null)
            {
                using (path)
                    g.DrawPath(pen, path);
            }

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            // approximate: subdivide the arc into line segments and test each
            if (!ComputeCircle(Start, MidPoint, End, out var center, out float radius))
                return DistanceToSegment(worldPoint, Start, End) <= tolerance;

            float startAngle = MathF.Atan2(Start.Y - center.Y, Start.X - center.X);
            float midAngle = MathF.Atan2(MidPoint.Y - center.Y, MidPoint.X - center.X);
            float endAngle = MathF.Atan2(End.Y - center.Y, End.X - center.X);
            float sweep = ComputeSweep(startAngle, midAngle, endAngle);

            const int steps = 30;
            PointF prev = Start;
            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                float angle = startAngle + sweep * t;
                var pt = new PointF(center.X + radius * MathF.Cos(angle), center.Y + radius * MathF.Sin(angle));
                if (DistanceToSegment(worldPoint, prev, pt) <= tolerance) return true;
                prev = pt;
            }
            return false;
        }

        public override RectangleF GetBounds()
        {
            float minX = Math.Min(Start.X, Math.Min(End.X, MidPoint.X));
            float minY = Math.Min(Start.Y, Math.Min(End.Y, MidPoint.Y));
            float maxX = Math.Max(Start.X, Math.Max(End.X, MidPoint.X));
            float maxY = Math.Max(Start.Y, Math.Max(End.Y, MidPoint.Y));
            return new RectangleF(minX, minY, Math.Max(maxX - minX, 0.01f), Math.Max(maxY - minY, 0.01f));
        }

        public override string ToTikZ()
        {
            if (!ComputeCircle(Start, MidPoint, End, out var center, out float radius))
                return $"\\draw {TikZCoord(Start)} -- {TikZCoord(End)};";

            string opts = GetTikZStyleOptions();
            string optStr = opts.Length > 0 ? $"[{opts}]" : "";

            // tikz angles are in degrees, measured from positive X axis, counterclockwise
            float startAngleDeg = MathF.Atan2(Start.Y - center.Y, Start.X - center.X) * (180f / MathF.PI);
            float midAngleRad = MathF.Atan2(MidPoint.Y - center.Y, MidPoint.X - center.X);
            float endAngleDeg = MathF.Atan2(End.Y - center.Y, End.X - center.X) * (180f / MathF.PI);

            float startRad = startAngleDeg * MathF.PI / 180f;
            float endRad = endAngleDeg * MathF.PI / 180f;
            float sweep = ComputeSweep(startRad, midAngleRad, endRad);

            float sweepDeg = sweep * (180f / MathF.PI);
            float tikzEndAngle = startAngleDeg + sweepDeg;

            return $"\\draw{optStr} {TikZCoord(Start)} arc ({startAngleDeg.ToString("F1", CultureInfo.InvariantCulture)}:{tikzEndAngle.ToString("F1", CultureInfo.InvariantCulture)}:{radius.ToString("F2", CultureInfo.InvariantCulture)});"; 
        }

        public override Shape Clone() => new ArcShape
        {
            Start = Start, End = End, MidPoint = MidPoint,
            StrokeColor = StrokeColor, StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle, FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new() { Start, MidPoint, End };

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            switch (handleIndex)
            {
                case 0: Start = newWorldPos; break;
                case 1: MidPoint = newWorldPos; break;
                case 2: End = newWorldPos; break;
            }
        }

        public override void Translate(float dx, float dy)
        {
            Start = new PointF(Start.X + dx, Start.Y + dy);
            End = new PointF(End.X + dx, End.Y + dy);
            MidPoint = new PointF(MidPoint.X + dx, MidPoint.Y + dy);
        }


        private GraphicsPath? BuildScreenPath(CoordinateTransform transform)
        {
            if (!ComputeCircle(Start, MidPoint, End, out var center, out float radius))
                return null;

            float startAngle = MathF.Atan2(Start.Y - center.Y, Start.X - center.X);
            float midAngle = MathF.Atan2(MidPoint.Y - center.Y, MidPoint.X - center.X);
            float endAngle = MathF.Atan2(End.Y - center.Y, End.X - center.X);
            float sweep = ComputeSweep(startAngle, midAngle, endAngle);

            // approximate the arc with line segments in screen space
            var path = new GraphicsPath();
            const int steps = 60;
            var pts = new PointF[steps + 1];
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                float angle = startAngle + sweep * t;
                var worldPt = new PointF(center.X + radius * MathF.Cos(angle), center.Y + radius * MathF.Sin(angle));
                pts[i] = transform.WorldToScreen(worldPt);
            }
            for (int i = 0; i < steps; i++)
                path.AddLine(pts[i], pts[i + 1]);

            return path;
        }


        public static bool ComputeCircle(PointF p1, PointF p2, PointF p3, out PointF center, out float radius)
        {
            center = PointF.Empty;
            radius = 0;

            float ax = p1.X, ay = p1.Y;
            float bx = p2.X, by = p2.Y;
            float cx = p3.X, cy = p3.Y;

            float d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
            if (MathF.Abs(d) < 1e-10f) return false; // Collinear

            float ux = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            float uy = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;

            center = new PointF(ux, uy);
            radius = MathF.Sqrt((ax - ux) * (ax - ux) + (ay - uy) * (ay - uy));
            return true;
        }

        private static float ComputeSweep(float startAngle, float midAngle, float endAngle)
        {
            // normalize angles relative to start
            float mid = NormalizeAngle(midAngle - startAngle);
            float end = NormalizeAngle(endAngle - startAngle);

            // if going counterclockwise (positive) from start hits mid before end, use positive sweep
            if (mid < end)
                return end; // ccw sweep that passes through mid
            else
                return end - 2 * MathF.PI; // cw sweep (negative) that passes through mid
        }

        private static float NormalizeAngle(float a)
        {
            a %= (2 * MathF.PI);
            if (a < 0) a += 2 * MathF.PI;
            return a;
        }
    }
}
