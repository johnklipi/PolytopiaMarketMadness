using BepInEx.Logging;
using HarmonyLib;
using Polytopia.Data;

namespace ModernWarfare;

public static class Main
{
    private static ManualLogSource? modLogger;
    public static void Load(ManualLogSource logger)
    {
        Harmony.CreateAndPatchAll(typeof(Main));
        modLogger = logger;
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(ActionUtils), nameof(ActionUtils.CalculateImprovementLevel))]
    private static void ActionUtils_CalculateImprovementLevel(ref int __result, GameState gameState, TileData tile)
    {
        if (tile.improvement == null)
            return;
        if (!gameState.GameLogicData.TryGetData(tile.improvement.type, out ImprovementData improvementData))
            return;

        if (!improvementData.HasAbility(EnumCache<ImprovementAbility.Type>.GetType("market_marmad")))
            return;

        __result /= 2;

        Il2CppSystem.Collections.Generic.List<TileData> area = gameState.Map.GetArea(tile.coordinates, 1, allowDiagonal: true, includeCenter: false);
        foreach (var areaTile in area)
        {
            if(areaTile.HasImprovement(EnumCache<ImprovementData.Type>.GetType("guild_marmad")))
                __result++;
        }

        __result = Math.Min(__result, improvementData.maxLevel);
    }
}