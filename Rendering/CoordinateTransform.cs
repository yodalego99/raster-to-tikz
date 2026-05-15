using System.Drawing;

namespace rastertotikz.Rendering
{
    public class CoordinateTransform
    {
        public float PixelsPerUnit { get; set; } = 50f;

        public PointF Pan { get; set; } = PointF.Empty;

        public float CanvasHeightPx { get; set; }

        public PointF ScreenToWorld(PointF screen)
        {
            float wx = (screen.X / PixelsPerUnit) + Pan.X;
            float wy = ((CanvasHeightPx - screen.Y) / PixelsPerUnit) + Pan.Y;
            return new PointF(wx, wy);
        }

        public PointF WorldToScreen(PointF world)
        {
            float sx = (world.X - Pan.X) * PixelsPerUnit;
            float sy = CanvasHeightPx - (world.Y - Pan.Y) * PixelsPerUnit;
            return new PointF(sx, sy);
        }

        public float WorldToScreenDistance(float worldDist)
        {
            return worldDist * PixelsPerUnit;
        }

        public float ScreenToWorldDistance(float screenDist)
        {
            return screenDist / PixelsPerUnit;
        }
    }
}
