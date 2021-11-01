using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Not", menuName = "HexaTile/TriRequirement/(Not)", order = 3)]
public class TriReqNot : TriRequirement {
    public TriRequirement condition = null;
    public override bool check(TileSystem system, HexCoordinate coord) {
        return !condition.check(system, coord);
    }
}