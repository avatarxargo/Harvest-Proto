using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerInfo", order = 1)]
public class PlayerInfo : ScriptableObject
{
    public string title;
    public Color playerColor;

    public List<int> scores = new List<int>(new int[9]);
    public List<HexTile> ownedTiles = new List<HexTile>();
    public List<HexEntity> ownedEntities = new List<HexEntity>();
}
