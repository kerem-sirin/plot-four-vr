namespace PlotFourVR.Events
{
    /// <summary>
    /// EventBus class that serves as a central hub for managing events in the game.
    /// </summary>
    public class EventBus
    {
        public UiEvents UiEvents { get; private set; }
        public SettingEvents SettingEvents { get; private set; }
        public InteractionEvents InteractionEvents { get; private set; }

        public EventBus()
        {
            // Initialize the event bus
            UiEvents = new UiEvents();
            SettingEvents = new SettingEvents();
            InteractionEvents = new InteractionEvents();
        }
    }
}