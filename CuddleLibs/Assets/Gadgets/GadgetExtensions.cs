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
    public static OutcropDropData SetOutcropDrop(this ICustomPrefab customPrefab, TechType outcropTechType, float chance = 0.25f)
    {
        return OutcropsUtils.EnsureOutcropDrop(outcropTechType, customPrefab.Info.TechType, chance);
    }

    /// <summary>
    /// Make the item spawnable into several outcrop types.
    /// <para>It can be used in addition to the other SetOutcropDrop overload, however the values will be overriden if a TechType exist in both overloads.</para>
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="float"/>: Chance of spawn of the item when this outcrop breaks.</item>
    /// </list>
    /// </summary>
    /// <param name="customPrefab">The custom prefab to set the outcrops drop datas for.</param>
    /// <param name="data"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the outcrop.</item>
    ///   <item><see cref="float"/>: Chance of spawn of the item when this outcrop breaks.</item>
    /// </list></param>
    /// <returns>Bunch of created or edited <see cref="OutcropDropData"/>.</returns>
    public static OutcropDropData[] SetOutcropDrop(this ICustomPrefab customPrefab, params (TechType, float)[] data)
    {
        var array = new OutcropDropData[data.Length];
        for(int i = 0; i < data.Length; i++)
        {
            var v = data[i];
            array[i] = OutcropsUtils.EnsureOutcropDrop(v.Item1, customPrefab.Info.TechType, v.Item2);
        }
        return array;
    }

    /// <summary>
    /// Make the item droppablke by a creature when it is killed.
    /// </summary>
    /// <param name="customPrefab">The custom prefab to set the creature drop data for.</param>
    /// <param name="creatureTechType">TechType of the creature to add the drop to.</param>
    /// <param name="chance">Chance of dropping the item when the creature is killed.</param>
    /// <param name="spawn_amount">Amount of items that must spawn when the creature is killed.</param>
    /// <param name="unique">Wether this is the only item that must spawn when the creature or if it allows other drops to spawn.</param>
    /// <returns>The created <see cref="CreatureDropData"/>.</returns>
    public static CreatureDropData SetCreatureDrop(this ICustomPrefab customPrefab, TechType creatureTechType, float chance = 0.25f, ushort spawn_amount = 1, bool unique = false)
    {
        return CreatureDropsUtils.EnsureCreatureDrop(creatureTechType, customPrefab.Info.TechType, chance, spawn_amount, unique);
    }

    /// <summary>
    /// Make the item droppable by a bunch of creatures when killed.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature.</item>
    ///   <item><see cref="float"/>: Percentage of chance of spawning the item between 0 and 1.</item>
    ///   <item><see cref="ushort"/>: Amount of this item that must spawn when a creature is killed.</item>
    ///   <item><see cref="bool"/>: If this item is an unique drop on this creature of if it can spawn with other items.</item>
    /// </list>
    /// </summary>
    /// <param name="customPrefab">The custom prefab to set the creatures drops datas for.</param>
    /// <param name="data"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature.</item>
    ///   <item><see cref="float"/>: Percentage of chance of spawning the item between 0 and 1.</item>
    ///   <item><see cref="ushort"/>: Amount of this item that must spawn when a creature is killed.</item>
    ///   <item><see cref="bool"/>: If this item is an unique drop on this creature of if it can spawn with other items.</item>
    /// </list></param>
    /// <returns></returns>
    public static CreatureDropData[] SetCreatureDrop(this ICustomPrefab customPrefab, params (TechType, float, ushort, bool)[] data)
    {
        var array = new CreatureDropData[data.Length];
        for(int i = 0; i < data.Length; i++)
        {
            var v = data[i];
            array[i] = CreatureDropsUtils.EnsureCreatureDrop(v.Item1, customPrefab.Info.TechType, v.Item2, v.Item3, v.Item4);
        }
        return array;
    }
}
