namespace CuddleLibs.Utility;

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
            EnsureOutcropDrop(outcropTechType, randPrefab.prefabTechType, randPrefab.chance);
        }

        foreach(OutcropDropData dropData in BreakableResourcePatcher.CustomDrops[outcropTechType])
        {
            InternalLogger.Debug($"Checking {dropData.resourceTechType}. Drop data:\n{dropData}");
            if (Player.main.gameObject.GetComponent<PlayerEntropy>().CheckChance(dropData.resourceTechType, dropData.chance))
            {
                result = dropData.resourceTechType;
                InternalLogger.Debug($"Chosed {dropData.resourceTechType}. Drop data:\n{dropData}");
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
    /// <param name="transform">Transform of the parent gameobject.</param>
    /// <returns></returns>
    public static IEnumerator SpawnResourceAsync(TechType techType, Vector3 position, Transform transform)
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
            EditorModifications.Instantiate<GameObject>(prefab, position, transform.rotation, true);
            Rigidbody rigidbody = prefab.EnsureComponent<Rigidbody>();
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false, false);
            rigidbody.AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
            rigidbody.AddForce(transform.up * 0.1f);
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
    /// Removes the vanilla drops of this breakable resource. You should probably only use it on custom outcrops.
    /// <para>N.B. Titanium still have a chance of spawn. You will be able to replace the default resource in a future version.</para>
    /// </summary>
    /// <param name="instance">Instance of <see cref="BreakableResource"/>.</param>
    /// <returns>true if it have been correctly removed. False if they were not removed or if there was nothing to remove.</returns>
    public static bool RemoveVanillaDrops(this BreakableResource instance)
    {
        TechType outcropTechType = CraftData.GetTechType(instance.gameObject);
        if(instance.prefabList.Count > 0)
        {
            if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
            {
                BreakableResourcePatcher.CustomDrops[outcropTechType].RemoveAll(
                    (ocdd) => instance.prefabList.Find((randPrefab) => randPrefab.prefabTechType == ocdd.resourceTechType) != null
                );
            }

            instance.prefabList.Clear();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes the vanilla drops of a specific breakable resource. You should probably only use it on custom outcrops.
    /// <para>N.B. Titanium still have  a chance of spawn. You will be able to replace the default resource in a future version.</para>
    /// </summary>
    /// <param name="outcropTechType"><see cref="TechType"/> of the outcrop to remove vanilla drops from.</param>
    /// <returns></returns>
    /// <exception cref="MissingComponentException">Can occur if there is no BreakableResource component on the designated outcrop.</exception>
    public static IEnumerator RemoveVanillaDropsAsync(TechType outcropTechType)
    {
        var task = CraftData.GetPrefabForTechTypeAsync(outcropTechType);
        yield return task;
        var prefab = task.GetResult();
        if(!prefab.TryGetComponent<BreakableResource>(out var component))
            throw new MissingComponentException($"Missing {nameof(BreakableResource)} component on the prefab of {outcropTechType}");

        component.RemoveVanillaDrops();
    }

    /// <summary>
    /// Ensures that the resourceTechType will spawn when outcropTechType is broken.<br/>
    /// Recommended value for chance: &lt;0.5f
    /// </summary>
    /// <param name="outcropTechType"><see cref="TechType"/> of the outcrop.</param>
    /// <param name="resourceTechType"><see cref="TechType"/> of the resource to spawn when an outcrop is broken.</param>
    /// <param name="chance">Spawn chance (between 0 and 1)<br/>Recommended value: &lt;0.5f </param>
    /// <returns>An instance of the created <see cref="OutcropDropData"/>.</returns>
    public static OutcropDropData EnsureOutcropDrop(TechType outcropTechType, TechType resourceTechType, float chance = 0.25f)
    {
        if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
        {
            var outcropDropsDatas = BreakableResourcePatcher.CustomDrops[outcropTechType];
            if (outcropDropsDatas.Find((ocdd) => ocdd.resourceTechType == resourceTechType) == null)
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
    /// <inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="dropData">An instance of drop data.</param>
    /// <returns><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/returns"/></returns>
    public static OutcropDropData EnsureOutcropDrop(TechType outcropTechType, OutcropDropData dropData)
    {
        return EnsureOutcropDrop(outcropTechType, dropData.resourceTechType, dropData.chance);
    }

    /// <summary>
    /// Ensure that resources TechTypes can spawn for outcrops TechTypes.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: percentage of chance to spawn the resource.</item>
    /// </list>
    /// </summary>
    /// <param name="values"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: percentage of chance to spawn the resource.</item>
    /// </list></param>
    /// <returns></returns>
    public static OutcropDropData[] EnsureOutcropDrop(params (TechType, TechType, float)[] values)
    {
        var array = new OutcropDropData[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = EnsureOutcropDrop(v.Item1, v.Item2, v.Item3);
        }
        return array;
    }

    /// <summary>
    /// Ensure that resources DropData can spawn for oucrops TechTypes.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="OutcropDropData"/>: OutcropDropData of the resource.</item>
    /// </list>
    /// </summary>
    /// <param name="values"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="OutcropDropData"/>: OutcropDropData of the resource.</item>
    /// </list></param>
    /// <returns></returns>
    public static OutcropDropData[] EnsureOutcropDrop(param (TechType, OutcropDropData)[] values)
    {
        var array = new OutcropDropData[values.Length];
        for (int i = 0; i < values.Length)
        {
            var v = values[i];
            array[i] = EnsureOutcropDrop(v.Item1, v.Item2);
        }
        return array;
    }

    /// <summary>
    /// Sets an outcrop drop data for a specific outcrop type if it does not already exists, otherwise abort.
    /// </summary>
    /// <param name="resourceTechType"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='resourceTechType']"/></param>
    /// <param name="outcropTechType"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="chance"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='chance']"/></param>
    /// <param name="dropData"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/returns"/></param>
    /// <returns>True if the outcrop drop data have been created, false if an entry was already existing for it.</returns>
    public static bool SetOutcropDrop(TechType outcropTechType, TechType resourceTechType, out OutcropDropData dropData, float chance = 0.25f)
    {
        if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
        {
            if (BreakableResourcePatcher.CustomDrops[outcropTechType].Exists((ocdd) => ocdd.resourceTechType == resourceTechType))
            {
                dropData = BreakableResourcePatcher.CustomDrops[outcropTechType].Find((ocdd) => ocdd.resourceTechType == resourceTechType);
                return false;  // No, it did not add a new OutcropDropData.
            }

            dropData = new() { resourceTechType = resourceTechType, chance = chance };
            BreakableResourcePatcher.CustomDrops[outcropTechType].Add(dropData);
            return true;
        }
        dropData = new() { resourceTechType = resourceTechType, chance = chance };
        BreakableResourcePatcher.CustomDrops.Add(new(outcropTechType, new() { dropData }));
        return true;
    }

    /// <summary>
    /// <inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="resourceTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='resourceTechType']"/></param>
    /// <param name="chance"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='chance']"/></param>
    /// <returns><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/returns"/></returns>
    public static bool SetOutcropDrop(TechType outcropTechType, TechType resourceTechType, float chance = 0.25f)
    {
        return SetOutcropDrop(outcropTechType, resourceTechType, out _, chance);
    }

    /// <summary>
    /// <inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="dropData">An instance of drop data.</param>
    /// <param name="outputDropData"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='dropData']"/></param>
    /// <returns></returns>
    public static bool SetOutcropDrop(TechType outcropTechType, OutcropDropData dropData, out OutcropDropData outputDropData)
    {
        return SetOutcropDrop(outcropTechType, dropData.resourceTechType, out outputDropData, dropData.chance);
    }

    /// <summary>
    /// <inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="dropData">An instance of drop data.</param>
    /// <returns><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/returns"/></returns>
    public static bool SetOutcropDrop(TechType outcropTechType, OutcropDropData dropData)
    {
        return SetOutcropDrop(outcropTechType, dropData.resourceTechType, dropData.chance);
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
