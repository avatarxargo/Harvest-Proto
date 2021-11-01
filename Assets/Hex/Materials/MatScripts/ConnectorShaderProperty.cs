using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    Grass, Sand, Untamed, Fertile, Lake, CliffsGrass,CliffsSand
}


public class TileTypeUtil {
    public static TileType getEffectiveType(TileType type) {
        if(type == TileType.Grass || type == TileType.CliffsGrass || type == TileType.Lake)
            return TileType.Grass;
        if(type == TileType.Sand || type == TileType.CliffsSand)
            return TileType.Sand;
        return type;
    }
}

public class ConnectorShaderProperty : MonoBehaviour
{
    private MaterialPropertyBlock propertyBlock;

    public TileType type = 0;
    public bool isGhost = false;
    public bool isWarning = false;
    void OnValidate()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        Renderer renderer = GetComponentInChildren<Renderer>();
        propertyBlock.SetInt("ctype", (int)TileTypeUtil.getEffectiveType(type));
        propertyBlock.SetInt("ghost", isGhost?1:0);
        propertyBlock.SetInt("warning", isWarning?1:0);
        renderer.SetPropertyBlock(propertyBlock);
    }

    public void setType(TileType newtype) {
        this.type = newtype;
        this.OnValidate();
    }
    public void setGhost(bool val) {
        this.isGhost = val;
        this.OnValidate();
    }
    public void setWarning(bool val) {
        this.isWarning = val;
        this.OnValidate();
    }

    public void toggleType() {
        type += 1;
        if((int)type > 2) {
            type = 0;
        }
        this.OnValidate();
    }
}
