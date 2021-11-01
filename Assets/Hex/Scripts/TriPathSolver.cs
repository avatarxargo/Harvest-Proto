using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Helps to find connections between places on the map through rails or pipes

public enum HexConnectivityMapType {
    Pipe, Rail
}

/// <summary>
/// one pair of linked connections
/// </summary>
public struct HexConnectivityMapItem {
    HexDirection dirIn;
    TriDirection dirInSub;
    HexDirection dirOut;
    TriDirection dirOutSub;
    HexConnectivityMapType connectionType;
}

public struct HexConnectivityMap {
    List<HexConnectivityMapItem> connections;
}

public struct HexConnectivityResult {
    public HexFlagType sourceConnectionType;
    public HexCoordinate sourceConnectionCoord;
    public TriDirection sourceConnectionDir;
    public TileType sourceTileType; //only for surface finder.

    //
    public bool foundConnection;
    public List<HexCoordinate> path;
    public List<HexEntity> reached;
    public List<HexFlagType> reachedType;
    public List<HexCoordinate> reachedCoord;
    public List<TriDirection> reachedDir;

    public void insertPathNode(HexCoordinate node) {
        path.Add(node);
    }
}

public class TriPathSolver
{
    private static Queue<HexCoordinate> openList;
    private static Queue<TriDirection> openListDir;
    private static List<HexCoordinate> visitedList;

    private static bool wasVisited(HexCoordinate coord) {
        foreach (HexCoordinate hc in visitedList) {
            if(hc ^ coord) 
                return true;
        }
        return false;
    }

/// <summary>
/// This is not really a path finding algorithm, it just checks a list of all connected inlets to our outlet.
/// </summary>

    public static HexConnectivityResult solveConnectionPath(TileSystem tileSystem, HexCoordinate startCoord, TriDirection startDir,
                        HexFlagType connectionType, bool check_regular = true, bool check_ghosts = false) {
        HexConnectivityResult result = new HexConnectivityResult();
        result.sourceConnectionCoord = startCoord;
        result.sourceConnectionDir = startDir;
        result.sourceConnectionType = connectionType;
        result.foundConnection = false;
        result.path = new List<HexCoordinate>();
        result.reached = new List<HexEntity>();
        result.reachedCoord = new List<HexCoordinate>();
        result.reachedDir = new List<TriDirection>();
        result.reachedType = new List<HexFlagType>();
        //
        HexFlagType conType = HexFlagUtil.getEffectiveType(connectionType);
        HexFlagType targetType = HexFlagUtil.getMatchedPairInOut(connectionType);
        if(conType != HexFlagType.PipeIO && conType != HexFlagType.RailIO) {
            Debug.LogError("solveConnectionPath: connectionType cannot be equal to "+conType+". Needs to be a source which evaluates to rail or pipe");
            return result;
        }
        //
        openList = new Queue<HexCoordinate>();
        openListDir = new Queue<TriDirection>();
        visitedList = new List<HexCoordinate>();
        openList.Enqueue(startCoord);
        openListDir.Enqueue(startDir);
        /*Debug.Log("Pathfinding...");
        Debug.Log("src type:" + connectionType);
        Debug.Log("con type:" + conType);
        Debug.Log("tgt type:" + targetType);*/
        //
        while (openList.Count > 0) {
            HexCoordinate current = openList.Dequeue();
            TriDirection currentDir = openListDir.Dequeue();
            visitedList.Add(current);
            result.path.Add(current);
            //Debug.Log("visiting: "+current.x+","+current.y+","+current.section);
            //
            TriDirection neighborsDir = currentDir;
            if(neighborsDir == TriDirection.Left) {
                neighborsDir = TriDirection.Right;
            } else if(neighborsDir == TriDirection.Right) {
                neighborsDir = TriDirection.Left;
            }
            //
            HexCoordinate neighbor = current.getTriNeighbor(currentDir);
            /*HexCoordinate nb = current.getTriNeighbor(TriDirection.Back);
            HexCoordinate nl = current.getTriNeighbor(TriDirection.Left);
            HexCoordinate nr = current.getTriNeighbor(TriDirection.Right);
            List<TileSystem.EntityFlagsListAtEnrty> nbflags = tileSystem.getEntityFlagsListAt(nb, TriDirection.Back, check_regular, check_ghosts);
            List<TileSystem.EntityFlagsListAtEnrty> nlflags = tileSystem.getEntityFlagsListAt(nb, TriDirection.Right, check_regular, check_ghosts);
            List<TileSystem.EntityFlagsListAtEnrty> nrflags = tileSystem.getEntityFlagsListAt(nb, TriDirection.Left, check_regular, check_ghosts);*/
            
            if(!wasVisited(neighbor)) {
                List<TileSystem.EntityFlagsListAtEnrty> nbflags = tileSystem.getEntityFlagsListAt(neighbor, neighborsDir, check_regular, check_ghosts);
                //bool flagMatch = false;
                List<HexEntity> matchedEntities = new List<HexEntity>();
                foreach(TileSystem.EntityFlagsListAtEnrty entry in nbflags) {
                    // if the entity isn't active it can't provide the resources:
                    if(!entry.owner.isActive)
                        continue;
                    //Debug.Log("Comparing: " + entry.flag +" vs "+ targetType);
                    bool isReceiver = ((entry.flag == targetType) || (targetType == HexFlagType.AnyOut && HexFlagUtil.isOutFlag(entry.flag)));
                    /*(conType == HexFlagType.PipeIO && entry.flag == HexFlagType.WaterIn) ||
                    (conType == HexFlagType.RailIO && (entry.flag == HexFlagType.CropsIn || entry.flag == HexFlagType.OreIn || entry.flag == HexFlagType.LogsIn || entry.flag == HexFlagType.IngotsIn || entry.flag == HexFlagType.FoodIn || entry.flag == HexFlagType.PlanksIn));*/
                    if( isReceiver ) {
                        //Debug.Log("Receiver");
                        result.reached.Add(entry.owner);
                        result.reachedCoord.Add(neighbor);
                        result.reachedDir.Add(neighborsDir);
                        result.reachedType.Add(entry.flag);
                    }
                    else if( HexFlagUtil.getEffectiveType(entry.flag) == conType) {
                        //Debug.Log("Match");
                        matchedEntities.Add(entry.owner);
                    }
                }
                foreach(HexEntity matched in matchedEntities) {
                    foreach(EntityConnectionPair cpair in matched.entityInfo.connectionMap) {
                        if( HexFlagUtil.getEffectiveType(cpair.connectionType) == conType ) {
                            HexDirection internalMatchedDir = matched.directionGlobal2Internal(neighbor.section);
                            bool isFirstSegment = internalMatchedDir == cpair.inletSegment;
                            bool isSecondSegment = internalMatchedDir == cpair.outletSegment;
                            bool isFirstSubSegment = neighborsDir == cpair.inletDirection;
                            bool isSecondSubSegment = neighborsDir == cpair.outletDirection;
                            if(isFirstSegment && isFirstSubSegment) {
                                // this means that the entity enters on a different tile than it exits.
                                if(internalMatchedDir != cpair.outletSegment) {
                                    result.path.Add(neighbor);
                                }
                                HexCoordinate otherSide = new HexCoordinate(neighbor.x,neighbor.y,matched.directionInternal2Global(cpair.outletSegment));
                                openList.Enqueue(otherSide);
                                openListDir.Enqueue(cpair.outletDirection);
                            } else if (isSecondSegment && isSecondSubSegment) {
                                // this means that the entity enters on a different tile than it exits.
                                if(internalMatchedDir != cpair.inletSegment) {
                                    result.path.Add(neighbor);
                                }
                                HexCoordinate otherSide = new HexCoordinate(neighbor.x,neighbor.y,matched.directionInternal2Global(cpair.inletSegment));
                                openList.Enqueue(otherSide);
                                openListDir.Enqueue(cpair.inletDirection);
                            } //else { /*this particular connection was not the one recognized.*/ }
                        }
                    }
                }
            }
        }
        //
        return result;
    }

