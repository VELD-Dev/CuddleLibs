using HarmonyLib;
using Nautilus.OutcropsHelper.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Nautilus.OutcropsHelper.Interfaces;
using Nautilus.Utility;

namespace Nautilus.OutcropsHelper.Patchers;

[HarmonyPatch(typeof(BreakableResource))]
internal class BreakableResourcePatcher
{
    internal static void Patch(Harmony harmony)
    {
        harmony.PatchAll(typeof(BreakableResourcePatcher));
        InternalLogger.Info("Finished patching BreakableResource.");
    }

    public static IDictionary<TechType, List<OutcropDropData>> CustomDrops = new SelfCheckingDictionary<TechType, List<OutcropDropData>>("CustomOutcropsDrops");

    [HarmonyPostfix]
    [HarmonyPatch(nameof(BreakableResource.Awake))]
    private static void Awake(BreakableResource __instance)
    {
        TechType outcropTechType = __instance.gameObject.GetComponent<TechTag>().type;
        List<OutcropDropData> convertedDropsDatas = new();
        if(!CustomDrops.ContainsKey(outcropTechType))
        {
            CustomDrops.Add(outcropTechType, new());
        }

        foreach (BreakableResource.RandomPrefab randPrefab in __instance.prefabList)
            convertedDropsDatas.Add(randPrefab.ToOutcropDropData());

        CustomDrops[outcropTechType].Union(convertedDropsDatas).ToList();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BreakableResource.BreakIntoResources))]
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
                    __instance.SpawnResourceFromTechType(chosenResource);
                    spawnSuccessful = true;
                }
            }
            if (!spawnSuccessful)
            {
                __instance.SpawnResourceFromPrefab(__instance.defaultPrefabReference);
            }
            FMODUWE.PlayOneShot(__instance.breakSound, __instance.transform.position, 1f);
            if (__instance.hitFX)
            {
                global::Utils.PlayOneShotPS(__instance.breakFX, __instance.transform.position, Quaternion.Euler(new Vector3(270f, 0f, 0f)), null);
            }
            return false;
        }
        return false;
    }
}
