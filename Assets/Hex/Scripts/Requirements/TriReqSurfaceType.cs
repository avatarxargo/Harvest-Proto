using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Surface Type", menuName = "HexaTile/TriRequirement/Surface Type", order = 4)]
public class TriReqSurfaceType : TriRequirement {
    public TriComboType matchType = TriComboType.ALL;
    public bool onEmpty = false;
    public bool checkGhosts = true;
    public bool checkReal = true;
    public TileType[] types;
    public override bool check(TileSystem system, HexCoordinate coord) {
        List<HexTile> tiles = system.getTilesAt(coord, checkReal, checkGhosts);
        if(tiles.Count > 0) {
            HexTile tile = tiles[0];
            HexDirection internalDir = tile.directionGlobal2Internal(coord.section);
            TileType type = tile.tileInfo.segmentType[(int)internalDir];

            switch(matchType) {
            default:
            case TriComboType.ALL:
            foreach(TileType t in types) {
                if(t!=type)
                    return false;
            }
            return true;
            //
            case TriComboType.AT_LEAST_ONE:
            foreach(TileType t in types) {
                if(t==type)
                    return true;
            }
            return false;
            //
            case TriComboType.NONE:
            foreach(TileType t in types) {
                if(t==type)
                    return false;
            }
            return true;
            }
        }
        // no tile
        return onEmpty;
    }
}