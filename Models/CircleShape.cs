using System.Drawing;
using System.Globalization;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class CircleShape : Shape
    {
        public PointF Center { get; set; }
        public float Radius { get; set; }

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible) return;

            var screenCenter = transform.WorldToScreen(Center);
            float screenRadius = transform.WorldToScreenDistance(Radius);

            using var pen = CreatePen(transform);
            g.DrawEllipse(pen, screenCenter.X - screenRadius, screenCenter.Y - screenRadius,
                screenRadius * 2, screenRadius * 2);

            if (FillColor.HasValue)
            {
                using var brush = new SolidBrush(FillColor.Value);
                g.FillEllipse(brush, screenCenter.X - screenRadius, screenCenter.Y - screenRadius,
                    screenRadius * 2, screenRadius * 2);
            }

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            float dx = worldPoint.X - Center.X;
            float dy = worldPoint.Y - Center.Y;
            float dist = MathF.Sqrt(dx * dx + dy * dy);
            // hit if close to the circle's outline
            return MathF.Abs(dist - Radius) <= tolerance;
        }

        public override RectangleF GetBounds()
        {
            return new RectangleF(Center.X - Radius, Center.Y - Radius, Radius * 2, Radius * 2);
        }

        public override string ToTikZ()
        {
            string opts = GetTikZStyleOptions();
            string optStr = opts.Length > 0 ? $"[{opts}]" : "";
            return $"\\draw{optStr} {TikZCoord(Center)} circle ({Radius.ToString("F2", CultureInfo.InvariantCulture)});"; 
        }

        public override Shape Clone() => new CircleShape
        {
            Center = Center, Radius = Radius,
            StrokeColor = StrokeColor, StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle, FillColor = FillColor
        };

        public override List<PointF> GetHandles()
        {
            return new()
            {
                new PointF(Center.X + Radius, Center.Y),
                new PointF(Center.X, Center.Y + Radius),
                new PointF(Center.X - Radius, Center.Y),
                new PointF(Center.X, Center.Y - Radius)
            };
        }

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            // adjust radius based on distance from center
            float dx = newWorldPos.X - Center.X;
            float dy = newWorldPos.Y - Center.Y;
            Radius = MathF.Max(MathF.Sqrt(dx * dx + dy * dy), 0.01f);
        }

        public override void Translate(float dx, float dy)
        {
            Center = new PointF(Center.X + dx, Center.Y + dy);
        }
    }
}
