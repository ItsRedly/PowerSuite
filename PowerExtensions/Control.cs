namespace PowerExtensions
{
    public abstract class Control
    {
        public string Name;
        public string Text;
        public Point Location = new Point(0, 0);
        public Size Size = new Size(50, 50);
        public bool IsEnabled = true;

        public abstract void Draw(IntPtr hwnd, IntPtr hdc);
    }
}