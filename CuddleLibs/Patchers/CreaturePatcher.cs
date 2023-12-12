using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuddleLibs.Patchers;

internal class CreaturePatcher
{
    internal static void Patch(Harmony harmony)
    {
        harmony.PatchAll(typeof(CreaturePatcher));
        InternalLogger.Info("Finished patching Creature.");
    }

    public static IDictionary<TechType, List<CreatureDropData>> CustomDrops = new Dictionary<TechType, List<CreatureDropData>>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.OnKill))]
    private static void OnKill(Creature __instance)
    {
        if (__instance.enabled)
            return;

        TechType creatureTechType = CraftData.GetTechType(__instance.gameObject);
        if (creatureTechType == TechType.None)
            throw new NullReferenceException($"The creature {__instance} does not have a TechType.");

        if (!CustomDrops.ContainsKey(creatureTechType))
            return;

        // Choose prefabs
        List<TechType> choosenTechTypes = new();
        for (int i = 0; i < CustomDrops.Count; i++)
        {
            var choosenTechType = CreatureDropsUtils.ChooseRandomResourceTechType(__instance);
            var dropData = CustomDrops[creatureTechType].Find((ctr) => ctr.TechType == choosenTechType);
            choosenTechTypes.Add(choosenTechType);

            if (dropData == null)
                InternalLogger.Error(new NullReferenceException($"No Creature Drop Data found for creature {creatureTechType} and resource {choosenTechType}").ToString());

            if (dropData.unique)
            {
                choosenTechTypes.Clear();
                choosenTechTypes.Add(choosenTechType);
                break;
            }
            else
            {
                choosenTechTypes.Add(choosenTechType);
                continue;
            }
        }

        // Spawn prefabs
        for (int i = 0; i < choosenTechTypes.Count; i++)
        {
            TechType techType = choosenTechTypes[i];
            var dropData = CustomDrops[creatureTechType].Find((ctr) => ctr.TechType == techType)
                ?? throw new NullReferenceException($"Cannot find the drop data assigned to TechType {techType}.");

            for(int j = 0; j < dropData.dropAmount; i++)
            {
                SpawningUtils.SpawnResourceFromTechType(__instance.gameObject, techType);
                InternalLogger.Debug($"Spawned prefab #{j} for resource item {techType}");
            }
        }

        var strBuilder = new StringBuilder();
        strBuilder.AppendLine($"Registered Custom Drops: {CustomDrops}");
        strBuilder.AppendLine("CustomDrops = {");
        foreach (var kvp in CustomDrops)
        {
            strBuilder.AppendLine($"{kvp.Key}:");
            strBuilder.AppendLine("\t{");
            foreach (var dropData in kvp.Value)
            {
                strBuilder.AppendLine($"{dropData.ToString("\t\t")}");
            }
            strBuilder.AppendLine("\t},");
        }
        strBuilder.AppendLine("}");
        InternalLogger.Debug(strBuilder.ToString());
    }
}
