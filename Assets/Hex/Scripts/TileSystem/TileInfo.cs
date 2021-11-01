using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Data", menuName = "HexaTile/Tile Info", order = 2)]
public class TileInfo : ScriptableObject
{
    public string prefabName;
    public Texture2D cardTex;
    public Texture2D meshTex;
    public Mesh mesh;
    public Material material;
    public Texture2D staticPreview;

    public TileType[] segmentType = {0,0,0,0,0,0};

    public int flagCount = 0;
    public List<HexFlag> flagsE = new List<HexFlag>();
    public List<HexFlag> flagsSE = new List<HexFlag>();
    public List<HexFlag> flagsSW = new List<HexFlag>();
    public List<HexFlag> flagsW = new List<HexFlag>();
    public List<HexFlag> flagsNW = new List<HexFlag>();
    public List<HexFlag> flagsNE = new List<HexFlag>();

    public bool requireRepaint = true;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
    private void OnValidate() {
        requireRepaint = true;
    }
}