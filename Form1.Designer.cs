using rastertotikz.Rendering;

namespace rastertotikz
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip1 = new ClickThroughToolStrip();
            B_Pan = new ToolStripButton();
            B_Select = new ToolStripButton();
            B_Point = new ToolStripButton();
            B_Line = new ToolStripButton();
            B_Circle = new ToolStripButton();
            B_Arc = new ToolStripButton();
            B_Bezier = new ToolStripButton();
            B_Polygon = new ToolStripButton();
            B_Text = new ToolStripButton();
            B_Freehand = new ToolStripButton();
            B_Help = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            OpacityBar = new TrackBar();
            ResolutionBar = new TrackBar();
            panel1 = new Panel();
            C_SnapOff = new CheckBox();
            label1 = new Label();
            CB_Font = new ComboBox();
            PropertiesTable = new TableLayoutPanel();
            LineRow = new Panel();
            LineStyleComboBox = new ComboBox();
            LineStyleComboBoxLabel = new Label();
            LineWidthInput = new NumericUpDown();
            LineWidthInputLabel = new Label();
            ColorRow = new Panel();
            LineColorComboBox = new ComboBox();
            LineColorComboBoxLabel = new Label();
            FillRow = new Panel();
            FillColorComboBox = new ComboBox();
            C_Fill = new CheckBox();
            ArrowRow = new Panel();
            ArrowLabel = new Label();
            RB_ArrowRight = new RadioButton();
            RB_ArrowLeft = new RadioButton();
            TextRow = new Panel();
            C_Italic = new CheckBox();
            C_Bold = new CheckBox();
            InTextSizeLabel = new Label();
            In_TextSize = new NumericUpDown();
            CBFontLabel = new Label();
            label2 = new Label();
            menuStrip1 = new MenuStrip();
            B_Import = new ToolStripMenuItem();
            B_ToFile = new ToolStripMenuItem();
            B_ToClipboard = new ToolStripMenuItem();
            B_Project = new ToolStripMenuItem();
            B_LoadProject = new ToolStripMenuItem();
            B_SaveProject = new ToolStripMenuItem();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OpacityBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ResolutionBar).BeginInit();
            panel1.SuspendLayout();
            PropertiesTable.SuspendLayout();
            LineRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LineWidthInput).BeginInit();
            ColorRow.SuspendLayout();
            FillRow.SuspendLayout();
            ArrowRow.SuspendLayout();
            TextRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)In_TextSize).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = SystemColors.ControlLight;
            toolStrip1.Dock = DockStyle.Left;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { B_Pan, B_Select, B_Point, B_Line, B_Circle, B_Arc, B_Bezier, B_Polygon, B_Text, B_Freehand, B_Help });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(115, 908);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // B_Pan
            // 
            B_Pan.AutoSize = false;
            B_Pan.BackgroundImage = (Image)resources.GetObject("B_Pan.BackgroundImage");
            B_Pan.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Pan.ImageScaling = ToolStripItemImageScaling.None;
            B_Pan.ImageTransparentColor = Color.Magenta;
            B_Pan.Margin = new Padding(0, 0, 0, 10);
            B_Pan.Name = "B_Pan";
            B_Pan.Size = new Size(64, 64);
            B_Pan.Text = "Mozgatás";
            // 
            // B_Select
            // 
            B_Select.AutoSize = false;
            B_Select.BackgroundImage = (Image)resources.GetObject("B_Select.BackgroundImage");
            B_Select.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Select.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 238);
            B_Select.ImageScaling = ToolStripItemImageScaling.None;
            B_Select.ImageTransparentColor = Color.Magenta;
            B_Select.Margin = new Padding(0, 0, 0, 10);
            B_Select.Name = "B_Select";
            B_Select.Size = new Size(64, 64);
            B_Select.TextImageRelation = TextImageRelation.ImageAboveText;
            // 
            // B_Point
            // 
            B_Point.AutoSize = false;
            B_Point.BackgroundImage = (Image)resources.GetObject("B_Point.BackgroundImage");
            B_Point.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Point.ImageScaling = ToolStripItemImageScaling.None;
            B_Point.ImageTransparentColor = Color.Magenta;
            B_Point.Margin = new Padding(0, 0, 0, 10);
            B_Point.Name = "B_Point";
            B_Point.Size = new Size(64, 64);
            B_Point.Text = "Pont";
            // 
            // B_Line
            // 
            B_Line.AutoSize = false;
            B_Line.BackgroundImage = (Image)resources.GetObject("B_Line.BackgroundImage");
            B_Line.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Line.ImageScaling = ToolStripItemImageScaling.None;
            B_Line.ImageTransparentColor = Color.Magenta;
            B_Line.Margin = new Padding(0, 0, 0, 10);
            B_Line.Name = "B_Line";
            B_Line.Size = new Size(64, 64);
            B_Line.Text = "Szakasz";
            // 
            // B_Circle
            // 
            B_Circle.AutoSize = false;
            B_Circle.BackgroundImage = (Image)resources.GetObject("B_Circle.BackgroundImage");
            B_Circle.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Circle.ImageScaling = ToolStripItemImageScaling.None;
            B_Circle.ImageTransparentColor = Color.Magenta;
            B_Circle.Margin = new Padding(0, 0, 0, 10);
            B_Circle.Name = "B_Circle";
            B_Circle.Size = new Size(64, 64);
            B_Circle.Text = "Kör";
            // 
            // B_Arc
            // 
            B_Arc.AutoSize = false;
            B_Arc.BackgroundImage = (Image)resources.GetObject("B_Arc.BackgroundImage");
            B_Arc.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Arc.ImageScaling = ToolStripItemImageScaling.None;
            B_Arc.ImageTransparentColor = Color.Magenta;
            B_Arc.Margin = new Padding(0, 0, 0, 10);
            B_Arc.Name = "B_Arc";
            B_Arc.Size = new Size(64, 64);
            B_Arc.Text = "Ív";
            // 
            // B_Bezier
            // 
            B_Bezier.AutoSize = false;
            B_Bezier.BackgroundImage = (Image)resources.GetObject("B_Bezier.BackgroundImage");
            B_Bezier.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Bezier.ImageScaling = ToolStripItemImageScaling.None;
            B_Bezier.ImageTransparentColor = Color.Magenta;
            B_Bezier.Margin = new Padding(0, 0, 0, 10);
            B_Bezier.Name = "B_Bezier";
            B_Bezier.Size = new Size(64, 64);
            B_Bezier.Text = "Bézier-görbe";
            // 
            // B_Polygon
            // 
            B_Polygon.AutoSize = false;
            B_Polygon.BackgroundImage = (Image)resources.GetObject("B_Polygon.BackgroundImage");
            B_Polygon.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Polygon.ImageScaling = ToolStripItemImageScaling.None;
            B_Polygon.ImageTransparentColor = Color.Magenta;
            B_Polygon.Margin = new Padding(0, 0, 0, 10);
            B_Polygon.Name = "B_Polygon";
            B_Polygon.Size = new Size(64, 64);
            B_Polygon.Text = "Sokszög";
            // 
            // B_Text
            // 
            B_Text.AutoSize = false;
            B_Text.BackgroundImage = (Image)resources.GetObject("B_Text.BackgroundImage");
            B_Text.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Text.ImageScaling = ToolStripItemImageScaling.None;
            B_Text.ImageTransparentColor = Color.Magenta;
            B_Text.Margin = new Padding(0, 0, 0, 10);
            B_Text.Name = "B_Text";
            B_Text.Size = new Size(64, 64);
            B_Text.Text = "Szöveg";
            // 
            // B_Freehand
            // 
            B_Freehand.AutoSize = false;
            B_Freehand.BackgroundImage = (Image)resources.GetObject("B_Freehand.BackgroundImage");
            B_Freehand.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Freehand.ImageScaling = ToolStripItemImageScaling.None;
            B_Freehand.ImageTransparentColor = Color.Magenta;
            B_Freehand.Margin = new Padding(0, 0, 0, 10);
            B_Freehand.Name = "B_Freehand";
            B_Freehand.Size = new Size(64, 64);
            B_Freehand.Text = "Ceruza";
            // 
            // B_Help
            // 
            B_Help.AutoSize = false;
            B_Help.BackgroundImage = (Image)resources.GetObject("B_Help.BackgroundImage");
            B_Help.DisplayStyle = ToolStripItemDisplayStyle.Image;
            B_Help.ImageTransparentColor = Color.Magenta;
            B_Help.Margin = new Padding(0, 64, 0, 2);
            B_Help.Name = "B_Help";
            B_Help.Size = new Size(64, 64);
            B_Help.Text = "Súgó";
            B_Help.Click += B_Help_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Location = new Point(115, 910);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1092, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // OpacityBar
            // 
            OpacityBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            OpacityBar.BackColor = SystemColors.Control;
            OpacityBar.Location = new Point(32, 679);
            OpacityBar.Name = "OpacityBar";
            OpacityBar.Size = new Size(199, 45);
            OpacityBar.TabIndex = 2;
            // 
            // ResolutionBar
            // 
            ResolutionBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ResolutionBar.BackColor = SystemColors.Control;
            ResolutionBar.Location = new Point(33, 587);
            ResolutionBar.Maximum = 50;
            ResolutionBar.Name = "ResolutionBar";
            ResolutionBar.Size = new Size(198, 45);
            ResolutionBar.TabIndex = 4;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.BackColor = SystemColors.ControlLight;
            panel1.Controls.Add(C_SnapOff);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(CB_Font);
            panel1.Controls.Add(PropertiesTable);
            panel1.Controls.Add(CBFontLabel);
            panel1.Controls.Add(OpacityBar);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(ResolutionBar);
            panel1.Location = new Point(936, 133);
            panel1.Name = "panel1";
            panel1.Size = new Size(271, 769);
            panel1.TabIndex = 5;
            // 
            // C_SnapOff
            // 
            C_SnapOff.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            C_SnapOff.AutoSize = true;
            C_SnapOff.Location = new Point(45, 743);
            C_SnapOff.Name = "C_SnapOff";
            C_SnapOff.Size = new Size(187, 19);
            C_SnapOff.TabIndex = 14;
            C_SnapOff.Text = "Rácshoz igazodás kikapcsolása";
            C_SnapOff.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(69, 560);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 6;
            label1.Text = "Rács felbontása";
            // 
            // CB_Font
            // 
            CB_Font.Enabled = false;
            CB_Font.FormattingEnabled = true;
            CB_Font.Location = new Point(19, 607);
            CB_Font.Name = "CB_Font";
            CB_Font.Size = new Size(121, 23);
            CB_Font.TabIndex = 12;
            CB_Font.Visible = false;
            // 
            // PropertiesTable
            // 
            PropertiesTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PropertiesTable.ColumnCount = 1;
            PropertiesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            PropertiesTable.Controls.Add(LineRow, 0, 0);
            PropertiesTable.Controls.Add(ColorRow, 0, 1);
            PropertiesTable.Controls.Add(FillRow, 0, 2);
            PropertiesTable.Controls.Add(ArrowRow, 0, 3);
            PropertiesTable.Controls.Add(TextRow, 0, 4);
            PropertiesTable.Location = new Point(3, 3);
            PropertiesTable.Name = "PropertiesTable";
            PropertiesTable.RowCount = 5;
            PropertiesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 61.3526573F));
            PropertiesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 38.6473427F));
            PropertiesTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 67F));
            PropertiesTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            PropertiesTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 137F));
            PropertiesTable.Size = new Size(265, 389);
            PropertiesTable.TabIndex = 6;
            // 
            // LineRow
            // 
            LineRow.Controls.Add(LineStyleComboBox);
            LineRow.Controls.Add(LineStyleComboBoxLabel);
            LineRow.Controls.Add(LineWidthInput);
            LineRow.Controls.Add(LineWidthInputLabel);
            LineRow.Dock = DockStyle.Fill;
            LineRow.Location = new Point(3, 3);
            LineRow.Name = "LineRow";
            LineRow.Size = new Size(259, 79);
            LineRow.TabIndex = 0;
            // 
            // LineStyleComboBox
            // 
            LineStyleComboBox.FormattingEnabled = true;
            LineStyleComboBox.Location = new Point(8, 34);
            LineStyleComboBox.Name = "LineStyleComboBox";
            LineStyleComboBox.Size = new Size(121, 23);
            LineStyleComboBox.TabIndex = 7;
            // 
            // LineStyleComboBoxLabel
            // 
            LineStyleComboBoxLabel.AutoSize = true;
            LineStyleComboBoxLabel.Location = new Point(8, 16);
            LineStyleComboBoxLabel.Name = "LineStyleComboBoxLabel";
            LineStyleComboBoxLabel.Size = new Size(63, 15);
            LineStyleComboBoxLabel.TabIndex = 8;
            LineStyleComboBoxLabel.Text = "Vonalstílus";
            // 
            // LineWidthInput
            // 
            LineWidthInput.Location = new Point(209, 34);
            LineWidthInput.Name = "LineWidthInput";
            LineWidthInput.Size = new Size(44, 23);
            LineWidthInput.TabIndex = 5;
            // 
            // LineWidthInputLabel
            // 
            LineWidthInputLabel.AutoSize = true;
            LineWidthInputLabel.Location = new Point(147, 34);
            LineWidthInputLabel.Name = "LineWidthInputLabel";
            LineWidthInputLabel.Size = new Size(56, 15);
            LineWidthInputLabel.TabIndex = 6;
            LineWidthInputLabel.Text = "Szélesség";
            // 
            // ColorRow
            // 
            ColorRow.Controls.Add(LineColorComboBox);
            ColorRow.Controls.Add(LineColorComboBoxLabel);
            ColorRow.Dock = DockStyle.Fill;
            ColorRow.Location = new Point(3, 88);
            ColorRow.Name = "ColorRow";
            ColorRow.Size = new Size(259, 47);
            ColorRow.TabIndex = 1;
            // 
            // LineColorComboBox
            // 
            LineColorComboBox.FormattingEnabled = true;
            LineColorComboBox.Location = new Point(84, 7);
            LineColorComboBox.Name = "LineColorComboBox";
            LineColorComboBox.Size = new Size(137, 23);
            LineColorComboBox.TabIndex = 18;
            // 
            // LineColorComboBoxLabel
            // 
            LineColorComboBoxLabel.AutoSize = true;
            LineColorComboBoxLabel.Location = new Point(13, 10);
            LineColorComboBoxLabel.Name = "LineColorComboBoxLabel";
            LineColorComboBoxLabel.Size = new Size(28, 15);
            LineColorComboBoxLabel.TabIndex = 9;
            LineColorComboBoxLabel.Text = "Szín";
            // 
            // FillRow
            // 
            FillRow.Controls.Add(FillColorComboBox);
            FillRow.Controls.Add(C_Fill);
            FillRow.Dock = DockStyle.Fill;
            FillRow.Location = new Point(3, 141);
            FillRow.Name = "FillRow";
            FillRow.Size = new Size(259, 61);
            FillRow.TabIndex = 2;
            // 
            // FillColorComboBox
            // 
            FillColorComboBox.FormattingEnabled = true;
            FillColorComboBox.Location = new Point(84, 20);
            FillColorComboBox.Name = "FillColorComboBox";
            FillColorComboBox.Size = new Size(137, 23);
            FillColorComboBox.TabIndex = 19;
            // 
            // C_Fill
            // 
            C_Fill.AutoSize = true;
            C_Fill.Location = new Point(13, 20);
            C_Fill.Name = "C_Fill";
            C_Fill.Size = new Size(65, 19);
            C_Fill.TabIndex = 11;
            C_Fill.Text = "Kitöltés";
            C_Fill.UseVisualStyleBackColor = true;
            // 
            // ArrowRow
            // 
            ArrowRow.Controls.Add(ArrowLabel);
            ArrowRow.Controls.Add(RB_ArrowRight);
            ArrowRow.Controls.Add(RB_ArrowLeft);
            ArrowRow.Dock = DockStyle.Fill;
            ArrowRow.Location = new Point(3, 208);
            ArrowRow.Name = "ArrowRow";
            ArrowRow.Size = new Size(259, 40);
            ArrowRow.TabIndex = 3;
            // 
            // ArrowLabel
            // 
            ArrowLabel.AutoSize = true;
            ArrowLabel.Location = new Point(13, 11);
            ArrowLabel.Name = "ArrowLabel";
            ArrowLabel.Size = new Size(28, 15);
            ArrowLabel.TabIndex = 23;
            ArrowLabel.Text = "Nyíl";
            // 
            // RB_ArrowRight
            // 
            RB_ArrowRight.AutoSize = true;
            RB_ArrowRight.Location = new Point(137, 9);
            RB_ArrowRight.Name = "RB_ArrowRight";
            RB_ArrowRight.Size = new Size(50, 19);
            RB_ArrowRight.TabIndex = 21;
            RB_ArrowRight.TabStop = true;
            RB_ArrowRight.Text = "Vége";
            RB_ArrowRight.UseVisualStyleBackColor = true;
            // 
            // RB_ArrowLeft
            // 
            RB_ArrowLeft.AutoSize = true;
            RB_ArrowLeft.Location = new Point(73, 9);
            RB_ArrowLeft.Name = "RB_ArrowLeft";
            RB_ArrowLeft.Size = new Size(49, 19);
            RB_ArrowLeft.TabIndex = 20;
            RB_ArrowLeft.TabStop = true;
            RB_ArrowLeft.Text = "Eleje";
            RB_ArrowLeft.UseVisualStyleBackColor = true;
            // 
            // TextRow
            // 
            TextRow.Controls.Add(C_Italic);
            TextRow.Controls.Add(C_Bold);
            TextRow.Controls.Add(InTextSizeLabel);
            TextRow.Controls.Add(In_TextSize);
            TextRow.Dock = DockStyle.Fill;
            TextRow.Location = new Point(3, 254);
            TextRow.Name = "TextRow";
            TextRow.Size = new Size(259, 132);
            TextRow.TabIndex = 4;
            // 
            // C_Italic
            // 
            C_Italic.AutoSize = true;
            C_Italic.Location = new Point(158, 59);
            C_Italic.Name = "C_Italic";
            C_Italic.Size = new Size(48, 19);
            C_Italic.TabIndex = 17;
            C_Italic.Text = "Dőlt";
            C_Italic.UseVisualStyleBackColor = true;
            // 
            // C_Bold
            // 
            C_Bold.AutoSize = true;
            C_Bold.Location = new Point(158, 34);
            C_Bold.Name = "C_Bold";
            C_Bold.Size = new Size(60, 19);
            C_Bold.TabIndex = 16;
            C_Bold.Text = "Vastag";
            C_Bold.UseVisualStyleBackColor = true;
            // 
            // InTextSizeLabel
            // 
            InTextSizeLabel.AutoSize = true;
            InTextSizeLabel.Location = new Point(25, 37);
            InTextSizeLabel.Name = "InTextSizeLabel";
            InTextSizeLabel.Size = new Size(62, 15);
            InTextSizeLabel.TabIndex = 15;
            InTextSizeLabel.Text = "Betűméret";
            // 
            // In_TextSize
            // 
            In_TextSize.Location = new Point(26, 55);
            In_TextSize.Name = "In_TextSize";
            In_TextSize.Size = new Size(120, 23);
            In_TextSize.TabIndex = 14;
            // 
            // CBFontLabel
            // 
            CBFontLabel.AutoSize = true;
            CBFontLabel.Location = new Point(19, 589);
            CBFontLabel.Name = "CBFontLabel";
            CBFontLabel.Size = new Size(57, 15);
            CBFontLabel.TabIndex = 13;
            CBFontLabel.Text = "Betűtípus";
            CBFontLabel.Visible = false;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(69, 652);
            label2.Name = "label2";
            label2.Size = new Size(107, 15);
            label2.TabIndex = 4;
            label2.Text = "Háttér átlátszósága";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { B_Import, B_ToFile, B_ToClipboard, B_Project });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1207, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // B_Import
            // 
            B_Import.Name = "B_Import";
            B_Import.Size = new Size(103, 20);
            B_Import.Text = "Kép megnyitása";
            // 
            // B_ToFile
            // 
            B_ToFile.Name = "B_ToFile";
            B_ToFile.Size = new Size(90, 20);
            B_ToFile.Text = "Mentés fájlba";
            B_ToFile.Click += B_ToFile_Click;
            // 
            // B_ToClipboard
            // 
            B_ToClipboard.Name = "B_ToClipboard";
            B_ToClipboard.Size = new Size(117, 20);
            B_ToClipboard.Text = "Vágólapra másolás";
            B_ToClipboard.Click += B_ToClipboard_Click;
            // 
            // B_Project
            // 
            B_Project.DropDownItems.AddRange(new ToolStripItem[] { B_LoadProject, B_SaveProject });
            B_Project.Enabled = false;
            B_Project.Name = "B_Project";
            B_Project.Size = new Size(56, 20);
            B_Project.Text = "Projekt";
            B_Project.Visible = false;
            // 
            // B_LoadProject
            // 
            B_LoadProject.Name = "B_LoadProject";
            B_LoadProject.Size = new Size(180, 22);
            B_LoadProject.Text = "Projekt betöltése";
            // 
            // B_SaveProject
            // 
            B_SaveProject.Name = "B_SaveProject";
            B_SaveProject.Size = new Size(180, 22);
            B_SaveProject.Text = "Projekt mentése";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1207, 932);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Raster 2 Tikz";
            Load += Form1_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)OpacityBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)ResolutionBar).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            PropertiesTable.ResumeLayout(false);
            LineRow.ResumeLayout(false);
            LineRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LineWidthInput).EndInit();
            ColorRow.ResumeLayout(false);
            ColorRow.PerformLayout();
            FillRow.ResumeLayout(false);
            FillRow.PerformLayout();
            ArrowRow.ResumeLayout(false);
            ArrowRow.PerformLayout();
            TextRow.ResumeLayout(false);
            TextRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)In_TextSize).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ClickThroughToolStrip toolStrip1;
        private ToolStripButton B_Point;
        private ToolStripButton B_Line;
        private ToolStripButton B_Circle;
        private ToolStripButton B_Arc;
        private ToolStripButton B_Bezier;
        private ToolStripButton B_Polygon;
        private ToolStripButton B_Text;
        private StatusStrip statusStrip1;
        private TrackBar OpacityBar;
        private TrackBar ResolutionBar;
        private Panel panel1;
        private Label label2;
        private Label label1;
        private ToolStripButton B_Select;
        private Label LineWidthInputLabel;
        private NumericUpDown LineWidthInput;
        private Label LineStyleComboBoxLabel;
        private ComboBox LineStyleComboBox;
        private CheckBox C_Fill;
        private Label LineColorComboBoxLabel;
        private Label InTextSizeLabel;
        private NumericUpDown In_TextSize;
        private Label CBFontLabel;
        private ComboBox CB_Font;
        private CheckBox C_Italic;
        private CheckBox C_Bold;
        private ComboBox FillColorComboBox;
        private ComboBox LineColorComboBox;
        private RadioButton RB_ArrowRight;
        private RadioButton RB_ArrowLeft;
        private Label ArrowLabel;
        private ToolStripButton B_Pan;
        private ToolStripButton B_Freehand;
        private ToolStripButton B_Help;
        private TableLayoutPanel PropertiesTable;
        private Panel LineRow;
        private Panel ColorRow;
        private Panel FillRow;
        private Panel ArrowRow;
        private Panel TextRow;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem B_Import;
        private ToolStripMenuItem B_ToFile;
        private ToolStripMenuItem B_ToClipboard;
        private ToolStripMenuItem B_Project;
        private ToolStripMenuItem B_LoadProject;
        private ToolStripMenuItem B_SaveProject;
        private CheckBox C_SnapOff;
    }
}
