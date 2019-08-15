namespace Sequencer
{
    public class TimelineEvents
    {
        public delegate void TimelineEventAction(TimelineEvent evt);
        public static event TimelineEventAction OnTimelineEvent;

        //public delegate void MarkerCreatedAction(MarkerCreatedEvent evt);
        //public static event MarkerCreatedAction OnMarkerCreatedEvent;

        //public delegate void MarkerDestroyedAction(MarkerDestroyedEvent evt);
        //public static event MarkerDestroyedAction OnMarkerDestroyedEvent;

        public static void Publish(TimelineEvent evt)
        {
            OnTimelineEvent?.Invoke(evt);
        }

        //public static void PublishMarkerCreatedEvent(MarkerCreatedEvent evt)
        //{
        //    OnMarkerCreatedEvent?.Invoke(evt);
        //}

        //public static void PublishMarkerDestroyedEvent(MarkerDestroyedEvent evt)
        //{
        //    OnMarkerDestroyedEvent?.Invoke(evt);
        //}
    }
}