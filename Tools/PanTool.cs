using System.Windows.Forms;
using rastertotikz.Rendering;

namespace rastertotikz.Tools
{
    public class PanTool : DrawingTool
    {
        public override string DisplayName => "Mozgatás";

        public PanTool(DrawingCanvas canvas) : base(canvas) { }

        public override void Activate()
        {
            Canvas.IsPanMode = true;
            Canvas.Cursor = Cursors.Hand;
        }

        public override void Deactivate()
        {
            Canvas.IsPanMode = false;
            Canvas.Cursor = Cursors.Default;
        }
    }
}
