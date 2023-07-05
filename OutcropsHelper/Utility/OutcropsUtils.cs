using Nautilus.OutcropsHelper.Interfaces;
using Nautilus.OutcropsHelper.Patchers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UWE;

namespace Nautilus.OutcropsHelper.Utility;

/// <summary>
/// An helper and extender for Breakable Resource.
/// </summary>
public static class OutcropsUtils
{
    /// <summary>
    /// Choose a resource among the 
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static TechType ChooseRandomResourceTechType(this BreakableResource instance)
    {
        TechType result = TechType.None;
        List<OutcropDropData> convertedDropsDatas = new();
        TechType outcropTechType = CraftData.GetTechType(instance.gameObject);

        foreach (BreakableResource.RandomPrefab randPrefab in instance.prefabList)
        {
            EnsureOutcropDrop(randPrefab.prefabTechType, outcropTechType, randPrefab.chance);
        }

        foreach(OutcropDropData dropData in BreakableResourcePatcher.CustomDrops[outcropTechType])
        {
            InternalLogger.Debug($"Checking drop data:\n{dropData}");
            if (Player.main.gameObject.GetComponent<PlayerEntropy>().CheckChance(dropData.resourceTechType, dropData.chance))
            {
                result = dropData.resourceTechType;
                InternalLogger.Debug($"Chosen drop data:\n{dropData}");
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Spawns a resource using its <see cref="TechType"/>
    /// </summary>
    /// <param name="instance">Instance of <see cref="BreakableResource"/>.</param>
    /// <param name="techType"><see cref="TechType"/> the resource to spawn.</param>
    public static void SpawnResourceFromTechType(this BreakableResource instance, TechType techType)
    {
        CoroutineHost.StartCoroutine(SpawnResourceAsync(techType, instance.transform.position + instance.transform.up * instance.verticalSpawnOffset, instance.transform));
    }

    /// <summary>
    /// Makes spawn asynchronely a resource at a position with a certain transform.
    /// </summary>
    /// <param name="techType">TechType of the prefab to spawn.</param>
    /// <param name="position">Position at spawn of the prefab.</param>
    /// <param name="parent">Transform of the "parent" object.</param>
    /// <returns></returns>
    public static IEnumerator SpawnResourceAsync(TechType techType, Vector3 position, Transform parent)
    {
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
        yield return task;
        try
        {
            var prefab = task.GetResult();
            if (prefab == null)
            {
                InternalLogger.Error("Failed to spawn {0}: Prefab is null.", techType);
                yield break;
            }
            global::Utils.SpawnPrefabAt(prefab, parent, position);
            Rigidbody rigidbody = prefab.EnsureComponent<Rigidbody>();
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false, false);
            rigidbody.AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
            rigidbody.AddForce(parent.up * 0.1f);
            yield break;
        }
        catch(Exception e)
        {
            InternalLogger.Error($"An error ocurred while spawning Resource prefab ({techType}).\n{e}");
            yield break;
        }
    }

    /// <summary>
    /// Converts a <see cref="BreakableResource.RandomPrefab"/> to a <see cref="OutcropDropData"/>.
    /// </summary>
    /// <param name="randomPrefab">Instance of random prefab to convert.</param>
    /// <returns>An instance of <see cref="OutcropDropData"/>.</returns>
    public static OutcropDropData ToOutcropDropData(this BreakableResource.RandomPrefab randomPrefab)
    {
        return new OutcropDropData()
        {
            resourceTechType = randomPrefab.prefabTechType,
            chance = randomPrefab.chance,
        };
    }

    /// <summary>
    /// Adds an outcrop drop data for a specific outcrop type.
    /// </summary>
    /// <param name="resourceTechType"><see cref="TechType"/> of the resource to spawn when an outcrop is broken.</param>
    /// <param name="outcropTechType"><see cref="TechType"/> of the outcrop.</param>
    /// <param name="chance">Spawn chance (between 0 and 1)</param>
    /// <returns>An instance of the created <see cref="OutcropDropData"/>.</returns>
    public static OutcropDropData EnsureOutcropDrop(TechType resourceTechType, TechType outcropTechType, float chance = 0.5f)
    {
        if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
        {
            var outcropDropsDatas = BreakableResourcePatcher.CustomDrops[outcropTechType];
            if (outcropDropsDatas.Find((odd) => odd.resourceTechType == resourceTechType) == null)
            {
                OutcropDropData data = new() { resourceTechType = resourceTechType, chance = chance };
                BreakableResourcePatcher.CustomDrops[outcropTechType].Add(data);
                return data;
            }
            else
            {
                var existingOutcrop = outcropDropsDatas.Find((odd) => odd.resourceTechType == resourceTechType);
                existingOutcrop.chance = chance;
                return existingOutcrop;
            }
        }
        else
        {
            OutcropDropData data = new() { resourceTechType = resourceTechType, chance = chance };
            BreakableResourcePatcher.CustomDrops.Add(new(outcropTechType, new() { data }));
            return data;
        }
    }

    /// <summary>
    /// This is mandatory. This is a class that checks the chance of spawning the techtype.
    /// <para>Normally you shouldn't have to set this manually, it is handled by all the functions adding a new drop.</para>
    /// </summary>
    /// <param name="techType"><see cref="TechType"/> of the item to ensure the <see cref="PlayerEntropy.TechEntropy"/> for.</param>
    /// <returns>An instance of the <see cref="PlayerEntropy.TechEntropy"/> created or found.</returns>
    public static PlayerEntropy.TechEntropy EnsureTechEntropy(TechType techType)
    {
        PlayerEntropy playerEntropy = Player.main.gameObject.GetComponent<PlayerEntropy>();
        PlayerEntropy.TechEntropy techEntropy = playerEntropy.randomizers.Find((pete) => pete.techType == techType);
        if (techEntropy == null)
        {
            techEntropy = new PlayerEntropy.TechEntropy()
            {
                techType = techType,
                entropy = new FairRandomizer()
            };

            playerEntropy.randomizers.Add(techEntropy);
        }
        InternalLogger.Debug($"Ensured a TechEntropy for {techEntropy.techType}, with a current Entropy of {techEntropy.entropy.entropy}.");
        return techEntropy;
    }
}
