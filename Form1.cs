using rastertotikz.Models;
using rastertotikz.Rendering;
using rastertotikz.Tools;
using System.Diagnostics;

namespace rastertotikz
{
    public partial class Form1 : Form
    {
        private DrawingCanvas _canvas = null!;
        private Document _document = null!;

        // status bar labels
        private ToolStripStatusLabel _lblPosition = null!;
        private ToolStripStatusLabel _lblTool = null!;
        private ToolStripStatusLabel _lblZoom = null!;

        // tools
        private SelectTool _selectTool = null!;
        private PointTool _pointTool = null!;
        private LineTool _lineTool = null!;
        private CircleTool _circleTool = null!;
        private CurveTool _curveTool = null!;
        private ArcTool _arcTool = null!;
        private TextTool _textTool = null!;
        private PolygonTool _polygonTool = null!;
        private FreehandTool _freehandTool = null!;
        private PanTool _panTool = null!;

        // guard: true while UpdatePropertyPanel is setting control values, to suppress change events
        private bool _updatingPanel;

        // original row heights for PropertiesTable
        private float[] _propertiesRowHeights = Array.Empty<float>();

        // colors for dropdowns
        private static readonly (string Name, Color Color)[] _namedColors =
        {
            ("Fekete",  Color.Black),
            ("Fehér",   Color.White),
            ("Piros",   Color.Red),
            ("Kék",     Color.Blue),
            ("Zöld",    Color.Green),
            ("Sárga",   Color.Yellow),
            ("Szürke",  Color.Gray),
            ("Narancs", Color.Orange),
            ("Lila",    Color.Purple),
            ("Barna",   Color.Brown),
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _document = new Document();
            _canvas = new DrawingCanvas
            {
                Document = _document,
                Dock = DockStyle.Fill
            };
            Controls.Add(_canvas);
            //_canvas.BringToFront();
            PropertiesTable.BringToFront();

            _selectTool = new SelectTool(_canvas);
            _pointTool = new PointTool(_canvas);
            _lineTool = new LineTool(_canvas);
            _circleTool = new CircleTool(_canvas);
            _curveTool = new CurveTool(_canvas);
            _arcTool = new ArcTool(_canvas);
            _textTool = new TextTool(_canvas);
            _polygonTool = new PolygonTool(_canvas);
            _freehandTool = new FreehandTool(_canvas);
            _panTool = new PanTool(_canvas);

            InitializePropertiesTableRows();

            // default - pan
            SetActiveTool(_panTool, B_Pan);

            B_Select.Click += (s, ev) => SetActiveTool(_selectTool, B_Select);
            B_Point.Click += (s, ev) => SetActiveTool(_pointTool, B_Point);
            B_Line.Click += (s, ev) => SetActiveTool(_lineTool, B_Line);
            B_Circle.Click += (s, ev) => SetActiveTool(_circleTool, B_Circle);
            B_Arc.Click += (s, ev) => SetActiveTool(_arcTool, B_Arc);
            B_Bezier.Click += (s, ev) => SetActiveTool(_curveTool, B_Bezier);
            B_Polygon.Click += (s, ev) => SetActiveTool(_polygonTool, B_Polygon);
            B_Text.Click += (s, ev) => SetActiveTool(_textTool, B_Text);
            B_Freehand.Click += (s, ev) => SetActiveTool(_freehandTool, B_Freehand);
            B_Pan.Click += (s, ev) => SetActiveTool(_panTool, B_Pan);
            B_Import.Click += (s, ev) => PromptForBackgroundImage();

            // status bar
            _lblPosition = new ToolStripStatusLabel("(0.00, 0.00) cm") { AutoSize = false, Width = 160 };
            _lblTool = new ToolStripStatusLabel("Kijelölés") { AutoSize = false, Width = 120 };
            _lblZoom = new ToolStripStatusLabel("100%") { AutoSize = false, Width = 80 };
            statusStrip1.Items.AddRange(new ToolStripItem[] { _lblPosition, _lblTool, _lblZoom });

            // events for statusbar
            _canvas.CursorWorldPositionChanged += (s, pt) =>
            {
                _lblPosition.Text = $"({pt.X:F2}, {pt.Y:F2}) cm";
            };

            ResolutionBar.Minimum = 1;
            ResolutionBar.Maximum = 40;
            // 1→5.0cm, 10→0.5cm, 20→0.25cm, 40→0.125cm
            int defaultResVal = (int)(5.0f / _document.GridSpacing);
            ResolutionBar.Value = Math.Clamp(defaultResVal, ResolutionBar.Minimum, ResolutionBar.Maximum);
            ResolutionBar.TickFrequency = 5;
            ResolutionBar.ValueChanged += (s, ev) =>
            {
                _document.GridSpacing = 5.0f / ResolutionBar.Value;
                _lblZoom.Text = $"Rács: {_document.GridSpacing:F2} cm";
                _canvas.Invalidate();
            };
            _lblZoom.Text = $"Rács: {_document.GridSpacing:F2} cm";

            // opacity slider
            OpacityBar.Minimum = 0;
            OpacityBar.Maximum = 100;
            OpacityBar.Value = (int)(_document.BackgroundOpacity * 100);
            OpacityBar.TickFrequency = 10;
            OpacityBar.ValueChanged += (s, ev) =>
            {
                _document.BackgroundOpacity = OpacityBar.Value / 100f;
                _canvas.Invalidate();
            };

            // Font selector disabled — users set font at document level in LaTeX
            // foreach (var fam in System.Drawing.FontFamily.Families)
            //     CB_Font.Items.Add(fam.Name);
            // CB_Font.Text = _textTool.FontFamily;
            In_TextSize.Minimum = 4;
            In_TextSize.Maximum = 100;
            In_TextSize.Value = (decimal)_textTool.FontSize;

            // CB_Font.SelectedIndexChanged += (s, ev) =>
            // {
            //     if (_updatingPanel) return;
            //     _textTool.FontFamily = CB_Font.Text;
            //     foreach (var ts in _document.Shapes.OfType<TextShape>().Where(t => t.IsSelected))
            //         ts.FontFamily = CB_Font.Text;
            //     _canvas.Invalidate();
            // };
            // CB_Font.TextChanged += (s, ev) =>
            // {
            //     if (_updatingPanel) return;
            //     _textTool.FontFamily = CB_Font.Text;
            //     foreach (var ts in _document.Shapes.OfType<TextShape>().Where(t => t.IsSelected))
            //         ts.FontFamily = CB_Font.Text;
            //     _canvas.Invalidate();
            // };
            In_TextSize.ValueChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                _textTool.FontSize = (float)In_TextSize.Value;
                foreach (var ts in _document.Shapes.OfType<TextShape>().Where(t => t.IsSelected))
                    ts.FontSize = (float)In_TextSize.Value;
                _canvas.Invalidate();
            };
            C_Bold.CheckedChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                _textTool.Bold = C_Bold.Checked;
                foreach (var ts in _document.Shapes.OfType<TextShape>().Where(t => t.IsSelected))
                    ts.Bold = C_Bold.Checked;
                _canvas.Invalidate();
            };
            C_Italic.CheckedChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                _textTool.Italic = C_Italic.Checked;
                foreach (var ts in _document.Shapes.OfType<TextShape>().Where(t => t.IsSelected))
                    ts.Italic = C_Italic.Checked;
                _canvas.Invalidate();
            };

            SetupPropertyPanel();

            _canvas.SelectionChanged += (s, ev) =>
            {
                UpdatePropertyPanel();
                UpdatePropertyControlVisibility();
                UpdateArrowControls();
            };

            RB_ArrowLeft.AutoCheck = false;
            RB_ArrowRight.AutoCheck = false;

            RB_ArrowLeft.Click += (s, ev) =>
            {
                RB_ArrowLeft.Checked = !RB_ArrowLeft.Checked;
                _document.DefaultArrowAtStart = RB_ArrowLeft.Checked;
                foreach (var shape in _document.Shapes.OfType<LineSegment>().Where(l => l.IsSelected))
                    shape.ArrowAtStart = RB_ArrowLeft.Checked;
                _canvas.Invalidate();
            };

            RB_ArrowRight.Click += (s, ev) =>
            {
                RB_ArrowRight.Checked = !RB_ArrowRight.Checked;
                _document.DefaultArrowAtEnd = RB_ArrowRight.Checked;
                foreach (var shape in _document.Shapes.OfType<LineSegment>().Where(l => l.IsSelected))
                    shape.ArrowAtEnd = RB_ArrowRight.Checked;
                _canvas.Invalidate();
            };

            C_SnapOff.CheckedChanged += (s, ev) =>
            {
                _canvas.SnapEnabled = !C_SnapOff.Checked;
            };

            _canvas.TabStop = true;
            _canvas.Click += (s, ev) => _canvas.Focus();

            UpdatePropertyControlVisibility();
        }

        private void SetActiveTool(DrawingTool tool, ToolStripButton button)
        {
            _canvas.ActiveTool?.Deactivate();

            _canvas.ActiveTool = tool;
            tool.Activate();

            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item is ToolStripButton btn)
                    btn.Checked = (btn == button);
            }

            // update status bar
            if (_lblTool != null)
                _lblTool.Text = tool.DisplayName;

            UpdatePropertyControlVisibility();
            UpdateArrowControls();
        }


        private void UpdatePropertyControlVisibility()
        {
            var selectedShapes = _document.Shapes.Where(s => s.IsSelected).ToList();
            bool hasSelection = selectedShapes.Count > 0;

            bool hasText = selectedShapes.OfType<TextShape>().Any();
            bool hasNonText = selectedShapes.Any(s => s is not TextShape);
            bool hasFillable = selectedShapes.Any(s => s is CircleShape || s is PolygonShape);

            bool textToolActive = _canvas.ActiveTool == _textTool;
            bool fillToolActive = _canvas.ActiveTool == _circleTool || _canvas.ActiveTool == _polygonTool;
            bool drawingToolActive = _canvas.ActiveTool != _selectTool && _canvas.ActiveTool != _panTool;

            bool showTextControls = hasText || textToolActive;
            bool showStrokeColor = hasSelection || drawingToolActive;
            bool showStrokeWidthAndStyle = hasNonText || (drawingToolActive && !textToolActive);
            bool showFillControls = hasFillable || fillToolActive;

            SetPropertiesRowVisible(LineRow, showStrokeWidthAndStyle);
            SetPropertiesRowVisible(ColorRow, showStrokeColor);
            SetPropertiesRowVisible(FillRow, showFillControls);
            SetPropertiesRowVisible(TextRow, showTextControls);
        }


        private void UpdateArrowControls()
        {
            bool lineToolActive = _canvas.ActiveTool == _lineTool;
            var selectedLines = _document.Shapes.OfType<LineSegment>().Where(l => l.IsSelected).ToList();
            bool show = lineToolActive || selectedLines.Count > 0;
            SetPropertiesRowVisible(ArrowRow, show);

            if (!show) return;

            _updatingPanel = true;
            try
            {
                if (selectedLines.Count == 0)
                {
                    // none selected -> show doc default
                    RB_ArrowLeft.Checked = _document.DefaultArrowAtStart;
                    RB_ArrowRight.Checked = _document.DefaultArrowAtEnd;
                }
                else
                {
                    // mixed -> unchecked
                    RB_ArrowLeft.Checked = selectedLines.All(l => l.ArrowAtStart);
                    RB_ArrowRight.Checked = selectedLines.All(l => l.ArrowAtEnd);
                }
            }
            finally
            {
                _updatingPanel = false;
            }
        }

        private void InitializePropertiesTableRows()
        {
            int rowCount = PropertiesTable.RowCount;
            if (rowCount <= 0 || PropertiesTable.RowStyles.Count < rowCount)
                return;

            _propertiesRowHeights = new float[rowCount];

            for (int row = 0; row < rowCount; row++)
            {
                Control? rowControl = PropertiesTable.GetControlFromPosition(0, row);
                float height = rowControl?.Height ?? PropertiesTable.RowStyles[row].Height;
                if (height <= 0f)
                    height = Math.Max(PropertiesTable.RowStyles[row].Height, 1f);

                _propertiesRowHeights[row] = height;
                PropertiesTable.RowStyles[row].SizeType = SizeType.Absolute;
                PropertiesTable.RowStyles[row].Height = height;
            }
        }

        private void SetPropertiesRowVisible(Control rowControl, bool visible)
        {
            if (PropertiesTable.RowCount <= 0 || PropertiesTable.RowStyles.Count == 0)
            {
                rowControl.Visible = visible;
                return;
            }

            int row = PropertiesTable.GetRow(rowControl);
            if (row < 0 || row >= PropertiesTable.RowStyles.Count)
            {
                rowControl.Visible = visible;
                return;
            }

            rowControl.Visible = visible;
            PropertiesTable.RowStyles[row].SizeType = SizeType.Absolute;

            if (_propertiesRowHeights.Length > row)
                PropertiesTable.RowStyles[row].Height = visible ? _propertiesRowHeights[row] : 0f;
        }

        /// file dialog for image input
        private void PromptForBackgroundImage()
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Háttérkép betöltése",
                Filter = "Képfájlok|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff;*.tif;*.webp|Minden fájl|*.*"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _document.BackgroundImage = new Bitmap(dlg.FileName);
                    FitCanvasToImage();
                    _canvas.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nem sikerült betölteni a képet:\n{ex.Message}",
                        "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FitCanvasToImage()
        {
            if (_document.BackgroundImage == null) return;

            // fit the world rect into the visible canvas area
            float canvasW = _canvas.Width;
            float canvasH = _canvas.Height;
            if (canvasW < 1 || canvasH < 1) return;

            float scaleX = canvasW / _document.WorldWidth;
            float scaleY = canvasH / _document.WorldHeight;
            _canvas.Transform.PixelsPerUnit = Math.Min(scaleX, scaleY);
            _canvas.Transform.Pan = PointF.Empty;
        }

        // shape property panel

        private void SetupPropertyPanel()
        {
            // line width: 0.5 – 10, step 0.5
            LineWidthInput.Minimum = 0.5m;
            LineWidthInput.Maximum = 10m;
            LineWidthInput.DecimalPlaces = 1;
            LineWidthInput.Increment = 0.5m;
            LineWidthInput.Value = 1.5m;

            // line style
            LineStyleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            LineStyleComboBox.Items.AddRange(new object[] { "Folytonos", "Szaggatott", "Pontozott" });
            LineStyleComboBox.SelectedIndex = 0;

            // color combos
            LineColorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            FillColorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var (name, _) in _namedColors)
            {
                LineColorComboBox.Items.Add(name);
                FillColorComboBox.Items.Add(name);
            }
            LineColorComboBox.SelectedIndex = 0;
            FillColorComboBox.SelectedIndex = 0;

            // for mixed selections
            C_Fill.ThreeState = true;
            C_Fill.CheckState = CheckState.Unchecked;

            // change events
            LineWidthInput.ValueChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                float w = (float)LineWidthInput.Value;
                _document.DefaultStrokeWidth = w;
                foreach (var shape in _document.Shapes.Where(sh => sh.IsSelected))
                    shape.StrokeWidth = w;
                _canvas.Invalidate();
            };

            LineStyleComboBox.SelectedIndexChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                if (LineStyleComboBox.SelectedIndex < 0) return;
                var style = (StrokeStyle)LineStyleComboBox.SelectedIndex;
                _document.DefaultStrokeStyle = style;
                foreach (var shape in _document.Shapes.Where(sh => sh.IsSelected))
                    shape.StrokeStyle = style;
                _canvas.Invalidate();
            };

            LineColorComboBox.SelectedIndexChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                if (LineColorComboBox.SelectedIndex < 0) return;
                var color = _namedColors[LineColorComboBox.SelectedIndex].Color;
                _document.DefaultStrokeColor = color;
                foreach (var shape in _document.Shapes.Where(sh => sh.IsSelected))
                    shape.StrokeColor = color;
                _canvas.Invalidate();
            };

            FillColorComboBox.SelectedIndexChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                if (FillColorComboBox.SelectedIndex < 0) return;
                var color = _namedColors[FillColorComboBox.SelectedIndex].Color;
                if (_document.DefaultFillColor.HasValue)
                    _document.DefaultFillColor = color;
                foreach (var shape in _document.Shapes.Where(sh => sh.IsSelected && sh.FillColor.HasValue))
                    shape.FillColor = color;
                _canvas.Invalidate();
            };

            C_Fill.CheckStateChanged += (s, ev) =>
            {
                if (_updatingPanel) return;
                if (C_Fill.CheckState == CheckState.Indeterminate) return;
                bool fill = C_Fill.Checked;
                Color fillColor = FillColorComboBox.SelectedIndex >= 0
                    ? _namedColors[FillColorComboBox.SelectedIndex].Color
                    : Color.White;
                _document.DefaultFillColor = fill ? fillColor : null;
                foreach (var shape in _document.Shapes.Where(sh => sh.IsSelected))
                    shape.FillColor = fill ? fillColor : null;
                _canvas.Invalidate();
            };
        }


        /// update property panel with current selection's properties
        private void UpdatePropertyPanel()
        {
            _updatingPanel = true;
            try
            {
                var sel = _document.Shapes.Where(s => s.IsSelected).ToList();

                if (sel.Count == 0)
                {
                    // no selection -> show current defaults
                    LineWidthInput.Value = (decimal)_document.DefaultStrokeWidth;
                    LineStyleComboBox.SelectedIndex = (int)_document.DefaultStrokeStyle;
                    int strokeIdx = Array.FindIndex(_namedColors, c => c.Color.ToArgb() == _document.DefaultStrokeColor.ToArgb());
                    LineColorComboBox.SelectedIndex = strokeIdx;
                    if (_document.DefaultFillColor.HasValue)
                    {
                        C_Fill.CheckState = CheckState.Checked;
                        int fillIdx = Array.FindIndex(_namedColors, c => c.Color.ToArgb() == _document.DefaultFillColor.Value.ToArgb());
                        FillColorComboBox.SelectedIndex = fillIdx;
                    }
                    else
                    {
                        C_Fill.CheckState = CheckState.Unchecked;
                        FillColorComboBox.SelectedIndex = -1;
                    }
                    // Text controls: show tool defaults
                    // CB_Font.Text = _textTool.FontFamily;
                    In_TextSize.Value = (decimal)_textTool.FontSize;
                    C_Bold.Checked = _textTool.Bold;
                    C_Italic.Checked = _textTool.Italic;
                    return;
                }

                // stroke width
                var widths = sel.Select(s => s.StrokeWidth).Distinct().ToList();
                if (widths.Count == 1)
                    LineWidthInput.Value = (decimal)widths[0];
                else
                    LineWidthInput.Text = "";

                // stroke style
                var styles = sel.Select(s => s.StrokeStyle).Distinct().ToList();
                LineStyleComboBox.SelectedIndex = styles.Count == 1 ? (int)styles[0] : -1;

                // stroke color
                var strokeColors = sel.Select(s => s.StrokeColor).Distinct().ToList();
                if (strokeColors.Count == 1)
                {
                    int idx = Array.FindIndex(_namedColors, c => c.Color.ToArgb() == strokeColors[0].ToArgb());
                    LineColorComboBox.SelectedIndex = idx; // -1 if custom color
                }
                else
                {
                    LineColorComboBox.SelectedIndex = -1;
                }

                // text properties
                var selText = sel.OfType<TextShape>().ToList();
                if (selText.Count > 0)
                {
                    // var fonts = selText.Select(t => t.FontFamily).Distinct().ToList();
                    // CB_Font.Text = fonts.Count == 1 ? fonts[0] : "";

                    var sizes = selText.Select(t => t.FontSize).Distinct().ToList();
                    if (sizes.Count == 1)
                        In_TextSize.Value = Math.Clamp((decimal)sizes[0], In_TextSize.Minimum, In_TextSize.Maximum);
                    else
                        In_TextSize.Text = "";

                    var bolds = selText.Select(t => t.Bold).Distinct().ToList();
                    C_Bold.Checked = bolds.Count == 1 && bolds[0];

                    var italics = selText.Select(t => t.Italic).Distinct().ToList();
                    C_Italic.Checked = italics.Count == 1 && italics[0];
                }

                // fill
                var fills = sel.Select(s => s.FillColor).Distinct().ToList();
                bool allNull = fills.All(f => !f.HasValue);
                bool noneNull = fills.All(f => f.HasValue);

                if (allNull)
                {
                    C_Fill.CheckState = CheckState.Unchecked;
                    FillColorComboBox.SelectedIndex = -1;
                }
                else if (noneNull)
                {
                    C_Fill.CheckState = CheckState.Checked;
                    var fillColors = fills.Where(f => f.HasValue).Select(f => f!.Value).Distinct().ToList();
                    if (fillColors.Count == 1)
                    {
                        int idx = Array.FindIndex(_namedColors, c => c.Color.ToArgb() == fillColors[0].ToArgb());
                        FillColorComboBox.SelectedIndex = idx;
                    }
                    else
                    {
                        FillColorComboBox.SelectedIndex = -1;
                    }
                }
                else
                {
                    C_Fill.CheckState = CheckState.Indeterminate;
                    FillColorComboBox.SelectedIndex = -1;
                }
            }
            finally
            {
                _updatingPanel = false;
            }
        }

        private void B_ToClipboard_Click(object sender, EventArgs e)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\\begin{tikzpicture}");
            foreach (var shape in _document.Shapes)
            {
                sb.AppendLine("  " + shape.ToTikZ());
            }
            sb.AppendLine("\\end{tikzpicture}");

            string tikz = sb.ToString();
            Clipboard.SetText(tikz);
            MessageBox.Show("TikZ kimenet a vágólapra másolva.",
                "Exportálás", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void B_ToFile_Click(object sender, EventArgs e)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\\documentclass{standalone}");
            sb.AppendLine("\\usepackage{tikz}");
            sb.AppendLine();
            sb.AppendLine("\\begin{document}");
            sb.AppendLine("\\begin{tikzpicture}");
            foreach (var shape in _document.Shapes)
            {
                sb.AppendLine("  " + shape.ToTikZ());
            }
            sb.AppendLine("\\end{tikzpicture}");
            sb.AppendLine("\\end{document}");

            string latex = sb.ToString();

            using var dlg = new SaveFileDialog();
            dlg.Filter = "PDF fájl (*.pdf)|*.pdf|LaTeX fájl (*.tex)|*.tex|Minden fájl (*.*)|*.*";
            dlg.DefaultExt = "pdf";
            dlg.FileName = "output.pdf";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                if (ext == ".pdf")
                {
                    if (TryExportPdf(latex, dlg.FileName, out string message))
                    {
                        MessageBox.Show(message,
                            "Exportálás", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(message,
                            "Exportálás", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    File.WriteAllText(dlg.FileName, latex);
                    MessageBox.Show("Fájl elmentve: " + dlg.FileName,
                        "Exportálás", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static bool TryExportPdf(string latex, string outputPdfPath, out string message)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "rastertotikz_pdf_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            string texPath = Path.Combine(tempDir, "output.tex");
            string generatedPdfPath = Path.Combine(tempDir, "output.pdf");
            File.WriteAllText(texPath, latex);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "pdflatex",
                    Arguments = "-interaction=nonstopmode -halt-on-error output.tex",
                    WorkingDirectory = tempDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null)
                {
                    message = "A PDF export nem indult el. A .tex fájl elkészült ide: " + texPath;
                    return false;
                }

                string stdOut = process.StandardOutput.ReadToEnd();
                string stdErr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(generatedPdfPath))
                {
                    File.Copy(generatedPdfPath, outputPdfPath, overwrite: true);
                    message = "PDF elmentve: " + outputPdfPath;
                    return true;
                }

                message = "A PDF fordítás nem sikerült. Ellenőrizd, hogy a pdflatex elérhető-e. "
                        + "A köztes .tex fájl itt maradt: " + texPath
                        + "\n\nHiba: " + (string.IsNullOrWhiteSpace(stdErr) ? stdOut : stdErr);
                return false;
            }
            catch (Exception ex)
            {
                message = "A PDF export nem érhető el ezen a gépen (pdflatex hiányozhat). "
                        + "A .tex fájl elkészült ide: " + texPath
                        + "\n\nRészletek: " + ex.Message;
                return false;
            }
        }

        private void B_Help_Click(object sender, EventArgs e)
        {
            string pdfPath = Path.Combine(AppContext.BaseDirectory, "sugo.pdf");
            if (!File.Exists(pdfPath))
            {
                MessageBox.Show("A súgó fájl nem található:\n" + pdfPath,
                    "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
        }
    }
}