    public static HexConnectivityResult solveConnectionSurface(TileSystem tileSystem, List<HexCoordinate> startCoords, bool matchSurfaceType,
        TileType testedType, HexFlagType testedFlag, bool check_regular = true, bool check_ghosts = false) {

        HexConnectivityResult result = new HexConnectivityResult();
        result.sourceConnectionCoord = startCoords[0];
        result.sourceConnectionDir = TriDirection.Back;
        result.sourceConnectionType = testedFlag;
        result.foundConnection = false;
        result.path = new List<HexCoordinate>();
        result.reached = new List<HexEntity>();
        result.reachedCoord = new List<HexCoordinate>();
        result.reachedDir = new List<TriDirection>();
        result.reachedType = new List<HexFlagType>();
        //
        openList = new Queue<HexCoordinate>();
        //openListDir = new Queue<TriDirection>();
        visitedList = new List<HexCoordinate>();
        foreach(HexCoordinate hc in startCoords)
            openList.Enqueue(hc);
        //openListDir.Enqueue(startDir);
        /*Debug.Log("Surfacefinding...");
        Debug.Log("matchSurfaceType:" + matchSurfaceType);
        Debug.Log("testedType:" + testedType);
        Debug.Log("testedFlag:" + testedFlag);*/
        //
        while (openList.Count > 0) {
            HexCoordinate current = openList.Dequeue();
            //Debug.Log("visiting: "+current.x+","+current.y+","+current.section);

            for(int td = 0; td < 3; ++td) {
                TriDirection tdd = (TriDirection)td;
                HexCoordinate neighbor = current.getTriNeighbor(tdd);

                if(!wasVisited(neighbor)) {
                    if(matchSurfaceType) {
                        // checking for surface flags (such as fertile)
                        List<HexTile> other = tileSystem.getTilesAt(neighbor, check_regular, check_ghosts);
                        if(other.Count > 0) {
                            HexTile first = other[0];
                            TileType otherType = TileTypeUtil.getEffectiveType(first.tileInfo.segmentType[(int)first.directionGlobal2Internal(neighbor.section)]);
                            if(otherType == testedType) {
                                visitedList.Add(neighbor);
                                openList.Enqueue(neighbor);
                                result.path.Add(neighbor);
                            }
                        }
                    } else {
                        // checking for entity flags (such as forests)
                        List<HexFlagType> other = tileSystem.getEntityFlagsAt(neighbor, TriDirection.Back, check_regular, check_ghosts);
                        if(other.Contains(testedFlag)) {
                            visitedList.Add(neighbor);
                            openList.Enqueue(neighbor);
                            result.path.Add(neighbor);
                        }
                    }
                }
            }
        }
        //
        return result;
    }
}
