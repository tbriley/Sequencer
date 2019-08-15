using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(AnimatorControllerClip))]
[TrackBindingType(typeof(Animator))]
public class AnimatorControllerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var binding = (graph.GetResolver() as PlayableDirector).GetGenericBinding(this) as Animator;
        binding.StopPlayback();

        var clips = GetClips();
        foreach (var clip in clips)
        {
            var clipAsset = clip.asset as AnimatorControllerClip;
            clipAsset.Animator.defaultValue = binding;
        }

        return ScriptPlayable<AnimatorControllerMixerBehaviour>.Create (graph, inputCount);
    }
}
