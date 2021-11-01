using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Entity Type", menuName = "HexaTile/TriRequirement/Entity Type", order = 5)]
public class TriReqEntityType : TriRequirement {
    public TriComboType matchType = TriComboType.AT_LEAST_ONE;
    public bool onEmpty = false;
    public bool checkGhosts = true;
    public bool checkReal = true;
    public EntityInfo[] types;
    public override bool check(TileSystem system, HexCoordinate coord) {
        List<HexEntity> entities = system.getEntitiesAt(coord, checkReal, checkGhosts);
        if(entities.Count > 0) {
            foreach(HexEntity entity in entities) {
                string name1 = entity.entityInfo.prefabName;
                foreach(EntityInfo type in types) {
                    string name2 = entity.entityInfo.prefabName;
                    if(name1 == name2) {
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