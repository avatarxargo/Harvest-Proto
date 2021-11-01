using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TriRequirementTest : MonoBehaviour
{
    public TriRequirement requirement;

    private void OnValidate() {
        /*TriRequirement req1 = new TriRequirementSurfaceType(new List<TileType> {TileType.Grass, TileType.Cliff});
        TriRequirement req2 = new TriRequirementSurfaceType(new List<TileType> {TileType.Sand});
        TriRequirement reqm = new TriRequirementCombo(TriComboType.ALL,new List<TriRequirement>{req1,req2});
        requirement = reqm;
        */
    }
}

[CustomEditor(typeof(TriRequirementTest))]
public class TriRequirementTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TriRequirementTest tile = (TriRequirementTest) target;
        /*tile.requirement.drawEditor(0,0,300);
        TriRequirement req = tile.requirement.getReplacement();
        if(req != null) {
            tile.requirement = req;
        }*/
    }
}