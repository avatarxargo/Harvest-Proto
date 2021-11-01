using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Flag", menuName = "HexaTile/TriRequirement/Entity Flag", order = 6)]
public class TriReqEntityFlag : TriRequirement {
    public TriComboType matchType = TriComboType.AT_LEAST_ONE;
    public TriDirection triDir;
    public bool onEmpty = false;
    public bool checkGhosts = true;
    public bool checkReal = true;
    public List<HexFlagType> flagTypes;

    public override bool check(TileSystem system, HexCoordinate coord) {
        List<HexFlagType> foundflags = system.getEntityFlagsAt(coord, triDir, checkReal, checkGhosts);
        if(foundflags.Count > 0) {
            foreach(HexFlagType flag in foundflags) {
                foreach(HexFlagType type in flagTypes) {
                    if(flag == type) {
                        // match
                        switch(matchType) {
                        case TriComboType.AT_LEAST_ONE:
                            return true;
                        case TriComboType.NONE:
                            return false;
                        default:
                        case TriComboType.ALL:
                            break;
                        }
                    } else {
                        // mismatch
                        switch(matchType) {
                        case TriComboType.AT_LEAST_ONE:
                        case TriComboType.NONE:
                            break;
                        default:
                        case TriComboType.ALL:
                            return false;
                        }
                    }
                }
            }
            // went through all without returning
            switch(matchType) {
            default:
            case TriComboType.AT_LEAST_ONE:
                return false;
            case TriComboType.ALL:
            case TriComboType.NONE:
                return true;
            }
        } else {
            // no entities there.
            return onEmpty;
        }
    }
}
