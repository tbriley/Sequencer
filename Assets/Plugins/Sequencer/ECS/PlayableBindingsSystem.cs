using Unity.Entities;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace Sequencer
{
    public class PlayableBindingsSystem : ComponentSystem
    {
        //ComponentGroup addedBindings;
        //ComponentGroup removedBindings;

        EntityQuery bindings;

        Dictionary<PlayableDirector, PlayableAsset> directorToAssetTable;

        //struct Initialized : ISystemStateComponentData { }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            //var addedQuery = new EntityArchetypeQuery
            //{
            //    All = new ComponentType[] { typeof(PlayableDirector), typeof(PlayableBindingsComponent), },
            //    None = new ComponentType[] { typeof(Initialized), }
            //};
            //addedBindings = GetComponentGroup(addedQuery);

            //var removedDirectorQuery = new EntityArchetypeQuery
            //{
            //    All = new ComponentType[] { typeof(Initialized), },
            //    None = new ComponentType[] { typeof(PlayableDirector), },
            //};
            //var removedBindingsQuery = new EntityArchetypeQuery
            //{
            //    All = new ComponentType[] { typeof(Initialized), },
            //    None = new ComponentType[] { typeof(PlayableBindingsComponent), },
            //};
            //removedBindings = GetComponentGroup(removedDirectorQuery, removedBindingsQuery);

            EntityQueryDesc query = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(PlayableDirector), typeof(PlayableBindingsComponent), },
            };
            bindings = GetEntityQuery(query);

            directorToAssetTable = new Dictionary<PlayableDirector, PlayableAsset>();
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            directorToAssetTable?.Clear();
        }

        protected override void OnUpdate()
        {
            //ForEach((Entity entity, PlayableDirector director, PlayableBindingsComponent bindings) =>
            //{

            //}, addedBindings);

            //ForEach((Entity entity, PlayableDirector director, PlayableBindingsComponent bindings) =>
            //{

            //}, removedBindings);

            //Entities.ForEach((PlayableDirector director, PlayableBindingsComponent bindings) =>
            //{
            //    var currentAsset = director.playableAsset;
            //    directorToAssetTable.TryGetValue(director, out var previousAsset);
            //    if (currentAsset != previousAsset)
            //    {
            //        //UnityEngine.Debug.Log("setting up bindings");
            //        director.TryBindTracks(bindings.Bindings);
            //        directorToAssetTable[director] = currentAsset;
            //    }
            //});
        }
    }
}

//public static class PlayableExtensions
//{
//    public static void TryBindTracks(this PlayableDirector director, StringToUnityObjectTable bindings)
//    {
//        var outputs = director.playableAsset.outputs;
//        foreach (var binding in outputs)
//        {
//            UnityEngine.Object value;
//            if (bindings.TryGetValue(binding.streamName, out value))
//            {
//                director.SetGenericBinding(binding.sourceObject, value);
//            }
//        }
//    }
//}
