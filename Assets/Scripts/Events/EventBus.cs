namespace PlotFourVR.Events
{
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