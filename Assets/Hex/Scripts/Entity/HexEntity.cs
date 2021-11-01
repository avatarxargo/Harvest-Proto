using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[ExecuteInEditMode]
public class HexEntity : MonoBehaviour
{
    public HexCoordinate coord;
    public HexCoordinate lastCoord;

    public EntityInfo entityInfo;
    public List<HexCoordinate> tris;

    public List<GameObject> meshes;
    public GameObject meshContainer;
    public TileShaderProperty meshShaderProperty;
    public bool isDelta = false;
    public HexStatus status = HexStatus.SEEKING;
    public List<Bilboard> indicators;

    public Bilboard inactiveIndicator;

    public Bilboard textIndicator;
    public bool generatedIndicators = false;

    // gizmos
    public bool showOccupancyIndicators = true;
    public bool showConnectivityIndicators = true;
    public bool showFlagIndicators = true;
    public bool showFlagIndicatorsFilterIO = true;

    public bool showEntityCenter = false;
    public bool calculatePath = true;
    public bool showCalculatedPath = false;
    public bool showPointPath = true;
    public bool showEntityLinkIndicators = true;
    public HexConnectivityResult calculatePathRes = new HexConnectivityResult();
    private bool calculatePathExists = false;
    public HexConnectivityResult pointValuePathRes = new HexConnectivityResult();
    private bool pointValuePathExists = false;


    // wait list
    public float waitTime = 0;
    public float waitTimeMax = 3;
    public bool isWaiting = false;
    public ProtoTile protoParent;
    public bool isGhost = false;
    public bool isWarning = false;
    public bool isOccupied = false;
    public bool isPlaced = false;

    public bool isValidPlacement = false;

    public int playerOwner = 0;
    public List<HexJobRecord> jobRecords = new List<HexJobRecord>();

    public bool isActive = true;
    public int openJobs = 0;
    public int completedJobs = 0;
    public int pointValue = 1;

    public Color playerColor = Color.white;

    // conditional elements
    ConditionalElement[] conditionals;

    public HexEntity() {
        tris = new List<HexCoordinate>();
        meshes = new List<GameObject>();
    }

    public void init(EntityInfo _entityInfo) {
        entityInfo = _entityInfo;
        spawnMeshes();
        forceRepaint();
        conditionals = GetComponentsInChildren<ConditionalElement>();
    }
    public void setGhost(bool val) {
        this.isGhost = val;
        RepaintMesh();
    }
    public void setWarning(bool val) {
        this.isWarning = val;
        RepaintMesh();
    }

    public void setColor(Color col) {
        this.playerColor = col;
        if(meshShaderProperty) {
            meshShaderProperty.setPlayerColor(playerColor);
        }
        RepaintMesh();
    }

    void RepaintMesh() {
        if(meshShaderProperty) {
            meshShaderProperty.setGhost(this.isGhost);
            meshShaderProperty.setWarning(this.isWarning);
        }
    }
    public void forceRepaint() {
        if(meshShaderProperty) {
            meshShaderProperty.artDelta = true;
        }
        RepaintMesh();
    }

    /// <summary>
    /// Says whether given global segment contains the entity on this hex.
    /// </summary>
    public bool testSegment(HexDirection gdir) {
        HexDirection ldir = directionGlobal2Internal(gdir);
        return entityInfo.occupy[(int)ldir] != OccupyType.EMPTY;
    }

    public void refreshActive() {
        isActive = true;
        completedJobs = 0;
        openJobs = 0;
        foreach (HexJobRecord job in jobRecords) {
            if(!job.completed) {
                isActive = false;
                openJobs++;
            } else {
                completedJobs++;
            }
        }
        if(entityInfo.autoActivate)
            isActive = true;
    }

