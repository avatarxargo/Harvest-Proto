using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SelectionBase]
[ExecuteInEditMode]
public class HexTile : MonoBehaviour
{

    // general properties
    public Guid id { get; }
    public HexCoordinate coord;
    private HexCoordinate lastCoord;
    public bool isDelta = false;
    public bool isGhost = false;
    public bool isWarning = false;
    public bool isPlaced = false;

    public HexStatus status = HexStatus.SEEKING;
    public TileInfo tileInfo;
    public List<HexEntity> entities;

    // visualization
    public HexMesh hexMesh;
    public TileShaderProperty[] hexMeshShaders;
    private GameObject[] connectors = new GameObject[6]{null,null,null,null,null,null};
    private DynamicTriangle[] triangles = new DynamicTriangle[6]{null,null,null,null,null,null};
    private ConnectorShaderProperty[] connectorsShader = new ConnectorShaderProperty[6];

    //private ConditionalElement[] autoConnectors = new ConditionalElement[6]{null,null,null,null,null,null};
    public bool showFlagIndicators = true;
    public int validConnectors = 6;
    public int attachedConnectors = 0;

    public bool isOccupied = false;
    public int playerOwner = 0;

    // wait list
    public float waitTime = 0;
    public float waitTimeMax = 3;
    public bool isWaiting = false;
    public ProtoTile protoParent;


    public HexTile() {
        id = Guid.NewGuid();
    }

    public void init(TileInfo _tileInfo) {
        tileInfo = _tileInfo;
        RefreshComponents();
        forceRepaint();
    }

    public void setGhost(bool val) {
        this.isGhost = val;
        RepaintMesh();
    }
    public void setWarning(bool val) {
        this.isWarning = val;
        RepaintMesh();
    }

    void RefreshComponents() {
        HexMesh tmp = this.GetComponentInChildren<HexMesh>();
        if(tmp != null)
            hexMesh = tmp;
        DynamicTriangle[] tris = this.GetComponentsInChildren<DynamicTriangle>();
        List<TileShaderProperty> tmp2 = new List<TileShaderProperty>();
        foreach(DynamicTriangle dt in tris) {
            TileShaderProperty tsp = dt.GetComponent<TileShaderProperty>();
            if(tsp)
                tmp2.Add(tsp);
        }
        hexMeshShaders = tmp2.ToArray();
    }

    void RepaintMesh() {
        foreach (TileShaderProperty hexMeshShader in hexMeshShaders) {
            hexMeshShader.setGhost(this.isGhost);
            hexMeshShader.setWarning(this.isWarning);
        }
        for(int i = 0; i < 6; ++i) {
            if(connectors[i]) {
                connectorsShader[i].setGhost(this.isGhost);
            }
        }
    }

    public void forceRepaint() {
        RefreshComponents();
        foreach (TileShaderProperty hexMeshShader in hexMeshShaders) {
            hexMeshShader.artDelta = true;
        }
        RepaintMesh();
    }

    void OnValidate() {
        RefreshComponents();
        RepaintMesh();
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
        if(isDelta) {
            this.snapToHexGrid(tileSystem.hexRadius);
            updateConnectors(tileSystem);
            updateConnectorsNeighbors(tileSystem, !(lastCoord%coord));
            updateDynamicTriangles(tileSystem);
            updateWaiting();
            // finish
            lastCoord = coord;
            isDelta = false;
            foreach(TileShaderProperty tsp in hexMeshShaders) {
                tsp.refeshShader();
            }
            /*for(int i = 0; i < 6; ++i) {
                if(connectors[i]) {
                    //connectorsShader[i].refeshShader();
                }
            }*/
        }
    }

    // =============== commands ====================
    public void snapParentToHexGrid(float hexRadius) {
        snapToHexGrid(hexRadius);
        if(hexMesh) {
            transform.position = new Vector3(hexMesh.transform.position.x,transform.position.y,hexMesh.transform.position.z);
            transform.eulerAngles = hexMesh.transform.eulerAngles;
        }
    }

    public void snapToHexGrid(float hexRadius) {
        if(!hexMesh)
            return;

        coord = HexCoordinate.snapToHexGrid(this.transform, hexRadius);
        Vector2 realcoord = coord.toPosition(hexRadius);
        float tgtheight = this.transform.position.y;
        if(isOccupied)
            tgtheight += 0.5f;
        hexMesh.transform.position = new Vector3(realcoord.x, tgtheight, realcoord.y);
        float snaprot = (int)coord.section*60.0f;
        hexMesh.transform.eulerAngles = new Vector3(0,snaprot,0);
        if(coord.x != lastCoord.x || coord.y != lastCoord.y)
            isDelta = true;
    }

