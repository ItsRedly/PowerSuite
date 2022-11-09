namespace PowerAPI.PowerDB
{
    public abstract class Plugin
    {
        public string Name;
        public string Description;
        public string[] MenuEntries;
        public bool CurrentlyRunning;

        public abstract void WhenSelected();
    }
}