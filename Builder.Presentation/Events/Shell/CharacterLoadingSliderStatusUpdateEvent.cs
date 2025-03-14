using Builder.Presentation.Events.Base;

namespace Builder.Presentation.Events.Shell
{
    public sealed class CharacterLoadingSliderStatusUpdateEvent : StatusUpdateEvent
    {
        public bool Success { get; }

        public CharacterLoadingSliderStatusUpdateEvent(string statusMessage, bool success = true)
            : base(statusMessage)
        {
            Success = success;
        }
    }
}
