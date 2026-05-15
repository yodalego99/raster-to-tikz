using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public enum StrokeStyle
    {
        Solid,
        Dashed,
        Dotted
    }
    public abstract class Shape
    {
        public Color StrokeColor { get; set; } = Color.Black;
        public float StrokeWidth { get; set; } = 1.5f;
        public StrokeStyle StrokeStyle { get; set; } = StrokeStyle.Solid;
        public Color? FillColor { get; set; } = null;
        public bool IsSelected { get; set; } = false;
        public bool IsVisible { get; set; } = true;

        public abstract void Draw(Graphics g, CoordinateTransform transform);

        public abstract bool HitTest(PointF worldPoint, float tolerance);

        public abstract RectangleF GetBounds();

        public abstract string ToTikZ();

        public abstract Shape Clone();

        public abstract List<PointF> GetHandles();

        public abstract void MoveHandle(int handleIndex, PointF newWorldPos);

        public abstract void Translate(float dx, float dy);

        protected Pen CreatePen(CoordinateTransform transform)
        {
            float screenWidth = transform.WorldToScreenDistance(StrokeWidth / 50f);
            // minimum 1px on screen so it's always visible
            screenWidth = Math.Max(screenWidth, 1f);

            var pen = new Pen(StrokeColor, screenWidth);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);

            switch (StrokeStyle)
            {
                case StrokeStyle.Dashed:
                    pen.DashStyle = DashStyle.Dash;
                    break;
                case StrokeStyle.Dotted:
                    pen.DashStyle = DashStyle.Dot;
                    break;
                default:
                    pen.DashStyle = DashStyle.Solid;
                    break;
            }

            return pen;
        }

        protected string GetTikZStyleOptions()
        {
            var options = new List<string>();

            // color (skip if black, its the default)
            if (StrokeColor != Color.Black)
            {
                options.Add($"color={{{TikZColorName(StrokeColor)}}}");
            }

            // line width
            if (StrokeWidth > 1.0f && StrokeWidth <= 2.0f)
                options.Add("thick");
            else if (StrokeWidth > 2.0f)
                options.Add($"line width={(StrokeWidth / 50f).ToString("F2", CultureInfo.InvariantCulture)}cm");

            // dash style
            if (StrokeStyle == StrokeStyle.Dashed)
                options.Add("dashed");
            else if (StrokeStyle == StrokeStyle.Dotted)
                options.Add("dotted");

            // fill
            if (FillColor.HasValue)
            {
                options.Add($"fill={{{TikZColorName(FillColor.Value)}}}");
            }

            return options.Count > 0 ? string.Join(", ", options) : "";
        }

        private static string TikZColorName(Color c)
        {
            if (c == Color.Black) return "black";
            if (c == Color.White) return "white";
            if (c == Color.Red) return "red";
            if (c == Color.Blue) return "blue";
            if (c == Color.Green) return "green";
            if (c == Color.Yellow) return "yellow";
            if (c == Color.Gray) return "gray";
            // Custom RGB fallback
            return $"{{rgb,255:red,{c.R};green,{c.G};blue,{c.B}}}";
        }

        protected static string TikZCoord(PointF p)
        {
            return $"({p.X.ToString("F2", CultureInfo.InvariantCulture)},{p.Y.ToString("F2", CultureInfo.InvariantCulture)})";
        }

        protected void DrawHandles(Graphics g, CoordinateTransform transform)
        {
            if (!IsSelected) return;

            const float handleSize = 6f;
            foreach (var worldPt in GetHandles())
            {
                var screenPt = transform.WorldToScreen(worldPt);
                g.FillRectangle(Brushes.White,
                    screenPt.X - handleSize / 2, screenPt.Y - handleSize / 2,
                    handleSize, handleSize);
                g.DrawRectangle(Pens.Black,
                    screenPt.X - handleSize / 2, screenPt.Y - handleSize / 2,
                    handleSize, handleSize);
            }
        }

        protected static float DistanceToSegment(PointF p, PointF a, PointF b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float lengthSq = dx * dx + dy * dy;

            if (lengthSq < 1e-10f)
                return MathF.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));

            float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lengthSq;
            t = Math.Clamp(t, 0f, 1f);

            float projX = a.X + t * dx;
            float projY = a.Y + t * dy;
            return MathF.Sqrt((p.X - projX) * (p.X - projX) + (p.Y - projY) * (p.Y - projY));
        }
    }
}
