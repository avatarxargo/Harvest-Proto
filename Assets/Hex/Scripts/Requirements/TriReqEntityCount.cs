using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrimathComparisonType {
    GreaterThan, GreaterOrEqual, Equal, LessOrEqual, LessThan, NotEqual
}

[CreateAssetMenu(fileName = "Entity Count", menuName = "HexaTile/TriRequirement/Entity Count", order = 6)]
public class TriReqEntityCount : TriRequirement
{
    public bool checkGhosts = true;
    public bool checkReal = true;
    public int number = 0;
    public TrimathComparisonType comparisonType = TrimathComparisonType.Equal;
    public override bool check(TileSystem system, HexCoordinate coord) {
        List<HexEntity> entities = system.getEntitiesAt(coord, checkReal, checkGhosts);
        switch(comparisonType) {
        default:
        case TrimathComparisonType.Equal:
            return entities.Count == number;
        case TrimathComparisonType.GreaterThan:
            return entities.Count > number;
        case TrimathComparisonType.GreaterOrEqual:
            return entities.Count >= number;
        case TrimathComparisonType.LessOrEqual:
            return entities.Count <= number;
        case TrimathComparisonType.LessThan:
            return entities.Count < number;
        }
    }
}