    public void updateWaiting() {
        if(validConnectors >= 6 && attachedConnectors > 0 && isGhost && !isOccupied) {
            // it's attachment time.
            if(!isWaiting) {
                waitTime = 0;
                isWaiting = true;
            }
            if(!(lastCoord^coord)) {
                waitTime = 0;
            }
            waitTime += Time.deltaTime;
            protoParent.isDelta = true;
            if(waitTime > waitTimeMax) {
                isWaiting = false;
                if(protoParent) {
                    protoParent.requestPlacement = true;
                }
            }
        } else {
            isWaiting = false;
            waitTime = 0;
        }
    }

    public void updateConnectors(TileSystem tileSystem) {
        validConnectors = 0;
        attachedConnectors = 0;

        List<HexTile> mytiles = tileSystem.getTilesAt(coord, true, false);
        isOccupied = false;
        if(isPlaced)
            return;
        foreach(HexTile mt in mytiles) {
            if(mt != this) {
                isOccupied = true;
                break;
            }
        }

        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            HexDirection gdir = directionInternal2Global(dir);
            HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,gdir);
            HexCoordinate other = dirc.getTriNeighbor(TriDirection.Back);

            if(!isOccupied) {
                List<HexTile> ntiles_ghost = tileSystem.getTilesAt(other,false,true);
                List<HexTile> ntiles_regular = tileSystem.getTilesAt(other,true,false);
                if(ntiles_regular.Count > 0 || ntiles_ghost.Count > 0) {
                    bool ghostlyn = false;
                    HexTile first = null;
                    if(ntiles_regular.Count > 0)
                        first = ntiles_regular[0];
                    else {
                        first = ntiles_ghost[0];
                        ghostlyn = true;
                    }
                    TileType mytype = TileTypeUtil.getEffectiveType(tileInfo.segmentType[i]);
                    TileType otherType = TileTypeUtil.getEffectiveType(first.tileInfo.segmentType[(int)first.directionGlobal2Internal(other.section)]);
                    bool untamed = mytype == TileType.Untamed || otherType == TileType.Untamed;
                    bool fieldPermission = (mytype == TileType.Grass && otherType == TileType.Fertile) || (otherType == TileType.Grass && mytype == TileType.Fertile);
                    bool sandPermission = (mytype == TileType.Grass && otherType == TileType.Sand) || (otherType == TileType.Grass && mytype == TileType.Sand);
                    if(otherType != mytype && !untamed && !fieldPermission && !sandPermission) {
                        // mismatch
                        if(!ghostlyn)
                            attachedConnectors++;
                        this.spawnConnector(dir, false, true);
                    } else {
                        // match
                        validConnectors++;
                        if(!ghostlyn)
                            attachedConnectors++; 
                        this.despawnConnector(dir);
                    }
                } else {
                    // open
                    validConnectors++;
                    this.spawnConnector(dir); 
                }
            } else { //is Occupied
                this.despawnConnector(dir);
            }
        }
        setWarning(validConnectors < 6 || isOccupied);
        RepaintMesh();
    }
    public void updateConnectorsNeighbors(TileSystem tileSystem, bool movedPlaces = false) {
        /*if(isOccupied) {
            List<HexTile> ntiles = tileSystem.getTilesAt(coord);
            if(ntiles.Count > 0) {
                HexTile first = ntiles[0];
                first.updateConnectors(tileSystem);
            }
        }*/

        for(int i = 0; i < 6; ++i) {
            /*current pos*/ {
                HexDirection dir = (HexDirection)i;
                HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,dir);
                HexCoordinate other = dirc.getTriNeighbor(TriDirection.Back);
                List<HexTile> ntiles = tileSystem.getTilesAt(other);
                if(ntiles.Count > 0) {
                    HexTile first = ntiles[0];
                    first.updateConnectors(tileSystem);
                    first.updateDynamicTriangles(tileSystem);
                }
            }
            if(movedPlaces) {
                HexDirection dir = (HexDirection)i;
                HexCoordinate dirc = new HexCoordinate(lastCoord.x,lastCoord.y,lastCoord.level,dir);
                HexCoordinate other = dirc.getTriNeighbor(TriDirection.Back);
                List<HexTile> ntiles = tileSystem.getTilesAt(other);
                if(ntiles.Count > 0) {
                    HexTile first = ntiles[0];
                    first.updateConnectors(tileSystem);
                    first.updateDynamicTriangles(tileSystem);
                }
            }
        }
    }

    public void updateDynamicTriangles(TileSystem tileSystem) {
        spawnDynamicTriangles();

        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            HexDirection gdir = directionInternal2Global(dir);
            HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,gdir);

            if(triangles[i]) {
                triangles[i].gameUpdate(tileSystem, dirc);
            }        
        }
    }

    public void spawnDynamicTriangles() {
        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            if(hexMesh) {
                if(!triangles[i]) {
                    GameObject trianglePrefab = Resources.Load("dynamicTriangle") as GameObject;
                    float off = 0.6f;
                    //triangles[i] = Instantiate(trianglePrefab, this.hexMesh.transform.position + new Vector3(Mathf.Sin(Mathf.PI*(i/3.0f+0.5f))*off,0.50f,Mathf.Cos(Mathf.PI*(i/3.0f+0.5f))*off), this.hexMesh.transform.rotation * Quaternion.Euler(0, 60*i, 0), this.hexMesh.transform).GetComponent<DynamicTriangle>();
                    triangles[i] = Instantiate(trianglePrefab, this.hexMesh.transform.position + new Vector3(Mathf.Sin(Mathf.PI*(i/3.0f+0.5f))*off,0.60f,Mathf.Cos(Mathf.PI*(i/3.0f+0.5f))*off), this.hexMesh.transform.rotation * Quaternion.Euler(0, -30+60*i, 0), this.hexMesh.transform).GetComponent<DynamicTriangle>();
                    triangles[i].gameObject.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
                    triangles[i].init();
                    triangles[i].refeshShader();
                    if(tileInfo.segmentType[i] == TileType.Untamed) {
                        triangles[i].gameObject.SetActive(false);
                    } else {
                        triangles[i].setType(tileInfo.segmentType[i]);
                        triangles[i].setLandmark(dir);
                    }
                }
            }
        }
        RefreshComponents();
    }

    /*public void spawnAutoconnectors() {
        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            this.spawnAutoconnector(dir);
        }
    }*/
    /*private void spawnAutoconnector(HexDirection dir) {
        HexDirection internalDir = dir;
        int index = (int) internalDir;
        if(hexMesh) {
            if(!autoConnectors[index]) {
                GameObject connectorPrefab = Resources.Load("Autoconnector") as GameObject;
                autoConnectors[index] = Instantiate(connectorPrefab, this.hexMesh.transform.position + new Vector3(0,0.25f,0), this.hexMesh.transform.rotation * Quaternion.Euler(0, -90+60*index, 0), this.hexMesh.transform).GetComponent<ConditionalElement>();
            }
            connectorsShader[index] = autoConnectors[index].GetComponent<ConnectorShaderProperty>();
            connectorsShader[index].setType(tileInfo.segmentType[index]);
            connectorsShader[index].setGhost(this.isGhost);
            connectorsShader[index].setWarning(isWarning);
        }
    }*/
    /*public void updateAutoconnectors(TileSystem tileSystem) {
        validConnectors = 0;
        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            HexDirection gdir = directionInternal2Global(dir);
            HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,gdir);
            HexCoordinate other = dirc.getTriNeighbor(TriDirection.Back);

            if(autoConnectors[i]) {
                autoConnectors[i].refresh(tileSystem, dirc);
            }

            List<HexTile> ntiles = tileSystem.getTilesAt(other);
            if(ntiles.Count > 0) {
                HexTile first = ntiles[0];
                TileType mytype = tileInfo.segmentType[i];
                TileType otherType = first.tileInfo.segmentType[(int)first.directionGlobal2Internal(other.section)];
                if(otherType != mytype)
                    // mismatch
                    if(connectorsShader[i] != null) {
                        connectorsShader[i].setWarning(true);
                        connectorsShader[i].setGhost(isGhost);
                    }
                else {
                    // match
                    validConnectors++;
                }
            } else {
                // open
                validConnectors++;
                if(connectorsShader[i] != null){
                    connectorsShader[i].setWarning(false);
                    connectorsShader[i].setGhost(isGhost);
                }
            }
        }
        if(hexMeshShader) {
            setWarning(validConnectors < 6);
            RepaintMesh();
        }
    }*/
    // -------------
    public void clearAllConnectors() {
        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            this.despawnConnector(dir);
        }
    }
    private void spawnConnector(HexDirection dir, bool isGlobal = false, bool isWarning = false) {
        return;
        HexDirection internalDir = dir;
        if(isGlobal)
            internalDir = this.directionGlobal2Internal(dir);
        int index = (int) internalDir;
        if(hexMesh) {
            if(!connectors[index]) {
                GameObject connectorPrefab = Resources.Load("Connector") as GameObject;
                connectors[index] = Instantiate(connectorPrefab, this.hexMesh.transform.position + new Vector3(0,0.25f,0), this.hexMesh.transform.rotation * Quaternion.Euler(0, -90+60*index, 0), this.hexMesh.transform);
            }
            connectorsShader[index] = connectors[index].GetComponent<ConnectorShaderProperty>();
            connectorsShader[index].setType(tileInfo.segmentType[index]);
            connectorsShader[index].setGhost(this.isGhost);
            connectorsShader[index].setWarning(isWarning);
        }
    }
    private void despawnConnector(HexDirection dir, bool isGlobal = false) {
        return;
         HexDirection internalDir = dir;
        if(isGlobal)
            internalDir = this.directionGlobal2Internal(dir);
        int index = (int) internalDir;
        if(connectors[index]) {
            if( Application.isEditor )
                DestroyImmediate(connectors[index]);
            else
                Destroy(connectors[index]);
            connectors[index] = null;
            connectorsShader[index] = null;
        }
    }
    public List<HexFlagType> getFlags(HexCoordinate tstCoord, TriDirection dir) {
        List<HexFlagType> found = new List<HexFlagType>();
        HexDirection internalDir = this.directionGlobal2Internal(tstCoord.section);
        List<HexFlag> dirFlags = null;
        switch(internalDir) {
            default:
            case HexDirection.HexEast:
                dirFlags = tileInfo.flagsE;
                break;
            case HexDirection.HexSouthEast:
                dirFlags = tileInfo.flagsSE;
                break;
            case HexDirection.HexSouthWest:
                dirFlags = tileInfo.flagsSW;
                break;
            case HexDirection.HexWest:
                dirFlags = tileInfo.flagsW;
                break;
            case HexDirection.HexNorthWest:
                dirFlags = tileInfo.flagsNW;
                break;
            case HexDirection.HexNorthEast:
                dirFlags = tileInfo.flagsNE;
                break;
        }
        foreach(HexFlag flag in dirFlags) {
            if(flag.side == dir) {
                found.Add(flag.flag);
            }
        }
        return found;
    }

    private void OnDrawGizmos() {
        /*for(int i = 0; i < 6; ++i) {
            HexDirection glob = directionInternal2Global((HexDirection)i);
            HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);
            HexCoordinate other = dirc.getTriNeighbor(TriDirection.Back);
            if(tileInfo.segmentType[i] == TileType.Sand) {
                HexCoordinate.visualizeTri(dirc,0.5f);
                HexCoordinate.visualizeTri(other,0.5f);
            }
        }*/
        if(showFlagIndicators) {
            for(int i = 0; i < 6; ++i) {
                HexDirection glob = directionInternal2Global((HexDirection)i);
                HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);
                List<HexFlag> dirFlags = null;
                switch((HexDirection)i) {
                    default:
                    case HexDirection.HexEast:
                        dirFlags = tileInfo.flagsE;
                        break;
                    case HexDirection.HexSouthEast:
                        dirFlags = tileInfo.flagsSE;
                        break;
                    case HexDirection.HexSouthWest:
                        dirFlags = tileInfo.flagsSW;
                        break;
                    case HexDirection.HexWest:
                        dirFlags = tileInfo.flagsW;
                        break;
                    case HexDirection.HexNorthWest:
                        dirFlags = tileInfo.flagsNW;
                        break;
                    case HexDirection.HexNorthEast:
                        dirFlags = tileInfo.flagsNE;
                        break;
                }
                foreach(HexFlag flag in dirFlags) {
                    flag.drawGizmo(dirc, -1);
                }
            }
        }
    }

    // ======================= UTIL =========================

    /// <summary>
    /// Converts world aligned direction to one relative to this piece's rotation (so when my west is rotateted 3 times it becomes east internaly)
    /// </summary>
    public HexDirection directionGlobal2Internal(HexDirection global) {
        return HexDirectionUtil.rotate(global,-(int)this.coord.section);
        //return (HexDirection)(((int)global+6-this.coord.level)%6);
    }
    
    /// <summary>
    /// Converts internally aligned direction to one relative to this piece's rotation (so when my west is rotateted 3 times it becomes east internaly)
    /// </summary>
    public HexDirection directionInternal2Global(HexDirection global) {
        return HexDirectionUtil.rotate(global,(int)this.coord.section);
        //return (HexDirection)(((int)global+this.coord.level)%6);
    }

    public HexDirection directionGlobal2Internal(Vector3Int pos, HexDirection global) {
        return (HexDirection)(((int)global+6-pos.z)%6);
    }

    public HexDirection directionInternal2Global(Vector3Int pos, HexDirection global) {
        return (HexDirection)(((int)global+pos.z)%6);
    }

}
