using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(EventPublisherClip))]
public class EventPublisherTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var clips = GetClips();
        foreach (var clip in clips)
        {
            var clipAsset = clip.asset as EventPublisherClip;
            clipAsset.Clip = clip;
        }
        return ScriptPlayable<EventPublisherMixerBehaviour>.Create (graph, inputCount);
    }
}
