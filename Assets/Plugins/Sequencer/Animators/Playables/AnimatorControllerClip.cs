using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AnimatorControllerClip : PlayableAsset, ITimelineClipAsset
{
    public AnimatorControllerBehaviour template = new AnimatorControllerBehaviour ();
    [HideInInspector]
    public ExposedReference<Animator> Animator;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        template.Animator = Animator;
        var playable = ScriptPlayable<AnimatorControllerBehaviour>.Create (graph, template);
        AnimatorControllerBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
