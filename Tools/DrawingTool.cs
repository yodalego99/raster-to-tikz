using System.Drawing;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public abstract class DrawingTool
    {
        protected DrawingCanvas Canvas { get; }
        protected Document Document => Canvas.Document;
        protected CoordinateTransform Transform => Canvas.Transform;

        public abstract string DisplayName { get; }

        protected DrawingTool(DrawingCanvas canvas)
        {
            Canvas = canvas;
        }

        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
        public virtual void OnMouseUp(MouseEventArgs e) { }
        public virtual void OnKeyDown(KeyEventArgs e) { }

        public virtual void OnPaint(Graphics g) { }

        protected PointF GetWorldPoint(MouseEventArgs e)
        {
            var raw = Transform.ScreenToWorld(e.Location);
            return Canvas.SnapToGrid(raw);
        }

        public virtual void Activate() { }

        public virtual void Deactivate() { }
    }
}
