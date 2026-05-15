using System.Drawing;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class SelectTool : DrawingTool
    {
        public override string DisplayName => "Kijelölés";

        private enum State { Idle, Dragging, DraggingHandle, RubberBand }
        private State _state = State.Idle;

        private Shape? _dragShape;
        private PointF _dragStartWorld;
        private int _dragHandleIndex;

        private PointF _rubberStart; //selection
        private PointF _rubberEnd;

        public SelectTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var worldPt = GetWorldPoint(e);
            var rawPt = Transform.ScreenToWorld(e.Location);
            float tolerance = Transform.ScreenToWorldDistance(8f);

            // first, check if clicking on a handle of an already-selected shape
            // use raw position so handles between grid points can be grabbed
            for (int i = Document.Shapes.Count - 1; i >= 0; i--)
            {
                var s = Document.Shapes[i];
                if (!s.IsSelected) continue;
                var handles = s.GetHandles();
                for (int h = 0; h < handles.Count; h++)
                {
                    float dx = rawPt.X - handles[h].X;
                    float dy = rawPt.Y - handles[h].Y;
                    if (dx * dx + dy * dy <= tolerance * tolerance)
                    {
                        _dragShape = s;
                        _dragHandleIndex = h;
                        _dragStartWorld = worldPt;
                        _state = State.DraggingHandle;
                        Canvas.Invalidate();
                        return;
                    }
                }
            }

            // hit-test shapes in reverse order (top shape first)
            Shape? hit = null;
            for (int i = Document.Shapes.Count - 1; i >= 0; i--)
            {
                if (Document.Shapes[i].HitTest(rawPt, tolerance))
                {
                    hit = Document.Shapes[i];
                    break;
                }
            }

            if (hit != null)
            {
                if ((Control.ModifierKeys & Keys.Control) == 0)
                {
                    foreach (var s in Document.Shapes)
                        s.IsSelected = false;
                }
                hit.IsSelected = true;

                _dragShape = hit;
                _dragStartWorld = worldPt;
                _state = State.Dragging;
            }
            else
            {
                foreach (var s in Document.Shapes)
                    s.IsSelected = false;

                _rubberStart = worldPt;
                _rubberEnd = worldPt;
                _state = State.RubberBand;
            }

            Canvas.NotifySelectionChanged();
            Canvas.Invalidate();
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            var worldPt = GetWorldPoint(e);

            if (_state == State.DraggingHandle && _dragShape != null)
            {
                _dragShape.MoveHandle(_dragHandleIndex, worldPt);
                Canvas.Invalidate();
            }
            else if (_state == State.Dragging && _dragShape != null)
            {
                float dx = worldPt.X - _dragStartWorld.X;
                float dy = worldPt.Y - _dragStartWorld.Y;
                _dragShape.Translate(dx, dy);
                _dragStartWorld = worldPt;
                Canvas.Invalidate();
            }
            else if (_state == State.RubberBand)
            {
                _rubberEnd = worldPt;
                Canvas.Invalidate();
            }
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (_state == State.RubberBand)
            {
                float x1 = Math.Min(_rubberStart.X, _rubberEnd.X);
                float y1 = Math.Min(_rubberStart.Y, _rubberEnd.Y);
                float x2 = Math.Max(_rubberStart.X, _rubberEnd.X);
                float y2 = Math.Max(_rubberStart.Y, _rubberEnd.Y);
                var selRect = new RectangleF(x1, y1, x2 - x1, y2 - y1);

                foreach (var s in Document.Shapes)
                {
                    var b = s.GetBounds();
                    if (selRect.Contains(b) || selRect.IntersectsWith(b))
                        s.IsSelected = true;
                }
            }

            _state = State.Idle;
            _dragShape = null;
            Canvas.NotifySelectionChanged();
            Canvas.Invalidate();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Document.Shapes.RemoveAll(s => s.IsSelected);
                Canvas.NotifySelectionChanged();
                Canvas.Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                foreach (var s in Document.Shapes)
                    s.IsSelected = false;
                Canvas.NotifySelectionChanged();
                Canvas.Invalidate();
                e.Handled = true;
            }
        }

        public override void OnPaint(Graphics g)
        {
            if (_state != State.RubberBand) return;

            var s1 = Transform.WorldToScreen(_rubberStart);
            var s2 = Transform.WorldToScreen(_rubberEnd);

            float x = Math.Min(s1.X, s2.X);
            float y = Math.Min(s1.Y, s2.Y);
            float w = Math.Abs(s2.X - s1.X);
            float h = Math.Abs(s2.Y - s1.Y);

            using var pen = new Pen(Color.FromArgb(100, 0, 120, 215), 1f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            using var brush = new SolidBrush(Color.FromArgb(30, 0, 120, 215));
            g.FillRectangle(brush, x, y, w, h);
            g.DrawRectangle(pen, x, y, w, h);
        }
    }
}
