using System.Drawing;

namespace rastertotikz.Models
{
    public class Document
    {
        private const float DefaultImagePixelsPerCm = 50f;

        public List<Shape> Shapes { get; } = new();

        public Bitmap? BackgroundImage { get; set; }

        public float BackgroundOpacity { get; set; } = 0.4f;

        public float WorldWidth => SourceRect.Width / DefaultImagePixelsPerCm;

        public float WorldHeight => SourceRect.Height / DefaultImagePixelsPerCm;

        public Rectangle SourceRect =>
            BackgroundImage != null
            ? new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height)
                : new Rectangle(0, 0, 800, 600);

        public float GridSpacing { get; set; } = 0.5f;

        // defaults for new shapes

        public Color DefaultStrokeColor { get; set; } = Color.Black;
        public float DefaultStrokeWidth { get; set; } = 1.5f;
        public StrokeStyle DefaultStrokeStyle { get; set; } = StrokeStyle.Solid;
        public Color? DefaultFillColor { get; set; } = null;
        public bool DefaultArrowAtStart { get; set; } = false;
        public bool DefaultArrowAtEnd { get; set; } = false;

        public void AddShape(Shape shape)
        {
            shape.StrokeColor = DefaultStrokeColor;
            shape.StrokeWidth = DefaultStrokeWidth;
            shape.StrokeStyle = DefaultStrokeStyle;
            shape.FillColor = DefaultFillColor;
            if (shape is LineSegment line)
            {
                line.ArrowAtStart = DefaultArrowAtStart;
                line.ArrowAtEnd   = DefaultArrowAtEnd;
            }
            Shapes.Add(shape);
        }
    }
}
