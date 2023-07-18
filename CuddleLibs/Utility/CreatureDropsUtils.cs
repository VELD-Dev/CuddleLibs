using CuddleLibs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuddleLibs.Utility;

/// <summary>
/// An helper and extender for Creatures Drops.
/// </summary>
public static class CreatureDropsUtils
{    
    /// <summary>
    /// Choose a resource among the 
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static TechType ChooseRandomResourceTechType(this Creature instance)
    {
        TechType result = TechType.None;
        List<CreatureDropData> convertedDropsDatas = new();
        TechType creatureTechType = CraftData.GetTechType(instance.gameObject);

        foreach (CreatureDropData dropData in CreaturePatcher.CustomDrops[creatureTechType])
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
    /// Ensures that the TechType will spawn when creatureTechType is killed.<br/>
    /// Recommended value for chance: &lt;0.5f
    /// </summary>
    /// <param name="creatureTechType"><see cref="TechType"/> of the creature to set the drop data for.</param>
    /// <param name="resourceTechType"><see cref="TechType"/> of the resource to spawn when the creature is killed.</param>
    /// <param name="chance">Precentage of chance for the resource to spawn when the creature is killed.</param>
    /// <param name="spawn_amount">Amount of that resource that must spawn when the creature is killed.</param>
    /// <param name="unique">If it is unique, this is the only resource that will spawn if it is picked when the creature is killed.<br/>
    /// You may want to avoid breaking other mods, for that set a chance under 0.5f.</param>
    /// <returns>Returns the created or edited drop data.</returns>
    public static CreatureDropData EnsureCreatureDrop(TechType creatureTechType, TechType resourceTechType, float chance = 0.25f, ushort spawn_amount = 1, bool unique = false)
    {
        if(CreaturePatcher.CustomDrops.ContainsKey(creatureTechType))
        {
            var creatureDropsDatas = CreaturePatcher.CustomDrops[creatureTechType];
            if (creatureDropsDatas.Find((cdd) => cdd.TechType == resourceTechType) == null)
            {
                CreatureDropData data = new()
                {
                    TechType = resourceTechType,
                    chance = chance,
                    dropAmount = spawn_amount,
                    unique = unique
                };
                CreaturePatcher.CustomDrops[creatureTechType].Add(data);
                return data;
            }
            else
            {
                var existingOutcrop = creatureDropsDatas.Find((cdd) => cdd.TechType == resourceTechType);
                existingOutcrop.chance = chance;
                existingOutcrop.dropAmount = spawn_amount;
                existingOutcrop.unique = unique;
                return existingOutcrop;
            }
        }
        else
        {
            CreatureDropData data = new()
            {
                TechType = resourceTechType,
                chance = chance,
                dropAmount = spawn_amount,
                unique = unique
            };
            CreaturePatcher.CustomDrops.Add(new(creatureTechType, new() { data }));
            return data;
        }
    }

    /// <summary>
    /// <inheritdoc cref="EnsureCreatureDrop(TechType, TechType, float, ushort, bool)"/>
    /// </summary>
    /// <param name="creatureTechType"><inheritdoc cref="EnsureCreatureDrop(TechType, TechType, float, ushort, bool)" path="/param[@name='creatureTechType']"/></param>
    /// <param name="data"><see cref="CreatureDropData"/> to add to the creature</param>
    /// <returns></returns>
    public static CreatureDropData EnsureCreatureDrop(TechType creatureTechType, CreatureDropData data)
    {
        return EnsureCreatureDrop(creatureTechType, data.TechType, data.chance, data.dropAmount, data.unique);
    }

    /// <summary>
    /// Ensures a bunch of resources drops for a bunch of creatures.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: Percentage of chance of dropping the item when creature is killed. (Between 0 and 1)</item>
    ///   <item><see cref="ushort"/>: Amount of dropped items when creature is killed.</item>
    ///   <item><see cref="bool"/>: Wether the resource should be an unique drop or not. See <see cref="CreatureDropData.unique"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="values"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature.</item>
    ///   <item><see cref="TechType"/>: TechType of the resource.</item>
    ///   <item><see cref="float"/>: Percentage of chance of dropping the item when creature is killed. (Between 0 and 1)</item>
    ///   <item><see cref="ushort"/>: Amount of dropped items when creature is killed.</item>
    ///   <item><see cref="bool"/>: Wether the resource should be an unique drop or not. See <see cref="CreatureDropData.unique"/>.</item>
    /// </list></param>
    /// <returns>Array of created or edited creature drops data.</returns>
    public static CreatureDropData[] EnsureCreatureDrop(params (TechType, TechType, float, ushort, bool)[] values)
    {
        var array = new CreatureDropData[values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = EnsureCreatureDrop(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5);
        }
        return array;
    }

    /// <summary>
    /// Ensures a bunch of resources drops for a bunch of creratures.
    /// <list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature to spawn the resource for.</item>
    ///   <item><see cref="CreatureDropData"/>: Drop data to add to the creature drops.</item>
    /// </list>
    /// </summary>
    /// <param name="values"><list type="number">
    ///   <item><see cref="TechType"/>: TechType of the creature to spawn the resource for.</item>
    ///   <item><see cref="CreatureDropData"/>: Drop data to add to the creature drops.</item>
    /// </list></param>
    /// <returns><inheritdoc cref="EnsureCreatureDrop(ValueTuple{TechType, TechType, float, ushort, bool}[])" path="/returns"/></returns>
    public static CreatureDropData[] EnsureCreatureDrop(params (TechType, CreatureDropData)[] values)
    {
        var array = new CreatureDropData[values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            var v = values[i];
            array[i] = EnsureCreatureDrop(v.Item1, v.Item2);
        }
        return array;
    }
}
