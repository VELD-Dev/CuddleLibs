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
            InternalLogger.Debug($"Checking {dropData.TechType}. Drop data:\n{dropData}");
            if (Player.main.gameObject.GetComponent<PlayerEntropy>().CheckChance(dropData.TechType, dropData.chance))
            {
                result = dropData.TechType;
                InternalLogger.Debug($"Chosed {dropData.TechType}. Drop data:\n{dropData}");
                break;
            }
        }
        return result;
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
            TechType = randomPrefab.prefabTechType,
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
                    (ocdd) => instance.prefabList.Find((randPrefab) => randPrefab.prefabTechType == ocdd.TechType) != null
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
    /// Ensures that the TechType will spawn when outcropTechType is broken.<br/>
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
            if (outcropDropsDatas.Find((ocdd) => ocdd.TechType == resourceTechType) == null)
            {
                OutcropDropData data = new() { TechType = resourceTechType, chance = chance };
                BreakableResourcePatcher.CustomDrops[outcropTechType].Add(data);
                return data;
            }
            else
            {
                var existingOutcrop = outcropDropsDatas.Find((odd) => odd.TechType == resourceTechType);
                existingOutcrop.chance = chance;
                return existingOutcrop;
            }
        }
        else
        {
            OutcropDropData data = new() { TechType = resourceTechType, chance = chance };
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
        return EnsureOutcropDrop(outcropTechType, dropData.TechType, dropData.chance);
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
    public static OutcropDropData[] EnsureOutcropDrop(params (TechType, OutcropDropData)[] values)
    {
        var array = new OutcropDropData[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = EnsureOutcropDrop(v.Item1, v.Item2);
        }
        return array;

    }

    /// <summary>
    /// Sets an outcrop drop data for a specific outcrop tech type if it does not already exists, otherwise abort.
    /// </summary>
    /// <param name="resourceTechType"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='TechType']"/></param>
    /// <param name="outcropTechType"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="chance"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/param[@name='chance']"/></param>
    /// <param name="dropData"><inheritdoc cref="EnsureOutcropDrop(TechType, TechType, float)" path="/returns"/></param>
    /// <returns>True if the outcrop drop data have been created, false if an entry was already existing for it.</returns>
    public static bool SetOutcropDrop(TechType outcropTechType, TechType resourceTechType, out OutcropDropData dropData, float chance = 0.25f)
    {
        if (BreakableResourcePatcher.CustomDrops.ContainsKey(outcropTechType))
        {
            if (BreakableResourcePatcher.CustomDrops[outcropTechType].Exists((ocdd) => ocdd.TechType == resourceTechType))
            {
                dropData = BreakableResourcePatcher.CustomDrops[outcropTechType].Find((ocdd) => ocdd.TechType == resourceTechType);
                return false;  // No, it did not add a new OutcropDropData.
            }

            dropData = new() { TechType = resourceTechType, chance = chance };
            BreakableResourcePatcher.CustomDrops[outcropTechType].Add(dropData);
            return true;
        }
        dropData = new() { TechType = resourceTechType, chance = chance };
        BreakableResourcePatcher.CustomDrops.Add(new(outcropTechType, new() { dropData }));
        return true;
    }

    /// <summary>
    /// <inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="resourceTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='TechType']"/></param>
    /// <param name="chance"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='chance']"/></param>
    /// <returns><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/returns"/></returns>
    public static bool SetOutcropDrop(TechType outcropTechType, TechType resourceTechType, float chance = 0.25f)
    {
        return SetOutcropDrop(outcropTechType, resourceTechType, out _, chance);
    }

    /// <summary>
    /// Sets a bunch of outcrop drop data for a bunch of outcrops tech types if they do not already exist, otherwise abort ofr X tech type.
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
    /// <returns>True if an outcrop drop data have been created, false if an entry was already existing for each item of the list.</returns>
    public static bool[] SetOutcropDrop(params (TechType, TechType, float)[] values)
    {
        var array = new bool[values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = SetOutcropDrop(v.Item1, v.Item2, v.Item3);
        }
        return array;
    }

    /// <summary>
    /// Sets a bunch of outcrop drop data for a bunch of outcrops tech types if they do not already exist, otherwise abort for X tech type.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: percentage of chance to spawn the resource.</item>
    /// </list>
    /// </summary>
    /// <param name="dropDatas">Instances of created outcrop drop datas.</param>
    /// <param name="values"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: percentage of chance to spawn the resource.</item>
    /// </list></param>
    /// <returns>True if an outcrop drop data have been created, false if an entry was already existing for each item of the list.</returns>
    public static bool[] SetOutcropDrop(out OutcropDropData[] dropDatas, params (TechType, TechType, float)[] values)
    {
        var array = new bool[values.Length];
        dropDatas = new OutcropDropData[values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = SetOutcropDrop(v.Item1, v.Item2, out dropDatas[i], v.Item3);
        }
        return array;
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
        return SetOutcropDrop(outcropTechType, dropData.TechType, out outputDropData, dropData.chance);
    }

    /// <summary>
    /// <inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)"/>
    /// </summary>
    /// <param name="outcropTechType"><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/param[@name='outcropTechType']"/></param>
    /// <param name="dropData">An instance of drop data.</param>
    /// <returns><inheritdoc cref="SetOutcropDrop(TechType, TechType, out OutcropDropData, float)" path="/returns"/></returns>
    public static bool SetOutcropDrop(TechType outcropTechType, OutcropDropData dropData)
    {
        return SetOutcropDrop(outcropTechType, dropData.TechType, dropData.chance);
    }
}
