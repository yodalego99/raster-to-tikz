using System.Drawing;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class PolygonShape : Shape
    {
        public List<PointF> Points { get; set; } = new();
        public bool IsClosed { get; set; } = true;

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible || Points.Count < 2) return;

            var screenPts = Points.Select(p => transform.WorldToScreen(p)).ToArray();
            using var pen = CreatePen(transform);

            if (IsClosed && Points.Count >= 3)
            {
                if (FillColor.HasValue)
                {
                    using var brush = new SolidBrush(FillColor.Value);
                    g.FillPolygon(brush, screenPts);
                }
                g.DrawPolygon(pen, screenPts);
            }
            else
            {
                g.DrawLines(pen, screenPts);
            }

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            if (Points.Count < 2) return false;

            int count = IsClosed && Points.Count >= 3 ? Points.Count : Points.Count - 1;
            for (int i = 0; i < count; i++)
            {
                var a = Points[i];
                var b = Points[(i + 1) % Points.Count];
                if (DistanceToSegment(worldPoint, a, b) <= tolerance) return true;
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

            var coords = string.Join(" -- ", Points.Select(TikZCoord));
            string closing = IsClosed && Points.Count >= 3 ? " -- cycle" : "";
            return $"\\draw{optStr} {coords}{closing};";
        }

        public override Shape Clone() => new PolygonShape
        {
            Points = new List<PointF>(Points),
            IsClosed = IsClosed,
            StrokeColor = StrokeColor,
            StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle,
            FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new(Points);

        public override void MoveHandle(int index, PointF newWorldPos)
        {
            if (index >= 0 && index < Points.Count)
                Points[index] = newWorldPos;
        }

        public override void Translate(float dx, float dy)
        {
            for (int i = 0; i < Points.Count; i++)
                Points[i] = new PointF(Points[i].X + dx, Points[i].Y + dy);
        }
    }
}
