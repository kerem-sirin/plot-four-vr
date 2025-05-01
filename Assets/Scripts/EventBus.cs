using UnityEngine;

namespace PlotFourVR
{
    public class EventBus
    {
        public UiEvents UiEvents { get; private set; }
        public SettingEvents SettingEvents { get; private set; }

        public EventBus()
        {
            // Initialize the event bus
            UiEvents = new UiEvents();
            SettingEvents = new SettingEvents();
        }
    }
}
