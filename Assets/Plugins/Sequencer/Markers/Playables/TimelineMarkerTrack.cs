using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(TimelineMarkerClip))]
public class TimelineMarkerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var clips = GetClips();
        foreach (var clip in clips)
        {
            var clipAsset = clip.asset as TimelineMarkerClip;
            clipAsset.Clip = clip;
        }
        return ScriptPlayable<TimelineMarkerMixerBehaviour>.Create (graph, inputCount);
    }
}
