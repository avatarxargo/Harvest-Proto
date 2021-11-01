using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShaderProperty : MonoBehaviour
{
    //private MaterialPropertyBlock propertyBlock;
    public TileType type = 0;

    public Material mat_regular;
    public Material mat_ghost;
    public Material mat_warn;
    public Material mat_ghost_warn;

    public Texture2D surfaceTex;
    public Texture2D alphaTex;
    public Texture2D worldTex;
    public Texture2D worldTexMask;
    public Texture2D tintMaskTex;
    public bool isGhost = false;
    public bool isWarning = false;
    public bool isWorldCoord = false;
    public Vector3 worldCoordOffset = new Vector3(0,0,0);
    public Vector3 worldCoordClamp = new Vector3(1,1,1);
    public Vector3 worldCoordScale = new Vector3(1,1,1);
    public Color playerColor = Color.white;

    /// <summary>
    /// Only repaint if a property changes - shader update is expensive
    /// </summary>
    public bool artDelta = true;
    void OnValidate()
    {
        artDelta = true;
        refeshShader();
    }
    public void refeshShader()
    {
        if(!artDelta)
            return;
        artDelta = false;
        /*if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetInt("ctype", (int)type);
        propertyBlock.SetInt("ghost", isGhost?1:0);
        propertyBlock.SetInt("warning", isWarning?1:0);
        propertyBlock.SetInt("worldcoord", isWorldCoord?1:0);
        propertyBlock.SetVector("wcclamp", worldCoordClamp);
        propertyBlock.SetVector("wcoffset", worldCoordOffset);
        propertyBlock.SetVector("wcscale", worldCoordScale);
        propertyBlock.SetColor("pcolor", playerColor);*/

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers) {
            /*PropTextureHelper helper = renderer.GetComponent<PropTextureHelper>();
            if(helper) {
                if(helper.appliedTexture)        
                    propertyBlock.SetTexture("meshMaterial", helper.appliedTexture);  
                if(helper.appliedWorldTexture)        
                    propertyBlock.SetTexture("worldMaterial", helper.appliedWorldTexture);  
                if(helper.appliedWorldTextureMask)        
                    propertyBlock.SetTexture("worldMaterialMask", helper.appliedWorldTextureMask);      
                if(helper.appliedAlphaTex)        
                    propertyBlock.SetTexture("alphaMaterial", helper.appliedAlphaTex);    
                if(helper.appliedTintMaskTex)      
                    propertyBlock.SetTexture("tinttex", helper.appliedTintMaskTex);                
            } else {
                if(surfaceTex)
                    propertyBlock.SetTexture("meshMaterial", surfaceTex);
                if(worldTex)
                    propertyBlock.SetTexture("worldMaterial", worldTex);
                if(worldTexMask)
                    propertyBlock.SetTexture("worldMaterialMask", worldTexMask);
                if(alphaTex)
                    propertyBlock.SetTexture("alphaMaterial", alphaTex);
                if(tintMaskTex)
                    propertyBlock.SetTexture("tinttex", tintMaskTex);
            }
            renderer.SetPropertyBlock(propertyBlock);*/
            //
            if(isGhost) {
                if(isWarning)
                    renderer.material = mat_ghost_warn;
                else
                    renderer.material = mat_ghost;
            } else {
                if(isWarning)
                    renderer.material = mat_warn;
                else
                    renderer.material = mat_regular;
            }
        }
    }

    public void setType(TileType newtype) {
        if(this.type != newtype)
            artDelta = true;
        this.type = newtype;
    }
    public void setGhost(bool val) {
        if(this.isGhost != val)
            artDelta = true;
        this.isGhost = val;
    }
    public void setWarning(bool val) {
        if(this.isWarning != val)
            artDelta = true;
        this.isWarning = val;
    }
    public void setWorldCoord(bool val) {
        if(this.isWorldCoord != val)
            artDelta = true;
        this.isWorldCoord = val;
    }
    public void setMaterial(Texture2D tex) {
        if(this.surfaceTex != tex)
            artDelta = true;
        this.surfaceTex = tex;
    }
    public void setPlayerColor(Color col) {
        if(this.playerColor != col)
            artDelta = true;
        this.playerColor = col;
    }
    public void setWorldTextureProps(Vector3 offset, Vector3 clamp, Vector3 scale) {
        if(this.worldCoordOffset != offset || this.worldCoordClamp != clamp || this.worldCoordScale != scale)
            artDelta = true;
        this.worldCoordOffset = offset;
        this.worldCoordClamp = clamp;
        this.worldCoordScale = scale;
    }

    public void toggleType() {
        type += 1;
        if((int)type > 3) {
            type = 0;
        }
        artDelta = true;
    }
}
