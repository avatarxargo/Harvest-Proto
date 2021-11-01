using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hex", menuName = "HexaTile/TriRequirement/(Hex)", order = 4)]
public class TriReqHex : TriRequirement {

    public TriRequirement conditionE = null;
    public TriRequirement conditionSE = null;
    public TriRequirement conditionSW = null;
    public TriRequirement conditionW = null;
    public TriRequirement conditionNW = null;
    public TriRequirement conditionNE = null;

    public TriComboType matchType = TriComboType.ALL;

    public override bool check(TileSystem system, HexCoordinate coord) {
        bool e = matchType!=TriComboType.NONE;
        bool se = matchType!=TriComboType.NONE;
        bool sw = matchType!=TriComboType.NONE;
        bool w = matchType!=TriComboType.NONE;
        bool nw = matchType!=TriComboType.NONE;
        bool ne = matchType!=TriComboType.NONE;
        if(conditionE) 
            e = conditionE.check(system, coord);
        if(conditionSE) 
            se = conditionSE.check(system, HexDirectionUtil.rotate(coord,1));
        if(conditionSW) 
            sw = conditionSW.check(system, HexDirectionUtil.rotate(coord,2));
        if(conditionW) 
            w = conditionW.check(system, HexDirectionUtil.rotate(coord,3));
        if(conditionNW) 
            nw = conditionNW.check(system, HexDirectionUtil.rotate(coord,4));
        if(conditionNE) 
            ne = conditionNE.check(system, HexDirectionUtil.rotate(coord,5));
         switch(matchType) {
            default:
            case TriComboType.ALL:
                return e && se && sw && w && nw && ne;
            //
            case TriComboType.AT_LEAST_ONE:
                return e || se || sw || w || nw || ne;
            //
            case TriComboType.NONE:
                return !(e || se || sw || w || nw || ne);
        }
    }
}