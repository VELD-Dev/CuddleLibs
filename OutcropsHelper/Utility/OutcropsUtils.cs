using Nautilus.Assets;
using Nautilus.OutcropsHelper.Interfaces;
using Nautilus.OutcropsHelper.Patchers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        foreach (BreakableResource.RandomPrefab randPrefab in instance.prefabList)
            convertedDropsDatas.Add(randPrefab.ToOutcropDropData());

        BreakableResourcePatcher.CustomDrops.TryGetValue(instance.gameObject.GetComponent<TechTag>().type, out var customDropsDatas);

        List<OutcropDropData> UnionedDropDatas = customDropsDatas.Union(convertedDropsDatas).ToList();

        foreach(OutcropDropData dropData in UnionedDropDatas)
        {
            InternalLogger.Debug($"Drop data: {dropData}");
            if (Player.main.gameObject.GetComponent<PlayerEntropy>().CheckChance(dropData.resourceTechType, dropData.chance))
            {
                result = dropData.resourceTechType;
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
    public static OutcropDropData AddOrUpdateOutcropDrop(TechType resourceTechType, TechType outcropTechType, float chance = 0.5f)
    {
        if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
        {
            if (BreakableResourcePatcher.CustomDrops[outcropTechType].Find((odd) => odd.resourceTechType == resourceTechType) == null)
            {
                OutcropDropData data = new() { resourceTechType = resourceTechType, chance = chance };

                BreakableResourcePatcher.CustomDrops[outcropTechType].Add(data);
                return data;
            }
            else
            {
                var existingOutcrop = BreakableResourcePatcher.CustomDrops[outcropTechType].Find((odd) => odd.resourceTechType == resourceTechType);
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
}
