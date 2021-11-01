using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexFlagGroup {
    Rail, Pipe,
    Water, Logs, Ore, Crops, Planks, Ingots, Food, Any
}
public enum HexFlagType {
    RailIO,
    PipeIO,
    WaterIn, WaterOut,
    LogsIn, LogsOut,
    OreIn, OreOut,
    CropsIn, CropsOut,
    PlanksIn, PlanksOut,
    IngotsIn, IngotsOut,
    FoodIn, FoodOut,
    AnyIn, AnyOut,
    Forest
}

public class HexFlagUtil {

    public static int FlagToScoreId(HexFlagType type) {
        if(type == HexFlagType.LogsOut) 
            return 1;
        if(type == HexFlagType.OreOut) 
            return 2;
        if(type == HexFlagType.CropsOut) 
            return 3;
        if(type == HexFlagType.LogsOut) 
            return 4;
        if(type == HexFlagType.IngotsOut) 
            return 5;
        if(type == HexFlagType.FoodOut) 
            return 6;
        return 0;
    }

    public static bool isOutFlag(HexFlagType type) {
        return (type == HexFlagType.WaterOut || type == HexFlagType.LogsOut || type == HexFlagType.OreOut || type == HexFlagType.CropsOut ||
                type == HexFlagType.PlanksOut || type == HexFlagType.IngotsOut || type == HexFlagType.FoodOut || type == HexFlagType.AnyOut );
    }
    public static bool isInFlag(HexFlagType type) {
        return (type == HexFlagType.WaterIn || type == HexFlagType.LogsIn || type == HexFlagType.OreIn || type == HexFlagType.CropsIn ||
                type == HexFlagType.PlanksIn || type == HexFlagType.IngotsIn || type == HexFlagType.FoodIn || type == HexFlagType.AnyIn );
    }

    public static HexFlagType getMatchedPairInOut(HexFlagType type) {
        if(type == HexFlagType.WaterIn)
            return HexFlagType.WaterOut;

        if(type == HexFlagType.LogsIn)
            return HexFlagType.LogsOut;
        if(type == HexFlagType.OreIn)
            return HexFlagType.OreOut;
        if(type == HexFlagType.CropsIn)
            return HexFlagType.CropsOut;

        if(type == HexFlagType.PlanksIn)
            return HexFlagType.PlanksOut;
        if(type == HexFlagType.IngotsIn)
            return HexFlagType.IngotsOut;
        if(type == HexFlagType.FoodIn)
            return HexFlagType.FoodOut;

        if(type == HexFlagType.AnyIn)
            return HexFlagType.AnyOut;

        return type;
    }
    public static HexFlagType getMatchedPairOutIn(HexFlagType type) {
        if(type == HexFlagType.WaterOut)
            return HexFlagType.WaterIn;

        if(type == HexFlagType.LogsOut)
            return HexFlagType.LogsIn;
        if(type == HexFlagType.OreOut)
            return HexFlagType.OreIn;
        if(type == HexFlagType.CropsOut)
            return HexFlagType.CropsIn;

        if(type == HexFlagType.PlanksOut)
            return HexFlagType.PlanksIn;
        if(type == HexFlagType.IngotsOut)
            return HexFlagType.IngotsIn;
        if(type == HexFlagType.FoodOut)
            return HexFlagType.FoodIn;

        return type;
    }

    public static HexFlagType getEffectiveType(HexFlagType type) {
        if(type == HexFlagType.RailIO || type == HexFlagType.LogsOut || type == HexFlagType.OreOut || type == HexFlagType.CropsOut ||
                                    type == HexFlagType.PlanksOut || type == HexFlagType.IngotsOut || type == HexFlagType.FoodOut ||
                                    type == HexFlagType.LogsIn || type == HexFlagType.OreIn || type == HexFlagType.CropsIn ||
                                    type == HexFlagType.PlanksIn || type == HexFlagType.IngotsIn || type == HexFlagType.FoodIn ||
                                    type == HexFlagType.AnyIn || type == HexFlagType.AnyOut)
            return HexFlagType.RailIO;
        if(type == HexFlagType.PipeIO || type == HexFlagType.WaterIn || type == HexFlagType.WaterOut)
            return HexFlagType.PipeIO;
        return type;
    }

