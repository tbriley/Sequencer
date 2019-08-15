using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPublisherBehaviour : MonoBehaviour, IPointerClickHandler
{
    public GameObjectEntity gameObjectEntity;

    EntityEventSystem eventSystem;
    EntityEventSystem EventSystem
    {
        get
        {
            if(eventSystem == null)
            {
                eventSystem = World.Active.GetExistingSystem<EntityEventSystem>();
            }
            return eventSystem;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.PublishData(new PointerEvent(gameObjectEntity.Entity, PointerEventType.Click));
    }
}

//public struct ClickEvent : IComponentData
//{
//    public Entity Entity;

//    public ClickEvent(Entity entity)
//    {
//        Entity = entity;
//    }
//}