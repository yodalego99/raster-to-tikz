using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using rastertotikz.Models;
using rastertotikz.Tools;

namespace rastertotikz.Rendering
{
    public class DrawingCanvas : UserControl
    {
        public Document Document { get; set; }
        public CoordinateTransform Transform { get; }
        public DrawingTool? ActiveTool { get; set; }

        public bool IsPanMode { get; set; }

        public bool SnapEnabled { get; set; } = true;

        public DrawingCanvas()
        {
            // eliminate flicker
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint
                   | ControlStyles.OptimizedDoubleBuffer, true);
            ResizeRedraw = true;

            Document = new Document();
            Transform = new CoordinateTransform();

            BackColor = Color.White;
        }

        // focus handling

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_ACTIVATE = 1;

        protected override void WndProc(ref Message m)
        {
            // When the user clicks this control to bring the window back
            // from another app, let the click pass through to the tool
            // instead of being consumed just to activate the window.
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_ACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        // rendering

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Transform.CanvasHeightPx = Height;

            DrawBackgroundImage(g);

            if (Document.GridSpacing > 0)
                DrawGrid(g);

            foreach (var shape in Document.Shapes)
            {
                shape.Draw(g, Transform);
            }

            ActiveTool?.OnPaint(g);
        }

        private void DrawBackgroundImage(Graphics g)
        {
            if (Document.BackgroundImage == null) return;

            var img = Document.BackgroundImage;
            var srcRect = Document.SourceRect;

            var screenTopLeft = Transform.WorldToScreen(new PointF(0, Document.WorldHeight));
            var screenBottomRight = Transform.WorldToScreen(new PointF(Document.WorldWidth, 0));

            var destRect = new RectangleF(
                screenTopLeft.X, screenTopLeft.Y,
                screenBottomRight.X - screenTopLeft.X,
                screenBottomRight.Y - screenTopLeft.Y);

            // apply opacity
            var cm = new ColorMatrix { Matrix33 = Document.BackgroundOpacity };
            using var attrs = new ImageAttributes();
            attrs.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            g.DrawImage(img,
                Rectangle.Round(new RectangleF(destRect.X, destRect.Y, destRect.Width, destRect.Height)),
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                GraphicsUnit.Pixel, attrs);
        }

        private void DrawGrid(Graphics g)
        {
            float spacing = Document.GridSpacing;
            using var pen = new Pen(Color.FromArgb(60, 180, 180, 180), 1f);

            // determine visible world-space bounds
            var topLeft = Transform.ScreenToWorld(new PointF(0, 0));
            var bottomRight = Transform.ScreenToWorld(new PointF(Width, Height));

            float minX = MathF.Floor(Math.Min(topLeft.X, bottomRight.X) / spacing) * spacing;
            float maxX = MathF.Ceiling(Math.Max(topLeft.X, bottomRight.X) / spacing) * spacing;
            float minY = MathF.Floor(Math.Min(topLeft.Y, bottomRight.Y) / spacing) * spacing;
            float maxY = MathF.Ceiling(Math.Max(topLeft.Y, bottomRight.Y) / spacing) * spacing;

            // vertical lines
            for (float x = minX; x <= maxX; x += spacing)
            {
                var s1 = Transform.WorldToScreen(new PointF(x, minY));
                var s2 = Transform.WorldToScreen(new PointF(x, maxY));
                g.DrawLine(pen, s1, s2);
            }

            // horizontal lines
            for (float y = minY; y <= maxY; y += spacing)
            {
                var s1 = Transform.WorldToScreen(new PointF(minX, y));
                var s2 = Transform.WorldToScreen(new PointF(maxX, y));
                g.DrawLine(pen, s1, s2);
            }
        }

