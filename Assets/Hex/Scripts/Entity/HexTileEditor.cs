using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(HexTile), true)]
[CanEditMultipleObjects]
public class HexTileEditor : Editor
{
    private static bool expandTools = true;
    private static bool expandCon = true;
    private static bool expandTst = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        expandTools = EditorGUILayout.Foldout(expandTools, "Tile Tools");
        if(expandTools) {
            if(GUILayout.Button("Snap to Hex Grid"))
            {
                foreach( HexTile target in targets ) {
                    target.snapParentToHexGrid(1.1f);
                }
            }

            /*GUILayout.BeginHorizontal();
            //if (!myScript.ring) {
            if(GUILayout.Button("Spawn Selection Ring(s)"))
            {
                foreach( HexTile target in targets ) {
                    target.selectionRingSpawn();
                }
            }
            //} else {
            if(GUILayout.Button("Despawn Selection Ring(s)"))
            {
                foreach( HexTile target in targets ) {
                    target.selectionRingDespawn();
                }
            }
            //}
            GUILayout.EndHorizontal();*/
        }
        /*
        expandCon = EditorGUILayout.Foldout(expandCon, "Connector Spawn");
        if(expandCon) {
        
            for(int i = 0; i < 6; ++i) {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Spawn Connector "+i))
                {
                    foreach( HexTile target in targets ) {
                        target.spawnConnector((HexDirection)i);
                    }
                }
                if(GUILayout.Button("Despawn Connector "+i))
                {
                    foreach( HexTile target in targets ) {
                        target.despawnConnector((HexDirection)i);
                    }
                }
                if(GUILayout.Button("Toggle"+i))
                {
                    foreach( HexTile target in targets ) {
                        target.toggleType(i);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        expandTst = EditorGUILayout.Foldout(expandTst, "Connector Tests");
        if(expandTst) {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Test All Connectors"))
            {
                foreach( HexTile target in targets ) {
                    target.testAllConnectors();
                }
            }
            if(GUILayout.Button("Clear All Connectors"))
            {
                foreach( HexTile target in targets ) {
                    target.clearAllConnectors();
                }
            }
            GUILayout.EndHorizontal();
            for(int i = 0; i < 6; ++i) {
                GUILayout.BeginHorizontal();
                HexDirection dir = (HexDirection)i;
                string name = dir.ToString();
                if(GUILayout.Button("Test Con (Glob) "+name))
                {
                    foreach( HexTile target in targets ) {
                        target.testSingleConnector(dir, true);
                    }
                }
                if(GUILayout.Button("Test Con (Rel) "+name))
                {
                    foreach( HexTile target in targets ) {
                        target.testSingleConnector(dir, false);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }*/
    }
}
