using System.Drawing;
using System.Windows.Forms;
using rastertotikz.Models;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{

    public class TextTool : DrawingTool
    {
        public override string DisplayName => "Szöveg";

        public string FontFamily { get; set; } = "Times New Roman";
        public float FontSize { get; set; } = 12f;
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        private TextBox? _editBox;
        private PointF _worldPosition;

        public TextTool(DrawingCanvas canvas) : base(canvas) { }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            // if there's already an edit box, commit the current text first
            if (_editBox != null)
            {
                CommitText();
                return;
            }

            _worldPosition = GetWorldPoint(e);

            var screenPt = Transform.WorldToScreen(_worldPosition);

            _editBox = new TextBox
            {
                Font = BuildFont(),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 255, 255),
                Location = new Point((int)screenPt.X, (int)(screenPt.Y - 12)),
                Width = 200,
                Parent = Canvas,
                ShortcutsEnabled = true
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Beillesztés", null, (_, _) => PasteIntoEditBox());
            _editBox.ContextMenuStrip = menu;

            _editBox.KeyDown += EditBox_KeyDown;
            _editBox.LostFocus += EditBox_LostFocus;

            Canvas.Controls.Add(_editBox);
            _editBox.BringToFront();
            _editBox.Focus();
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _editBox != null)
            {
                CancelEdit();
                e.Handled = true;
            }
        }

        public override void Deactivate()
        {
            if (_editBox != null)
                CommitText();
        }

        private void EditBox_LostFocus(object? sender, EventArgs e)
        {
            Canvas.BeginInvoke(() =>
            {
                if (_editBox != null && !_editBox.Focused)
                    CommitText();
            });
        }

        private void EditBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.V)
                || (e.Shift && e.KeyCode == Keys.Insert))
            {
                e.SuppressKeyPress = true;
                PasteIntoEditBox();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CommitText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                CancelEdit();
            }
        }

        private void CommitText()
        {
            if (_editBox == null) return;

            string text = _editBox.Text.Trim();
            RemoveEditBox();

            if (!string.IsNullOrEmpty(text))
            {
                var shape = new TextShape
                {
                    Position = _worldPosition,
                    Text = text,
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    Bold = Bold,
                    Italic = Italic
                };
                Document.AddShape(shape);
            }

            Canvas.Invalidate();
        }

        private void CancelEdit()
        {
            RemoveEditBox();
            Canvas.Invalidate();
        }

        private void RemoveEditBox()
        {
            if (_editBox == null) return;
            var box = _editBox;
            _editBox = null;
            box.KeyDown -= EditBox_KeyDown;
            box.LostFocus -= EditBox_LostFocus;
            box.ContextMenuStrip?.Dispose();
            Canvas.Controls.Remove(box);
            box.Dispose();
            Canvas.Focus();
        }

        private void PasteIntoEditBox()
        {
            if (_editBox == null) return;
            if (!Clipboard.ContainsText()) return;

            string text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text)) return;

            int start = _editBox.SelectionStart;
            int length = _editBox.SelectionLength;
            string current = _editBox.Text;
            _editBox.Text = current.Remove(start, length).Insert(start, text);
            _editBox.SelectionStart = start + text.Length;
            _editBox.SelectionLength = 0;
        }

        private Font BuildFont()
        {
            var style = FontStyle.Regular;
            if (Bold) style |= FontStyle.Bold;
            if (Italic) style |= FontStyle.Italic;
            try
            {
                return new Font(FontFamily, Math.Max(FontSize, 8f), style, GraphicsUnit.Point);
            }
            catch
            {
                return new Font("Microsoft Sans Serif", Math.Max(FontSize, 8f), style, GraphicsUnit.Point);
            }
        }
    }
}
