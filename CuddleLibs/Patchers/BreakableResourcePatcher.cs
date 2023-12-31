﻿namespace CuddleLibs.Patchers;

internal static class BreakableResourcePatcher
{
    internal static void Patch(Harmony harmony)
    {
        harmony.PatchAll(typeof(BreakableResourcePatcher));
        InternalLogger.Info("Finished patching BreakableResource.");
    }

    public static IDictionary<TechType, List<OutcropDropData>> CustomDrops = new Dictionary<TechType, List<OutcropDropData>>();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HandTarget), nameof(HandTarget.Awake))]
    private static void Awake(HandTarget __instance)
    {
        if (__instance is not BreakableResource)
            return;

        var instance = __instance as BreakableResource;
        TechType outcropTechType = CraftData.GetTechType(instance.gameObject);
        try
        {
            foreach (BreakableResource.RandomPrefab randPrefab in instance.prefabList)
                OutcropsUtils.SetOutcropDrop(outcropTechType, randPrefab.prefabTechType, randPrefab.chance);
        }
        catch(Exception e)
        {
            InternalLogger.Error($"An error has ocurred while patching HandTarget.Awake() when adding original drops for outcrop {outcropTechType}.\n{e}");
        }

        // Ensuring entropy, it can only be called when Player.main exists.
        foreach (var kvp in CustomDrops)
            foreach (var dropData in kvp.Value)
                EntropyUtils.EnsureTechEntropy(dropData.TechType);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    private static bool BreakIntoResources(BreakableResource __instance)
    {
        if (!__instance.broken)
        {
            __instance.broken = true;
            __instance.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);
            if (__instance.gameObject.GetComponent<VFXBurstModel>())
            {
                __instance.gameObject.BroadcastMessage("OnKill");
            }
            else
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
            }
            if (__instance.customGoalText != "")
            {
                GoalManager.main.OnCustomGoalEvent(__instance.customGoalText);
            }
            bool choosed = false;
            for (int i = 0; i < __instance.numChances; i++)
            {
                TechType chosenResource = __instance.ChooseRandomResourceTechType();
                if (chosenResource != TechType.None)
                {
                    InternalLogger.Info($"Chosen resource TechType: {chosenResource}");
                    SpawningUtils.SpawnResourceFromTechType(__instance.gameObject, chosenResource);
                    choosed = true;
                }
            }
            if (!choosed)
            {
                // If not choosed, then spawn titanium instead.
                __instance.SpawnResourceFromPrefab(__instance.defaultPrefabReference);
            }
            FMODUWE.PlayOneShot(__instance.breakSound, __instance.transform.position, 1f);
            if (__instance.hitFX)
            {
                global::Utils.PlayOneShotPS(__instance.breakFX, __instance.transform.position, Quaternion.Euler(new Vector3(270f, 0f, 0f)), null);
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
            return false;
        }
        return false;
    }
}
