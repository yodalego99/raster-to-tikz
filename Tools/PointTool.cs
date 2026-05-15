using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class PointTool : DrawingTool
    {
        public override string DisplayName => "Pont";

        public PointTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var worldPt = GetWorldPoint(e);
            var point = new PointShape { Position = worldPt };
            Document.AddShape(point);
            Canvas.Invalidate();
        }
    }
}
