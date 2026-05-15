using System.Drawing;
using System.Drawing.Drawing2D;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class LineSegment : Shape
    {
        public PointF Start { get; set; }
        public PointF End { get; set; }
        public bool ArrowAtStart { get; set; }
        public bool ArrowAtEnd { get; set; }

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible) return;

            var s = transform.WorldToScreen(Start);
            var e = transform.WorldToScreen(End);

            using var pen = CreatePen(transform);

            AdjustableArrowCap? startCap = null;
            AdjustableArrowCap? endCap = null;
            try
            {
                if (ArrowAtStart) { startCap = new AdjustableArrowCap(5f, 5f); pen.CustomStartCap = startCap; }
                if (ArrowAtEnd)   { endCap   = new AdjustableArrowCap(5f, 5f); pen.CustomEndCap   = endCap;   }
                g.DrawLine(pen, s, e);
            }
            finally
            {
                startCap?.Dispose();
                endCap?.Dispose();
            }

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            return DistanceToSegment(worldPoint, Start, End) <= tolerance;
        }

        public override RectangleF GetBounds()
        {
            float x = Math.Min(Start.X, End.X);
            float y = Math.Min(Start.Y, End.Y);
            float w = Math.Abs(End.X - Start.X);
            float h = Math.Abs(End.Y - Start.Y);
            return new RectangleF(x, y, Math.Max(w, 0.01f), Math.Max(h, 0.01f));
        }

        public override string ToTikZ()
        {
            var opts = new List<string>();

            // arrow spec must come first in TikZ options
            if (ArrowAtStart && ArrowAtEnd) opts.Add("<->");
            else if (ArrowAtStart)          opts.Add("<-");
            else if (ArrowAtEnd)            opts.Add("->");

            string styleOpts = GetTikZStyleOptions();
            if (styleOpts.Length > 0) opts.Add(styleOpts);

            string optStr = opts.Count > 0 ? $"[{string.Join(", ", opts)}]" : "";
            return $"\\draw{optStr} {TikZCoord(Start)} -- {TikZCoord(End)};";
        }

        public override Shape Clone() => new LineSegment
        {
            Start = Start, End = End,
            ArrowAtStart = ArrowAtStart, ArrowAtEnd = ArrowAtEnd,
            StrokeColor = StrokeColor, StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle, FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new() { Start, End };

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            if (handleIndex == 0) Start = newWorldPos;
            else End = newWorldPos;
        }

        public override void Translate(float dx, float dy)
        {
            Start = new PointF(Start.X + dx, Start.Y + dy);
            End = new PointF(End.X + dx, End.Y + dy);
        }
    }
}
