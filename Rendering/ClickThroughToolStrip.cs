using System.Windows.Forms;

namespace rastertotikz.Rendering
{
    /// <summary>
    /// ToolStrip subclass that allows the first click to pass through
    /// when the parent window is being re-activated from another application.
    /// </summary>
    public class ClickThroughToolStrip : ToolStrip
    {
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_ACTIVATE = 1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE && CanFocus && !Focused)
            {
                m.Result = (IntPtr)MA_ACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
