using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Multi", menuName = "HexaTile/TriRequirement/(Multi)", order = 1)]
public class TriReqMulti : TriRequirement {
    public TriComboType matchType = TriComboType.ALL;
    public TriRequirement[] conditions;
    public override bool check(TileSystem system, HexCoordinate coord) {
        switch(matchType) {
            default:
            case TriComboType.ALL:
            foreach(TriRequirement req in conditions) {
                if(!req.check(system,coord))
                    return false;
            }
            return true;
            //
            case TriComboType.AT_LEAST_ONE:
            foreach(TriRequirement req in conditions) {
                if(req.check(system,coord))
                    return true;
            }
            return false;
            //
            case TriComboType.NONE:
            foreach(TriRequirement req in conditions) {
                if(req.check(system,coord))
                    return false;
            }
            return true;
        }
    }
}