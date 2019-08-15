//using UnityEngine;
//using UnityEditor;
//using UnityEngine.Playables;

//[CustomEditor(typeof(PlayableBindingsComponent))]
//public class PlayableBindingsComponentEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        if (GUILayout.Button("Try Bind Tracks"))
//        {
//            var script = (PlayableBindingsComponent)target;
//            script.gameObject.GetComponent<PlayableDirector>().TryBindTracks(script.Bindings);
//        }
//    }
//}
