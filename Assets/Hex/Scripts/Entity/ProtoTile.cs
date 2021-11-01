using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ProtoType {
    HEX, ENTITY
}

[ExecuteInEditMode]
public class ProtoTile : MonoBehaviour
{
    public Guid id { get; }
    public ProtoType type = ProtoType.HEX;
    public TileInfo tileInfo;
    public HexTile projectedTile;
    public EntityInfo entityInfo;
    public HexEntity projectedEntity;
    public bool placed = false;
    public bool requestPlacement = false;
    public bool isDelta = false;

    private MaterialPropertyBlock propertyBlock;

    public ProtoTile() {
        id = Guid.NewGuid();
    }

    private void OnValidate() {
        refreshTexture();
    }

    public void initializeTI(TileInfo _tileInfo) {
        tileInfo = _tileInfo;
        refreshTexture();
    }
    public void initializeEI(EntityInfo _entityInfo) {
        entityInfo = _entityInfo;
        refreshTexture();
    }
    
    void refreshTexture()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        Renderer renderer = GetComponentInChildren<Renderer>();
        if(tileInfo && type == ProtoType.HEX) {
            if(projectedTile)
                propertyBlock.SetFloat("progress", (projectedTile.waitTime/projectedTile.waitTimeMax));
            else
                propertyBlock.SetFloat("progress", 0);
            propertyBlock.SetTexture("tex", tileInfo.cardTex);
        }
        if(entityInfo && type == ProtoType.ENTITY) {
            if(projectedEntity)
                propertyBlock.SetFloat("progress", (projectedEntity.waitTime/projectedEntity.waitTimeMax));
            else
                propertyBlock.SetFloat("progress", 0);
            propertyBlock.SetTexture("tex", entityInfo.protoTex);
        }
        renderer.SetPropertyBlock(propertyBlock);
    }

    /// <summary>
    /// Update from the game system. If Delta happened, causes refreshing of stuff.
    /// </summary>
    public void gameUpdate(TileSystem tileSystem) {
        if (this.transform.hasChanged)
        {
            isDelta = true;
            this.transform.hasChanged = false;
        }
        refreshTexture();
        if(isDelta) {
            if(projectedTile) {
                projectedTile.transform.position = new Vector3(this.transform.position.x,0,this.transform.position.z);
                projectedTile.transform.rotation = this.transform.rotation;
                projectedTile.isDelta = true;
            }
            if(projectedEntity) {
                projectedEntity.transform.position = new Vector3(this.transform.position.x,0,this.transform.position.z);
                projectedEntity.transform.rotation = this.transform.rotation;
                projectedEntity.isDelta = true;
            }
            isDelta = false;
        }
    }

    /*private void LateUpdate()
    {
        if (this.transform.hasChanged)
        {
            isDelta = true;
            this.transform.hasChanged = false;
        }
    }*/
}