    /// <summary>
    /// Update from the game system. If Delta happened, causes refreshing of stuff.
    /// </summary>
    public void gameUpdate(TileSystem tileSystem) {
        if(!generatedIndicators) {
            generatedIndicators = true;
            generateIcons(tileSystem);
        }
        if(!isGhost && !isActive && !inactiveIndicator) {
            string path =  "UiIcons/hexico_warn";
            inactiveIndicator = tileSystem.spawnPopup(this,HexDirection.HexEast, HexFlagType.Forest, Resources.Load<Sprite>(path));
            inactiveIndicator.expires = false;
            Vector2 ctr = getEntityCenter();
            inactiveIndicator.anchor = new Vector3(ctr.x, 1, ctr.y);
        }
        /*if(!textIndicator) {
            string path =  "UiIcons/hexico_fence";
            textIndicator = tileSystem.spawnPopup(this,Resources.Load<Sprite>(path));
            textIndicator.expires = false;
            Vector2 ctr = getEntityCenter();
            textIndicator.anchor = new Vector3(ctr.x, 1.2f, ctr.y);
            //textIndicator.setText("Hello");
        }*/
        if(isActive && inactiveIndicator) {
            Destroy(inactiveIndicator.gameObject);
            //inactiveIndicator = null;
        }
        if(isDelta) {
            this.snapToHexGrid(tileSystem.hexRadius);
            // finish
            isDelta = false;
            checkConditions(tileSystem);
            checkElements(tileSystem);
            updateWaiting();
            checkNeighborElements(tileSystem);
            setWarning(!isValidPlacement && !isPlaced);   
            if (entityInfo.pointValueOverrideTypeActive || entityInfo.pointValueOverrideFlagActive) {
                List<HexCoordinate> searchStart = new List<HexCoordinate>();
                for(int d = 0; d < 6; ++d) {
                    if(entityInfo.occupy[d] != OccupyType.EMPTY) {
                        searchStart.Add(HexDirectionUtil.rotate(coord,d));
                    }
                }
                pointValuePathRes = TriPathSolver.solveConnectionSurface(tileSystem,searchStart,entityInfo.pointValueOverrideTypeActive,entityInfo.pointValueOverrideType,entityInfo.pointValueOverrideFlag,true,false);
                pointValue = pointValuePathRes.path.Count;   
                pointValuePathExists = true;   
            } 
            if(calculatePath) {
                foreach (HexJobRecord job in jobRecords) {
                //foreach (HexFlag flag in entityInfo.flagsE) {
                    if(!job.completed /*&& HexFlagUtil.isInFlag(job.resource)*/) {
                        calculatePathRes = TriPathSolver.solveConnectionPath(tileSystem,HexDirectionUtil.rotate(coord,(int)job.direction),job.side,job.resource);
                        calculatePathExists = true;
                        if(calculatePathRes.reached.Count > 0) {
                            //Debug.Log("REACHED");
                            job.completed = true;
                            string path = HexFlag.getIco(job.resource);
                            if(job.resource == HexFlagType.AnyIn) {
                                path = HexFlag.getIco(HexFlagUtil.getMatchedPairOutIn(calculatePathRes.reachedType[0]));
                            }
                            path =  "UiIcons/" + path.Substring(0,path.Length-4);
                            tileSystem.spawnPopup(this/*calculatePathRes.reached[0], calculatePathRes.reachedCoord[0].section*/, job.direction, calculatePathRes.reachedType[0], Resources.Load<Sprite>(path));
                            Debug.Log(path);
                            TileSystem.HistoryRecord lastItem = tileSystem.getLatestHistoryItem();
                            if(!lastItem.isTile) {
                                job.completingPlayer = lastItem.entity.playerOwner;
                                tileSystem.playerData.playerInfo[job.completingPlayer].scores[HexFlagUtil.FlagToScoreId( calculatePathRes.reachedType[0])] += job.pointValue + calculatePathRes.reached[0].pointValue;
                                //tileSystem.playerData.playerInfo[job.establishingPlayer].scores[0] += job.pointValue + calculatePathRes.reached[0].pointValue;
                                tileSystem.playerData.scoreDelta = true;
                                refreshActive();
                            } else {
                                Debug.LogError("Somehow the job trigger event was a tile and not an entity!");
                            }
                        }
                    }
                }
            }
            lastCoord = coord;
            if(meshShaderProperty) {
                meshShaderProperty.refeshShader();
            }
            refreshActive();
        }
    }

