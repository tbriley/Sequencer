using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sequencer;

[Serializable]
public class EventPublisherBehaviour : PlayableBehaviour
{
    public EventData EventData;
    bool hasFired;

    public TimelineClip Clip;

    EntityEventSystem entityEventSystem;
    EntityEventSystem EntityEventSystem
    {
        get
        {
            if (entityEventSystem == null)
            {
                entityEventSystem = World.Active.GetExistingSystem<EntityEventSystem>();
            }
            return entityEventSystem;
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);

        //if (!Application.isPlaying)
            //return;

        if (info.deltaTime != 0f)
        { hasFired = false; }

        if (info.deltaTime == 0f && hasFired)
            return;
            
        //var timelineEvent = new TimelineEvent(playable.GetGraph(), EventData);
        //var wrapper = new GameObject().AddComponent<TimelineEventWrapper>();
        //wrapper.TimelineEvent.PlayableGraph = playable.GetGraph();
        //wrapper.TimelineEvent.Clip = Clip;
        //wrapper.TimelineEvent.EventData = EventData;

        //wrapper.gameObject.name = "";
        //foreach (var kvp in EventData)
        //{
        //    wrapper.gameObject.name += kvp.Key + kvp.Value;
        //}

        //EntityEventSystem.PublishObject(wrapper);

        var evt = new TimelineEvent();
        evt.PlayableGraph = playable.GetGraph();
        evt.Clip = Clip;
        evt.EventData = EventData;
        TimelineEvents.Publish(evt);

        hasFired = true;
    }
}