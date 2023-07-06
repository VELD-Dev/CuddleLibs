namespace CuddleLibs.Assets.Gadgets;

/// <summary>
/// <inheritdoc cref="Nautilus.Assets.Gadgets.GadgetExtensions"/>
/// </summary>
public static class GadgetExtension
{
    /// <summary>
    /// Make the item spawnable in a certain outcrop.
    /// </summary>
    /// <param name="customPrefab">The custom prefab to set the outcrops drop data for.</param>
    /// <param name="outcropTechType">The outcrop techtype to spawn the item in.</param>
    /// <param name="chance">Chance of spawn of the item when the outcrop breaks.</param>
    /// <returns>An instance of the created OutcropDropData for debug purposes.</returns>
    public static OutcropDropData SetOutcropDrop(this ICustomPrefab customPrefab, TechType outcropTechType, float chance = 0.5f)
    {
        return OutcropsUtils.EnsureOutcropDrop(outcropTechType, customPrefab.Info.TechType, chance);
    }

    /// <summary>
    /// Make the item spawnable into several outcrop types.
    /// <para>It can be used in addition to the other SetOutcropDrop overload, however the values will be overriden if a TechType exist in both overloads.</para>
    /// </summary>
    /// <param name="customPrefab"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static OutcropDropData[] SetOutcropDrop(this ICustomPrefab customPrefab, params KeyValuePair<TechType, float>[] data)
    {
        OutcropDropData[] array = new OutcropDropData[data.Length];
        foreach(var kvp in data)
        {
            array.Add(OutcropsUtils.EnsureOutcropDrop(kvp.Key, customPrefab.Info.TechType, kvp.Value));
        }
        return array;
    }
}
