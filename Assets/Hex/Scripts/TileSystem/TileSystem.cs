using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileSystem : MonoBehaviour
{
    public GameObject holderProtoTile;
    public GameObject holderGhostTile;
    public GameObject holderBoardTile;
    public GameObject holderPopup;
    public Camera cam;
    public PlayerData playerData;

    public float hexRadius = 1.1f;
    //List<HexTile> registeredTiles;
    //Dictionary<Guid,HexTile> tileIdMap;

    public List<HexTile> historyTiles;
    public List<HexEntity> historyEntities;
    public List<bool> historyIsTile;

    private void OnValidate() {
        init();
    }

    public void init() {
       /* registeredTiles = new List<HexTile>();
        tileIdMap = new Dictionary<Guid,HexTile>();*/
        historyTiles = new List<HexTile>();
        historyEntities = new List<HexEntity>();
        historyIsTile = new List<bool>();
    }

    public bool gameUpdate() {
        if(updateProto())
            return true;
        updateGhost();
        updateBoard();
        return false;
    }

    private bool updateProto() {
        ProtoTile[] protos = holderProtoTile.GetComponentsInChildren<ProtoTile>();
        List<ProtoTile> toDelete = new List<ProtoTile>();
        bool placed = false;
        foreach(ProtoTile proto in protos) {
            proto.gameUpdate(this);
            if(proto.requestPlacement) {
                if(proto.type == ProtoType.HEX) {
                    if(proto.projectedTile) {
                        HexTile tile = proto.projectedTile;
                        if(tile.validConnectors >= 6) {
                            tile.setGhost(false);
                            tile.snapToHexGrid(hexRadius);
                            tile.updateConnectors(this);
                            tile.updateConnectorsNeighbors(this);
                            tile.status = HexStatus.ANCHORED;
                            tile.isPlaced = true;
                            tile.isDelta = true;
                            tile.transform.SetParent(holderBoardTile.transform);
                            tile.spawnDynamicTriangles();
                            tile.forceRepaint();
                            //
                            toDelete.Add(proto);
                            historyTiles.Add(tile);
                            historyIsTile.Add(true);
                            placed = true;
                            break;
                        }
                    }
                }
                if(proto.type == ProtoType.ENTITY) {
                    if(proto.projectedEntity) {
                        HexEntity entity = proto.projectedEntity;
                        if(entity) {
                            entity.checkConditions(this);
                            bool valid = entity.isValidPlacement;
                            if(valid) {
                                entity.setGhost(false);
                                entity.snapToHexGrid(hexRadius);
                                entity.status = HexStatus.ANCHORED;
                                entity.isPlaced = true;
                                entity.isDelta = true;
                                entity.transform.SetParent(holderBoardTile.transform);
                                toDelete.Add(proto);
                                historyEntities.Add(entity);
                                historyIsTile.Add(false);
                                placed = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        foreach(ProtoTile d in toDelete) {
            Destroy(d.gameObject);
        }
        toDelete.Clear();
        return placed;
    }

    private void updateGhost() {
        /*Tile*/ {
            HexTile[] ghosts = holderGhostTile.GetComponentsInChildren<HexTile>();
            List<HexTile> toDelete = new List<HexTile>();
            foreach(HexTile ghost in ghosts) {
                ghost.gameUpdate(this);
            }
            foreach(HexTile d in toDelete) {
                Destroy(d.gameObject);
            }
            toDelete.Clear();
        }
        /*Entity*/ {
            HexEntity[] ghosts = holderGhostTile.GetComponentsInChildren<HexEntity>();
            List<HexEntity> toDelete = new List<HexEntity>();
            foreach(HexEntity ghost in ghosts) {
                ghost.gameUpdate(this);
            }
            foreach(HexEntity d in toDelete) {
                Destroy(d.gameObject);
            }
            toDelete.Clear();
        }
    }
    private void updateBoard() {
        /*Tile*/ {
            HexTile[] tiles = holderBoardTile.GetComponentsInChildren<HexTile>();
            List<HexTile> toDelete = new List<HexTile>();
            foreach(HexTile tile in tiles) {
                tile.gameUpdate(this);
            }
            foreach(HexTile d in toDelete) {
                Destroy(d.gameObject);
            }
            toDelete.Clear();
        }
        /*Entity*/ {
            HexEntity[] entities = holderBoardTile.GetComponentsInChildren<HexEntity>();
            List<HexEntity> toDelete = new List<HexEntity>();
            foreach(HexEntity entity in entities) {
                entity.gameUpdate(this);
            }
            foreach(HexEntity d in toDelete) {
                Destroy(d.gameObject);
            }
            toDelete.Clear();
        }
    }

    public void updateJobs() {
        HexEntity[] entities = holderBoardTile.GetComponentsInChildren<HexEntity>();
        foreach(HexEntity entity in entities) {
            entity.updateJobs(this);
        }
    }

    // ==== fetching neighbors ====
    /*public List<HexCoordinate> getAdjacentTiles(HexCoordinate coord) {
        List<HexCoordinate> neighbors = new List<HexCoordinate>();

        //bool oddx = 0==coord.x%2;
        bool oddy = 0==Mathf.Abs(coord.y)%2;

        HexCoordinate current;
        
        if(oddy) {
            current = coord+new Vector2Int(1,0);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(0,1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(0,-1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(-1,0);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(1,1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(1,-1);
            if(coordinateExists(current))
                neighbors.Add(current);
        }
        
        if(!oddy) {
            current = coord+new Vector2Int(1,0);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(0,1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(0,-1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(-1,0);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(-1,1);
            if(coordinateExists(current))
                neighbors.Add(current);

            current = coord+new Vector2Int(-1,-1);
            if(coordinateExists(current))
                neighbors.Add(current);
        }

        return neighbors;
    }*/

    public List<HexTile> getTilesAt(HexCoordinate coord, bool check_regular = true, bool check_ghosts = true) {
        List<HexTile> found = new List<HexTile>();
        if(check_ghosts) {
        HexTile[] ghosts = holderGhostTile.GetComponentsInChildren<HexTile>();
            foreach(HexTile ghost in ghosts) {
                if(ghost.coord % coord)
                    found.Add(ghost);
            }
        }
        if(check_regular) {
            HexTile[] baords = holderBoardTile.GetComponentsInChildren<HexTile>();
            foreach(HexTile board in baords) {
                if(board.coord % coord)
                    found.Add(board);
            }
        }
        return found;
    }

    public List<HexEntity> getEntitiesAt(HexCoordinate coord, bool check_regular = true, bool check_ghosts = true) {
        List<HexEntity> found = new List<HexEntity>();
        if(check_ghosts) {
            HexEntity[] ghosts = holderGhostTile.GetComponentsInChildren<HexEntity>();
            foreach(HexEntity ghost in ghosts) {
                if(!(ghost.coord % coord))
                    continue;
                //bool math = matchTri ? (ghost.coord ^ coord) : (ghost.coord % coord);
                bool math = ghost.testSegment(coord.section);
                if(math)
                    found.Add(ghost);
            }
        }
        if(check_regular) {
            HexEntity[] baords = holderBoardTile.GetComponentsInChildren<HexEntity>();
            foreach(HexEntity board in baords) {
                if(!(board.coord % coord))
                    continue;
                //bool math = matchTri ? (board.coord ^ coord) : (board.coord % coord);
                bool math = board.testSegment(coord.section);
                if(math)
                    found.Add(board);
            }
        }
        return found;
    }
    public List<HexEntity> getEntitiesOnHex(HexCoordinate coord, bool check_regular = true, bool check_ghosts = true) {
        List<HexEntity> found = new List<HexEntity>();
        if(check_ghosts) {
            HexEntity[] ghosts = holderGhostTile.GetComponentsInChildren<HexEntity>();
            foreach(HexEntity ghost in ghosts) {
                if(!(ghost.coord % coord))
                    continue;
                found.Add(ghost);
            }
        }
        if(check_regular) {
            HexEntity[] baords = holderBoardTile.GetComponentsInChildren<HexEntity>();
            foreach(HexEntity board in baords) {
                if(!(board.coord % coord))
                    continue;
                found.Add(board);
            }
        }
        return found;
    }

    public List<HexFlagType> getTileFlagsAt(HexCoordinate coord, TriDirection dir, bool check_regular = true, bool check_ghosts = true) {
        List<HexFlagType> foundf = new List<HexFlagType>();
        List<HexTile> foundt = this.getTilesAt(coord, check_regular, check_ghosts);
        foreach(HexTile tile in foundt) {
            List<HexFlagType> tflags = tile.getFlags(coord,dir);
            foreach(HexFlagType tf in tflags) {
                foundf.Add(tf);
            }
        }
        return foundf;
    }

    public List<HexFlagType> getEntityFlagsAt(HexCoordinate coord, TriDirection dir, bool check_regular = true, bool check_ghosts = true) {
        List<HexFlagType> foundf = new List<HexFlagType>();
        List<HexEntity> founde = this.getEntitiesAt(coord, check_regular, check_ghosts);
        foreach(HexEntity entity in founde) {
            List<HexFlagType> eflags = entity.getFlags(coord,dir);
            foreach(HexFlagType ef in eflags) {
                foundf.Add(ef);
            }
        }
        return foundf;
    }

    public struct EntityFlagsListAtEnrty {
        public HexFlagType flag;
        public HexEntity owner;
    }
    public List<EntityFlagsListAtEnrty> getEntityFlagsListAt(HexCoordinate coord, TriDirection dir, bool check_regular = true, bool check_ghosts = true) {
        List<EntityFlagsListAtEnrty> foundf = new List<EntityFlagsListAtEnrty>();
        List<HexEntity> founde = this.getEntitiesAt(coord, check_regular, check_ghosts);
        foreach(HexEntity entity in founde) {
            List<HexFlagType> eflags = entity.getFlags(coord,dir);
            foreach(HexFlagType ef in eflags) {
                EntityFlagsListAtEnrty entry = new EntityFlagsListAtEnrty();
                entry.flag = ef;
                entry.owner = entity;
                foundf.Add(entry);
            }
        }
        return foundf;
    }

    // === processing ====

    /*public bool coordinateExists(HexCoordinate vec) {
        foreach(HexTile tile in this.registeredTiles) {
            if(tile.coord ^ vec)
                return true;
        }            
        return false;
    }*/

    // ==== general management ====
    /*public void registerTile(HexTile tile) {
        registeredTiles.Add(tile);
        tileIdMap.Add(tile.id, tile);
    }

    public HexTile GetTile(Guid id) {
        return tileIdMap[id];
    }

    public void removeTile(Guid id) {
        registeredTiles.Remove(GetTile(id));
        tileIdMap.Remove(id);
    }*/

    // === spawning ===

    public void spawnProtoTile(TileInfo ti) {

        // proto
        GameObject protoPrefab = Resources.Load("Proto Tile Prefab") as GameObject;
        GameObject goproto = Instantiate(protoPrefab, new Vector3(0,2,0), Quaternion.identity, holderProtoTile.transform);
        goproto.name = "Proto Tile: "+ti.prefabName;
        ProtoTile pt = goproto.GetComponent<ProtoTile>();
        pt.initializeTI(ti);

        // ghost
        GameObject ghostPrefab = Resources.Load("BaseHex") as GameObject;
        GameObject goghost = Instantiate(ghostPrefab, new Vector3(0,0,0), Quaternion.identity, holderGhostTile.transform);
        goghost.name = "Tile: "+ti.prefabName;
        HexTile ht = goghost.GetComponent<HexTile>();
        
        ht.setGhost(true);

        // exchage refs
        ht.protoParent = pt;
        pt.projectedTile = ht;
        ht.init(ti);
    }

    public void despawnProtoTile(ProtoTile pt) {
        if(pt.projectedTile)
            Destroy(pt.projectedTile);
        if(pt.projectedEntity)
            Destroy(pt.projectedEntity);
        Destroy(pt);
    }

    public void spawnProtoTile(EntityInfo ei) {

        // proto
        GameObject protoPrefab = Resources.Load("Proto Tile Prefab") as GameObject;
        GameObject goproto = Instantiate(protoPrefab, new Vector3(0,2,0), Quaternion.identity, holderProtoTile.transform);
        goproto.name = "Proto Entity: "+ei.prefabName;
        ProtoTile pt = goproto.GetComponent<ProtoTile>();
        pt.type = ProtoType.ENTITY;
        pt.initializeEI(ei);

        // ghost
        GameObject ghostPrefab = Resources.Load("BaseEntity") as GameObject;
        GameObject goghost = Instantiate(ghostPrefab, new Vector3(0,0,0), Quaternion.identity, holderGhostTile.transform);
        goghost.name = "Entity: "+ei.prefabName;
        HexEntity he = goghost.GetComponent<HexEntity>();
        he.setGhost(true);

        // exchage refs
        he.protoParent = pt;
        pt.projectedEntity = he;

        he.init(ei);
    }

    public Bilboard spawnPopup(HexEntity target, HexDirection dir, HexFlagType type, Sprite image) {
        GameObject popupPrefab = Resources.Load("2D Popup") as GameObject;
        GameObject popup = Instantiate(popupPrefab, target.transform.position, Quaternion.identity, holderPopup.transform);
        Bilboard bilboard = popup.GetComponent<Bilboard>();
        bilboard.cam = cam;
        bilboard.owner = target;
        bilboard.ownerDir = dir;
        bilboard.ownerType = type;
        Vector2 spot = target.getEntityCenter();
        bilboard.anchor = new Vector3(spot.x,0.5f,spot.y);
        bilboard.init();
        bilboard.setImage(image);
        return bilboard;
    }

    // === misc ===
    public void clearAll() {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in holderProtoTile.transform)
            children.Add(child.gameObject);
        foreach (Transform child in holderGhostTile.transform)
            children.Add(child.gameObject);
        foreach (Transform child in holderBoardTile.transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }


    // === history ===
    public int getHistoryLength() {
        return historyIsTile.Count;
    }

    public HistoryRecord getLatestHistoryItem() {
        HistoryRecord record = new HistoryRecord();
        record.isTile = historyIsTile[historyIsTile.Count - 1];
        if(record.isTile) {
            record.tile = historyTiles[historyTiles.Count - 1];
            return record;
        } else {
            record.entity = historyEntities[historyEntities.Count - 1];
        }
        return record;
    }
    public HistoryRecord getHistoryItem(int index) {
        HistoryRecord record = new HistoryRecord();
        record.isTile = historyIsTile[index];
        if(record.isTile) {
            int tindex = -1;
            for(int i = 0; i <= index; ++i) {
                if(historyIsTile[i]) {
                    tindex++;
                }
            }
            record.tile = historyTiles[tindex];
            return record;
        } else {
            int eindex = -1;
            for(int i = 0; i <= index; ++i) {
                if(!historyIsTile[i]) {
                    eindex++;
                }
            }
            record.entity = historyEntities[eindex];
            return record;
        }
    }

    public struct HistoryRecord {
        public bool isTile;
        public HexTile tile;
        public HexEntity entity;
    }
}
