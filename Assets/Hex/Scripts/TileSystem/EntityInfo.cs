using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OccupyType {
    EMPTY, STRUCTURE, WATER, BLOCK, FOREST
}

[CreateAssetMenu(fileName = "Data", menuName = "HexaTile/Entity Info", order = 3)]
public class EntityInfo : ScriptableObject
{
    public string prefabName;
    public List<GameObject> meshes;

    /// Texture on the prototile mesh
    public Texture2D protoTex;
    bool collision = true;
    public TriRequirement requirement;
    public List<HexFlag> flagsE = new List<HexFlag>();
    public List<HexFlag> flagsSE = new List<HexFlag>();
    public List<HexFlag> flagsSW = new List<HexFlag>();
    public List<HexFlag> flagsW = new List<HexFlag>();
    public List<HexFlag> flagsNW = new List<HexFlag>();
    public List<HexFlag> flagsNE = new List<HexFlag>();
    public List<EntityConnectionPair> connectionMap = new List<EntityConnectionPair>();
    public OccupyType[] occupy = new OccupyType[]{OccupyType.EMPTY,OccupyType.EMPTY,OccupyType.EMPTY,OccupyType.EMPTY,OccupyType.EMPTY,OccupyType.EMPTY};
    
    public TileType pointValueOverrideType;
    public bool pointValueOverrideTypeActive = false;
    public HexFlagType pointValueOverrideFlag;
    public bool pointValueOverrideFlagActive = false;
    
    public Texture2D staticPreview;

    public bool autoActivate = false;

    /// <summary>
    /// Abstract checker function. Entity info should be extended by other classes.
    /// </summary>
    /// <param name="system"> system for checking connections and neighbors </param>
    /// <param name="placement"> anticipated placement position </param>
    /// <returns>true if placement is valid given parameters.</returns>
    public bool checkRequirements(TileSystem system, HexCoordinate placement) {
        if(!requirement)
            return true;
        return requirement.check(system, placement);
    }
}