    public void updateWaiting() {
        if(isValidPlacement && isGhost) {
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

    public void checkConditions(TileSystem tileSystem) {
        isValidPlacement = true;
        TriRequirement req = entityInfo.requirement;
        if(req!=null) {
            isValidPlacement = req.check(tileSystem, coord);
        }
        checkOverlap(tileSystem);
    }
    public void checkElements(TileSystem tileSystem) {
        if(conditionals == null) 
            conditionals = GetComponentsInChildren<ConditionalElement>();
        foreach(ConditionalElement ele in conditionals) {
            ele.refresh(tileSystem, coord);
        }
    }
    public void checkOverlap(TileSystem tileSystem) {
        isOccupied = false;
        if(isPlaced)
            return;
        for(int i = 0; i < 6; ++i) {
            if(entityInfo.occupy[i] != OccupyType.EMPTY) {
                HexDirection dir = (HexDirection)i;
                HexDirection gdir = directionInternal2Global(dir);
                HexCoordinate dirc = new HexCoordinate(lastCoord.x,lastCoord.y,lastCoord.level,gdir);

                List<HexEntity> entities = tileSystem.getEntitiesAt(dirc,true,false);
                if(entities.Count > 0)
                {
                    isOccupied = true;
                    isValidPlacement = false;
                    break;
                }
            }
        }
    }

    public void checkNeighborElements(TileSystem tileSystem) {
        List<HexEntity> entities = tileSystem.getEntitiesOnHex(coord);
        foreach(HexEntity ent in entities)
            ent.checkElements(tileSystem);
        List<HexEntity> lentities = tileSystem.getEntitiesOnHex(lastCoord);
        foreach(HexEntity ent in lentities)
            ent.checkElements(tileSystem);
        for(int i = 0; i < 6; ++i) {
            HexDirection dir = (HexDirection)i;
            List<HexEntity> nentities = tileSystem.getEntitiesOnHex(coord.offset(dir));
            foreach(HexEntity ent in nentities)
                ent.checkElements(tileSystem);
            List<HexEntity> lnentities = tileSystem.getEntitiesOnHex(lastCoord.offset(dir));
            foreach(HexEntity ent in lnentities)
                ent.checkElements(tileSystem);
        }
    }

    public void spawnMeshes() {
        if(!meshContainer) {
            meshContainer = new GameObject();
            meshContainer.name = "Mesh Container";
            //meshContainer.AddComponent<TileShaderProperty>();
            meshContainer.transform.SetParent(this.transform);
        }
        if(meshes.Count <= 0 && entityInfo) {
            foreach( GameObject m in entityInfo.meshes) {
                //GameObject blankMesh = Resources.Load("EntityMesh") as GameObject;
                GameObject inst = Instantiate(m, m.transform.position, m.transform.rotation, meshContainer.transform);
                inst.transform.localScale = new Vector3(0.6f,0.6f,0.6f);
                //MeshFilter mf = inst.GetComponents<MeshFilter>()[0];
                //mf.mesh = m;
                meshes.Add(inst);
            }
        }
        conditionals = GetComponentsInChildren<ConditionalElement>();
        meshShaderProperty = meshContainer.GetComponentInChildren<TileShaderProperty>();
        isDelta = true;
    }

    public void updateJobs(TileSystem tileSystem) {
        this.isDelta = true;
    }
    public void generateIcons(TileSystem tileSystem) {
        for(int i = 0; i < 6; ++i) {
            HexDirection glob = directionInternal2Global((HexDirection)i);
            HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);
            List<HexFlag> dirFlags = null;
            switch((HexDirection)i) {
                default:
                case HexDirection.HexEast:
                    dirFlags = entityInfo.flagsE;
                    break;
                case HexDirection.HexSouthEast:
                    dirFlags = entityInfo.flagsSE;
                    break;
                case HexDirection.HexSouthWest:
                    dirFlags = entityInfo.flagsSW;
                    break;
                case HexDirection.HexWest:
                    dirFlags = entityInfo.flagsW;
                    break;
                case HexDirection.HexNorthWest:
                    dirFlags = entityInfo.flagsNW;
                    break;
                case HexDirection.HexNorthEast:
                    dirFlags = entityInfo.flagsNE;
                    break;
            }
            foreach(HexFlag flag in dirFlags) {
                if(flag.flag != HexFlagType.RailIO && flag.flag != HexFlagType.PipeIO && flag.flag != HexFlagType.Forest) {
                    string path = HexFlag.getIco(flag.flag);
                    path =  "UiIcons/" + path.Substring(0,path.Length-4);
                    Bilboard jbil = tileSystem.spawnPopup(this,(HexDirection)i,flag.flag,Resources.Load<Sprite>(path));
                    jbil.setFixedAnchor(this, (HexDirection)i, flag.side);
                } 
            }
        }
    }

    private void flagToJob(TileSystem tileSystem, HexFlag flag, HexDirection dir) {
        if(HexJobRecord.isJobFlag(flag.flag)) {
            HexJobRecord job = new HexJobRecord();
            job.completed = false;
            job.direction = dir;
            job.side = flag.side;
            job.resource = flag.flag;
            job.establishingPlayer = playerOwner;
            jobRecords.Add(job);
            //Debug.Log("created job: "+job.resource);
            //Debug.Log("job dir "+job.direction);
        }
    }

    public void generateJobs(TileSystem tileSystem) {
        if(jobRecords.Count <= 0) {
            foreach(HexFlag flag in entityInfo.flagsE)
                flagToJob(tileSystem, flag, HexDirection.HexEast);
            foreach(HexFlag flag in entityInfo.flagsSE)
                flagToJob(tileSystem, flag, HexDirection.HexSouthEast);
            foreach(HexFlag flag in entityInfo.flagsSW)
                flagToJob(tileSystem, flag, HexDirection.HexSouthWest);
            foreach(HexFlag flag in entityInfo.flagsW)
                flagToJob(tileSystem, flag, HexDirection.HexWest);
            foreach(HexFlag flag in entityInfo.flagsNW)
                flagToJob(tileSystem, flag, HexDirection.HexNorthWest);
            foreach(HexFlag flag in entityInfo.flagsNE)
                flagToJob(tileSystem, flag, HexDirection.HexNorthEast);
            if(jobRecords.Count > 0)
                isActive = false;
            if(entityInfo.autoActivate)
                isActive = true;
        }
    }

    public void snapToHexGrid(float hexRadius) {
        coord = HexCoordinate.snapToHexGrid(this.transform, hexRadius);
        Vector2 realcoord = coord.toPosition(hexRadius);
        float snaprot = (int)coord.section*60.0f;
        /*foreach( GameObject m in meshes) {
            m.transform.position = new Vector3(realcoord.x, m.transform.position.y, realcoord.y);
            m.transform.eulerAngles = new Vector3(0,snaprot,0);
               
        }*/
        if(meshContainer) {
            float tgtheight = this.transform.position.y + 0.25f;
            if(isOccupied)
                tgtheight += 0.5f;
            meshContainer.transform.position = new Vector3(realcoord.x, tgtheight/*meshContainer.transform.position.y*/, realcoord.y);
            meshContainer.transform.eulerAngles = new Vector3(0,snaprot,0);
        }
        /*if(coord.x != lastCoord.x || coord.y != lastCoord.y)
            isDelta = true;*/
    }
    public Vector2 getEntityCenter() {
        Vector2 spot = new Vector3(0,0);
        int count = 0;
        for(int i = 0; i < 6; ++i) {
            //HexDirection dir = (HexDirection)i;
            if(entityInfo.occupy[i] != OccupyType.EMPTY) {
                count++;
                HexCoordinate seg = HexDirectionUtil.rotate(coord,i);
                spot = spot + HexCoordinate.getHexSectionCenter(seg);
            }
        }
        if(count != 0) {
            spot /= count;
        }
        return spot;
    }

    private void LateUpdate()
    {
        if (this.transform.hasChanged)
        {
            isDelta = true;
            this.transform.hasChanged = false;
        }
    }
    private void OnDrawGizmos() {
        if(showOccupancyIndicators)
            for(int i = 0; i < 6; ++i) {
                HexDirection glob = directionInternal2Global((HexDirection)i);
                HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);
                switch(entityInfo.occupy[i]) {
                case OccupyType.EMPTY:
                    break;
                case OccupyType.WATER:
                    HexCoordinate.visualizeTri(dirc,0.7f, new Color(0,0,1,1));
                    break;
                case OccupyType.STRUCTURE:
                    HexCoordinate.visualizeTri(dirc,0.7f, new Color(0.65f,0.8f,0.3f,1));
                    break;
                default:
                    HexCoordinate.visualizeTri(dirc,0.7f, new Color(1,1,1,1));
                    break;
                }
            }
        if(showConnectivityIndicators)
            foreach(EntityConnectionPair pair in entityInfo.connectionMap) {
                /*HexDirection glob = directionInternal2Global(coord.section);
                HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);*/
                HexCoordinate.visualizeConnectionPair(coord, pair, 0.7f, new Color(pair.connectionType==HexFlagType.RailIO?1:0,pair.connectionType==HexFlagType.RailIO?1:0,pair.connectionType==HexFlagType.RailIO?0:1,1));
            }
        if(showFlagIndicators) {
            for(int i = 0; i < 6; ++i) {
                HexDirection glob = directionInternal2Global((HexDirection)i);
                HexCoordinate dirc = new HexCoordinate(coord.x,coord.y,coord.level,glob);
                List<HexFlag> dirFlags = null;
                switch((HexDirection)i) {
                    default:
                    case HexDirection.HexEast:
                        dirFlags = entityInfo.flagsE;
                        break;
                    case HexDirection.HexSouthEast:
                        dirFlags = entityInfo.flagsSE;
                        break;
                    case HexDirection.HexSouthWest:
                        dirFlags = entityInfo.flagsSW;
                        break;
                    case HexDirection.HexWest:
                        dirFlags = entityInfo.flagsW;
                        break;
                    case HexDirection.HexNorthWest:
                        dirFlags = entityInfo.flagsNW;
                        break;
                    case HexDirection.HexNorthEast:
                        dirFlags = entityInfo.flagsNE;
                        break;
                }
                foreach(HexFlag flag in dirFlags) {
                    if(!showFlagIndicatorsFilterIO || (flag.flag != HexFlagType.RailIO && flag.flag != HexFlagType.PipeIO))
                    flag.drawGizmo(dirc);
                }
            }
        }
        if(showCalculatedPath && calculatePathExists) {
            foreach(HexCoordinate co in calculatePathRes.path) {
                HexCoordinate.visualizeTri(co,1,Color.red);
            }
            foreach(HexEntity en in calculatePathRes.reached) {
                HexCoordinate.visualizeTri(en.coord,1,Color.green);
            }
        }
        if(showPointPath && pointValuePathExists) {
            foreach(HexCoordinate co in pointValuePathRes.path) {
                HexCoordinate.visualizeTri(co,1.05f,Color.magenta);
            }
        }
        if(showEntityLinkIndicators && calculatePathExists) {
            for(int i = 0; i < calculatePathRes.reached.Count; ++i) {
                HexEntity en = calculatePathRes.reached[i];
                HexCoordinate enc = calculatePathRes.reachedCoord[i];
                TriDirection end = calculatePathRes.reachedDir[i];
                HexCoordinate.visualizeTriLink(calculatePathRes.sourceConnectionCoord,calculatePathRes.sourceConnectionDir,enc,end,0.7f,Color.green);
            }
        }
        if(showEntityCenter) {
            Vector2 spot = getEntityCenter();
            Vector3 a = new Vector3(spot.x,0.5f,spot.y);
            Vector3 b = new Vector3(spot.x,1.3f,spot.y);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(a,b);
        }
    }
    void OnValidate() {
        RepaintMesh();
    }
    
    public List<HexFlagType> getFlags(HexCoordinate tstCoord, TriDirection dir) {
        List<HexFlagType> found = new List<HexFlagType>();
        HexDirection internalDir = this.directionGlobal2Internal(tstCoord.section);
        List<HexFlag> dirFlags = null;
        switch(internalDir) {
            default:
            case HexDirection.HexEast:
                dirFlags = entityInfo.flagsE;
                break;
            case HexDirection.HexSouthEast:
                dirFlags = entityInfo.flagsSE;
                break;
            case HexDirection.HexSouthWest:
                dirFlags = entityInfo.flagsSW;
                break;
            case HexDirection.HexWest:
                dirFlags = entityInfo.flagsW;
                break;
            case HexDirection.HexNorthWest:
                dirFlags = entityInfo.flagsNW;
                break;
            case HexDirection.HexNorthEast:
                dirFlags = entityInfo.flagsNE;
                break;
        }
        foreach(HexFlag flag in dirFlags) {
            if(flag.side == dir) {
                found.Add(flag.flag);
            }
        }
        return found;
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
