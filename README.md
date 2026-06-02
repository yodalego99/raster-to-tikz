# Raster to TikZ

A Windows desktop app for tracing raster images and exporting clean TikZ code.

Raster to TikZ helps you place geometric primitives over a reference image, tune styling, and export directly to LaTeX/TikZ for reports, worksheets, and technical docs.

The program's language is currently only available in Hungarian. English coming in a future version! 

## Features

- Trace over any reference image with adjustable opacity
- Draw points, lines, circles, arcs, polygons, curves, freehand paths, and text
- Edit geometry with selection handles and drag interactions
- Snap to grid for consistent construction geometry
- Export as TikZ to clipboard, LaTeX .tex, or directly to PDF (via pdflatex)
- Built with .NET 8 WinForms, lightweight and fast

### Editing and styling

- Stroke width and style (solid, dashed, dotted)
- Stroke color and fill color presets
- Fill toggle for fillable shapes
- Arrow toggles for line segments
- Shape selection and transformation with handles
- Per-shape style edits and document-level defaults for newly created shapes

### Prerequisites

- Windows
- .NET 8 SDK
- Optional: TeX distribution with pdflatex on PATH (for PDF export)

## Keyboard shortcuts

- Delete: remove selected shapes
- Escape:
  - clear selection (Select tool)
  - cancel current drawing operation (shape tools)
  - cancel text edit
- Backspace: remove last control point while drawing polygon/curve
- Ctrl+V or Shift+Insert: paste text into active text editor box
- Shift + Mouse Wheel: horizontal scrolling when not in pan mode

## Sample output

Example TikZ produced by the app:

```tex
\begin{tikzpicture}
\draw[->, thick] (0.52,1.33)-- (1.39,0.47);
\draw[->, thick] (1.65,0.47)-- (2.48,1.31);
\draw[->, thick] (0.57,1.46)-- (2.42,1.46);
\node[font=\scriptsize\itshape] at (0.75,0.79) {f};
\node[font=\scriptsize\itshape] at (1.45,1.62) {k};
\node[font=\scriptsize\itshape] at (2.13,0.80) {g};
\node[font=\footnotesize\itshape] at (0.42,1.46) {A};
\node[font=\footnotesize\itshape] at (2.58,1.45) {C};
\node[font=\footnotesize\itshape] at (1.51,0.34) {B};
\end{tikzpicture}
```

## Planned updates

- Save and reload project files
- Undo/redo history
- Custom color picker and palettes
- Better text alignment and anchoring options

## Contributing

Issues and pull requests are welcome. If you want to contribute, open an issue with the feature/bug first so scope is clear.

## License

This project is licensed under the MIT License.
See the LICENSE file for details.


