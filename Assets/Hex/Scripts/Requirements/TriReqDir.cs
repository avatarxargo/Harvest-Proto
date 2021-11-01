using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Directional", menuName = "HexaTile/TriRequirement/(Directional)", order = 2)]
public class TriReqDir : TriRequirement {
    public TriDirection direction = TriDirection.Back;
    public TriRequirement condition = null;
    public override bool check(TileSystem system, HexCoordinate coord) {
        return condition.check(system, coord.getTriNeighbor(direction));
    }
}