        // mouse events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Middle || (IsPanMode && e.Button == MouseButtons.Left))
            {
                _isPanning = true;
                _panStart = e.Location;
                _panOriginal = Transform.Pan;
                Cursor = Cursors.SizeAll;
                return;
            }
            ActiveTool?.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isPanning)
            {
                float dx = (e.X - _panStart.X) / Transform.PixelsPerUnit;
                float dy = -(e.Y - _panStart.Y) / Transform.PixelsPerUnit;
                Transform.Pan = new PointF(_panOriginal.X - dx, _panOriginal.Y - dy);
                ClampPanToBounds();
                Invalidate();
                return;
            }

            ActiveTool?.OnMouseMove(e);

            // update status bar 
            var worldPt = Transform.ScreenToWorld(e.Location);
            CursorWorldPositionChanged?.Invoke(this, worldPt);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isPanning && (e.Button == MouseButtons.Middle || (IsPanMode && e.Button == MouseButtons.Left)))
            {
                _isPanning = false;
                Cursor = IsPanMode ? Cursors.Hand : Cursors.Default;
                return;
            }
            ActiveTool?.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (IsPanMode)
            {
                // cursor zoom in/out
                float factor = e.Delta > 0 ? 1.15f : 1f / 1.15f;
                var worldBefore = Transform.ScreenToWorld(e.Location);
                Transform.PixelsPerUnit = Math.Clamp(Transform.PixelsPerUnit * factor, 10f, 2000f);
                // adjust pan so the world point under the cursor stays fixed
                var screenAfter = Transform.WorldToScreen(worldBefore);
                float dxPx = e.Location.X - screenAfter.X;
                float dyPx = e.Location.Y - screenAfter.Y;
                Transform.Pan = new PointF(
                    Transform.Pan.X - dxPx / Transform.PixelsPerUnit,
                    Transform.Pan.Y + dyPx / Transform.PixelsPerUnit);
                ClampPanToBounds();
                Invalidate();
                return;
            }

            // scroll vertically (shift+wheel = horizontal)
            float scrollAmount = Transform.ScreenToWorldDistance(40f);
            if ((Control.ModifierKeys & Keys.Shift) != 0)
            {
                float dx = e.Delta > 0 ? -scrollAmount : scrollAmount;
                Transform.Pan = new PointF(Transform.Pan.X + dx, Transform.Pan.Y);
            }
            else
            {
                float dy = e.Delta > 0 ? scrollAmount : -scrollAmount;
                Transform.Pan = new PointF(Transform.Pan.X, Transform.Pan.Y + dy);
            }

            ClampPanToBounds();
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ActiveTool?.OnKeyDown(e);
        }

        // pan state
        private bool _isPanning;
        private Point _panStart;
        private PointF _panOriginal;

        // ui events

        public event EventHandler<PointF>? CursorWorldPositionChanged;

        public event EventHandler? SelectionChanged;
        public void NotifySelectionChanged() => SelectionChanged?.Invoke(this, EventArgs.Empty);

        private void ClampPanToBounds()
        {
            float marginFrac = 0.10f;
            float ww = Document.WorldWidth;
            float wh = Document.WorldHeight;
            float marginX = ww * marginFrac;
            float marginY = wh * marginFrac;

            // visible world rect size at current zoom
            float visW = Transform.ScreenToWorldDistance(Width);
            float visH = Transform.ScreenToWorldDistance(Height);

            // allowed pan range: the view must overlap the image+margin area
            float minPanX = -marginX;
            float maxPanX = (ww + marginX) - visW;
            float minPanY = -marginY;
            float maxPanY = (wh + marginY) - visH;

            // if the visible area is larger than the image+margins, center it
            if (maxPanX < minPanX) { float mid = (minPanX + maxPanX) / 2f; minPanX = maxPanX = mid; }
            if (maxPanY < minPanY) { float mid = (minPanY + maxPanY) / 2f; minPanY = maxPanY = mid; }

            Transform.Pan = new PointF(
                Math.Clamp(Transform.Pan.X, minPanX, maxPanX),
                Math.Clamp(Transform.Pan.Y, minPanY, maxPanY));
        }

        public PointF SnapToGrid(PointF worldPoint)
        {
            if (!SnapEnabled || Document.GridSpacing <= 0)
                return worldPoint;

            float s = Document.GridSpacing;
            return new PointF(
                MathF.Round(worldPoint.X / s) * s,
                MathF.Round(worldPoint.Y / s) * s);
        }
    }
}
