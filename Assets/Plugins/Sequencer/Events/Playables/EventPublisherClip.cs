using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class EventPublisherClip : PlayableAsset, ITimelineClipAsset
{
    public EventPublisherBehaviour template = new EventPublisherBehaviour ();
    public TimelineClip Clip;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        template.Clip = Clip;
        return ScriptPlayable<EventPublisherBehaviour>.Create (graph, template);
    }
}
