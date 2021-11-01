using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pair", menuName = "HexaTile/Entity Connection Pair", order = 6)]
public class EntityConnectionPair : ScriptableObject
{
    public HexFlagType connectionType;
    public HexDirection inletSegment;
    public TriDirection inletDirection;
    public HexDirection outletSegment;
    public TriDirection outletDirection;
}
