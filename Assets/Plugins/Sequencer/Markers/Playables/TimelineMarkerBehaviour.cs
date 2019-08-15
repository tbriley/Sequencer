using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimelineMarkerBehaviour : PlayableBehaviour
{
    public string Name;
    public TimelineClip Clip;
    public PlayableGraph Graph;

    Entity Entity;
    GameObject gameObject;

    EntityManager EntityManager => World.Active.EntityManager;

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);

        if (!Application.isPlaying)
            return;
            
        //Debug.Log("creating marker " + Name);
        //var evt = new MarkerCreatedEvent(this);
        //Sequencer.EventManager.PublishMarkerCreatedEvent(evt);
        gameObject = new GameObject();
        //gameObject.AddComponent<GameObjectEntity>();
        var wrapper = gameObject.AddComponent<TimelineMarkerBehaviourWrapper>();
        wrapper.name = this.Name;
        wrapper.TimelineMarkerBehaviour = this;
        Entity = GameObjectEntity.AddToEntityManager(EntityManager, gameObject);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        base.OnPlayableDestroy(playable);

        if (!Application.isPlaying)
            return;

        //Debug.Log("destroying marker " + Name);
        //var evt = new MarkerDestroyedEvent(this);
        //Sequencer.EventManager.PublishMarkerDestroyedEvent(evt);
        //EventsSystem.Publish(evt);
        GameObjectEntity.Destroy(gameObject);
        EntityManager.DestroyEntity(Entity);
    }
}
