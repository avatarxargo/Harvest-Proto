using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "HexaTile/Surface Info", order = 4)]
public class SurfaceInfo : ScriptableObject
{
    public string prefabName;

    public TileType type;
    public bool isLandmark = false;
    public Mesh centerMesh;
    public List<Mesh> sideMeshVariants = new List<Mesh>();

    public Material matReg;
    public Material matGhost;
    public Material matWarn;
    public Material matGhostWarn;

    public Texture2D diffuseTexture;
    public Texture2D alphaTexture;
    public Texture2D maskTexture;
    public Texture2D worldTexture;
}