    public static HexFlagGroup getFlagGroup(HexFlagType type) {
        if(type == HexFlagType.LogsIn || type == HexFlagType.LogsOut)
            return HexFlagGroup.Logs;
        if(type == HexFlagType.OreIn || type == HexFlagType.OreOut)
            return HexFlagGroup.Ore;
        if(type == HexFlagType.CropsIn || type == HexFlagType.CropsOut)
            return HexFlagGroup.Crops;

        if(type == HexFlagType.PlanksIn || type == HexFlagType.PlanksOut)
            return HexFlagGroup.Planks;
        if(type == HexFlagType.IngotsIn || type == HexFlagType.IngotsOut)
            return HexFlagGroup.Ingots;
        if(type == HexFlagType.FoodIn || type == HexFlagType.FoodOut)
            return HexFlagGroup.Food;
        if(type == HexFlagType.AnyIn || type == HexFlagType.AnyOut)
            return HexFlagGroup.Any;

        if(type == HexFlagType.PipeIO)
            return HexFlagGroup.Pipe;
        return HexFlagGroup.Rail;
    }
}

[CreateAssetMenu(fileName = "Hex Flag", menuName = "HexaTile/Hex Flag", order = 4)]
public class HexFlag : ScriptableObject {
    public HexFlagType flag;
    public TriDirection side;

    public static string getIco(HexFlagType type) {
        switch(type) {
            default:
                return "hexico_fence.png";

            case HexFlagType.RailIO:
                return "hexico_rail.png";
            case HexFlagType.PipeIO:
                return "hexico_pipe.png";
            case HexFlagType.Forest:
                return "hexico_fance.png";

            case HexFlagType.WaterIn:
                return "hexico_water_in.png";
            case HexFlagType.LogsIn:
                return "hexico_logs_in.png";
            case HexFlagType.OreIn:
                return "hexico_ore_in.png";
            case HexFlagType.CropsIn:
                return "hexico_crops_in.png";
            case HexFlagType.PlanksIn:
                return "hexico_planks_in.png";
            case HexFlagType.IngotsIn:
                return "hexico_ingots_in.png";
            case HexFlagType.FoodIn:
                return "hexico_food_in.png";
            case HexFlagType.AnyIn:
                return "hexico_rail_in.png";
                
            case HexFlagType.WaterOut:
                return "hexico_water_out.png";
            case HexFlagType.LogsOut:
                return "hexico_logs_out.png";
            case HexFlagType.OreOut:
                return "hexico_ore_out.png";
            case HexFlagType.CropsOut:
                return "hexico_crops_out.png";
            case HexFlagType.PlanksOut:
                return "hexico_planks_out.png";
            case HexFlagType.IngotsOut:
                return "hexico_ingots_out.png";
            case HexFlagType.FoodOut:
                return "hexico_food_out.png";
        }
    }

    public static string getIco(HexFlagGroup group) {
        switch(group) {
            default:
                return "hexico_fence.png";

            case HexFlagGroup.Rail:
                return "hexico_rail.png";
            case HexFlagGroup.Pipe:
                return "hexico_pipe.png";
                
            case HexFlagGroup.Water:
                return "hexico_water.png";
            case HexFlagGroup.Logs:
                return "hexico_logs.png";
            case HexFlagGroup.Ore:
                return "hexico_ore.png";
            case HexFlagGroup.Crops:
                return "hexico_crops.png";
            case HexFlagGroup.Planks:
                return "hexico_planks.png";
            case HexFlagGroup.Ingots:
                return "hexico_ingots.png";
            case HexFlagGroup.Food:
                return "hexico_food.png";

        }
    }

    public void drawGizmo(HexCoordinate coord, int offset = 0) {
        HexCoordinate.visualizeFlag(coord,side,getIco(flag),0.7f,offset);
    }
}