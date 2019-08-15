using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AnimatorControllerBehaviour : PlayableBehaviour
{
    [HideInInspector]
    public ExposedReference<Animator> Animator;
    public AnimationClip AnimationClip;
    public EventData EventData;

    bool hasFired;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        if (!Application.isPlaying)
            return;

        if (info.deltaTime != 0f)
        { hasFired = false; }

        if (info.deltaTime == 0f && hasFired)
            return;

        //var timelineEvent = new TimelineEvent(playable.GetGraph(), EventData);
        //Sequencer.EventManager.PublishTimelineEvent(timelineEvent);
        var animator = Animator.Resolve(playable.GetGraph().GetResolver());

        foreach(var kvp in EventData)
        {
            var command = kvp.Key;
            var data = kvp.Value;
            if (command == "Play")
            {
                animator.Play(AnimationClip.name);
            }
            else if (command == "Stop")
            {
                animator.StopPlayback();
            }
            else if (command == "Time")
            {
                animator.playbackTime = float.Parse(data);
            }
            else if (command == "Frame")
            {
                //TODO - is this possible???
            }
            else if (command == "CrossFade")
            {
                animator.CrossFade(AnimationClip.name, float.Parse(data));
            }
        }

        hasFired = true;
    }
}
