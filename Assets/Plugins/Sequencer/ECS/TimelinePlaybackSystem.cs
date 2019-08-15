using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Entities;
using UnityEngine.Timeline;
using Sequencer;

namespace Sequencer
{
    //[DisableAutoCreation]
    [ExecuteAlways]
    public class TimelinePlaybackSystem : ComponentSystem
    {
        EntityQuery addedControllers;
        EntityQuery removedControllers;

        EntityQuery addedMarkers;
        EntityQuery removedMarkers;

        Dictionary<Entity, PlayableDirectorController> entityToPlayableControllerTable = new Dictionary<Entity, PlayableDirectorController>();
        Dictionary<PlayableGraph, Dictionary<string, TimelineMarkerBehaviourWrapper>> graphToMarkerTable = new Dictionary<PlayableGraph, Dictionary<string, TimelineMarkerBehaviourWrapper>>();
        Dictionary<Entity, TimelineMarkerBehaviourWrapper> markerEntityToBehaviourTable = new Dictionary<Entity, TimelineMarkerBehaviourWrapper>();

        EntityEventSystem eventsSystem;
        EntityEventSystem EventsSystem
        {
            get
            {
                if (eventsSystem == null)
                {
                    eventsSystem = World.Active.GetExistingSystem<EntityEventSystem>();
                }
                return eventsSystem;
            }
        }

