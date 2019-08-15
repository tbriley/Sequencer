using System;
using Unity.Entities;
using UnityEngine;

public class EventDataWrapper : MonoBehaviour
{
    public EventData EventData;
}

[Serializable]
public class EventData : SerializableDictionary<string, string> { }

public struct EventComponentData : IComponentData { public int Index; }