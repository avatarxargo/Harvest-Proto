using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexJobRecord
{
    public HexDirection direction = HexDirection.HexEast;
    public TriDirection side = TriDirection.Back;
    public HexFlagType resource = HexFlagType.WaterIn;

    //public bool established = false;
    public bool completed = false;

    public int pointValue = 5;

    public int establishingPlayer = 0;
    public int completingPlayer = 0;

    public static bool isJobFlag(HexFlagType flag) {
        return flag == HexFlagType.WaterIn || flag == HexFlagType.OreIn || flag == HexFlagType.CropsIn || flag == HexFlagType.LogsIn || flag == HexFlagType.IngotsIn || flag == HexFlagType.FoodIn || flag == HexFlagType.PlanksIn || flag == HexFlagType.AnyIn;
    }
}
