using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//public class TimelineEventWrapper : MonoBehaviour, IConvertGameObjectToEntity
//{
//    public TimelineEvent TimelineEvent = new TimelineEvent();

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        //TODO -> it seems to support adding any object, however our ForEach still says it requires ICD, ISCD, Component, etc...
//        //...at some point we should just be able to enable this here though
//        //dstManager.AddComponentObject(entity, TimelineEvent);
//    }
//}

[Serializable]
public class TimelineEvent
{
    public PlayableGraph PlayableGraph;
    public TimelineClip Clip;
    public EventData EventData;
}