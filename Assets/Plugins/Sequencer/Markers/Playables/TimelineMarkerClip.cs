using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineMarkerClip : PlayableAsset, ITimelineClipAsset
{
    public TimelineMarkerBehaviour template = new TimelineMarkerBehaviour ();
    public TimelineClip Clip;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        template.Clip = Clip;
        template.Graph = graph;
        return ScriptPlayable<TimelineMarkerBehaviour>.Create (graph, template);
    }
}
