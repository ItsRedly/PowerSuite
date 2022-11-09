using System.Drawing;
using PowerAPI.Constants;
using PowerAPI.Extensions;

namespace PowerAPI.GUI
{
    public delegate void ClickEvent(MouseButton button);
    public delegate void KeyPressEvent(Key key);
    public abstract class Control
    {
        public Color BackgroundColor = Color.Transparent;
        public Window Parent;
        public ClickEvent Clicked;
        public KeyPressEvent GotKey;
        public KeyPressEvent GotSystemKey;
        public string Name;
        public string Text;
        public Point Location = new(0, 0);
        public Size Size = new(50, 50);
        public bool IsEnabled = true;
        public bool HasFocus = false;
        public (AnchorX X, AnchorY Y) Anchor = (AnchorX.Left, AnchorY.Top);

        public Control(Window parent) { Parent = parent; }

        public abstract void Draw(IntPtr hWnd, IntPtr hDC, uint drawArgs);
        public uint GetDefaultDrawArgs() {
            uint drawArgs = 0;
            drawArgs |= Anchor.X == AnchorX.Center ? Win32DTConstants.DT_CENTER : (uint)0;
            drawArgs |= Anchor.Y == AnchorY.Center ? Win32DTConstants.DT_VCENTER : (uint)0;
            drawArgs |= Anchor.X == AnchorX.Right ? Win32DTConstants.DT_RIGHT : (uint)0;
            drawArgs |= Anchor.Y == AnchorY.Bottom ? Win32DTConstants.DT_BOTTOM : (uint)0;
            return drawArgs;
        }
    }
}