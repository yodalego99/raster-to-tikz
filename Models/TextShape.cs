using System.Drawing;
using rastertotikz.Rendering;

namespace rastertotikz.Models
{
    public class TextShape : Shape
    {
        public PointF Position { get; set; }
        public string Text { get; set; } = "";
        public string FontFamily { get; set; } = "Times New Roman";
        public float FontSize { get; set; } = 12f;
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        public override void Draw(Graphics g, CoordinateTransform transform)
        {
            if (!IsVisible || string.IsNullOrEmpty(Text)) return;

            var screenPt = transform.WorldToScreen(Position);

            // scale font size: FontSize is in pt; scale proportionally with world→screen
            float screenFontSize = transform.WorldToScreenDistance(FontSize / 50f);
            screenFontSize = Math.Max(screenFontSize, 6f); // minimum readable

            var style = FontStyle.Regular;
            if (Bold) style |= FontStyle.Bold;
            if (Italic) style |= FontStyle.Italic;

            using var font = new Font(FontFamily, screenFontSize, style, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(StrokeColor);

            var size = g.MeasureString(Text, font);
            // draw centered on position
            g.DrawString(Text, font, brush, screenPt.X - size.Width / 2, screenPt.Y - size.Height / 2);

            DrawHandles(g, transform);
        }

        public override bool HitTest(PointF worldPoint, float tolerance)
        {
            // approximate bounding box in world space
            float halfW = Text.Length * FontSize / 100f * 0.5f;
            float halfH = FontSize / 100f * 0.5f;
            return worldPoint.X >= Position.X - halfW - tolerance
                && worldPoint.X <= Position.X + halfW + tolerance
                && worldPoint.Y >= Position.Y - halfH - tolerance
                && worldPoint.Y <= Position.Y + halfH + tolerance;
        }

        public override RectangleF GetBounds()
        {
            float halfW = Text.Length * FontSize / 100f * 0.5f;
            float halfH = FontSize / 100f * 0.5f;
            return new RectangleF(Position.X - halfW, Position.Y - halfH, halfW * 2, halfH * 2);
        }

        public override string ToTikZ()
        {
            var parts = new List<string>();

            // build a single font= value with size + style commands
            string sizeCmd = FontSize switch
            {
                <= 6 => "\\tiny",
                <= 8 => "\\scriptsize",
                <= 9 => "\\footnotesize",
                <= 10 => "\\small",
                <= 11 => "\\normalsize",
                <= 14 => "\\large",
                <= 17 => "\\Large",
                <= 20 => "\\LARGE",
                <= 25 => "\\huge",
                _ => "\\Huge"
            };

            var fontParts = new List<string> { sizeCmd };
            if (Bold) fontParts.Add("\\bfseries");
            if (Italic) fontParts.Add("\\itshape");
            parts.Add($"font={string.Join("", fontParts)}");

            // color (skip if black, its the default)
            if (StrokeColor != Color.Black)
            {
                string colorName = StrokeColor.Name switch
                {
                    "Red" => "red",
                    "Blue" => "blue",
                    "Green" => "green",
                    "White" => "white",
                    "Yellow" => "yellow",
                    "Gray" => "gray",
                    _ => $"{{rgb,255:red,{StrokeColor.R};green,{StrokeColor.G};blue,{StrokeColor.B}}}"
                };
                parts.Add($"text={colorName}");
            }

            string opts = string.Join(", ", parts);

            string nodeContent = BuildTikZNodeContent(Text);
            return $"\\node[{opts}] at {TikZCoord(Position)} {{{nodeContent}}};";
        }

        private static string BuildTikZNodeContent(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            // collapse $$ to $ so users can type either form
            text = text.Replace("$$", "$");

            int dollarCount = text.Count(c => c == '$');
            if ((dollarCount % 2) != 0)
                return EscapeLatexText(text);

            var sb = new System.Text.StringBuilder();
            bool inMath = false;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '$')
                {
                    inMath = !inMath;
                    sb.Append('$');
                    continue;
                }

                if (inMath)
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append(ch switch
                    {
                        '\\' => "\\textbackslash{}",
                        '{' => "\\{",
                        '}' => "\\}",
                        '&' => "\\&",
                        '#' => "\\#",
                        '%' => "\\%",
                        '_' => "\\_",
                        '^' => "\\^{}",
                        _ => ch.ToString()
                    });
                }
            }

            return sb.ToString();
        }

        private static string EscapeLatexText(string text)
        {
            return text
                .Replace("\\", "\\textbackslash{}")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace("$", "\\$")
                .Replace("&", "\\&")
                .Replace("#", "\\#")
                .Replace("%", "\\%")
                .Replace("_", "\\_")
                .Replace("^", "\\^{}");
        }

        public override Shape Clone() => new TextShape
        {
            Position = Position,
            Text = Text,
            FontFamily = FontFamily,
            FontSize = FontSize,
            Bold = Bold,
            Italic = Italic,
            StrokeColor = StrokeColor,
            StrokeWidth = StrokeWidth,
            StrokeStyle = StrokeStyle,
            FillColor = FillColor
        };

        public override List<PointF> GetHandles() => new() { Position };

        public override void MoveHandle(int handleIndex, PointF newWorldPos)
        {
            if (handleIndex == 0) Position = newWorldPos;
        }

        public override void Translate(float dx, float dy)
        {
            Position = new PointF(Position.X + dx, Position.Y + dy);
        }
    }
}
