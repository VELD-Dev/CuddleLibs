using HarmonyLib;
using Nautilus.OutcropsHelper.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nautilus.OutcropsHelper.Interfaces;
using Nautilus.Utility;
using System;

namespace Nautilus.OutcropsHelper.Patchers;

internal class BreakableResourcePatcher
{
    internal static void Patch(Harmony harmony)
    {
        harmony.PatchAll(typeof(BreakableResourcePatcher));
        InternalLogger.Info("Finished patching BreakableResource.");
    }

    public static IDictionary<TechType, List<OutcropDropData>> CustomDrops = new SelfCheckingDictionary<TechType, List<OutcropDropData>>("CustomOutcropsDrops");

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HandTarget), nameof(HandTarget.Awake))]
    private static void Awake(HandTarget __instance)
    {
        if (__instance is not BreakableResource)
            return;

        var instance = __instance as BreakableResource;
        try
        {
            TechType outcropTechType = CraftData.GetTechType(instance.gameObject);
            List<OutcropDropData> convertedDropsDatas = new();
            if (!CustomDrops.ContainsKey(outcropTechType))
            {
                CustomDrops.Add(outcropTechType, new());
            }

            foreach (BreakableResource.RandomPrefab randPrefab in instance.prefabList)
                convertedDropsDatas.Add(randPrefab.ToOutcropDropData());

            CustomDrops[outcropTechType].Union(convertedDropsDatas).ToList();
        }
        catch(Exception e)
        {
            InternalLogger.Error($"An error has ocurred while patching HandTarget.Awake():\n{e}");
        }
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
            bool spawnSuccessful = false;
            for (int i = 0; i < __instance.numChances; i++)
            {
                TechType chosenResource = __instance.ChooseRandomResourceTechType();
                if (chosenResource != TechType.None)
                {
                    InternalLogger.Info($"Chosen resource TechType: {chosenResource}");
                    __instance.SpawnResourceFromTechType(chosenResource);
                    spawnSuccessful = true;
                }
            }
            if (!spawnSuccessful)
            {
                InternalLogger.Error("Spawn wasn't successful. Inspection needed at line BreakableResourcePatcher.cs:84");
                __instance.SpawnResourceFromPrefab(__instance.defaultPrefabReference);
            }
            FMODUWE.PlayOneShot(__instance.breakSound, __instance.transform.position, 1f);
            if (__instance.hitFX)
            {
                global::Utils.PlayOneShotPS(__instance.breakFX, __instance.transform.position, Quaternion.Euler(new Vector3(270f, 0f, 0f)), null);
            }
            InternalLogger.Debug($"Registered Custom Drops: {CustomDrops}");
            InternalLogger.Debug("CustomDrops = {");
            foreach (var kvp in CustomDrops)
            {
                InternalLogger.Debug($"{kvp.Key}:");
                InternalLogger.Debug("{");
                foreach (var dropData in kvp.Value)
                {
                    InternalLogger.Debug(dropData.ToString());
                }
                InternalLogger.Debug("},");
            }
            InternalLogger.Debug("}");

            return false;
        }
        return false;
    }
}