        struct InitializedController : ISystemStateComponentData { }
        struct InitializedMarker : ISystemStateComponentData { }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            EntityQueryDesc addedControllerQuery = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(PlayableDirectorController), },
                None = new ComponentType[] { typeof(InitializedController), }
            };
            addedControllers = GetEntityQuery(addedControllerQuery);

            EntityQueryDesc removedControllerQuery = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(InitializedController), },
                None = new ComponentType[] { typeof(PlayableDirectorController), },
            };
            removedControllers = GetEntityQuery(removedControllerQuery);

            EntityQueryDesc addedMarkerQuery = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(TimelineMarkerBehaviourWrapper), },
                None = new ComponentType[] { typeof(InitializedMarker), }
            };
            addedMarkers = GetEntityQuery(addedMarkerQuery);

            EntityQueryDesc removedMarkerQuery = new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(InitializedMarker), },
                None = new ComponentType[] { typeof(TimelineMarkerBehaviourWrapper), }
            };
            removedMarkers = GetEntityQuery(removedMarkerQuery);

            TimelineEvents.OnTimelineEvent += OnTimelineEvent;
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();

            graphToMarkerTable.Clear();
            markerEntityToBehaviourTable.Clear();

            TimelineEvents.OnTimelineEvent -= OnTimelineEvent;
        }

        void OnTimelineEvent(TimelineEvent evt)
        {
            foreach (var kvp in evt.EventData)
            {
                var command = kvp.Key;
                var data = kvp.Value;
                var director = (PlayableDirector)evt.PlayableGraph.GetResolver();
                if (command.Equals("Play"))
                {
                    director.Play();
                    director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                }
                else if (command.Equals("Resume"))
                {
                    //director.Resume();
                    director.playableGraph.GetRootPlayable(0).SetSpeed(1);
                }
                else if (command.Equals("Stop"))
                {
                    director.time = evt.Clip.start;
                    //director.playableGraph.Evaluate();
                    director.Stop();
                }
                else if (command.Equals("Pause"))
                {
                    director.time = evt.Clip.start;
                    //director.playableGraph.Evaluate();
                    //director.Pause();
                    director.playableGraph.GetRootPlayable(0).SetSpeed(0);
                }
                else if (command.Equals("Time"))
                {
                    string timeString;
                    if (evt.EventData.TryGetValue("Time", out timeString))
                    {
                        director.time = double.Parse(timeString);
                    }
                }
                else if (command.Equals("Frame"))
                {
                    //TODO - is this possible???
                }
                else if (command.Equals("Marker"))
                {
                    var markerId = data;

                    //UnityEngine.Debug.Log("skipping to marker " + markerId);

                    Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
                    if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
                        return;

                    //UnityEngine.Debug.Log("found graph");

                    TimelineMarkerBehaviourWrapper marker;
                    if (!markers.TryGetValue(markerId, out marker))
                        return;

                    //UnityEngine.Debug.Log("found marker");

                    director.time = marker.TimelineMarkerBehaviour.Clip.start;
                    //director.Evaluate();
                }
                else if (command.Equals("NextMarker"))
                {
                    Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
                    if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
                        return;

                    //Debug.Log("next marker");

                    var currentTime = director.time;
                    var markerStart = (double)Mathf.Infinity;
                    var markerId = "";
                    var foundMarker = false;
                    foreach (var pair in markers)
                    {
                        var id = pair.Key;
                        var marker = pair.Value.TimelineMarkerBehaviour;
                        if (marker.Clip.start <= currentTime)
                            continue;

                        if (marker.Clip.start < markerStart)
                        {
                            foundMarker = true;
                            markerId = id;
                            markerStart = marker.Clip.start;
                        }
                    }

                    //Debug.Log(foundMarker);

                    if (foundMarker)
                    {
                        //var goToMarkerData = new EventData();
                        //goToMarkerData.Add("Marker", markerId);
                        ////EventSystem.Publish(new TimelineEvent(evt.PlayableGraph, goToMarkerData));
                        //var newWrapper = new GameObject().AddComponent<TimelineEventWrapper>();
                        //newWrapper.TimelineEvent.PlayableGraph = wrapper.TimelineEvent.PlayableGraph;
                        //newWrapper.TimelineEvent.EventData = goToMarkerData;

                        //wrapper.gameObject.name = "GoToMarker" + markerId;

                        //EventsSystem.PublishObject(newWrapper);

                        director.time = markerStart;
                        //director.Evaluate();
                    }
                }
                else if (command.Equals("PreviousMarker"))
                {
                    Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
                    if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
                        return;

                    //Debug.Log("previous marker");

                    var currentTime = director.time;
                    var markerStart = (double)Mathf.NegativeInfinity;
                    var markerId = "";
                    var foundMarker = false;
                    foreach (var pair in markers)
                    {
                        var id = pair.Key;
                        var marker = pair.Value.TimelineMarkerBehaviour;
                        //we use the end here because if we're playing and we want to quickly skip back several chapters...
                        //...if the playhead is moving, we'll never be able to jump if we just check against the clip's start time...
                        //...as the playhead has already moved beyond it by the time our next rapid click is registered
                        if (marker.Clip.end >= currentTime)
                            continue;

                        if (marker.Clip.start > markerStart)
                        {
                            foundMarker = true;
                            markerId = id;
                            markerStart = marker.Clip.start;
                        }
                    }

                    //Debug.Log(foundMarker);

                    if (foundMarker)
                    {
                        //var goToMarkerData = new EventData();
                        //goToMarkerData.Add("Marker", markerId);
                        ////EventSystem.Publish(new TimelineEvent(evt.PlayableGraph, goToMarkerData));
                        //var newWrapper = new GameObject().AddComponent<TimelineEventWrapper>();
                        //newWrapper.TimelineEvent.PlayableGraph = wrapper.TimelineEvent.PlayableGraph;
                        //newWrapper.TimelineEvent.EventData = goToMarkerData;

                        //wrapper.gameObject.name = "GoToMarker" + markerId;

                        //EventsSystem.PublishObject(newWrapper);

                        director.time = markerStart;
                        //director.Evaluate();
                    }
                }
                else if (command.Equals("MuteTrack"))
                {
                    var outputs = director.playableAsset.outputs;
                    var index = -1;
                    foreach (var binding in outputs)
                    {
                        if (binding.streamName.Equals(data))
                            break;
                        index += 1;
                    }
                    if (index != -1)
                    {
                        var output = director.playableGraph.GetOutput(index + 1);
                        output.SetWeight(0);
                        //Debug.Log("muting " + index);
                    }

                    //var asset = director.playableAsset as TimelineAsset;
                    //foreach (var track in asset.GetOutputTracks())
                    //{
                    //    if (track.name.Equals(data))
                    //    {
                    //        track.muted = true;
                    //    }
                    //}
                    //director.RebuildGraph();
                }
                else if (command.Equals("UnmuteTrack"))
                {
                    var outputs = director.playableAsset.outputs;
                    var index = -1;
                    foreach (var binding in outputs)
                    {
                        if (binding.streamName.Equals(data))
                            break;
                        index += 1;
                    }
                    if (index != -1)
                    {
                        var output = director.playableGraph.GetOutput(index + 1);
                        output.SetWeight(1);
                        //Debug.Log("unmuting " + index);
                    }

                    //var asset = director.playableAsset as TimelineAsset;
                    //foreach (var track in asset.GetOutputTracks())
                    //{
                    //    if (track.name.Equals(data))
                    //    {
                    //        track.muted = false;
                    //    }
                    //}
                    //director.RebuildGraph();
                }
            };
        }

        protected override void OnUpdate()
        {
            Entities.With(addedControllers).ForEach((Entity entity, PlayableDirectorController controller) =>
            {
                entityToPlayableControllerTable.Add(entity, controller);
                PostUpdateCommands.AddComponent(entity, new InitializedController());
            });

            Entities.With(removedControllers).ForEach((Entity entity) =>
            {
                entityToPlayableControllerTable.Remove(entity);
                PostUpdateCommands.RemoveComponent<InitializedController>(entity);
            });

            Entities.With(addedMarkers).ForEach((Entity entity, TimelineMarkerBehaviourWrapper marker) =>
            {
                Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
                if (!graphToMarkerTable.TryGetValue(marker.TimelineMarkerBehaviour.Graph, out markers))
                {
                    markers = new Dictionary<string, TimelineMarkerBehaviourWrapper>();
                    graphToMarkerTable.Add(marker.TimelineMarkerBehaviour.Graph, markers);
                }
                markers.Add(marker.TimelineMarkerBehaviour.Name, marker);
                markerEntityToBehaviourTable.Add(entity, marker);

                PostUpdateCommands.AddComponent(entity, new InitializedMarker());
            });

            Entities.With(removedMarkers).ForEach((Entity entity) =>
            {
                var marker = markerEntityToBehaviourTable[entity];
                markerEntityToBehaviourTable.Remove(entity);

                Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
                if (!graphToMarkerTable.TryGetValue(marker.TimelineMarkerBehaviour.Graph, out markers))
                    return;

                markers.Remove(marker.TimelineMarkerBehaviour.Name);
                if (markers.Count == 0)
                {
                    graphToMarkerTable.Remove(marker.TimelineMarkerBehaviour.Graph);
                }
                PostUpdateCommands.RemoveComponent<InitializedMarker>(entity);
            });

            Entities.ForEach((ref PointerEvent pointerEvent) =>
            {
                if (pointerEvent.EventType != PointerEventType.Click)
                    return;

                Debug.Log("CLICKED " + pointerEvent.Entity.Index);

                PlayableDirectorController component;
                if(entityToPlayableControllerTable.TryGetValue(pointerEvent.Entity, out component))
                {
                    //Debug.Log("clicked " + click.Entity.Index + " " + Time.frameCount);
                    //var wrapper = new GameObject().AddComponent<TimelineEventWrapper>();
                    //wrapper.TimelineEvent.PlayableGraph = component.PlayableDirector.playableGraph;
                    //wrapper.TimelineEvent.EventData = component.EventData;

                    //wrapper.gameObject.name = "";
                    //foreach (var kvp in component.EventData)
                    //{
                    //    wrapper.gameObject.name += kvp.Key + kvp.Value;
                    //}

                    //EventsSystem.PublishObject(wrapper);
                    PublishEvent(component.EventData, component.PlayableDirector);
                }
            });

            //ForEach((TimelineEventWrapper wrapper) =>
            //{
            //    if (!wrapper.TimelineEvent.PlayableGraph.IsValid())
            //        return;

            //    var evt = wrapper.TimelineEvent;
            //    foreach(var kvp in evt.EventData)
            //    {
            //        var command = kvp.Key;
            //        var data = kvp.Value;
            //        var director = (PlayableDirector)evt.PlayableGraph.GetResolver();
            //        if (command.Equals("Play"))
            //        {
            //            director.Play();
            //        }
            //        else if (command.Equals("Resume"))
            //        {
            //            director.Resume();
            //        }
            //        else if (command.Equals("Stop"))
            //        {
            //            director.time = evt.Clip.start;
            //            director.playableGraph.Evaluate();
            //            director.Stop();
            //        }
            //        else if (command.Equals("Pause"))
            //        {
            //            director.time = evt.Clip.start;
            //            director.playableGraph.Evaluate();
            //            director.Pause();
            //        }
            //        else if (command.Equals("Time"))
            //        {
            //            string timeString;
            //            if (evt.EventData.TryGetValue("Time", out timeString))
            //            {
            //                director.time = double.Parse(timeString);
            //            }
            //        }
            //        else if (command.Equals("Frame"))
            //        {
            //            //TODO - is this possible???
            //        }
            //        else if (command.Equals("Marker"))
            //        {
            //            var markerId = data;

            //            //UnityEngine.Debug.Log("skipping to marker " + markerId);

            //            Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
            //            if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
            //                return;

            //            //UnityEngine.Debug.Log("found graph");

            //            TimelineMarkerBehaviourWrapper marker;
            //            if (!markers.TryGetValue(markerId, out marker))
            //                return;

            //            //UnityEngine.Debug.Log("found marker");

            //            director.time = marker.TimelineMarkerBehaviour.Clip.start;
            //            director.Evaluate();
            //        }
            //        else if (command.Equals("NextMarker"))
            //        {
            //            Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
            //            if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
            //                return;

            //            //Debug.Log("next marker");

            //            var currentTime = director.time;
            //            var markerStart = (double)Mathf.Infinity;
            //            var markerId = "";
            //            var foundMarker = false;
            //            foreach (var pair in markers)
            //            {
            //                var id = pair.Key;
            //                var marker = pair.Value.TimelineMarkerBehaviour;
            //                if (marker.Clip.start <= currentTime)
            //                    continue;

            //                if (marker.Clip.start < markerStart)
            //                {
            //                    foundMarker = true;
            //                    markerId = id;
            //                    markerStart = marker.Clip.start;
            //                }
            //            }

            //            //Debug.Log(foundMarker);

            //            if (foundMarker)
            //            {
            //                //var goToMarkerData = new EventData();
            //                //goToMarkerData.Add("Marker", markerId);
            //                ////EventSystem.Publish(new TimelineEvent(evt.PlayableGraph, goToMarkerData));
            //                //var newWrapper = new GameObject().AddComponent<TimelineEventWrapper>();
            //                //newWrapper.TimelineEvent.PlayableGraph = wrapper.TimelineEvent.PlayableGraph;
            //                //newWrapper.TimelineEvent.EventData = goToMarkerData;

            //                //wrapper.gameObject.name = "GoToMarker" + markerId;

            //                //EventsSystem.PublishObject(newWrapper);

            //                director.time = markerStart;
            //                director.Evaluate();
            //            }
            //        }
            //        else if (command.Equals("PreviousMarker"))
            //        {
            //            Dictionary<string, TimelineMarkerBehaviourWrapper> markers;
            //            if (!graphToMarkerTable.TryGetValue(evt.PlayableGraph, out markers))
            //                return;

            //            //Debug.Log("previous marker");

            //            var currentTime = director.time;
            //            var markerStart = (double)Mathf.NegativeInfinity;
            //            var markerId = "";
            //            var foundMarker = false;
            //            foreach (var pair in markers)
            //            {
            //                var id = pair.Key;
            //                var marker = pair.Value.TimelineMarkerBehaviour;
            //                //we use the end here because if we're playing and we want to quickly skip back several chapters...
            //                //...if the playhead is moving, we'll never be able to jump if we just check against the clip's start time...
            //                //...as the playhead has already moved beyond it by the time our next rapid click is registered
            //                if (marker.Clip.end >= currentTime)
            //                    continue;

            //                if (marker.Clip.start > markerStart)
            //                {
            //                    foundMarker = true;
            //                    markerId = id;
            //                    markerStart = marker.Clip.start;
            //                }
            //            }

            //            //Debug.Log(foundMarker);

            //            if (foundMarker)
            //            {
            //                //var goToMarkerData = new EventData();
            //                //goToMarkerData.Add("Marker", markerId);
            //                ////EventSystem.Publish(new TimelineEvent(evt.PlayableGraph, goToMarkerData));
            //                //var newWrapper = new GameObject().AddComponent<TimelineEventWrapper>();
            //                //newWrapper.TimelineEvent.PlayableGraph = wrapper.TimelineEvent.PlayableGraph;
            //                //newWrapper.TimelineEvent.EventData = goToMarkerData;

            //                //wrapper.gameObject.name = "GoToMarker" + markerId;

            //                //EventsSystem.PublishObject(newWrapper);

            //                director.time = markerStart;
            //                director.Evaluate();
            //            }
            //        }
            //        else if(command.Equals("MuteTrack"))
            //        {
            //            var outputs = director.playableAsset.outputs;
            //            var index = -1;
            //            foreach (var binding in outputs)
            //            {
            //                if(binding.streamName.Equals(data))
            //                    break;
            //                index += 1;
            //            }
            //            if(index != -1)
            //            {
            //                var output = director.playableGraph.GetOutput(index + 1);
            //                output.SetWeight(0);
            //                //Debug.Log("muting " + index);
            //            }

            //            //var asset = director.playableAsset as TimelineAsset;
            //            //foreach (var track in asset.GetOutputTracks())
            //            //{
            //            //    if (track.name.Equals(data))
            //            //    {
            //            //        track.muted = true;
            //            //    }
            //            //}
            //            //director.RebuildGraph();
            //        }
            //        else if (command.Equals("UnmuteTrack"))
            //        {
            //            var outputs = director.playableAsset.outputs;
            //            var index = -1;
            //            foreach (var binding in outputs)
            //            {
            //                if (binding.streamName.Equals(data))
            //                    break;
            //                index += 1;
            //            }
            //            if (index != -1)
            //            {
            //                var output = director.playableGraph.GetOutput(index + 1);
            //                output.SetWeight(1);
            //                //Debug.Log("unmuting " + index);
            //            }

            //            //var asset = director.playableAsset as TimelineAsset;
            //            //foreach (var track in asset.GetOutputTracks())
            //            //{
            //            //    if (track.name.Equals(data))
            //            //    {
            //            //        track.muted = false;
            //            //    }
            //            //}
            //            //director.RebuildGraph();
            //        }
            //    };
            //});
        }

        void PublishEvent(EventData eventData, PlayableDirector director)
        {
            //var wrapper = new GameObject().AddComponent<TimelineEventWrapper>();
            //wrapper.TimelineEvent.PlayableGraph = director.playableGraph;
            //wrapper.TimelineEvent.EventData = eventData;

            //wrapper.gameObject.name = "";
            //foreach (var kvp in component.EventData)
            //{
            //    wrapper.gameObject.name += kvp.Key + kvp.Value;
            //}

            //EventsSystem.PublishObject(wrapper);

            var evt = new TimelineEvent();
            evt.PlayableGraph = director.playableGraph;
            evt.EventData = eventData;
            TimelineEvents.Publish(evt);
        }
    }
}