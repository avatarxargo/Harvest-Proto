using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HelperSpawner))]
public class HelperSpawnerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HelperSpawner tgt = target as HelperSpawner;
        HarvestGameState gs = tgt.gameState;

        if(gs == null)
            return;

        if(GUILayout.Button("Spawn ProtoTile"))
        {
            gs.spawnProtoTile(tgt.protoSpawnindex);
        }

        if(GUILayout.Button("Spawn ProtoEntity"))
        {
            gs.spawnProtoEntity(tgt.protoSpawnindex);
        }

        if(GUILayout.Button("Clear All"))
        {
            gs.clearAll();
        }
    }
}