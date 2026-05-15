using System.Drawing;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class PointShape : Shape
    {
        public PointF Position { get; set; }

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible) return;

            var screenPt = transform.WorldToScreen(Position);
            float radius = 3f; // screen pixels

            using var brush = new SolidBrush(StrokeColor);
            g.FillEllipse(brush, screenPt.X - radius, screenPt.Y - radius, radius * 2, radius * 2);

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            float dx = worldPoint.X - Position.X;
            float dy = worldPoint.Y - Position.Y;
            return (dx * dx + dy * dy) <= tolerance * tolerance;
        }

        public override RectangleF GetBounds()
        {
            return new RectangleF(Position.X - 0.05f, Position.Y - 0.05f, 0.1f, 0.1f);
        }

        public override string ToTikZ()
        {
            string opts = GetTikZStyleOptions();
            string optStr = opts.Length > 0 ? $"[{opts}]" : "";
            return $"\\filldraw{optStr} {TikZCoord(Position)} circle (1.5pt);";
        }

        public override Shape Clone() => new PointShape
        {
            Position = Position,
            StrokeColor = StrokeColor, StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle, FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new() { Position };

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            Position = newWorldPos;
        }

        public override void Translate(float dx, float dy)
        {
            Position = new PointF(Position.X + dx, Position.Y + dy);
        }
    }
}
