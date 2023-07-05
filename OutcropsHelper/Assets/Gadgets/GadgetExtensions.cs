using Nautilus.Assets;
using Nautilus.OutcropsHelper.Interfaces;
using Nautilus.OutcropsHelper.Patchers;
using Nautilus.OutcropsHelper.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nautilus.OutcropsHelper;

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
        return OutcropsUtils.AddOrUpdateOutcropDrop(customPrefab.Info.TechType, outcropTechType, chance);
    }

}
